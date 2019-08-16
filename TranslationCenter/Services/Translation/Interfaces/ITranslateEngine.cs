using WpfTranslator.Services.Translation.Enums;

namespace WpfTranslator.Services.Translation.Interfaces
{
    public interface ITranslateEngine
    {
        string Name { get;  }
        EngineTypes EngineType { get; }
        ResultTypes ResultType { get; }
    }
}