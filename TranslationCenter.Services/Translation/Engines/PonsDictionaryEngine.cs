using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Web;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Dictionary, "Pons")]
    public class PonsDictionaryEngine : TranslateEngine
    {
        protected override bool IsTranslateUnsupported => false;
        protected override string MediaType => "text/html";

        protected override string UrlBase => "https://en.pons.com"; // https://en.pons.com/translate?q=car&l=deen&in=&lf=en&qnac=

        protected override string UrlBaseAdditional =>
            $"/translate?q={HttpUtility.UrlEncode(TranslationArgs.Text)}&l={TranslationArgs.IsoFrom}{TranslationArgs.IsoTo}&in=&lf=en&qnac=";
        public override TranslateArgs TranslationArgs { get; protected set; }

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

            var divResult = document.DocumentNode.SelectSingleNode("//div[@id='results-tab-dict']");

            if (divResult != null)
            {
                var links = document.DocumentNode.SelectNodes("//a");
                base.UpdateUrlElements(links, "href", baseUrl, (node) => node.SetAttributeValue("target", this.Category.ToString()));

                base.RemoveElements(divResult.SelectNodes("//script"));

                links = divResult.SelectNodes("//link");
                base.UpdateUrlElements(links, "href", baseUrl,
                    (node) =>
                    {
                        var href = node.Attributes["href"]?.Value ?? string.Empty;
                        if (href.EndsWith(".css"))
                            translatedText.AppendLine(node.OuterHtml);
                    });

                translatedText.AppendLine(divResult?.InnerHtml);
            }

            return translatedText.ToString();
        }
    }
}