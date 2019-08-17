using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Windows.Threading;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Enums;
using TranslationCenter.Services.Translation.Interfaces;

namespace TranslationCenter
{
    public partial class TranslateWindowModel : INotifyPropertyChanged
    {
        private string _currentIso;
        private IEnumerable<Language> _languages;
        private ObservableCollection<ListBoxItem> _languagesToTranslate;
        private Dictionary<EngineTypes, ITranslateResult> _searchResults;
        private string _text;
        private string _translatedText;
        private Language _translateFrom;
        private Language _translateTo;
        private Dictionary<string, Translation> _translationsDicionary;
        private Dispatcher currentDispatcher;

        public TranslateWindowModel()
        {
            currentDispatcher = Dispatcher.CurrentDispatcher;
            PrepareUi();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTranslateEnabled { get; set; }

        public IEnumerable<Language> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ListBoxItem> LanguagesToTranslate
        {
            get => _languagesToTranslate;
            set
            {
                _languagesToTranslate = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<EngineTypes, ITranslateResult> SearchResults
        {
            get => _searchResults;
            private set
            {
                _searchResults = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(SourceResults));
            }
        }

        public IEnumerable<TabItem> SelectedLanguages => 
            LanguagesToTranslate?.Where(i => i.IsSelected).Select(i => new TabItem() { Tag = i.Tag, Header = i.Content });

        public IEnumerable<TabItem> SourceResults => 
            SearchResults?.Values?.Select(translateResult => new TabItem() { Tag = translateResult.Source.EngineType, Header = translateResult.Source.DisplayName });

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                    _translationsDicionary = new Dictionary<string, Translation>();
                _text = value;
                Translate();
                NotifyPropertyChanged();
            }
        }

        public string TranslatedText
        {
            get =>
                _translatedText; set
            {
                _translatedText = value;
                NotifyPropertyChanged();
            }
        }

        public Language TranslateFrom
        {
            get => _translateFrom;
            set
            {
                _translateFrom = value;
                Translate();
                NotifyPropertyChanged();
            }
        }

        public Language TranslateTo
        {
            get => _translateTo;
            set
            {
                _translateTo = value;
                Translate();
                NotifyPropertyChanged();
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SetTranslatedText(string iso)
        {
            this._currentIso = iso;
            if (_translationsDicionary != null && _translationsDicionary.TryGetValue(iso, out var translation))
                TranslatedText = translation.Text;
            else
                TranslatedText = string.Empty;
        }

        internal async void Translate()
        {
            if (!IsTranslateEnabled) return;

            _translationsDicionary = new Dictionary<string, Translation>();
            var isos = SelectedLanguages.Select(t => t.Tag?.ToString());
            foreach (var iso in isos)
            {
                if (!_translationsDicionary.TryGetValue(iso, out var translatedText) || string.IsNullOrWhiteSpace(translatedText?.Text))
                {
                    var engines = new EngineTypes[]
                    {
                         EngineTypes.Bing,
                         EngineTypes.Leo
                    };

                    var translateResults = await TranslationService.Instance.Translate(TranslateFrom.Iso, iso, Text, engines);
                    this.SearchResults = translateResults.ToDictionary(tr => tr.Source.EngineType);
                    if (translateResults?.Any() ?? false)
                        _translationsDicionary[iso] = new Translation() { Text = translateResults.FirstOrDefault().Result };
                }
            }

            if (string.IsNullOrEmpty(_currentIso))
                _currentIso = isos.FirstOrDefault();

            SetTranslatedText(_currentIso);
        }

        private async void PrepareUi()
        {
            IsTranslateEnabled = false;
            await SetLanguage();
            IsTranslateEnabled = true;
        }

        private async Task SetLanguage()
        {
            IEnumerable<Country> countries;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://restcountries.eu/rest/v2/all");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetStringAsync("");

                JavaScriptSerializer ser = new JavaScriptSerializer();
                countries = ser.Deserialize<IEnumerable<Country>>(response);
            }

            this.Languages = countries
                                .SelectMany(c => c.Languages)
                                .Where(l => !string.IsNullOrEmpty(l.Iso))
                                .ToHashSet(new LanguageComparer())
                                .OrderBy(l => l.Name);

            var isoToTranslate = new string[] { "en", "pt", "es" };
            this.LanguagesToTranslate = new ObservableCollection<ListBoxItem>();
            foreach (var language in Languages)
            {
                this.LanguagesToTranslate.Add(new ListBoxItem()
                {
                    Tag = language.Iso,
                    Content = language.Name,
                    IsSelected = (isoToTranslate.Contains(language.Iso))
                });
            }

            this.TranslateFrom = this.Languages.FirstOrDefault(l => l.Iso == "de");

            NotifyPropertyChanged(nameof(SelectedLanguages));
        }
    }
}