using System.Windows;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window
    {
        private TranslateWindowModel model = new TranslateWindowModel();

        public TranslateWindow()
        {
            InitializeComponent();
            this.DataContext = model;
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.CurrentResult))
            {
                webBrowserResult.NavigateToString(model.CurrentResult);
            }
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == TranslateWindowCommands.SelectEngineCommand)
                model.SelectEngines(this);
            else if (e.Command == TranslateWindowCommands.SelectLanguageCommand)
                model.SelectLanguages(this);
            else if (e.Command == TranslateWindowCommands.TranslateCommand)
                model.Translate();
        }
    }
}