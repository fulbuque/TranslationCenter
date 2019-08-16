using WpfTranslator.Services.Translation.Enums;
using WpfTranslator.Services.Translation.Interfaces;

namespace WpfTranslator.Services.Translation
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
