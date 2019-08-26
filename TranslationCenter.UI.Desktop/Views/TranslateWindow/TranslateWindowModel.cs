using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string[] _mostUsedIsos = new string[] { "es", "en", "pt", "ru", "fr", "it", "de" };

        private readonly string modelConfigFileName = Path.Combine(Directory.GetCurrentDirectory(), ".config");
        private IEnumerable<ILanguage> _allLanguages;
        private IAvaliableEngine _currentEngine;
        private ILanguage _currentLanguage;
        private ILanguage _currentLanguageFrom;
        private string _currentResult;
        private SearchProtocol _lastSearchProtocol;
        private Dictionary<Type, System.Collections.IList> _ordemListCustomDictionary = new Dictionary<Type, System.Collections.IList>();
        private Dictionary<Type, System.Collections.IList> _ordemListDictionary = new Dictionary<Type, System.Collections.IList>();
        private Dictionary<string, Dictionary<string, ITranslateResult>> _resultsDictionary;
        private IEnumerable<IAvaliableEngine> _selectedEngines;
        private IEnumerable<ILanguage> _selectedLanguages;
        private string _textSearch;
        private TranslationService _translationService = new TranslationService();

        private JsonSerializer jsonSerializer = new JsonSerializer();

        public TranslateWindowModel()
        {
            AllLanguages = CountryService.Languages.OrderBy(l => l.Name);
            AllAvaliableEngines = TranslationService.GetAvaliableEngines();

            if (!LoadModelState())
            {
                SaveOrdemList(AllLanguages, l => l.Name);
                SaveOrdemList(AllAvaliableEngines, l => l.Name);

                CurrentLanguageFrom = AllLanguages.FirstOrDefault(l => l.Iso == "de");
                SelectedLanguages = AllLanguages.Where(l => _mostUsedIsos.Contains(l.Iso));
                CurrentLanguage = SelectedLanguages.FirstOrDefault(l => l.Iso == "en");
                SelectedEngines = AllAvaliableEngines;
                CurrentEngine = SelectedEngines.FirstOrDefault(e => e.Name == nameof(BingTranslatorEngine));
            }
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

        public IEnumerable<IAvaliableEngine> SelectedEngines
        {
            get => _selectedEngines;
            set
            {
                _selectedEngines = OrderBy<IAvaliableEngine>(value, engine => engine.Name);
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<ILanguage> SelectedLanguages
        {
            get => _selectedLanguages;
            set
            {
                _selectedLanguages = OrderBy<ILanguage>(value, language => language.Iso, language=> language.Iso != CurrentLanguageFrom.Iso);
                NotifyPropertyChanged();
            }
        }

        public string TextSearch
        {
            get => _textSearch;
            set
            {
                _textSearch = value;
                NotifyPropertyChanged();
                Task.Run(Translate);
            }
        }

        internal void ChangePositionItem(object target, object source)
        {
            if (target.GetType() != source.GetType() || target == source)
                return;

            if (target is ILanguage languageTarget && source is ILanguage languageSource)
            {
                SelectedLanguages = ChangePositionItem<ILanguage>(SelectedLanguages, languageTarget, languageSource, language => language.Iso);
            }
            else if (target is IAvaliableEngine engineTarget && source is IAvaliableEngine engineSource)
            {
                SelectedEngines = ChangePositionItem<IAvaliableEngine>(SelectedEngines, engineTarget, engineSource, engine => engine.Name);
            }
        }

        internal void GoToNextEngine()
        {
            var index = SelectedEngines.ToList().IndexOf(CurrentEngine);
            if (index + 1 < SelectedEngines.Count())
                CurrentEngine = SelectedEngines.ElementAt(index + 1);
        }

        internal void GoToNextLanguage()
        {
            var index = SelectedLanguages.ToList().IndexOf(CurrentLanguage);
            if (index + 1 < SelectedLanguages.Count())
                CurrentLanguage = SelectedLanguages.ElementAt(index + 1);
        }

        internal void GoToPreviousEngine()
        {
            var index = SelectedEngines.ToList().IndexOf(CurrentEngine);
            if (index - 1 >= 0)
                CurrentEngine = SelectedEngines.ElementAt(index - 1);
        }

        internal void GoToPreviousLanguage()
        {
            var index = SelectedLanguages.ToList().IndexOf(CurrentLanguage);
            if (index - 1 >= 0)
                CurrentLanguage = SelectedLanguages.ElementAt(index - 1);
        }

        internal void SaveModelState()
        {
            using (var stringWriter = new StreamWriter(modelConfigFileName))
            {
                jsonSerializer.Serialize(stringWriter, new ModelState(this));
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
                                    null,
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
                Translate();
                if (!SelectedEngines.Contains(CurrentEngine))
                    CurrentEngine = SelectedEngines.FirstOrDefault();
            }
        }

        internal void SelectLanguages(Window owner)
        {
            var selectedLanguages = OpenSelectWindow<ILanguage>(owner,
                                    "Languages",
                                    "Select one or more languages",
                                    AllLanguages.Where(l => l.Iso != CurrentLanguageFrom.Iso),
                                    SelectedLanguages,
                                    nameof(ILanguage.Name),
                                    (lang) => lang.Iso,
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
                                        new FilterOptionItem<ILanguage>("Most Used", (e) => _mostUsedIsos.Contains(e.Iso), true),
                                    });

            if (selectedLanguages != null)
            {
                SelectedLanguages = selectedLanguages;
                Translate();
                if (!SelectedLanguages.Contains(CurrentLanguage))
                    CurrentLanguage = SelectedLanguages.FirstOrDefault();
            }
        }

        internal void SortEnginesByCustomOrder()
        {
            if (_ordemListDictionary.TryGetValue(typeof(IAvaliableEngine), out var list)
                && _ordemListCustomDictionary.TryGetValue(typeof(IAvaliableEngine), out var listCustom))
            {
                _ordemListDictionary[typeof(IAvaliableEngine)] = listCustom;
            }
            SelectedEngines = SelectedEngines;
        }

        internal void SortEnginesByName()
        {
            SaveOrdemList(AllAvaliableEngines.OrderBy(l => l.Name), l => l.Name);
            SelectedEngines = SelectedEngines;
        }

        internal void SortLanguagesByCustomOrder()
        {
            if (_ordemListDictionary.TryGetValue(typeof(ILanguage), out var list)
                && _ordemListCustomDictionary.TryGetValue(typeof(ILanguage), out var listCustom))
            {
                _ordemListDictionary[typeof(ILanguage)] = listCustom;
            }
            SelectedLanguages = SelectedLanguages;
        }

        internal void SortLanguagesByIso()
        {
            SaveOrdemList(AllLanguages.OrderBy(l => l.Iso), l => l.Iso);
            SelectedLanguages = SelectedLanguages;
        }

        internal void SortLanguagesByName()
        {
            SaveOrdemList(AllLanguages.OrderBy(l => l.Name), l => l.Iso);
            SelectedLanguages = SelectedLanguages;
        }

        internal void SwitchLanguages()
        {
             var newLanguage = CountryService.GetLanguage(CurrentLanguageFrom.Iso);
            CurrentLanguageFrom = CountryService.GetLanguage(CurrentLanguage.Iso); ;
            CurrentLanguage = newLanguage;

            var newList = SelectedLanguages.ToList();
            newList.Add(newLanguage);

            SelectedLanguages = newList;

            Translate();
        }
        internal void Translate()
        {
            _translationService.AddEngines(SelectedEngines);

            var searchProtocol = new SearchProtocol(TextSearch, CurrentLanguageFrom, SelectedEngines, SelectedLanguages);
            if (!searchProtocol.Equals(_lastSearchProtocol))
            {
                _resultsDictionary = new Dictionary<string, Dictionary<string, ITranslateResult>>();
                _lastSearchProtocol = searchProtocol;
            }

            foreach (var language in SelectedLanguages)
            {
                if (_resultsDictionary.Values.Any(i => i.ContainsKey(language.Iso)))
                    continue;

                var translateResults = _translationService.Translate(new TranslateArgs(CurrentLanguageFrom.Iso, language.Iso, TextSearch));
                foreach (var result in translateResults)
                {
                    if (!_resultsDictionary.TryGetValue(result.Source.Name, out var isoTextDic))
                    {
                        isoTextDic = new Dictionary<string, ITranslateResult>();
                        _resultsDictionary[result.Source.Name] = isoTextDic;
                    }
                    isoTextDic[language.Iso] = result;
                }
            }

            SetCurrentResult();
        }

        private IEnumerable<T> ChangePositionItem<T>(IEnumerable<T> items, T target, T source, Func<T, string> getIndexer)
        {
            var list = items.ToList();
            var index = list.IndexOf(target);
            list.Remove(source);
            list.Insert(index, source);
            SaveOrdemList(list, getIndexer);
            SaveOrdemList(list, getIndexer, _ordemListCustomDictionary);
            return list;
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
  </head>
  <body style=""overflow: auto !important; height: auto !important; width: auto !important; margin: 15px; "">
    <section id=""section-result"">
        { contentResult }
    </section>
  </body>
</html>
                          ";
        }

        private bool LoadModelState()
        {
            try
            {
                if (!File.Exists(modelConfigFileName))
                    return false;

                ModelState modelState = null;
                using (var stringReader = new StreamReader(modelConfigFileName))
                using (var reader = new JsonTextReader(stringReader))
                {
                    modelState = jsonSerializer.Deserialize<ModelState>(reader);
                }

                if (modelState == null) return false;

                this._ordemListDictionary = modelState.CurrentOrder;
                this._ordemListCustomDictionary = modelState.CurrentCustomOrder;
                this._selectedLanguages = modelState.CurrentSelectedIsos.Select(iso => CountryService.GetLanguage(iso));
                this._selectedEngines = modelState.CurrentSelectedEngines.Select(engineName => TranslationService.GetAvaliableEngine(engineName));
                this._currentEngine = TranslationService.GetAvaliableEngine(modelState.CurrentEngineName);
                this._currentLanguageFrom = CountryService.GetLanguage(modelState.CurrentIsoFrom);
                this._currentLanguage = CountryService.GetLanguage(modelState.CurrentIsoTo);
                this._textSearch = modelState.TextSearch;
                Translate();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("It was not possible to read the configuration file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private IEnumerable<T> OpenSelectWindow<T>(Window owner, string title, string message,
            IEnumerable<T> items, IEnumerable<T> currentSelectedItems, string displayMemberName, Func<T, string> tooltip,
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
            selectWindowModel.Tooltip = tooltip;
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

        private IEnumerable<T> OrderBy<T>(IEnumerable<T> items, Func<T, string> getIndexer, Func<T, bool> filter = null)
        {
            if (_ordemListDictionary.TryGetValue(typeof(T), out var orderList))
            {
                var list = items.Select(item => new { item, order = orderList.IndexOf(getIndexer(item)) }).OrderBy(item => item.order).Select(item => item.item);
                if (filter != null)
                    list = list.Where(filter);
                return list;
            }
            return items;
        }
        private void SaveOrdemList<T>(IEnumerable<T> selectedLanguages, Func<T, string> getIndexer, Dictionary<Type, System.Collections.IList> ordemListCustomDictionary = null)
        {
            var newlist = selectedLanguages
                                                .Select((item, index) => new { Indexer = getIndexer(item), Index = index })
                                                .OrderBy(i => i.Index)
                                                .Select(i => i.Indexer).ToList();

            (ordemListCustomDictionary ?? _ordemListDictionary)[typeof(T)] = newlist;
        }
        private void SetCurrentResult(string engineName = null, string iso = null)
        {
            engineName = engineName ?? CurrentEngine?.Name;
            iso = iso ?? CurrentLanguage?.Iso;

            if (engineName != null && iso != null && _resultsDictionary != null
                && _resultsDictionary.TryGetValue(engineName, out var isoDic)
                && isoDic.TryGetValue(iso, out var translateResult))
            {
                Task.Run(() =>
                {
                    translateResult.Render();
                    CurrentResult = translateResult.Result;
                });
            }
            else
            {
                CurrentResult = string.Empty;
            }
        }

        private class ModelState
        {
            private TranslateWindowModel model;

            public ModelState()
            {
            }

            public ModelState(TranslateWindowModel model)
            {
                this.model = model;
                this.CurrentIsoFrom = model.CurrentLanguageFrom.Iso;
                this.CurrentIsoTo = model.CurrentLanguage.Iso;
                this.CurrentEngineName = model.CurrentEngine.Name;
                this.TextSearch = model.TextSearch;
                this.CurrentSelectedIsos = model.SelectedLanguages.Select(l => l.Iso);
                this.CurrentSelectedEngines = model.SelectedEngines.Select(e => e.Name);
                this.CurrentOrder = model._ordemListDictionary;
                this.CurrentCustomOrder = model._ordemListCustomDictionary;
            }

            public Dictionary<Type, IList> CurrentCustomOrder { get; set; }
            public string CurrentEngineName { get; set; }
            public string CurrentIsoFrom { get; set; }
            public string CurrentIsoTo { get; set; }
            public Dictionary<Type, IList> CurrentOrder { get; set; }
            public IEnumerable<string> CurrentSelectedEngines { get; set; }
            public IEnumerable<string> CurrentSelectedIsos { get; set; }
            public string TextSearch { get; set; }
        }

        private class SearchProtocol
        {
            private string currentLanguageFrom;
            private string selectedEngines;
            private string selectedLanguages;
            private string textSearch;

            public SearchProtocol(string textSearch, ILanguage currentLanguageFrom, IEnumerable<IAvaliableEngine> selectedEngines, IEnumerable<ILanguage> selectedLanguages)
            {
                this.textSearch = textSearch;
                this.currentLanguageFrom = currentLanguageFrom.Iso;
                this.selectedEngines = string.Join(", ", selectedEngines.OrderBy(e => e.Name).Select(e => e.Name).ToArray());
                this.selectedLanguages = string.Join(", ", selectedLanguages.OrderBy(e => e.Iso).Select(e => e.Iso).ToArray());
            }

            public string TextSearch { get => textSearch; }

            public bool Equals(SearchProtocol searchProtocol)
            {
                return searchProtocol != null
                        && TextSearch == searchProtocol.TextSearch
                        && selectedEngines == searchProtocol.selectedEngines
                        && selectedLanguages == searchProtocol.selectedLanguages;
            }
        }
    }
}