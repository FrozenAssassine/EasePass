using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasePass.Core.Database;
using EasePass.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class DatabaseComplexTests
    {
        [TestMethod]
        public async Task TestDetailedItemPreservation()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("securePass");

            try
            {
                DatabaseItem db = Database.CreateNewDatabase(dbPath, password);
                PasswordManagerItem item = new PasswordManagerItem()
                {
                    DisplayName = "Detailed service",
                    Password = "superSecretPassword",
                    Username = "admin",
                    Email = "admin@example.com",
                    Notes = "Some \n multi-line \n notes.",
                    Secret = "TOTPSECRET",
                    Digits = "8",
                    Interval = "60",
                    Algorithm = "SHA256",
                    Tags = new string[] { "Work", "Critical" },
                    Website = "https://example.com"
                };
                db.AddItem(item);
                await db.ForceSaveAsync();

                // Reload
                DatabaseItem dbLoaded = new DatabaseItem(dbPath);
                await dbLoaded.Load(password);

                Assert.HasCount(1, dbLoaded.Items);
                var loadedItem = dbLoaded.Items[0];

                Assert.AreEqual("Detailed service", loadedItem.DisplayName);
                Assert.AreEqual("superSecretPassword", loadedItem.Password);
                Assert.AreEqual("admin", loadedItem.Username);
                Assert.AreEqual("admin@example.com", loadedItem.Email);
                Assert.AreEqual("Some \n multi-line \n notes.", loadedItem.Notes);
                Assert.AreEqual("TOTPSECRET", loadedItem.Secret);
                Assert.AreEqual("8", loadedItem.Digits);
                Assert.AreEqual("60", loadedItem.Interval);
                Assert.AreEqual("SHA256", loadedItem.Algorithm);
                Assert.HasCount(2, loadedItem.Tags);
                Assert.AreEqual("https://example.com", loadedItem.Website);
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [TestMethod]
        public void TestClearOldClicksCache()
        {
            DatabaseItem db = new DatabaseItem("test.db");
            PasswordManagerItem item = new PasswordManagerItem() { DisplayName = "Item" };

            // Add recent click
            string today = DateTime.Now.ToString("dd.MM.yyyy");
            item.Clicks.Add(today);

            // Add old click (more than 365 days)
            string oldDate = DateTime.Now.AddDays(-400).ToString("dd.MM.yyyy");
            item.Clicks.Add(oldDate);

            db.AddItem(item);

            Assert.HasCount(2, item.Clicks);

            db.ClearOldClicksCache();

            Assert.HasCount(1, item.Clicks);
            Assert.AreEqual(today, item.Clicks[0]);
            Assert.DoesNotContain(oldDate, item.Clicks);
        }

        [TestMethod]
        public void TestSetNewPasswords()
        {
            DatabaseItem db = new DatabaseItem("test.db");

            var list1 = new ObservableCollection<PasswordManagerItem>();
            list1.Add(new PasswordManagerItem() { DisplayName = "A" });
            db.SetNewPasswords(list1);
            Assert.HasCount(1, db.Items);
            Assert.AreEqual("A", db.Items[0].DisplayName);

            var list2 = new ObservableCollection<PasswordManagerItem>();
            list2.Add(new PasswordManagerItem() { DisplayName = "B" });
            list2.Add(new PasswordManagerItem() { DisplayName = "C" });
            db.SetNewPasswords(list2);
            Assert.HasCount(2, db.Items);
            Assert.AreEqual("B", db.Items[0].DisplayName);
            Assert.AreEqual("C", db.Items[1].DisplayName);
        }

        [TestMethod]
        public async Task TestGetItemsFromFile()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("pw");

            try
            {
                DatabaseItem db = Database.CreateNewDatabase(dbPath, password);
                db.AddItem(new PasswordManagerItem() { DisplayName = "Item1" });
                db.AddItem(new PasswordManagerItem() { DisplayName = "Item2" });
                await db.ForceSaveAsync();

                var items = await Database.GetItemsFromFile(dbPath, password);
                Assert.HasCount(2, items);
                Assert.AreEqual("Item1", items[0].DisplayName);
                Assert.AreEqual("Item2", items[1].DisplayName);
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [TestMethod]
        public async Task TestIncorrectPassword()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("correct");
            var wrongPassword = DatabaseTestHelper.ToSecureString("wrong");

            try
            {
                DatabaseItem db = Database.CreateNewDatabase(dbPath, password);
                db.Items.Add(new PasswordManagerItem() { Email = "test1", DisplayName = "display name1" });
                db.Items.Add(new PasswordManagerItem() { Email = "test2", DisplayName = "display name2" });
                db.Items.Add(new PasswordManagerItem() { Email = "test3", DisplayName = "display name3" });
                await db.ForceSaveAsync();

                // Load with wrong password
                DatabaseItem dbWrong = new DatabaseItem(dbPath);
                var result = await dbWrong.Load(wrongPassword, false);
                Assert.IsFalse(result);
                Assert.HasCount(0, dbWrong.Items);

                // Load with correct password
                var result2 = await dbWrong.Load(password);
                Assert.IsTrue(result2);
                Assert.HasCount(3, dbWrong.Items);
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [TestMethod]
        public async Task TestDatabaseSettingsPersistence()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("pw");

            try
            {
                DatabaseItem db = Database.CreateNewDatabase(dbPath, password);
                // Modify settings
                db.Settings.UseSecondFactor = false; // Default is false, let's keep it false as we can't easily test interactions
                                                     // But we can check if other settings persist if added in future.
                                                     // Currently DatabaseSettings only has UseSecondFactor and SecondFactorType.
                db.Settings.SecondFactorType = EasePass.Core.Database.Enums.SecondFactorType.Authenticator;

                await db.ForceSaveAsync();

                DatabaseItem dbLoaded = new DatabaseItem(dbPath);
                await dbLoaded.Load(password);

                Assert.IsNotNull(dbLoaded.Settings);
                Assert.AreEqual(EasePass.Core.Database.Enums.SecondFactorType.Authenticator, dbLoaded.Settings.SecondFactorType);
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }
    }
}