using TranslationCenter.Services.Translation.Engines;

namespace TranslationCenter.Services.Translation.Types
{
    public interface ITranslateResult
    {
        bool IsRendered { get; }

        string Result { get; }

        TranslateEngine Source { get; }

        void Render();
    }
}