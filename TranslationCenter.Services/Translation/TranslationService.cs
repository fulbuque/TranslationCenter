using System;
using System.Collections.Generic;
using System.Linq;
using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Interfaces;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Translation
{
    public partial class TranslationService
    {
        private Dictionary<Type, TranslateEngine> _engines = new Dictionary<Type, TranslateEngine>();

        public static AvaliableEngine[] GetAvaliableEngines()
        {
            var type = typeof(TranslateEngine);
            var assemblyName = type.Assembly.GetName().Name;

            var avaliableEngines = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(s => s.GetName().Name == assemblyName)
                                    .SelectMany(s => s.DefinedTypes)
                                    .Where(p => type.IsAssignableFrom(p))
                                    .Where(i => i.Name != type.Name)
                                    .Select(i => new AvaliableEngine(i))
                                    .ToArray();

            return avaliableEngines;
        }

        public void AddEngine<EngineType>() where EngineType : TranslateEngine => AddEngineInternal(typeof(EngineType));

        public void AddEngine(AvaliableEngine avaliableEngine) => AddEngineInternal(avaliableEngine.Type);

        public void RemoveEngine<EngineType>() where EngineType : TranslateEngine => RemoveEngineInternal(typeof(EngineType));

        public void RemoveEngine(AvaliableEngine avaliableEngine) => RemoveEngineInternal(avaliableEngine.Type);

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

        private void AddEngineInternal(Type type)
        {
            if (!_engines.ContainsKey(type))
                _engines[type] = (TranslateEngine)Activator.CreateInstance(type);
        }
        private void RemoveEngineInternal(Type type) => _engines.Remove(type);
    }
}