using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Interfaces;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation
{
    public partial class TranslationService
    {
        private Dictionary<Type, TranslateEngine> _engines = new Dictionary<Type, TranslateEngine>();

        public void AddEngine<EngineType>() where EngineType : TranslateEngine
        {
            var @type = typeof(EngineType);
            if (!_engines.ContainsKey(@type))
                _engines[@type] = Activator.CreateInstance<EngineType>();
        }

        public void RemoveEngine<EngineType>() where EngineType : TranslateEngine
        {
            _engines.Remove(typeof(EngineType));
        }

        public IEnumerable<ITranslateResult> Translate(TranslateArgs translateArgs)
        {
            List<ITranslateResult> translateResults = new List<ITranslateResult>();

            if (!string.IsNullOrEmpty(translateArgs.Text))
            {
                foreach (var engine in _engines)
                {
                    try
                    {
                        var result = engine.Value.GetTranslate(translateArgs);
                        translateResults.Add(result);
                    }
                    finally { }
                }
            }

            return translateResults;
        }
    }
}