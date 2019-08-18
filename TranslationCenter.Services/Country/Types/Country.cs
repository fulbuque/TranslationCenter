using Newtonsoft.Json;
using System.Collections.Generic;
using TranslationCenter.Services.Country.Types.Interfaces;

namespace TranslationCenter.Services.Country.Types
{
    internal class Country : ICountry
    {
        public ILanguage[] Languages => LanguagesInternal;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "translations")]
        public Dictionary<string, string> Translations { get; internal set; }

        [JsonProperty(PropertyName = "languages")]
        internal Language[] LanguagesInternal { get; set; }
    }
}