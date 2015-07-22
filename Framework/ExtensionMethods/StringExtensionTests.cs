using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SpecFlow.Extensions.Framework.ExtensionMethods
{
    [TestFixture]
    public class StringExtensionTests
    {
        [SetUp]
        public void ResetStringExtensionHashes()
        {
            StringExtension.TesterHash = string.Empty;
            StringExtension.TestHash = string.Empty;
        }


        [Test]
        public void StringExtensionCreatesHash()
        {
            Assert.AreEqual(2, StringExtension.GenerateRandomHash(2).Length);
            Assert.AreEqual(4, StringExtension.GenerateRandomHash(4).Length);
            Assert.AreEqual(6, StringExtension.GenerateRandomHash(6).Length);
        }

        [Test]
        public void StringExtensionAppendsHash()
        {
            string temp = "temp";

            Assert.IsTrue(temp.Randomize().Contains(DateTime.Now.ToString("-yyyyMMddHHmmss")));
        }

        [Test]
        public void StringExtensionAppendsShortHash()
        {
            string temp1 = "temp";
            string temp2 = temp1.RandomizeShort();

            Assert.IsFalse(temp1.RandomizeShort().Contains(DateTime.Now.ToString("-yyyyMMddHHmmss")));
            Assert.AreEqual(13, temp1.RandomizeShort().Length, temp1);
            Assert.AreEqual(13, temp2.RandomizeShort().Length, temp2);
            Assert.IsTrue(temp1 != temp2);
        }

        [Test]
        public void StringExtensionUpdatesHash()
        {
            List<string> hashes = new List<string>();

            for (int i = 0; i < 500; i++)
            {
                string temp = "temp";
                string temp1 = temp.Randomize();
                string temp2 = temp1.Randomize();

                hashes.Add(temp1);
                hashes.Add(temp2);

                Assert.AreEqual(temp1.Substring(0, 23), temp2.Substring(0, 23));
                Assert.AreNotEqual(temp1.Substring(23, 4), temp2.Substring(23, 4), string.Format("temp1:{0} temp2:{1}", temp1, temp2));
            }

            Assert.AreEqual(hashes.Count, hashes.Distinct().Count());
        }

        [Test]
        public void StringExtensionSetsHash()
        {
            string tester = "ELH";
            string test = "SESH";
            string temp = "temp";

            StringExtension.TestHash = test;

            Assert.IsTrue(temp.Randomize().StartsWith(test + "-temp"));

            StringExtension.TesterHash = tester;

            Assert.IsTrue(temp.Randomize().StartsWith(tester + "-" + test + "-temp"));

            StringExtension.TestHash = string.Empty;

            Assert.IsTrue(temp.Randomize().StartsWith(tester + "-temp-"));
            

                        
        }
    }
}
