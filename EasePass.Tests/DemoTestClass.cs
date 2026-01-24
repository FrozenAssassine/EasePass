using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Tests
{
    [TestClass]
    public class DemoTestClass
    {

        [TestMethod]
        public void TestName()
        {
            //create helper e.g. Core/DatabaseTestHelper.cs for repetitive actions

            //testing
            Assert.IsTrue(true);
            Assert.IsFalse(false);
            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        public async Task AsyncTestName()
        {
            //async tests work as well

            //testing
            Assert.IsTrue(true);
            Assert.IsFalse(false);
            Assert.AreEqual(0, 0);
        }
    }
}
