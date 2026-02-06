using EasePass.Core.Database.Enums;
using EasePass.Core.Database.Format.epdb;
using EasePass.Core.Database.Format.Serialization;
using EasePass.Extensions;
using EasePass.Helper.Database;
using EasePass.Helper.Security;
using EasePass.Models;
using EasePassExtensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class DatabaseLocaleTests
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        [TestInitialize]
        public void Setup()
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
            originalUICulture = Thread.CurrentThread.CurrentUICulture;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }

        [TestMethod]
        public async Task TestLoadDifferentLocales()
        {
            var password = DatabaseTestHelper.ToSecureString("testPassword");
            var items = new ObservableCollection<PasswordManagerItem>();
            items.Add(new PasswordManagerItem() { DisplayName = "TestItem", Password = "SecretPassword" });
            var settings = new DatabaseSettings();
            var source = new MockDatabaseSource();

            // Save in en-US
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            bool saved = await MainDatabaseLoader.Save(source, password, null, settings, items);
            Assert.IsTrue(saved, "Should save successfully in en-US");
            byte[] dbBytes = await source.GetDatabaseFileBytes();

            // Load in de-DE
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            var result = await MainDatabaseLoader.Load(source, password, false, dbBytes);
            
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load successfully in de-DE");
            Assert.IsNotNull(result.database, "Database object should not be null");
            Assert.AreEqual(1, result.database.Items.Count);
            Assert.AreEqual("TestItem", result.database.Items[0].DisplayName);

            // 3. Load in tr-TR (Turkish has unique casing rules and uses comma for decimals)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            result = await MainDatabaseLoader.Load(source, password, false, dbBytes);
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load successfully in tr-TR");
        }

        [TestMethod]
        public async Task TestLoadLegacyDatabase_CommaDecimal()
        {
            // Create a DB file using "1,4" associated data (simulating old bug in comma locale)
            var passwordStr = "testPassword";
            var password = DatabaseTestHelper.ToSecureString(passwordStr);
            var items = new ObservableCollection<PasswordManagerItem>();
            items.Add(new PasswordManagerItem() { DisplayName = "LegacyItem", Password = "LegacyPassword" });
            
            byte[] dbContent = CreateDatabaseContent(items, passwordStr, "1,4");
            var source = new MockDatabaseSource(dbContent);

            // Try load in en-US
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var result = await MainDatabaseLoader.Load(source, password, false);
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load legacy comma DB in en-US");
            Assert.AreEqual("LegacyItem", result.database.Items[0].DisplayName);

            // Try load in de-DE
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            result = await MainDatabaseLoader.Load(source, password, false, dbContent);
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load legacy comma DB in de-DE");
        }

        [TestMethod]
        public async Task TestLoadDatabase_DotDecimal()
        {
            // Create a DB file using "1.4" associated data (standard format)
            var passwordStr = "testPassword";
            var password = DatabaseTestHelper.ToSecureString(passwordStr);
            var items = new ObservableCollection<PasswordManagerItem>();
            items.Add(new PasswordManagerItem() { DisplayName = "StandardItem", Password = "StandardPassword" });

            byte[] dbContent = CreateDatabaseContent(items, passwordStr, "1.4");
            var source = new MockDatabaseSource(dbContent);

            // Try load in en-US
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var result = await MainDatabaseLoader.Load(source, password, false);
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load standard dot DB in en-US");
            Assert.AreEqual("StandardItem", result.database.Items[0].DisplayName);

            // Try load in de-DE
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            result = await MainDatabaseLoader.Load(source, password, false, dbContent);
            Assert.AreEqual(PasswordValidationResult.Success, result.result, "Should load standard dot DB in de-DE");
        }

        private byte[] CreateDatabaseContent(ObservableCollection<PasswordManagerItem> items, string password, string versionString)
        {
            // Replicate MainDatabaseLoader.Save logic but with controlled associatedData
            
            byte[] salt = Encoding.UTF8.GetBytes("EasePassArgonHash");
            byte[] associatedData = Encoding.UTF8.GetBytes("Database_Version_" + versionString);
            
            string json = PasswordManagerItem.SerializeItems(items);

            byte[] pass;
            using (var securePW = DatabaseTestHelper.ToSecureString(password))
            {
                char[] base64Chars = securePW.ToBytes().ToBase64();
                Array.Reverse(base64Chars); // EasePass internal logic revers base64 pw
                pass = HashHelper.HashPasswordWithArgon2id(base64Chars, salt, associatedData);
            }
            
            byte[] data = EncryptDecryptHelper.EncryptStringAES(json, pass);

            DatabaseFile database = new DatabaseFile();
            database.DatabaseFileType = DatabaseFileType.epdb;
            database.Version = 1.4;
            database.Settings = new DatabaseSettings();
            database.Data = data;

            json = database.Serialize();

            // Outer layer encryption (No associated data)
            using (SecureString securePass = DatabaseTestHelper.ToSecureString(password))
            {
                pass = HashHelper.HashPasswordWithArgon2id(securePass, salt);
                data = EncryptDecryptHelper.EncryptStringAES(json, pass);
            }

            return DatabaseVersionTagHelper.AddVersionTag(data, (int)DatabaseVersionTag.EpdbV2DbVersion);
        }

        private class MockDatabaseSource : IDatabaseSource
        {
            private byte[] _content;

            public MockDatabaseSource(byte[] content = null)
            {
                _content = content ?? [];
            }

            public string DatabaseName => "MockDB";
            public string SourceDescription => "MockSource";
            public bool IsReadOnly => false;
            public Action OnPropertyChanged { get; set; }
            public IDatabaseSource.DatabaseAvailability Availability => IDatabaseSource.DatabaseAvailability.Available;
            public DateTime LastTimeModified => DateTime.Now;

            public Task<byte[]> GetDatabaseFileBytes()
            {
                return Task.FromResult(_content);
            }

            public Task<bool> SaveDatabaseFileBytes(byte[] databaseFileBytes)
            {
                _content = databaseFileBytes;
                return Task.FromResult(true);
            }

            public void Login() { }
            public void Logout() { }
        }
    }
}
