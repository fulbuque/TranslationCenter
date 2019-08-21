using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TranslationCenter.Services.Country;
using TranslationCenter.Services.Country.Types.Interfaces;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Types;
using TranslationCenter.UI.Desktop.Views.SelectWindow;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
{
    internal class TranslateWindowModel : ViewModelBase
    {
        private IAvaliableEngine _currentEngine;
        private ILanguage _currentLanguage;
        private ILanguage _currentLanguageFrom;
        private IEnumerable<ILanguage> _selectedLanguages;
        private IEnumerable<IAvaliableEngine> avaliableEngines;
        private readonly string[] mostUsedIsos = new string[] { "cn", "es", "en", "hi", "ar", "pt", "nl", "ru", "jp", "fr", "tr", "it" };
        CountryService countryService = new CountryService();
        TranslationService translationService = new TranslationService();

        public TranslateWindowModel()
        {
            AllLanguages = countryService.GetLanguages();
            AllAvaliableEngines = TranslationService.GetAvaliableEngines();

            CurrentLanguageFrom = AllLanguages.FirstOrDefault(l => l.Iso == "de");
            SelectedLanguages = AllLanguages.Where(l => mostUsedIsos.Contains( l.Iso) );
            CurrentLanguage = SelectedLanguages.FirstOrDefault(l => l.Iso == "en");
            SelectedEngines = AllAvaliableEngines;
            CurrentEngine = SelectedEngines.FirstOrDefault(e => e.Name == nameof(BingTranslatorEngine));
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
        public ILanguage CurrentLanguageFrom
        {
            get => _currentLanguageFrom;
            set
            {
                _currentLanguageFrom = value;
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

        public IAvaliableEngine[] AllAvaliableEngines { get; }

        internal void SelectEngines(Window owner)
        {
            var selectedEngines = OpenSelectWindow<IAvaliableEngine>(owner,
                                    "Engines",
                                    "Select one or more engines",
                                    AllAvaliableEngines,
                                    SelectedEngines,
                                    nameof(IAvaliableEngine.DisplayName),
                                    (selectedItems) => {
                                        if (selectedItems.Count == 0)
                                        {
                                            MessageBox.Show("Select at least one search engine.", "Engines", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                            return false;
                                        }
                                        return true;
                                    },
                                    new FilterOptionItem<IAvaliableEngine>[]
                                    {
                                        new FilterOptionItem<IAvaliableEngine>("All", (e) => true, true),
                                        new FilterOptionItem<IAvaliableEngine>("Translators", (e) => e.Category == EngineCategory.Translator),
                                        new FilterOptionItem<IAvaliableEngine>("Dictionaries", (e) => e.Category == EngineCategory.Dictionary)
                                    });
 
            if (selectedEngines != null)
            {
                SelectedEngines = selectedEngines;
                if (!SelectedEngines.Contains(CurrentEngine))
                    CurrentEngine = SelectedEngines.FirstOrDefault();

            }
        }

        internal void SelectLanguages(Window owner)
        {
            
            var selectedLanguages = OpenSelectWindow<ILanguage>(owner,
                                    "Languages",
                                    "Select one or more languages",
                                    AllLanguages,
                                    SelectedLanguages,
                                    nameof(ILanguage.Name),
                                    (selectedItems) => {
                                        if (selectedItems.Count == 0)
                                        {
                                            MessageBox.Show("Select at least one language.", "Languages", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                            return false;
                                        }
                                        return true;
                                    },
                                    new FilterOptionItem<ILanguage>[]
                                    {
                                        new FilterOptionItem<ILanguage>("All", (e) => true),
                                        new FilterOptionItem<ILanguage>("Most Used", (e) => mostUsedIsos.Contains(e.Iso), true),
                                    });

            if (selectedLanguages != null)
            {
                SelectedLanguages = selectedLanguages;
                if (!SelectedLanguages.Contains(CurrentLanguage))
                    CurrentLanguage = SelectedLanguages.FirstOrDefault();
            }
        }

        internal void Translate()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<T> OpenSelectWindow<T>(Window owner, string title, string message,
            IEnumerable<T> items, IEnumerable<T> currentSelectedItems, string displayMemberName,
            Func<System.Collections.IList, bool> validateSelectedItems,
            params FilterOptionItem<T>[] filterOptionItems)
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
            selectWindowModel.CurrentSelectedItems = currentSelectedItems.ToHashSet();
            selectWindowModel.Items = items;

            selectWindowModel.ValidateSelectedItems = validateSelectedItems;

            var selectWindow = new SelectWindow.SelectWindow() { DataContext = selectWindowModel };
            selectWindow.Owner = owner;
            
            if (selectWindow.ShowDialog() ?? false)
                return selectWindowModel.SelectedItems;

            return null;
        }

    }
}