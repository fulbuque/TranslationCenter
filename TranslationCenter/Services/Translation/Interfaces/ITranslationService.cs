using System.Collections.Generic;
using System.Threading.Tasks;
using WpfTranslator.Services.Translation.Enums;

namespace WpfTranslator.Services.Translation.Interfaces
{
    public interface ITranslationService
    {
        Task<IEnumerable<ITranslateResult>> Translate(string isoFrom, string isoTo, string text, params TranslateEnginesTypes[] translateEnginesTypes);
    }
}