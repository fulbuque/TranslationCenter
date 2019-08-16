using System;
using System.Collections.Generic;
using static WpfTranslator.TranslateWindowModel;

namespace WpfTranslator
{
    public class Country
    {
        public string Name { get; set; }

        public Language[] Languages { get; set; }

        public Dictionary<string, string> Translations { get; set; }
    }
}
