using System;
using System.Collections.Generic;
using System.Text;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    class FilterOptionItem<T>
    {
        public FilterOptionItem(string text, Func<T, bool> filter, bool isSelected)
        {
            Text = text;
            Filter = filter;
            IsSelected = isSelected;
        }

        public string Text { get; set; }
        public Func<T, bool> Filter { get; set; }
        public bool IsSelected { get; set; }

    }
}
