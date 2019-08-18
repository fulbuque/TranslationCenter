using Newtonsoft.Json;
using TranslationCenter.Services.Country.Types.Interfaces;

namespace TranslationCenter.Services.Country.Types
{
    internal class Language : ILanguage
    {
        public string Iso => Iso639_1;

        [JsonProperty(PropertyName = "iso639_1")]
        public string Iso639_1 { get; internal set; }

        [JsonProperty(PropertyName = "iso639_2")]
        public string Iso639_2 { get; internal set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "nativeName")]
        public string NativeName { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj is Language language)
                return Iso == language.Iso;
            return false;
        }

        public override int GetHashCode()
        {
            return (Iso ?? string.Empty).GetHashCode();
        }
    }
}