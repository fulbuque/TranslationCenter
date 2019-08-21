using System;
using System.Collections.Generic;
using System.Text;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    class FilterOptionItem<T> : IFilterOptionItem
    {
        public FilterOptionItem(string text, Func<T, bool> filter, bool isSelected = false, bool isDefault = false)
        {
            Text = text;
            Filter = filter;
            IsSelected = isSelected;
            IsDefault = isDefault;
        }

        public string Text { get; set; }
        public Func<T, bool> Filter { get; set; }
        public bool IsSelected { get; }
        public bool IsDefault { get; }

        public object GetFilter => Filter;
    }
}
