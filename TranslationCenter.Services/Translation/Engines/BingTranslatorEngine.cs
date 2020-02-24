using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Translator, "Bing")]
    public class BingTranslatorEngine : TranslateEngine
    {
        protected override string MediaType => "application/json";

        public override TranslateArgs TranslationArgs { get; protected set; }

        protected override string UrlBase => "https://www.bing.com/ttranslatev3?isVertical=1&&IG=F0EBBD7A827A44AAA8BFAE165F78444B&IID=translator.5025.1";

        protected override HttpResponseMessage GetResponseMessage(HttpClient httpClient)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("fromLang", TranslationArgs.IsoFrom);
            dict.Add("text", TranslationArgs.Text);
            dict.Add("to", TranslationArgs.IsoTo);

            var req = new HttpRequestMessage(HttpMethod.Post, httpClient.BaseAddress) { Content = new FormUrlEncodedContent(dict) };

            var task = httpClient.SendAsync(req);
            task.Wait();
            return task.Result;
        }

        protected override string GetTranslatedText(string response)
        {
            using (var stringReader = new StringReader(response))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var result = new JsonSerializer().Deserialize<BingResult[]>(jsonReader)?.FirstOrDefault();

                if (result != null && result.Translations != null)
                    return CleanUp(result.Translations?.FirstOrDefault().Text);
            }
            return default;
        }

        private string CleanUp(string text)
        {
            return text?.Replace(Environment.NewLine, "<br />");
        }

        private class BingResult
        {
            [JsonProperty(PropertyName = "detectedLanguage")]
            public DetectedLanguage DetectedLanguage { get; set; }

            [JsonProperty(PropertyName = "translations")]
            public Translation[] Translations { get; set; }
        }

        private class DetectedLanguage
        {
            [JsonProperty(PropertyName = "language")]
            public string Language { get; set; }

            [JsonProperty(PropertyName = "score")]
            public decimal Score { get; set; }
        }

        private class Translation
        {
            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }
        }
    }
}