using System.Collections.Generic;

namespace TranslationCenter
{
    public partial class TranslateWindowModel
    {
        public class LanguageComparer : IEqualityComparer<Language>
        {
            public bool Equals(Language x, Language y)
            {
                return x.Iso == y.Iso;
            }

            public int GetHashCode(Language obj)
            {
                return (obj.Iso ?? string.Empty).GetHashCode();
            }
        }
    }
}