using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter.Services.Translation.Interfaces
{
    public interface ITranslationService
    {
        void AddEngine<EngineType>() where EngineType : Engines.TranslateEngine;
        void ClearEngine<EngineType>() where EngineType : Engines.TranslateEngine;
        Task<IEnumerable<ITranslateResult>> Translate(string isoFrom, string isoTo, string text, params EngineTypes[] translateEnginesTypes);
    }
}