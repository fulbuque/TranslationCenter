using TranslationCenter.Services.Translation.Engines;

namespace TranslationCenter.Services.Translation.Types
{
    public interface ITranslateResult
    {
        string Result { get; }
        TranslateEngine Source { get; }
    }
}