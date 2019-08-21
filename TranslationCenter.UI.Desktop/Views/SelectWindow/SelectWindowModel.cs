using System;
using System.Collections.Generic;
using System.Linq;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal partial class SelectWindowModel<T> : ViewModelBase, ISelectWindowModel
    {
        private HashSet<T> _currentSelectedItems;
        private string _displayName;
        private Dictionary<string, FilterOptionItem<T>> _filterOptions;
        private FilterOptionItem<T> _filterOptionSelected;
        private IEnumerable<SelectWindowItem> _items;
        private string _message;
        private IEnumerable<T> _selectedItems;
        private string _title;

        public SelectWindowModel()
        {
            _filterOptions = new Dictionary<string, FilterOptionItem<T>>();
        }

        public HashSet<T> CurrentSelectedItems
        {
            get => _currentSelectedItems;
            set
            {
                _currentSelectedItems = value;
                if (_items != null)
                {
                    foreach (var item in _items)
                    {
                        if (_currentSelectedItems.Contains(item.Data))
                            item.IsSelected = true;
                    }
                }
                NotifyPropertyChanged();
            }
        }

        public string DisplayMemberName
        {
            get => _displayName;
            set
            {
                _displayName = value;
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

        public FilterOptionItem<T>[] FilterOptions => _filterOptions.Values.ToArray();

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

        public bool HasFilterOptions => FilterOptions.Any();

        public IEnumerable<T> Items
        {
            get => _items.Select(i => i.Data);
            set
            {
                _items = value.Select(i => new SelectWindowItem(i, DisplayMemberName, CurrentSelectedItems)).ToArray();
                NotifyPropertyChanged();
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

        public Func<System.Collections.IList, bool> ValidateSelectedItems { get; set; }

        public void AddFilterOption(string text, Func<T, bool> filter, bool isSelected = false)
        {
            AddFilterOption(new FilterOptionItem<T>(text, filter, isSelected));
        }

        public void SetSelectedItems(System.Collections.IList list)
        {
            SelectedItems = list.Cast<SelectWindowItem>().Select(i => i.Data);
        }

        internal void AddFilterOption(FilterOptionItem<T> filterOptionItem)
        {
            _filterOptions[filterOptionItem.Text] = filterOptionItem;
            if (filterOptionItem.IsSelected)
                FilterOptionSelected = _filterOptions[filterOptionItem.Text];
        }
    }
}