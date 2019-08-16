using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfTranslator.Services.Translation.Enums;
using WpfTranslator.Services.Translation.Interfaces;

namespace WpfTranslator.Services.Translation
{
    public partial class TranslationService : ITranslationService
    {
        private static ITranslationService _instance;

        public static ITranslationService Instance => _instance ?? Initialize();

        private static ITranslationService Initialize()
        {
            if (_instance != null)
                return _instance;

            _instance = new TranslationService();
            return _instance;
        }

        public async Task<IEnumerable<ITranslateResult>> Translate(string isoFrom, string isoTo, string text, params TranslateEnginesTypes[] translateEnginesTypes)
        {
            List<ITranslateResult> translateResults = new List<ITranslateResult>();

            if (!string.IsNullOrEmpty(text))
            {
                if (!translateEnginesTypes?.Any() ?? false)
                    translateEnginesTypes = new TranslateEnginesTypes[] { TranslateEnginesTypes.Bing };

                foreach (var translateEngineType in translateEnginesTypes)
                {
                    var engine = TranslateEngine.GetEngine(translateEngineType, isoFrom, isoTo, text);
                    try
                    {
                        var result = await engine.GetTranslate();
                        translateResults.Add(result);
                    }
                    finally { }
                }
            }

            return translateResults;
        }
    }
}
