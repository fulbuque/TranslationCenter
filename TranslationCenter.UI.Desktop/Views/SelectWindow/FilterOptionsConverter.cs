using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    public class FilterOptionsConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<FilterOptionItem<object>> filterOptions)
            {
                //return filterOptions.Select(item => new TabItem() {
                //    Tag = item.filter ,
                //    Header =  item.filterCaption  
                //});
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
