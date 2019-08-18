using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Tests
{
    [TestClass]
    public class TranslateMethodTest
    {
        [TestMethod]
        public void TestTranslateMethod()
        {
            TranslationService translationService = new TranslationService();

            var avaliableEngines = TranslationService.GetAvaliableEngines();
            foreach (var avaliableEngine in avaliableEngines)
            {
                translationService.AddEngine(avaliableEngine);
            }

            var result = translationService.Translate(new TranslateArgs("de", "en", "Stuhl"));
            Assert.IsTrue(result.Count() == avaliableEngines.Length);
        }  
        
        [TestMethod]
        public void TestGetAllEngines()
        {
            TranslationService translationService = new TranslationService();
            var a = TranslationService.GetAvaliableEngines();
        }
    }
}