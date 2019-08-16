using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslationService
    {
        Task<IEnumerable<ITranslateResult>> Translate(string isoFrom, string isoTo, string text, params EngineTypes[] translateEnginesTypes);
    }
}