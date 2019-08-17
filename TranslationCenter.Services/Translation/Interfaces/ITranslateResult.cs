using TranslationCenter.Services.Translation.Engines;

namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslateResult
    {
        string Result { get; }
        TranslateEngine Source { get; }
    }
}