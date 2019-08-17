using System;
using System.Threading.Tasks;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Extensions;
using TranslationCenter.Services.Translation.Interfaces;

namespace TranslationCenter.Services.Translation
{
    public partial class TranslationService
    {
        private abstract class TranslateEngine : ITranslateEngine
        {
            public TranslateEngine(string isoFrom, string isoTo, string text)
            {
                IsoFrom = isoFrom;
                IsoTo = isoTo;
                Text = text;
            }

            public string DisplayName => EngineType.GetDescription();
            public abstract EngineTypes EngineType { get; }
            public string IsoFrom { get; }
            public string IsoTo { get; }
            public abstract string Name { get; }
            public abstract ResultTypes ResultType { get; }
            public string Text { get; }

            internal static TranslateEngine GetEngine(EngineTypes translateEngineType, string isoFrom, string isoTo, string text)
            {
                Type engineType;

                if (translateEngineType == EngineTypes.Bing)
                    engineType = typeof(BingTranslateEngine);
                else if (translateEngineType == EngineTypes.Leo)
                    engineType = typeof(LeoTranslateEngine);
                else
                    throw new ArgumentOutOfRangeException("Invalid Translate Engine");

                return Activator.CreateInstance(engineType, new object[] { isoFrom, isoTo, text }) as TranslateEngine;
            }

            internal virtual async Task<ITranslateResult> GetTranslate()
            {
                return null;
            }
        }
    }
}