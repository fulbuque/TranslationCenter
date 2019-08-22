using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Xml.Linq;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Extensions;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Dictionary, "Reverso")]
    public class ReversoDictionaryEngine : TranslateEngine
    {
        private string[] supportedLanguages = new string[] { "ar",  "de", "pl", "pt", "ru", "ch", "it", "es", "fr", "en" };

        protected override string MediaType => "text/html";

        protected override string UrlBase => "https://dict.leo.org/";

        protected override string UrlBaseAdditional
        {
            get
            {
                var isoFrom = TranslationArgs.IsoFrom;
                var isoTo = TranslationArgs.IsoTo;

                if (isoFrom != "de" && isoTo != "de")
                    return string.Empty;

                if (isoFrom == "pl" || isoTo == "pl")
                    return "polnisch-deutsch/";
                else if (isoFrom == "pt" || isoTo == "pt")
                    return "portugiesisch-deutsch/";
                else if (isoFrom == "ru" || isoTo == "ru")
                    return "russisch-deutsch/";
                else if (isoFrom == "ch" || isoTo == "ch")
                    return "chinesisch-deutsch/";
                else if (isoFrom == "it" || isoTo == "it")
                    return "italienisch-deutsch/";
                else if (isoFrom == "es" || isoTo == "es")
                    return "spanisch-deutsch/";
                else if (isoFrom == "fr" || isoTo == "fr")
                    return "französisch-deutsch/";
                else if (isoFrom == "en" || isoTo == "en")
                    return "englisch-deutsch/";

                return string.Empty;
            }
        }

        protected override bool IsTranslateUnsupported => (TranslationArgs.IsoFrom != "de"
                                                             && TranslationArgs.IsoTo != "de")
                                                             || !supportedLanguages.Contains(TranslationArgs.IsoFrom)
                                                             || !supportedLanguages.Contains(TranslationArgs.IsoTo);

        protected override HttpResponseMessage GetResponseMessage(HttpClient httpClient)
        {
            var task = httpClient.GetAsync(HttpUtility.UrlEncode(TranslationArgs.Text));
            task.Wait();
            return task.Result;
        }

        protected override string GetTranslatedText(string response)
        {
            var document = XDocument.Parse(response);
            return CleanUp(UrlBase, document);
        }

        private string CleanUp(string baseUrl, XDocument document)
        {
            string translatedText;
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
                    link.SetAttributeValue("target", this.Category);
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

            return translatedText;
        }
    }
}