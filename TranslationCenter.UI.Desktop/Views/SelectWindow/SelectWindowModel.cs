using System;
using System.Collections.Generic;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal class SelectWindowModel<T> : ViewModelBase
    {
        private List<(string filterCaption, Func<T, bool> filter)> _filterOptions;
        private IEnumerable<T> _items;
        private string _message;
        private IEnumerable<T> _selectedItems;
        private string _title;

        public SelectWindowModel()
        {
            _filterOptions = new List<(string filterCaption, Func<T, bool> filter)>();
        }

        public List<(string filterCaption, Func<T, bool> filter)> FilterOptions
        {
            get => _filterOptions;
            set
            {
                _filterOptions = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<T> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<T> FilteredItems => Items;

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
    }
}