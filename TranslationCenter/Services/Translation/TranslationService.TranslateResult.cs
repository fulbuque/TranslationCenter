using TranslationCenter.Services.Translation.Interfaces;

namespace TranslationCenter.Services.Translation
{
    public partial class TranslationService
    {
        private class TranslateResult : ITranslateResult
        {
            public TranslateResult(ITranslateEngine source, string result)
            {
                Source = source;
                Result = result;
            }

            public ITranslateEngine Source { get; }

            public string Result { get; }
        }
    }
}