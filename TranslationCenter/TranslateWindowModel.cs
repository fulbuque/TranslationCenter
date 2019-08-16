using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Enums;

namespace TranslationCenter
{
    public partial class TranslateWindowModel : INotifyPropertyChanged
    {

        private Dispatcher currentDispatcher;
        private Dictionary<string, Translation> _translationsDicionary;

        public event PropertyChangedEventHandler PropertyChanged;

        public TranslateWindowModel()
        {
            currentDispatcher = Dispatcher.CurrentDispatcher;
            PrepareUi();
        }

        private async void PrepareUi()
        {
            IsTranslateEnabled = false;
            await SetLanguage();
            IsTranslateEnabled = true;

        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsTranslateEnabled { get; set; }

        private IEnumerable<Language> _languages;
        public IEnumerable<Language> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ListBoxItem> _languagesToTranslate;

        public ObservableCollection<ListBoxItem> LanguagesToTranslate
        {
            get => _languagesToTranslate;
            set
            {
                _languagesToTranslate = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<TabItem> SelectedLanguages => LanguagesToTranslate?.Where(i => i.IsSelected).Select(i => new TabItem() { Tag = i.Tag, Header = i.Content });

        private Language _translateTo;
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

        private Language _translateFrom;
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

        private string _text;
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

        private string _translatedText;
        private string _currentIso;

        public string TranslatedText
        { 
            get =>
                _translatedText; set
            {
                _translatedText = value;
                NotifyPropertyChanged();
            }
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
                this.LanguagesToTranslate.Add(new ListBoxItem() {
                    Tag = language.Iso,
                    Content = language.Name,
                    IsSelected = (isoToTranslate.Contains(language.Iso))
                });
            }

            this.TranslateFrom = this.Languages.FirstOrDefault(l => l.Iso == "de");

            NotifyPropertyChanged(nameof(SelectedLanguages));
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
                         //EngineTypes.Bing,
                         EngineTypes.Leo
                    };

                    var translateResults = await TranslationService.Instance.Translate(TranslateFrom.Iso, iso, Text, engines);
                    //var translations = await TranslateInternal(TranslateFrom.Iso, Text, iso);
                    if (translateResults?.Any() ?? false)
                        _translationsDicionary[iso] = new Translation() { Text = translateResults.FirstOrDefault().Result };
                }
            }

            if (string.IsNullOrEmpty(_currentIso))
                _currentIso = isos.FirstOrDefault();

            SetTranslatedText(_currentIso);
        }

        internal void SetTranslatedText(string iso)
        {
            this._currentIso = iso;
            if (_translationsDicionary != null && _translationsDicionary.TryGetValue(iso, out var translation))
                TranslatedText = translation.Text;
            else
                TranslatedText = string.Empty;
        }

    }
}
