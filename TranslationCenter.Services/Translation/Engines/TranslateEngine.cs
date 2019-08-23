using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    public abstract class TranslateEngine
    {
        private EngineInfoAttribute _engineInfo;

        public TranslateEngine()
        {
            _engineInfo = this.GetType().GetCustomAttributes(typeof(EngineInfoAttribute), false).OfType<EngineInfoAttribute>().FirstOrDefault();
            if (_engineInfo == null)
                throw new NotImplementedException($"{ nameof(EngineInfoAttribute) } not implemented!");
        }

        protected delegate HttpResponseMessage GetResponseMessageHandle(HttpClient httpClient);

        protected delegate string GetTranslatedTextHandle(string response);

        public string DisplayName => _engineInfo.DisplayName;
        public EngineCategory Category => _engineInfo.Category;
        public string Name => GetType().Name;
        public ResultTypes ResultType { get; }
        public TranslateArgs TranslationArgs { get; private set; }

        protected abstract string MediaType { get; }

        protected abstract string UrlBase { get; }

        protected virtual string UrlBaseAdditional => string.Empty;

        protected virtual bool IsTranslateUnsupported => false;

        internal ITranslateResult GetTranslate(TranslateArgs args)
        {
            TranslationArgs = args;

            var result = GetResponse(GetResponseMessage, GetTranslatedText);

            return new TranslateResult(this, result);
        }

        protected abstract HttpResponseMessage GetResponseMessage(HttpClient httpClient);

        protected abstract string GetTranslatedText(string response);

        private string GetResponse(GetResponseMessageHandle getResponseMessage,
                                     GetTranslatedTextHandle getTranslatedText)
        {
            string translatedText = string.Empty;

            if (IsTranslateUnsupported)
                return $"Translation not Suported!";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(UrlBase + UrlBaseAdditional);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var response = getResponseMessage(client);

                if (response.IsSuccessStatusCode)
                {
                    var taskReadString = response.Content.ReadAsStringAsync();
                    taskReadString.Wait();
                    var responseText = taskReadString.Result;

                    try
                    {
                        translatedText = getTranslatedText(responseText);
                    }
                    catch
                    {
                        translatedText = "!Error!";
                    }
                }
            }

            return translatedText;
        }

        internal void UpdateUrlElements(HtmlNodeCollection htmlNodes, string attributeName, string urlBase, Action<HtmlNode> additionalAction = null)
        {
            if (htmlNodes != null)
            {
                foreach (var node in htmlNodes)
                {
                    var href = node.Attributes[attributeName];
                    var value = href?.Value ?? string.Empty;
                    if (value.StartsWith("javascript")) continue;
                    if (value.StartsWith("about:")) continue;
                    if (!string.IsNullOrWhiteSpace(value) && !value.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !value.StartsWith("//"))
                        node.SetAttributeValue(attributeName, $"{urlBase}{href.Value}");
                    additionalAction?.Invoke(node);
                }
            }
        }

        internal void RemoveElements(HtmlNodeCollection htmlNodeCollection)
        {
            if (htmlNodeCollection != null)
            {
                foreach (var node in htmlNodeCollection)
                {
                    node.Remove();
                }
            }
        }
    }
}