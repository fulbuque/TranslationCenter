using System.Collections.Generic;
using static TranslationCenter.TranslateWindowModel;

namespace TranslationCenter
{
    public class Country
    {
        public string Name { get; set; }

        public Language[] Languages { get; set; }

        public Dictionary<string, string> Translations { get; set; }
    }
}