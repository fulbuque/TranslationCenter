using System;
using System.Windows;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;
using TranslationCenter.UI.Desktop.ViewModels;

namespace TranslationCenter.UI.Desktop
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

            var selectWindowModel = new SelectWindowModel<AvaliableEngine>() { 
             Title="Avaliable Engines",
             Message = "Select one or more Engines",
            };

            selectWindowModel.FilterOptions.Add(("All", (e) => true));
            Func<AvaliableEngine, bool> onlyTranslators = (e) => e.Category == EngineCategory.Translator;
            selectWindowModel.FilterOptions.Add(("Translators", onlyTranslators));
            Func<AvaliableEngine, bool> onlyDictionaries = (e) => e.Category == EngineCategory.Dictionary;
            selectWindowModel.FilterOptions.Add(("Dictionaries", onlyDictionaries));

            var avaliableEngines = TranslationService.GetAvaliableEngines();
            selectWindowModel.Items = avaliableEngines;

            var selectWindow = new SelectWindow() { DataContext = selectWindowModel };
            selectWindow.ShowDialog();
        }
    }
}