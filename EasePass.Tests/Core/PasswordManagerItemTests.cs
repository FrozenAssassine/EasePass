using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class PasswordManagerItemTests
    {
        [TestMethod]
        public void TestPropertyNotifications()
        {
            var item = new PasswordManagerItem();
            string changedProp = null;
            item.PropertyChanged += (s, e) => { changedProp = e.PropertyName; };

            item.Password = "pass";
            Assert.AreEqual("Password", changedProp);

            item.Username = "user";
            Assert.AreEqual("Username", changedProp);

            item.Email = "mail";
            Assert.AreEqual("Email", changedProp);

            item.Notes = "notes";
            Assert.AreEqual("Notes", changedProp);

            item.Tags = new string[] { "tag" };
            Assert.AreEqual("Tags", changedProp);
        }

        [TestMethod]
        public void TestDisplayNameLogic()
        {
            var item = new PasswordManagerItem();
            bool firstCharChanged = false;
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "FirstChar") firstCharChanged = true;
            };

            item.DisplayName = "Google";
            Assert.AreEqual("G", item.FirstChar);
            Assert.IsTrue(firstCharChanged);

            item.DisplayName = "";
            Assert.AreEqual("", item.FirstChar);

            item.DisplayName = null;
            Assert.AreEqual("", item.FirstChar);
        }

        [TestMethod]
        public void TestWebsiteUrlFormatting()
        {
            // Ensure icons are enabled for this test
            AppSettings.ShowIcons = true;

            var item = new PasswordManagerItem();
            Assert.IsTrue(item.ShowIcon);

            // Test case: no protocol
            item.Website = "google.com";
            Assert.AreEqual("http://google.com", item.Website);

            // Test case: http protocol
            item.Website = "http://example.com";
            Assert.AreEqual("http://example.com", item.Website);

            // Test case: https protocol
            item.Website = "https://example.com";
            Assert.AreEqual("https://example.com", item.Website);

            // Test case: mixed case protocol
            item.Website = "HTTP://TEST.COM";
            Assert.AreEqual("HTTP://TEST.COM", item.Website);

            // Test case: trimming
            item.Website = "  test.com  ";
            Assert.AreEqual("http://test.com", item.Website);

            // Test case: null
            item.Website = null;
            Assert.IsNull(item.Website);
        }

        [TestMethod]
        public void TestWebsiteUrlFormatting_IconsDisabled()
        {
            AppSettings.ShowIcons = false;
            var item = new PasswordManagerItem();
            Assert.IsFalse(item.ShowIcon);

            // When icons are disabled, the logic returns early before adding http://
            item.Website = "google.com";
            Assert.AreEqual("google.com", item.Website);

            item.Website = "  test.com  ";
            Assert.AreEqual("test.com", item.Website);
        }

        [TestMethod]
        public void TestShowIconSettingChange()
        {
            AppSettings.ShowIcons = false;
            var item = new PasswordManagerItem { Website = "example.com" };

            // Should be clean without http because icons off
            Assert.AreEqual("example.com", item.Website);

            // Enable icons - should trigger update via event handler in constructor
            AppSettings.ShowIcons = true;

            Assert.IsTrue(item.ShowIcon);
            Assert.AreEqual("http://example.com", item.Website);
        }

        /* 
        //Needs fix, because it has to run on UI thread. but no idea how to get this working
        [TestMethod]
        public void TestColorGeneration()
        {
            var item = new PasswordManagerItem { DisplayName = "Test" };
            Debug.WriteLine(item.BackColor.Opacity);
            Assert.IsNotNull(item.BackColor);
            Assert.IsInstanceOfType(item.BackColor, typeof(SolidColorBrush));
            Assert.IsNotNull(item.ForeColor);
            Assert.IsInstanceOfType(item.ForeColor, typeof(SolidColorBrush));

            // Deterministic check
            var item2 = new PasswordManagerItem { DisplayName = "Test" };
            var brush1 = (SolidColorBrush)item.BackColor;
            var brush2 = (SolidColorBrush)item2.BackColor;
            Assert.AreEqual(brush1.Color, brush2.Color);
        }*/

        [TestMethod]
        public void TestSerialization()
        {
            var items = new ObservableCollection<PasswordManagerItem>
            {
                new PasswordManagerItem
                {
                    DisplayName = "Item1",
                    Username = "User1",
                    Password = "Pwd",
                    Tags = new string[] { "A", "B" },
                    // 2FA properties (ignored in logic tests but checked for serialization)
                    Digits = "6"
                },
                new PasswordManagerItem
                {
                    DisplayName = "Item2",
                    Digits = "4"
                }
            };

            string json = PasswordManagerItem.SerializeItems(items);
            Assert.IsFalse(string.IsNullOrEmpty(json));

            var loaded = PasswordManagerItem.DeserializeItems(json);
            Assert.HasCount(2, loaded);

            Assert.AreEqual("Item1", loaded[0].DisplayName);
            Assert.AreEqual("User1", loaded[0].Username);
            Assert.AreEqual("Pwd", loaded[0].Password);
            Assert.HasCount(2, loaded[0].Tags);
            Assert.AreEqual("A", loaded[0].Tags[0]);
            Assert.AreEqual("6", loaded[0].Digits);

            Assert.AreEqual("Item2", loaded[1].DisplayName);
            Assert.AreEqual("4", loaded[1].Digits);
        }

        [TestMethod]
        public void TestSerialization_InvalidJson()
        {
            var result = PasswordManagerItem.DeserializeItems("invalid json");
            Assert.IsNull(result);
        }
           
        [TestMethod]
        public void TestSerialization_Empty()
        {
             var items = new ObservableCollection<PasswordManagerItem>();
             string json = PasswordManagerItem.SerializeItems(items);
             var loaded = PasswordManagerItem.DeserializeItems(json);
             Assert.IsNotNull(loaded);
             Assert.HasCount(0, loaded);
        }
    }
}
