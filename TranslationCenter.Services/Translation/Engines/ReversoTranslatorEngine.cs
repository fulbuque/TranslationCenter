using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        protected override string MediaType => "text/html";

        protected override string UrlBase => "https://dictionary.reverso.net/"; // https://dictionary.reverso.net/english-german/car

        protected override string UrlBaseAdditional => 
            $"{TranslationArgs.LanguageFrom.Name.Trim().ToLower() }-{TranslationArgs.LanguageTo.Name.Trim().ToLower()}/";

        protected override bool IsTranslateUnsupported => false;

        protected override HttpResponseMessage GetResponseMessage(HttpClient httpClient)
        {
            var task = httpClient.GetAsync(HttpUtility.UrlEncode(TranslationArgs.Text));
            task.Wait();
            return task.Result;
        }

        protected override string GetTranslatedText(string response)
        {
            //return response?.Trim();
            // html.DocumentNode.ChildNodes[2].ChildNodes[3].
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(response?.Trim());
            return CleanUp(UrlBase, html);
        }

        private string CleanUp(string baseUrl, HtmlDocument document)
        {
            string translatedText = string.Empty;
            var links = document.DocumentNode.SelectNodes("//a").ToList();
            if (links != null)
            {
                foreach (var link in links)
                {
                    var href = link.Attributes["href"];
                    if (href != null && !(href.Value ?? string.Empty).StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        link.SetAttributeValue("href", $"{baseUrl}{href.Value}");

                    link.SetAttributeValue("target", this.Category.ToString());
                }
            }

            links = document.DocumentNode.SelectNodes("//link").ToList();
            if (links != null)
            {
                foreach (var link in links)
                {
                    var href = link.Attributes["href"];
                    if (href != null && !(href.Value ?? string.Empty).StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        link.SetAttributeValue("href", $"{baseUrl}{href.Value}");
                }
            }

            translatedText = document.DocumentNode.OuterHtml;

            //var body = document.DocumentNode.ChildNodes["html"]?.ChildNodes["body"];

            //var scripts = body.Descendants("script").ToList();
            //foreach (var script in scripts)
            //{
            //    script.Remove();
            //}

            //var links = body.Descendants("link").ToList();

            //var divResult = document.DocumentNode.ChildNodes["html"]
            //                    ?.ChildNodes["body"]
            //                    ?.Descendants()
            //                    ?.FirstOrDefault(n => n.Name == "div" && n.Attributes["class"]?.Value == "results-found");


            //scripts = divResult.SelectNodes("//script").ToList();
            //foreach (var script in scripts)
            //{
            //    script.Remove();
            //}

            //if (divResult != null)
            //{
            //    var listToRemove = new List<(string nodeName, string attr, string value)>();
            //    listToRemove.Add(("div", "id", "ctl00_cC_tblCMHelp"));
            //    listToRemove.Add(("div", "id", "g_menu"));
            //    listToRemove.Add(("fieldset", "id", "contextSection"));
            //    listToRemove.Add(("div", "class", "options-list"));
            //    listToRemove.Add(("div", "id", "VocabLoginDiag1"));
            //    listToRemove.Add(("div", "id", "VocabLoginDiag2"));
            //    listToRemove.Add(("div", "id", "dialog-modal2"));
            //    listToRemove.Add(("div", "id", "dialog-modal1"));
            //    listToRemove.Add(("div", "id", "ctl00_cC_ucResEM_opossiteEntries"));
            //    listToRemove.Add(("div", "id", "ctl00_cC_ucResPM_addCommLine"));
            //    listToRemove.Add(("div", "id", "ctl00_cC_ucResPM_p1"));
            //    listToRemove.Add(("div", "class", "logocorner"));
            //    foreach (var itemToRemove in listToRemove)
            //    {
            //        var res = divResult.SelectNodes($"//{itemToRemove.nodeName}[@{itemToRemove.attr}='{itemToRemove.value}']");
            //        if (res == null) continue;
            //        foreach (var r in res)
            //            r.Remove();
            //    }

            //    translatedText = divResult?.InnerHtml;
            //}


            //if (font )
            //var selectedNodes = divResult.Descendants().Where(n => n.Attributes["id"]?.Value == "ctl00_cC_translate_box").ToList();

            //foreach (var node in selectedNodes)
            //{
            //    translatedText += node.OuterHtml;
            //}



            // id="ctl00_cC_translate_box"
            // id="ctl00_cC_ucResPM_divEM"

            //var resultElement = document.Root
            //   .Element("body")
            //   ?.GetElement("class", "results-found");

            //if (resultElement != null)
            //{
            //    foreach (var link in resultElement.Descendants("a"))
            //    {
            //        var href = link.Attribute("href")?.Value;
            //        if (!string.IsNullOrEmpty(href) && !href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            //            link.SetAttributeValue("href", $"{baseUrl}{href}");
            //        link.SetAttributeValue("target", this.Category);
            //    }

            //    var toRemove = resultElement.Elements().ToList();
            //    foreach (var element in toRemove)
            //    {
            //        var data_dz_name = element.Attribute("data-dz-name");
            //        if (data_dz_name == null)
            //            element.Remove();
            //    }

            //    translatedText = resultElement.ToString();
            //}
            //else
            //{
            //    using (var reader = document.Root.Element("body").CreateReader())
            //    {
            //        reader.MoveToContent();
            //        translatedText = reader.ReadInnerXml();
            //    }
            //}

            return translatedText;
        }
    }
}