using SpecFlow.Extensions.Framework.ExtensionMethods;
using TechTalk.SpecFlow;
using System.Linq;

namespace SpecFlow.Extensions.Framework.Hooks
{
    [Binding]
    public sealed class SetTestHash
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            StringExtension.FeatureHash = GetUpperCaseOrFirstCharacterOfEachWord(FeatureContext.Current.FeatureInfo.Title);
            StringExtension.ScenarioHash = GetUpperCaseOrFirstCharacterOfEachWord(ScenarioContext.Current.ScenarioInfo.Title);
        }

        private static string GetUpperCaseOrFirstCharacterOfEachWord(string sentence)
        {
            return string.Join(string.Empty, sentence.Where((character, index) => char.IsUpper(character) || (character != ' ' && (index == 0 || sentence[index - 1] == ' '))));
        }
    }
}
