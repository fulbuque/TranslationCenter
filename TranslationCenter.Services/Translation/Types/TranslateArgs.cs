namespace TranslationCenter.Services.Translation.Types
{
    public sealed class TranslateArgs
    {
        public TranslateArgs(string isoFrom, string isoTo, string text)
        {
            IsoFrom = isoFrom;
            IsoTo = isoTo;
            Text = text;
        }

        public string IsoFrom { get; }
        public string IsoTo { get; }
        public string Text { get; }
    }
}