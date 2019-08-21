using System.Collections.Generic;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    internal partial class SelectWindowModel<T>
    {
        public class SelectWindowItem : ViewModelBase
        {
            private HashSet<T> _selectedItems;
            private string _displayMemberName;
            private bool _isSelected;

            public SelectWindowItem(T data, string displayMemberName, HashSet<T> currentSelectedItems)
            {
                Data = data;
                _displayMemberName = displayMemberName;
                if(currentSelectedItems != null)
                {
                    _selectedItems = currentSelectedItems;
                    if (_selectedItems.Contains(Data))
                        IsSelected = true;
                }
            }
            public T Data { get; }


            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;

                    if (_selectedItems != null)
                    {
                        if (_isSelected)
                            _selectedItems.Add(Data);
                        else
                            _selectedItems.Remove(Data);
                    }

                    NotifyPropertyChanged();
                }
            }

            public string DisplayMemberName
            {
                get
                {
                    var prop = Data.GetType().GetProperty(_displayMemberName);
                    if (prop != null)
                        return prop.GetValue(Data)?.ToString();

                    return default;
                }
            }
        }
    }
}