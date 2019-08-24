using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Web;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Dictionary, "Collins")]
    public class CollinsDictionaryEngine : TranslateEngine
    {
        public override TranslateArgs TranslationArgs { get; protected set; }

        protected override bool IsTranslateUnsupported => false;

        protected override string MediaType => "text/html";

        protected override string UrlBase => "https://www.collinsdictionary.com"; // https://www.collinsdictionary.com/dictionary/german-english/schliessen

        protected override string UrlBaseAdditional =>
            $"/dictionary/{TranslationArgs.LanguageFrom.Name.ToLower()}-{TranslationArgs.LanguageTo.Name.ToLower()}/{HttpUtility.UrlEncode(TranslationArgs.Text)}";

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

            var divResult = document.DocumentNode.SelectSingleNode("//div[@class='page']");
            //var divResult = document.DocumentNode.SelectSingleNode("//div[@class='cB cB-def dictionary biling']");

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
                        if (href.Contains(".css"))
                            translatedText.AppendLine(node.OuterHtml);
                    });

                translatedText.AppendLine(base.GetContentWithHierarchy(divResult));
            }

            return translatedText.ToString();
        }
    }
}