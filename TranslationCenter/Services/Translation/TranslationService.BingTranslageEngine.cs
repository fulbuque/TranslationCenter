using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WpfTranslator.Services.Translation.Interfaces;

namespace WpfTranslator.Services.Translation
{
    public partial class TranslationService
    {
        private class BingTranslageEngine : TranslateEngine
        {
            public BingTranslageEngine(string isoFrom, string isoTo, string text) : base(isoFrom, isoTo, text) { }

            internal override async Task<ITranslateResult> GetTranslate()
            {
                var translatedText = await GetTranslateInternal();
                return new TranslateResult(Enums.ResultTypes.PlainText, translatedText);
            }

            private class Translation
            {
                public string Text { get; set; }
            }

            private class TranslateResultBing
            {
                public DetectedLanguage DetectedLanguage { get; set; }
                public Translation[] Translations { get; set; }
            }

            private async Task<string> GetTranslateInternal()
            {
                string translatedText = string.Empty;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://www.bing.com/ttranslatev3?isVertical=1&IG=504D0D4AC9C1430B92775346964CDE30&IID=translator.5026.4");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var dict = new Dictionary<string, string>();
                    dict.Add("fromLang", IsoFrom );
                    dict.Add("text", Text);
                    dict.Add("to", IsoTo);

                    var req = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress) { Content = new FormUrlEncodedContent(dict) };

                    var response = await client.SendAsync(req);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseText = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var result = new JavaScriptSerializer().Deserialize<TranslateResultBing[]>(responseText)?.FirstOrDefault();
                            if (result != null && result.Translations != null)
                                translatedText = result.Translations?.FirstOrDefault().Text;
                        }
                        catch (Exception ex) {
                            throw ex;
                        }
                    }
                }
                return translatedText;
            }


        }
    }
}
