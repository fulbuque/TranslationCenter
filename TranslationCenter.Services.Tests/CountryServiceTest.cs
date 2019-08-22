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
            var countries = CountryService.Countries;
            Assert.IsTrue(countries.Any());
        }

        [TestMethod]
        public void TestGetAllCountries2()
        {
            var languagesNames = new string[] { "de", "en", "pt", "es", "cn" }.Select(iso => CountryService.GetLanguageName(iso)).ToArray();
            Assert.IsTrue(languagesNames.Any());
        }
    }
}