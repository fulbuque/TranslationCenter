using System;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    interface IFilterOptionItem
    {
        object GetFilter { get;  }
        string Text { get; }
    }
}