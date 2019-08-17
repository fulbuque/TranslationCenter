using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TranslationCenter.Services.Translation;
using TranslationCenter.Services.Translation.Engines;
using TranslationCenter.Services.Translation.Types;

namespace TranslationCenter.Services.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TranslationService translationService = new TranslationService();
            translationService.AddEngine<BingEngine>();
            translationService.AddEngine<LeoEngine>();
            var x = translationService.Translate(new TranslateArgs("de", "pt", "Stuhl"));
        }

    }
}
