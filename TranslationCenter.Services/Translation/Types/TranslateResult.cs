using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Interfaces;

namespace TranslationCenter.Services.Translation.Types
{
    internal class TranslateResult : ITranslateResult
    {
        public TranslateResult(TranslateEngine source, string result)
        {
            Source = source;
            Result = result;
        }

        public string Result { get; }
        public TranslateEngine Source { get; }
    }
}