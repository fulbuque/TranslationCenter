using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TranslationCenter.Services.Country.Types;
using TranslationCenter.Services.Country.Types.Interfaces;

namespace TranslationCenter.Services.Country
{
    public class CountryService
    {
        private static ICountry[] _countries = new ICountry[] { };
        private static LanguageComparer _languageComparer = new LanguageComparer();
        private static ILanguage[] _languages;

        static CountryService()
        {
            SetLanguageDictionary();
        }

        private CountryService()
        {
        }

        public static ICountry[] Countries => GetCountries();

        public static Dictionary<string, ILanguage> LanguageDicitionary { get; private set; } = new Dictionary<string, ILanguage>();

        public static ILanguage[] Languages => _languages;

        public static string GetLanguageName(string iso)
        {
            LanguageDicitionary.TryGetValue(iso, out var language);
            return language?.Name;
        }

        public static ILanguage GetLanguage(string iso)
        {
            LanguageDicitionary.TryGetValue(iso, out var language);
            return language;
        }

        private static ICountry[] GetCountries()
        {
            if (_countries?.Any() ?? false)
                return _countries;

            using (var client = new HttpClient())
            {
                var ContryJsonFile = Path.Combine(Directory.GetCurrentDirectory() + @"\Resources\", "Country.json");
                var hasCountryJsonFile = File.Exists(ContryJsonFile);

                string url = "";
                if (hasCountryJsonFile)
                    url = ContryJsonFile;
                else
                    url = "https://restcountries.eu/rest/v2/all";

                //string url = "https://restcountries.eu/rest/v2/all";

                client.BaseAddress = new Uri("https://restcountries.eu/rest/v2/all");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetStringAsync("");
                response.Wait();

                if (!hasCountryJsonFile)
                {
                    var fileInfo = new FileInfo(ContryJsonFile);
                    if (!fileInfo.Directory.Exists)
                        fileInfo.Directory.Create();
                    File.WriteAllText(ContryJsonFile, response.Result);
                }

                using (var stringReader = new StringReader(response.Result))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    lock (_countries)
                    {
                        _countries = new JsonSerializer().Deserialize<IEnumerable<Types.Country>>(jsonReader).ToArray();
                    }
                }

                return _countries;
            }
        }

        private static void SetLanguageDictionary()
        {
            _languages = Countries.SelectMany(c => c.Languages).Where(l => !string.IsNullOrEmpty(l.Iso)).ToHashSet(_languageComparer).ToArray();
            LanguageDicitionary = _languages.ToDictionary(language => language.Iso);
        }
    }
}