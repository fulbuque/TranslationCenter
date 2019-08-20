using System;
using System.Collections.Generic;
using System.Text;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    class FilterOptionItem<T> : IFilterOptionItem
    {
        public FilterOptionItem(string text, Func<T, bool> filter)
        {
            Text = text;
            Filter = filter;
        }

        public string Text { get; set; }
        public Func<T, bool> Filter { get; set; }

        public object GetFilter => Filter;
    }
}
