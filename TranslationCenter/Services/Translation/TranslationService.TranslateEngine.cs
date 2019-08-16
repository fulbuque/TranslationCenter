﻿using System;
using System.Threading.Tasks;
using WpfTranslator.Services.Translation.Enums;
using WpfTranslator.Services.Translation.Interfaces;

namespace WpfTranslator.Services.Translation
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

            public string IsoFrom { get; }
            public string IsoTo { get; }
            public string Text { get; }

            public abstract string Name { get; }

            public abstract EngineTypes EngineType { get; }

            public abstract ResultTypes ResultType { get; }

            internal virtual async Task<ITranslateResult> GetTranslate()
            {
                return null;
            }

            internal static TranslateEngine GetEngine(EngineTypes translateEngineType, string isoFrom, string isoTo, string text) 
            {
                Type engineType;

                if (translateEngineType == EngineTypes.Bing)
                    engineType = typeof(BingTranslateEngine);
                else
                    throw new ArgumentOutOfRangeException("Invalid Translate Engine");

                return Activator.CreateInstance(engineType, new object[] { isoFrom, isoTo, text }) as TranslateEngine;
            }
        }
    }
}
