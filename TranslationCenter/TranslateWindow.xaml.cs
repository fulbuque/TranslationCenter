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

namespace TranslationCenter
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
            model.PropertyChanged += Model_PropertyChanged;
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

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.TranslatedText))
            {
                var content = $@"
<!doctype html>
<html lang=""en"">
  <head>
    <!-- Required meta tags -->
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
    <!-- Bootstrap CSS -->
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" integrity=""sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"" crossorigin=""anonymous"">
  </head>
  <body>
    <section id=""section-result"">                
        { model.TranslatedText}
    </section>
  </body>
</html>
                          ";

                webBrowserResult.NavigateToString(content);
            }
        }

    }
}
