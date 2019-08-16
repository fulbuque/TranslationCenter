using WpfTranslator.Services.Translation.Enums;

namespace WpfTranslator.Services.Translation.Interfaces
{
    public interface ITranslateResult
    {
        ResultTypes Type { get; }

        string Result { get; }
    }
}