using System;
using System.Net.Http;
using System.Net.Http.Headers;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Interfaces;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    public abstract class TranslateEngine
    {
        protected delegate HttpResponseMessage GetResponseMessageHandle(HttpClient httpClient);

        protected delegate string GetTranslatedTextHandle(string response);

        public virtual string DisplayName => Name;
        public abstract EngineTypes EngineType { get; }
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
                    catch (Exception ex)
                    {
                        translatedText = "!Error!";
                    }
                }
            }

            return translatedText;
        }
    }
}