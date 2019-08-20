using System;
using System.Collections.Generic;
using System.Linq;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal class SelectWindowModel<T> : ViewModelBase
    {
        private Dictionary<string, FilterOptionItem<T>> _filterOptions;
        private IEnumerable<T> _items;
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

        public IEnumerable<FilterOptionItem<T>> FilterOptions=> _filterOptions.Values;

        public IEnumerable<T> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<T> FilteredItems
        {
            get
            {
                if (FilterOptionSelected != null)
                    return Items.Where(FilterOptionSelected?.Filter);
                return Items;
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

        private string _displayName = "DisplayName";

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