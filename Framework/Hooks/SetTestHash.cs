using SpecFlow.Extensions.Framework.ExtensionMethods;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpecFlow.Extensions.Framework.Hooks
{
    [Binding]
    public sealed class SetTestHash
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            StringRandomize.FeatureHash = GetUpperCaseOrFirstCharacterOfEachWord(FeatureContext.Current.FeatureInfo.Title);
            StringRandomize.ScenarioHash = GetUpperCaseOrFirstCharacterOfEachWord(ScenarioContext.Current.ScenarioInfo.Title);
        }

        private static string GetUpperCaseOrFirstCharacterOfEachWord(string sentence)
        {
            return string.Join(string.Empty, sentence.Where((character, index) => char.IsUpper(character) || (character != ' ' && (index == 0 || sentence[index - 1] == ' '))));
        }
    }
}