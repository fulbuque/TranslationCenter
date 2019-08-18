namespace TranslationCenter.Services.Country.Types.Interfaces
{
    public interface ILanguage
    {
        string Iso { get; }
        string Iso639_1 { get; }
        string Iso639_2 { get; }
        string Name { get; }
        string NativeName { get; }

        bool Equals(object obj);

        int GetHashCode();
    }
}