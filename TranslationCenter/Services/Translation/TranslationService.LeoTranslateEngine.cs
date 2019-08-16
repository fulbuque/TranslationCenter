using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Extensions;
using TranslationCenter.Services.Translation.Interfaces;

namespace TranslationCenter.Services.Translation
{
    public partial class TranslationService
    {
        private class LeoTranslateEngine : TranslateEngine
        {
            private string[] supportedLanguages = new string[] {"de", "pl", "pt", "ru", "ch", "it", "es", "fr", "en"};

            public override string Name => this.GetType().Name;

            public override EngineTypes EngineType => EngineTypes.Leo;

            public override ResultTypes ResultType => ResultTypes.Html;

            public LeoTranslateEngine(string isoFrom, string isoTo, string text) : base(isoFrom, isoTo, text) { }

            internal override async Task<ITranslateResult> GetTranslate()
            {
                var translatedText = await GetTranslateInternal();
                return new TranslateResult(this, translatedText);
            }

            private async Task<string> GetTranslateInternal()
            {
                string translatedText = string.Empty;

                if (!supportedLanguages.Contains(IsoFrom) || !supportedLanguages.Contains(IsoTo))
                    return $"Translation not Suported for [{ IsoFrom }-{ IsoTo }]!";

                var baseUrl = "https://dict.leo.org/";

                var urlDicitionary = string.Empty;
                if (IsoFrom == "pl" || IsoTo == "pl")
                    urlDicitionary = "polnisch-deutsch/";
                else if (IsoFrom == "pt" || IsoTo == "pt")
                    urlDicitionary = "portugiesisch-deutsch/";
                else if (IsoFrom == "ru" || IsoTo == "ru")
                    urlDicitionary = "russisch-deutsch/";
                else if (IsoFrom == "ch" || IsoTo == "ch")
                    urlDicitionary = "chinesisch-deutsch/";
                else if (IsoFrom == "it" || IsoTo == "it")
                    urlDicitionary = "italienisch-deutsch/";
                else if (IsoFrom == "es" || IsoTo == "es")
                    urlDicitionary = "spanisch-deutsch/";
                else if (IsoFrom == "fr" || IsoTo == "fr")
                    urlDicitionary = "französisch-deutsch/";
                else if (IsoFrom == "en" || IsoTo == "en")
                    urlDicitionary = "englisch-deutsch/";

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(baseUrl + urlDicitionary);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                    var response = await client.GetAsync(HttpUtility.UrlEncode(Text));

                    if (response.IsSuccessStatusCode)
                    {
                        var responseText = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var document = XDocument.Parse(responseText);

                            var resultElement = document.Root
                                                    .Element("body")
                                                    ?.GetElement("id", "mainContent")
                                                    ?.GetElement("id", "centerColumn")
                                                    ?.GetElement("data-dz-search", "result");

                            if (resultElement != null)
                            {
                                foreach (var link in resultElement.Descendants("a"))
                                {
                                    var href = link.Attribute("href")?.Value;
                                    if (!string.IsNullOrEmpty(href) && !href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                                        link.SetAttributeValue("href", $"{baseUrl}{href}");
                                    link.SetAttributeValue("target", this.EngineType);
                                }

                                var toRemove = resultElement.Elements().ToList();
                                foreach (var element in toRemove)
                                {
                                    var data_dz_name = element.Attribute("data-dz-name");
                                    if (data_dz_name == null)
                                        element.Remove();
                                }

                                translatedText = resultElement.ToString();
                            }
                            else
                            {
                                using (var reader = document.Root.Element("body").CreateReader())
                                {
                                    reader.MoveToContent();
                                    translatedText = reader.ReadInnerXml();
                                }
                            }

                            //var result = new JavaScriptSerializer().Deserialize<XDocument>(responseText);
                            ////if (result != null && result.Translations != null)
                            ////    translatedText = result.Translations?.FirstOrDefault().Text;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                return translatedText;
            }


        }
    }
}
