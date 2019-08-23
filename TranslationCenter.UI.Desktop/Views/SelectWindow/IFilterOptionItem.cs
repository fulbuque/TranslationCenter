namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal interface IFilterOptionItem
    {
        object GetFilter { get; }
        string Text { get; }
    }
}