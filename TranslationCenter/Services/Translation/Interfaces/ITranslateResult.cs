namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslateResult
    {
        ITranslateEngine Source { get; }

        string Result { get; }
    }
}