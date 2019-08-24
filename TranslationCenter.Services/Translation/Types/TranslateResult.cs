using System;
using TranslationCenter.Services.Translation.Engines;

namespace TranslationCenter.Services.Translation.Types
{
    internal class TranslateResult : ITranslateResult
    {

        private Func<string> _renderAction;

        //public TranslateResult(TranslateEngine source, string result)
        //{
        //    Source = source;
        //    Result = result;
        //}

        public TranslateResult(TranslateEngine source, Func<string> renderAction)
        {
            Source = source;
            _renderAction = renderAction;
        }

        public string Result { get; private set; }

        public TranslateEngine Source { get; }

        public bool IsRendered { get; private set; }

        public void Render()
        {
            if (!IsRendered)
            {
                Result = _renderAction?.Invoke();
                IsRendered = true;
            }
        }
    }
}