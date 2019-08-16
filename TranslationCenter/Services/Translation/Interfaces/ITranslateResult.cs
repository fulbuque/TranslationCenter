using WpfTranslator.Services.Translation.Enums;

namespace WpfTranslator.Services.Translation.Interfaces
{
    public interface ITranslateResult
    {
        ITranslateEngine Source { get; }

        string Result { get; }
    }
}