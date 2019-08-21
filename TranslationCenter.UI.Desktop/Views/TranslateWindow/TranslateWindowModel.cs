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
        private readonly string[] mostUsedIsos = new string[] {"cn", "es", "en", "ar", "pt", "nl", "ru", "jp", "fr", "tr", "it" };
        private IEnumerable<ILanguage> _allLanguages;
        private IAvaliableEngine _currentEngine;
        private ILanguage _currentLanguage;
        private ILanguage _currentLanguageFrom;
        private string _currentResult;
        private Dictionary<string, Dictionary<string, string>> _resultsDictionary;
        private string _lastSearchText;
        private string _searchText;
        private IEnumerable<ILanguage> _selectedLanguages;
        private IEnumerable<IAvaliableEngine> avaliableEngines;
        private CountryService countryService = new CountryService();
        private TranslationService translationService = new TranslationService();
        public TranslateWindowModel()
        {
            AllLanguages = countryService.GetLanguages();
            AllAvaliableEngines = TranslationService.GetAvaliableEngines();

            CurrentLanguageFrom = AllLanguages.FirstOrDefault(l => l.Iso == "de");
            SelectedLanguages = AllLanguages.Where(l => mostUsedIsos.Contains(l.Iso));
            CurrentLanguage = SelectedLanguages.FirstOrDefault(l => l.Iso == "en");
            SelectedEngines = AllAvaliableEngines;
            CurrentEngine = SelectedEngines.FirstOrDefault(e => e.Name == nameof(BingTranslatorEngine));
        }

        public IAvaliableEngine[] AllAvaliableEngines { get; }

        public IEnumerable<ILanguage> AllLanguages
        {
            get => _allLanguages;
            set
            {
                _allLanguages = value;

                NotifyPropertyChanged();
            }
        }

        public IAvaliableEngine CurrentEngine
        {
            get => _currentEngine;
            set
            {
                _currentEngine = value;
                SetCurrentResult();
                NotifyPropertyChanged();
            }
        }

        public ILanguage CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                SetCurrentResult();
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

        public string CurrentResult
        {
            get => _currentResult;
            set
            {
                _currentResult = GetFormattedResult(value);
                NotifyPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
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
            get => _selectedLanguages.OrderBy(l => l.Name);
            set
            {
                _selectedLanguages = value;
                NotifyPropertyChanged();
            }
        }

        internal void SelectEngines(Window owner)
        {
            var selectedEngines = OpenSelectWindow<IAvaliableEngine>(owner,
                                    "Engines",
                                    "Select one or more engines",
                                    AllAvaliableEngines,
                                    SelectedEngines,
                                    nameof(IAvaliableEngine.DisplayName),
                                    (selectedItems) =>
                                    {
                                        if (selectedItems.Count == 0)
                                        {
                                            MessageBox.Show("Select at least one search engine.", "Engines", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                            return false;
                                        }
                                        return true;
                                    },
                                    new FilterOptionItem<IAvaliableEngine>[]
                                    {
                                        new FilterOptionItem<IAvaliableEngine>("All", (e) => true, true, true),
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
                                    (selectedItems) =>
                                    {
                                        if (selectedItems.Count == 0)
                                        {
                                            MessageBox.Show("Select at least one language.", "Languages", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                            return false;
                                        }
                                        return true;
                                    },
                                    new FilterOptionItem<ILanguage>[]
                                    {
                                        new FilterOptionItem<ILanguage>("All", (e) => true, false, true),
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
            translationService.AddEngines(SelectedEngines);

            var keySearch = $"{CurrentLanguageFrom.Iso}:{SearchText}";
            if (_lastSearchText == null || _lastSearchText != keySearch)
                _resultsDictionary = new Dictionary<string, Dictionary<string, string>>();

            _lastSearchText = keySearch;

            foreach (var language in SelectedLanguages)
            {
                if (_resultsDictionary.Values.Any(i => i.ContainsKey(language.Iso)))
                    continue;

                var translateResults = translationService.Translate(new TranslateArgs(CurrentLanguageFrom.Iso, language.Iso, SearchText));
                foreach (var result in translateResults)
                {
                    if (!_resultsDictionary.TryGetValue(result.Source.Name, out var isoTextDic))
                    {
                        isoTextDic = new Dictionary<string, string>();
                        _resultsDictionary[result.Source.Name] = isoTextDic;
                    }
                    isoTextDic[language.Iso] = result.Result;
                }
            }

            SetCurrentResult();
        }

        private string GetFormattedResult(string contentResult)
        {
            return $@"
<!doctype html>
<html lang=""en"">
  <head>
    <!-- Required meta tags -->
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
    <!-- Bootstrap CSS -->
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" integrity=""sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"" crossorigin=""anonymous"">
    <link rel=""stylesheet"" href=""https://dict.leo.org/js/dist/dict.webpack-ef27251d.css"">
  </head>
  <body>
    <section id=""section-result"">
        { contentResult }
    </section>
  </body>
</html>
                          ";
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

            FilterOptionItem<T> filterOptionItemDefault = null;
            foreach (var filterOptionItem in filterOptionItems)
            {
                selectWindowModel.AddFilterOption(filterOptionItem);
                if (filterOptionItem.IsDefault)
                    filterOptionItemDefault = filterOptionItem;
            }

            selectWindowModel.DisplayMemberName = displayMemberName;
            selectWindowModel.CurrentSelectedItems = currentSelectedItems.ToHashSet();
            selectWindowModel.Items = items;

            var testFilter = items.Where(selectWindowModel.FilterOptionSelected.Filter).ToArray();
            if (!currentSelectedItems.Intersect(testFilter).Any())
                selectWindowModel.FilterOptionSelected = filterOptionItemDefault ?? filterOptionItems.FirstOrDefault();

            selectWindowModel.ValidateSelectedItems = validateSelectedItems;

            var selectWindow = new SelectWindow.SelectWindow() { DataContext = selectWindowModel };
            selectWindow.Owner = owner;

            if (selectWindow.ShowDialog() ?? false)
                return selectWindowModel.SelectedItems;

            return null;
        }

        private void SetCurrentResult(string engineName = null, string iso = null)
        {
            engineName = engineName ?? CurrentEngine?.Name;
            iso = iso ?? CurrentLanguage?.Iso;

            if (engineName != null && iso != null &&_resultsDictionary != null  
                &&  _resultsDictionary.TryGetValue(engineName, out var isoDic)
                && isoDic.TryGetValue(iso, out var text))
                CurrentResult = text;
            else
                CurrentResult = string.Empty;
        }
    }
}