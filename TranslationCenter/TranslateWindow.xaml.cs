using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window
    {
        TranslateWindowModel model = new TranslateWindowModel();
        public TranslateWindow()
        {
            InitializeComponent();
            this.DataContext = model;
        }

        private void BtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            model.Translate();
        }

        private void ChkLanguage_Checked(object sender, RoutedEventArgs e)
        {
            model.NotifyPropertyChanged(nameof(model.SelectedLanguages));
            model.Translate();
        }
        private void ChkLanguage_Unchecked(object sender, RoutedEventArgs e)
        {
            model.NotifyPropertyChanged(nameof(model.SelectedLanguages));
        }

        private void TabSelectedLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                model.SetTranslatedText(e.AddedItems.Cast<TabItem>().FirstOrDefault().Tag?.ToString());
        }


    }
}
