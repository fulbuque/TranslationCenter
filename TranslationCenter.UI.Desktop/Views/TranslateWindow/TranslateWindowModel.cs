using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TranslationCenter.Services.Country;
using TranslationCenter.Services.Country.Types.Interfaces;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;
using TranslationCenter.UI.Desktop.Views.SelectWindow;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
{
    internal class TranslateWindowModel : ViewModelBase
    {
        private IAvaliableEngine _currentEngine;
        private ILanguage _currentLanguage;
        private IEnumerable<ILanguage> _selectedLanguages;
        private IEnumerable<IAvaliableEngine> avaliableEngines;
        CountryService countryService = new CountryService();
        TranslationService translationService = new TranslationService();

        public TranslateWindowModel()
        {
            AllLanguages = countryService.GetLanguages();
        }

        public IAvaliableEngine CurrentEngine
        {
            get => _currentEngine;
            set
            {
                _currentEngine = value;
                NotifyPropertyChanged();
            }
        }

        public ILanguage CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<IAvaliableEngine> SelectedEngines
        {
            get => avaliableEngines;
            set
            {
                avaliableEngines = value;
                NotifyPropertyChanged();
            }
        }
        public IEnumerable<ILanguage> SelectedLanguages
        {
            get => _selectedLanguages;
            set
            {
                _selectedLanguages = value;
                NotifyPropertyChanged();
            }
        }

        private IEnumerable<ILanguage> _allLanguages;

        public IEnumerable<ILanguage> AllLanguages
        {
            get => _allLanguages;
            set
            {
                _allLanguages = value;
                NotifyPropertyChanged();
            }
        }


        internal void SelectEngines(Window owner)
        {
            var selectedEngines = OpenSelectWindow<IAvaliableEngine>(owner,
                                    "Engines",
                                    "Select one or more engines",
                                    TranslationService.GetAvaliableEngines(),
                                    nameof(IAvaliableEngine.DisplayName),
                                    new FilterOptionItem<IAvaliableEngine>[]
                                    {
                                        new FilterOptionItem<IAvaliableEngine>("All", (e) => true, true),
                                        new FilterOptionItem<IAvaliableEngine>("Translators", (e) => e.Category == EngineCategory.Translator),
                                        new FilterOptionItem<IAvaliableEngine>("Dictionaries", (e) => e.Category == EngineCategory.Dictionary)
                                    });
 
            if (selectedEngines != null)
                SelectedEngines = selectedEngines;
        }

        internal void SelectLanguages(Window owner)
        {
            var mostUsedIsos = new string[] { "cn", "es", "en", "hi", "ar", "pt", "nl", "ru", "jp", "fr", "tr", "it" };
            var selectedLanguages = OpenSelectWindow<ILanguage>(owner,
                                    "Languages",
                                    "Select one or more languages",
                                    countryService.GetLanguages(),
                                    nameof(ILanguage.Name),
                                    new FilterOptionItem<ILanguage>[]
                                    {
                                        new FilterOptionItem<ILanguage>("All", (e) => true),
                                        new FilterOptionItem<ILanguage>("Most Used", (e) => mostUsedIsos.Contains(e.Iso), true),
                                    });

            if (selectedLanguages != null)
                SelectedLanguages = selectedLanguages;
        }

        internal void Translate()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<T> OpenSelectWindow<T>(Window owner, string title, string message,
            IEnumerable<T> items, string displayMemberName, params FilterOptionItem<T>[] filterOptionItems)
        {

            var selectWindowModel = new SelectWindow.SelectWindowModel<T>()
            {
                Title = title,
                Message = message,
            };

            foreach (var filterOptionItem in filterOptionItems)
            {
                selectWindowModel.AddFilterOption(filterOptionItem);

            }

            selectWindowModel.DisplayMemberName = displayMemberName;
            selectWindowModel.Items = items;

            var selectWindow = new SelectWindow.SelectWindow() { DataContext = selectWindowModel };
            selectWindow.Owner = owner;
            if (selectWindow.ShowDialog() ?? false)
                return selectWindowModel.SelectedItems;

            return null;
        }

    }
}