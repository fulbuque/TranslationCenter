using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TranslationCenter.Services.Country;

namespace TranslationCenter.Services.Tests
{
    [TestClass]
    public class CountryServiceTest
    {
        [TestMethod]
        public void TestGetAllCountries()
        {
            CountryService countryService = new CountryService();

            var countries = countryService.GetCountries();
            countries = countryService.GetCountries();

            Assert.IsTrue(countries.Any());
        }

        [TestMethod]
        public void TestGetAllCountries2()
        {
            CountryService countryService = new CountryService();

            var countries = countryService.GetTranslatedCountryNames("de");
            

            Assert.IsTrue(countries.Any());
        }
    }
}