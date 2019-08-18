using System.Collections.Generic;
using TranslationCenter.Services.Country.Types.Interfaces;

namespace TranslationCenter.Services.Country.Types
{
    public class LanguageComparer : IEqualityComparer<ILanguage>
    {
        public bool Equals(ILanguage x, ILanguage y)
        {
            return x.Iso == y.Iso;
        }

        public int GetHashCode(ILanguage obj)
        {
            return (obj.Iso ?? string.Empty).GetHashCode();
        }
    }
}