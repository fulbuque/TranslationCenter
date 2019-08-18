using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslateEngine
    {
        string Name { get; }
        string DisplayName { get; }
        EngineTypes EngineType { get; }
        ResultTypes ResultType { get; }
    }
}