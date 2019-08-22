using TranslationCenter.Services.Country;
using TranslationCenter.Services.Country.Types.Interfaces;

namespace TranslationCenter.Services.Translation.Types
{
    public sealed class TranslateArgs
    {
        public TranslateArgs(string isoFrom, string isoTo, string text)
        {
            IsoFrom = isoFrom;
            IsoTo = isoTo;
            LanguageFrom = CountryService.GetLanguage(isoFrom);
            LanguageTo = CountryService.GetLanguage(isoTo);
            Text = text;
        }

        public string IsoFrom { get; }
        public string IsoTo { get; }

        public ILanguage LanguageFrom { get; }
        public ILanguage LanguageTo { get; }

        public string Text { get; }
    }
}