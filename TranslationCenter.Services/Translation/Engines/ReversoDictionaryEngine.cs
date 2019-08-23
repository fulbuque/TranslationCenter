using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation.Engines
{
    [EngineInfo(EngineCategory.Dictionary, "Reverso")]
    public class ReversoDictionaryEngine : TranslateEngine
    {
        protected override bool IsTranslateUnsupported => false;
        protected override string MediaType => "text/html";

        protected override string UrlBase => "https://dictionary.reverso.net/"; // https://dictionary.reverso.net/english-german/car

        protected override string UrlBaseAdditional =>
            $"{TranslationArgs.LanguageFrom.Name.Trim().ToLower() }-{TranslationArgs.LanguageTo.Name.Trim().ToLower()}/";

        protected override HttpResponseMessage GetResponseMessage(HttpClient httpClient)
        {
            var task = httpClient.GetAsync(HttpUtility.UrlEncode(TranslationArgs.Text));
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
            string translatedText = string.Empty;

            var divResult = document.DocumentNode.SelectSingleNode("//div[@class='results-found']");

            if (divResult != null)
            {
                var tableHtmlResultFake = divResult.SelectNodes("//div[@id='TableHTMLResult']");
                if (tableHtmlResultFake.Count > 1)
                    tableHtmlResultFake[1].Remove();

                var links = document.DocumentNode.SelectNodes("//a");
                base.UpdateUrlElements(links, "href", baseUrl, (node) => node.SetAttributeValue("target", this.Category.ToString()));

                base.RemoveElements(divResult.SelectNodes("//script"));

                var markedToRemove = new List<(string nodeName, string attr, string value)>();
                markedToRemove.Add(("div", "id", "ctl00_cC_tblCMHelp"));
                markedToRemove.Add(("div", "id", "g_menu"));
                markedToRemove.Add(("fieldset", "id", "contextSection"));
                markedToRemove.Add(("div", "class", "options-list"));
                markedToRemove.Add(("div", "id", "VocabLoginDiag1"));
                markedToRemove.Add(("div", "id", "VocabLoginDiag2"));
                markedToRemove.Add(("div", "id", "dialog-modal2"));
                markedToRemove.Add(("div", "id", "dialog-modal1"));
                markedToRemove.Add(("div", "id", "ctl00_cC_ucResEM_opossiteEntries"));
                markedToRemove.Add(("div", "id", "ctl00_cC_ucResPM_addCommLine"));
                markedToRemove.Add(("div", "id", "ctl00_cC_ucResPM_p1"));
                markedToRemove.Add(("div", "class", "logocorner"));

                foreach (var itemToRemove in markedToRemove)
                {
                    var nodes = divResult.SelectNodes($"//{itemToRemove.nodeName}[@{itemToRemove.attr}='{itemToRemove.value}']");
                    if (nodes == null) continue;
                    foreach (var node in nodes)
                        node.Remove();
                }

                translatedText = divResult?.InnerHtml;
            }

            return translatedText;
        }
    }
}