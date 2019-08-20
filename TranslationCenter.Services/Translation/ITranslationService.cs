using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation
{
    public interface ITranslationService
    {
        void AddEngine<EngineType>() where EngineType : Engines.TranslateEngine;

        void ClearEngine<EngineType>() where EngineType : Engines.TranslateEngine;

        Task<IEnumerable<ITranslateResult>> Translate(string isoFrom, string isoTo, string text, params EngineCategory[] translateEnginesTypes);
    }
}