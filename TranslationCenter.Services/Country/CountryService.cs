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

        public ICountry[] GetCountries()
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


        public ILanguage[] GetLanguages()
        {
            return GetCountries().SelectMany(c => c.Languages).ToHashSet(_languageComparer).OrderBy(l => l.Name).ToArray();
        }

        public Dictionary<string, string> GetTranslatedCountryNames(string iso)
        {
            var countries = GetCountries();

            var x = countries.SelectMany(c => c.Translations).Where(t => t.Key == iso).ToHashSet();

            return null;
        }
    }
}