using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslateEngine
    {
        string Name { get;  }
        EngineTypes EngineType { get; }
        ResultTypes ResultType { get; }
    }
}