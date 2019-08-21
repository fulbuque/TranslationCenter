namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal interface ISelectWindowModel
    {
        System.Func<System.Collections.IList, bool> ValidateSelectedItems { get; }

        void SetSelectedItems(System.Collections.IList list);
    }
}