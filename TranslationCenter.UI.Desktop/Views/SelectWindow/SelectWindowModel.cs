using System;
using System.Collections.Generic;
using System.Linq;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal class SelectWindowModel<T> : ViewModelBase where T : class
    {
        private Dictionary<string, FilterOptionItem<T>> _filterOptions;
        private IEnumerable<SelectWindowItem> _items;
        private string _message;
        private IEnumerable<T> _selectedItems;
        private string _title;

        public SelectWindowModel()
        {
            _filterOptions = new Dictionary<string, FilterOptionItem<T>>();
        }

        private FilterOptionItem<T> _filterOptionSelected;
        public FilterOptionItem<T> FilterOptionSelected
        {
            get => _filterOptionSelected;
            set
            {
                _filterOptionSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FilteredItems));
            }
        }
        public class SelectWindowItem
        {

            private string _displayNameProperty;

            public SelectWindowItem(T data, string displayName)
            {
                Data = data;
                _displayNameProperty = displayName;
            }
            public T Data { get; }

            public string DisplayName
            {
                get
                {
                    var prop = Data.GetType().GetProperty(_displayNameProperty);
                    if (prop != null)
                        return prop.GetValue(Data)?.ToString();

                    return default;
                }
            }

        }

        public FilterOptionItem<T>[] FilterOptions=> _filterOptions.Values.ToArray();

        public bool HasFilterOptions => FilterOptions.Any();

        public IEnumerable<T> Items
        {
            get => _items.Select(i => i.Data);
            set
            {
                _items = value.Select(i => new SelectWindowItem(i, DisplayName));
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<SelectWindowItem> FilteredItems
        {
            get
            {
                if (FilterOptionSelected != null)
                    return _items.Where(i => FilterOptionSelected.Filter(i.Data));
                return _items;
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                NotifyPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public void AddFilterOption(string text, Func<T, bool> filter, bool isSelected = false)
        {
            _filterOptions[text] = new FilterOptionItem<T>(text, filter);
            if (isSelected)
                FilterOptionSelected = _filterOptions[text];
        }

        private string _displayName;

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                NotifyPropertyChanged();
            }
        }


    }
}