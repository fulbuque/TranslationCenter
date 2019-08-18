using System.Collections.Generic;

namespace TranslationCenter.Services.Country.Types.Interfaces
{
    public interface ICountry
    {
        ILanguage[] Languages { get; }
        string Name { get; }
        Dictionary<string, string> Translations { get; }
    }
}