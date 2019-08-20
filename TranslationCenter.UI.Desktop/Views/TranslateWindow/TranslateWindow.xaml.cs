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
        public TranslateWindow()
        {
            InitializeComponent();
        }

        private void MnuEngines_Click(object sender, RoutedEventArgs e)
        {

            var selectWindowModel = new SelectWindow.SelectWindowModel<AvaliableEngine>() { 
             Title="Avaliable Engines",
             Message = "Select one or more Engines",
            };

            selectWindowModel.AddFilterOption("All", (e) => true);
            selectWindowModel.AddFilterOption("Translators", (e) => e.Category == EngineCategory.Translator, true);
            selectWindowModel.AddFilterOption("Dictionaries", (e) => e.Category == EngineCategory.Dictionary);

            var avaliableEngines = TranslationService.GetAvaliableEngines();
            selectWindowModel.Items = avaliableEngines;
            selectWindowModel.DisplayName = nameof(AvaliableEngine.DisplayName);

            var selectWindow = new SelectWindow.SelectWindow() { DataContext = selectWindowModel };
            selectWindow.Owner = this;
            selectWindow.ShowDialog();
        }

        private void MnuLanguages_Click(object sender, RoutedEventArgs e)
        {
            var selectWindowModel = new SelectWindow.SelectWindowModel<ILanguage>()
            {
                Title = "Languages",
                Message = "Select one or more languages",
            };

            var languages = new CountryService().GetLanguages();
            selectWindowModel.DisplayName = nameof(ILanguage.Name);
            selectWindowModel.Items = languages;

            var selectWindow = new SelectWindow.SelectWindow() { DataContext = selectWindowModel };
            selectWindow.Owner = this;
            selectWindow.ShowDialog();
        }
    }
}