using System;
using System.Windows;
using TranslationCenter.Services.Country;
using TranslationCenter.Services.Country.Types.Interfaces;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
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