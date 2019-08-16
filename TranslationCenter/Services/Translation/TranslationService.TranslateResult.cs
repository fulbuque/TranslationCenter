using WpfTranslator.Services.Translation.Enums;
using WpfTranslator.Services.Translation.Interfaces;

namespace WpfTranslator.Services.Translation
{
    public partial class TranslationService
    {
        private class TranslateResult : ITranslateResult
        {
            public TranslateResult(ResultTypes type, string result)
            {
                Type = type;
                Result = result;
            }

            public ResultTypes Type { get; }

            public string Result { get; }
        }
    }
}
