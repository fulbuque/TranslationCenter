using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Dictionary, "Dict.cc")]
    public class DictCCDictionaryEngine : TranslateEngine
    {
        public override TranslateArgs TranslationArgs { get; protected set; }

        protected override bool IsTranslateUnsupported => false;
        protected override string MediaType => "text/html";

        protected override string UrlBase => $"https://{TranslationArgs.IsoFrom}-{TranslationArgs.IsoTo}.dict.cc"; // https://pt-de.dict.cc/?s=celular

        protected override string UrlBaseAdditional =>
            $"/?s={HttpUtility.UrlEncode(TranslationArgs.Text)}";

        protected override HttpResponseMessage GetResponseMessage(HttpClient httpClient)
        {
            var task = httpClient.GetAsync("");
            task.Wait();
            return task.Result;
        }

        protected override string GetTranslatedText(string response)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(response?.Trim());
            return CleanUp(UrlBase, html);
        }

        private string CleanUp(string baseUrl, HtmlDocument document)
        {
            StringBuilder translatedText = new StringBuilder();

            var divResult = document.DocumentNode.SelectSingleNode("//div[@id='maincontent']");

            if (divResult != null)
            {
                var tables = divResult.Elements("table");
                if (tables.Count() >= 1)
                    divResult = tables.ElementAt(1);

                var links = document.DocumentNode.SelectNodes("//a");
                base.UpdateUrlElements(links, "href", baseUrl, (node) => node.SetAttributeValue("target", this.Category.ToString()));

                base.RemoveElements(divResult.SelectNodes("//script"));

                links = divResult.SelectNodes("//link");
                base.UpdateUrlElements(links, "href", baseUrl,
                    (node) =>
                    {
                        var href = node.Attributes["href"]?.Value ?? string.Empty;
                        if (href.Contains(".css"))
                            translatedText.AppendLine(node.OuterHtml);
                    });

                translatedText.AppendLine(base.GetContentWithHierarchy(divResult));
            }

            return translatedText.ToString();
        }
    }
}