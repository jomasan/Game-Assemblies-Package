using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameAssemblyLab.TestPackage;

namespace GameAssemblyLab.TestPackage.Tests
{
    public class TestPackageTests
    {
        [Test]
        public void TestPackage_SimplePasses()
        {
            // Use the Assert class to test conditions
            Assert.IsTrue(true);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public System.Collections.IEnumerator TestPackage_WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
