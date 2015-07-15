using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Hap.Cloud.Tests.SpecFlow.Web.Hooks
{
    [Binding]
    public sealed class EmptyScenarioIsInconclusive
    {
        private bool isEmpty = true;

        [BeforeStep]
        public void BeforeStep()
        {
            isEmpty = false;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (isEmpty)
                Assert.Inconclusive("The scenario is empty");
        }
    }
}
