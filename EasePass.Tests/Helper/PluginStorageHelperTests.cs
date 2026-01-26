using EasePass.Helper.FileSystem;
using EasePass.Tests.Core;
using EasePassExtensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace EasePass.Tests.Helper
{
    [TestClass]
    public class PluginStorageHelperTests
    {
        [TestMethod]

        public void SaveLoadString()
        {
            var settings = new TestStorageInjectableClass();
            PluginStorageHelper.Initialize(settings, "testplugin", DatabaseTestHelper.GetTempPath());

            string testval = "test Value";
            settings.SaveString("mykey", testval);
            Assert.AreEqual(testval, settings.LoadString("mykey"));
        }


        [TestMethod]
        public void SaveLoadFile()
        {
            var settings = new TestStorageInjectableClass();
            PluginStorageHelper.Initialize(settings, "testplugin", DatabaseTestHelper.GetTempPath());

            string fileContent = string.Join(", ", Enumerable.Range(0, 50).Select(x => "Hello World" + x));
            settings.SaveFile("tests.text.easeapsstest", Encoding.UTF8.GetBytes(fileContent));
            string retrievedFileData = Encoding.UTF8.GetString(settings.LoadFile("tests.text.easeapsstest"));
            Assert.AreEqual(fileContent, retrievedFileData);
        }

        [TestMethod]
        public void SaveLoadSameKey()
        {
            var settingsP1 = new TestStorageInjectableClass();
            PluginStorageHelper.Initialize(settingsP1, "testUUID1", DatabaseTestHelper.GetTempPath());

            var settingsP2 = new TestStorageInjectableClass();
            PluginStorageHelper.Initialize(settingsP2, "testUUID2", DatabaseTestHelper.GetTempPath());

            string testvalP1 = "First Test Value";
            string testvalP2 = "Second Test Value";

            settingsP1.SaveString("key1", testvalP1);
            settingsP2.SaveString("key1", testvalP2);

            Assert.AreEqual(testvalP1, settingsP1.LoadString("key1"));
            Assert.AreEqual(testvalP2, settingsP2.LoadString("key1"));
        }

        [TestMethod]
        public void TestDeleteKey()
        {
            var settings = new TestStorageInjectableClass();
            PluginStorageHelper.Initialize(settings, "testplugin", DatabaseTestHelper.GetTempPath());
            string testval = "test Value";
            settings.SaveString("mykey", testval);
            Assert.AreEqual(testval, settings.LoadString("mykey"));

            PluginStorageHelper.Clean("testplugin");

            Assert.IsTrue(string.IsNullOrEmpty(settings.LoadString("mykey")));
        }
    }

    class TestStorageInjectableClass : IStorageInjectable
    {
        public SaveString SaveString { get; set; }
        public LoadString LoadString { get; set; }
        public SaveFile SaveFile { get; set; }
        public LoadFile LoadFile { get; set; }
    }
}
