using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        
        public abstract TranslateArgs TranslationArgs { get; protected set; }

        protected abstract string MediaType { get; }

        protected abstract string UrlBase { get; }

        protected virtual string UrlBaseAdditional => string.Empty;

        protected virtual bool IsTranslateUnsupported => false;

        internal ITranslateResult GetTranslate(TranslateArgs translateArgs, bool renderize = false)
        {
            //TranslationArgs = translateArgs;

            var translateResult = new TranslateResult(this, ()=> GetResponse(translateArgs, GetResponseMessage, GetTranslatedText));

            if (renderize)
                translateResult.Render();

            return translateResult;
        }

        protected abstract HttpResponseMessage GetResponseMessage(HttpClient httpClient);

        protected abstract string GetTranslatedText(string response);

        private string GetResponse(TranslateArgs translateArgs, GetResponseMessageHandle getResponseMessage,
                                     GetTranslatedTextHandle getTranslatedText)
        {
            TranslationArgs = translateArgs;

            string translatedText = string.Empty;

            //if (IsTranslateUnsupported)
            //    return $"Translation not Suported!";

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
                        if (string.IsNullOrWhiteSpace(translatedText))
                            translatedText = $"<strong>Sorry, no results found for {translateArgs.LanguageFrom.Name} -> {translateArgs.LanguageTo.Name}!</strong>";
                    }
                    catch
                    {
                        translatedText = "!Error!";
                    }
                } else
                {
                    translatedText = $"<strong>Sorry, no results found for {translateArgs.LanguageFrom.Name} -> {translateArgs.LanguageTo.Name}!</strong>";
                }
            }

            return translatedText;
        }

        internal string GetContentWithHierarchy(HtmlNode node)
        {
            var content = node.OuterHtml;

            node = node.ParentNode;

            while (node != null)
            {
                var nodeName = node.Name;
                var idValue = node.Attributes["id"]?.Value;
                var classValue = node.Attributes["class"]?.Value;
                if (nodeName.Equals("body", StringComparison.OrdinalIgnoreCase) || nodeName.Equals("html", StringComparison.OrdinalIgnoreCase))
                    break;
                content = @$"<{nodeName} id=""{idValue}"" class=""{ classValue }"" style=""overflow: auto !important; height: auto !important; width: auto !important;"">{content}</{nodeName}>";
                node = node.ParentNode;
            }

            return content;
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
                    if (value.StartsWith("//www."))
                    {
                        node.SetAttributeValue(attributeName, $"http://{value.Substring(2)}");
                    }
                    else if (!string.IsNullOrWhiteSpace(value) && !value.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !value.StartsWith("//"))
                    {
                        node.SetAttributeValue(attributeName, $"{urlBase}{value}");
                    }
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