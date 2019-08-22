using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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
                var content = model.CurrentResult;
                if (string.IsNullOrWhiteSpace(content))
                    content = "<HTML />";
                webBrowserResult.NavigateToString(content);
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

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WebBrowserResult_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            HideJsScriptErrors((WebBrowser)sender);
        }

        public void HideJsScriptErrors(WebBrowser wb)
        {
            // IWebBrowser2 interface
            // Exposes methods that are implemented by the WebBrowser control  
            // Searches for the specified field, using the specified binding constraints.
            FieldInfo fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            object obj = fld.GetValue(wb);
            if (obj == null)
                return;
            // Silent: Sets or gets a value that indicates whether the object can display dialog boxes.
            // HRESULT IWebBrowser2::get_Silent(VARIANT_BOOL *pbSilent);HRESULT IWebBrowser2::put_Silent(VARIANT_BOOL bSilent);
            obj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }
    }
}