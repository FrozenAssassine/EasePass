using EasePass.Core.Database;
using EasePass.Extensions;
using EasePass.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class DatabaseItemTests
    {

        [TestMethod]
        public void TestAddItem()
        {
             DatabaseItem db = new DatabaseItem("test.db");
             PasswordManagerItem item = new PasswordManagerItem() { DisplayName = "Test Item" };
             db.AddItem(item);

             Assert.HasCount(1, db.Items);
             Assert.AreEqual(item, db.Items[0]);
        }
        
        [TestMethod]
        public void TestDeleteItem()
        {
             DatabaseItem db = new DatabaseItem("test.db");
             PasswordManagerItem item = new PasswordManagerItem() { DisplayName = "Test Item" };
             db.AddItem(item);
             db.DeleteItem(item);

             Assert.IsEmpty(db.Items);
        }

        [TestMethod]
        public async Task TestLoadAndSave()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("securePass");
             
            try
            {
                // Create and Save
                Debug.WriteLine(dbPath);
                DatabaseItem db = Database.CreateNewDatabase(dbPath, password);
                PasswordManagerItem item = new PasswordManagerItem() { DisplayName = "Google", Password = "123", Username = "user" };
                db.AddItem(item);
                bool saved = await db.ForceSaveAsync();
                db.Dispose();
                Assert.IsTrue(saved, "Save returned false");

                // Reload
                DatabaseItem dbLoaded = new DatabaseItem(dbPath);
                var loaded = await dbLoaded.Load(password, false);
                 
                Assert.IsTrue(loaded, "Load returned false");
                Assert.HasCount(1, dbLoaded.Items);
                Assert.AreEqual("Google", dbLoaded.Items[0].DisplayName);
            }
            finally
            {
                if(File.Exists(dbPath)) File.Delete(dbPath);
            }
        }
        
        [TestMethod]
        public void TestPasswordAlreadyExists()
        {
             DatabaseItem db = new DatabaseItem("test.epdb");
             db.AddItem(new PasswordManagerItem() { Password = "abc" });
             Assert.IsTrue(db.PasswordAlreadyExists("abc"));
             Assert.IsFalse(db.PasswordAlreadyExists("def"));
        }

        [TestMethod]
        public async Task TestChangePassword()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var oldPass = "old".ConvertToSecureString();
            var newPass = "new".ConvertToSecureString();

            try
            {
                Debug.WriteLine(dbPath);
                DatabaseItem db = Database.CreateNewDatabase(dbPath, oldPass);
                db.AddItem(new PasswordManagerItem() { DisplayName = "Test" });
                await db.ForceSaveAsync();

                DatabaseItem checker = new DatabaseItem(dbPath);
                var check = await checker.CheckPasswordCorrect(oldPass);
                Assert.AreEqual(PasswordValidationResult.Success, check.result, "Initial password should be valid");

                bool loaded = await checker.Load(oldPass);
                Assert.IsTrue(loaded, "Should load with the initial password");

                Database.LoadedInstance.MasterPassword = newPass;
                await Database.LoadedInstance.ForceSaveAsync();

                Database.LoadedInstance.Dispose();

                // Reload with new password -> must succeed
                DatabaseItem dbNew = new DatabaseItem(dbPath);
                var result = await dbNew.Load(newPass);
                Assert.IsTrue(result, "Should load with new password");

                // Try old password -> must fail
                DatabaseItem dbOld = new DatabaseItem(dbPath);
                var resultOld = await dbOld.Load(oldPass, false);
                Assert.IsFalse(resultOld, "Should not load with old password");
            }
            finally
            {
                if (File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [TestMethod]
        public void TestUnloadDispose()
        {
             DatabaseItem db = new DatabaseItem("t.db");
             db.AddItem(new PasswordManagerItem());
             
             Database.LoadedInstance = db;
             db.Dispose();
             
             Assert.IsEmpty(db.Items);
             Assert.IsNull(db.MasterPassword);
        }

        [TestMethod]
        public void TestFindItems()
        {
            DatabaseItem db = new DatabaseItem("t.db");
            db.AddItem(new PasswordManagerItem() { DisplayName = "Alpha" });
            db.AddItem(new PasswordManagerItem() { DisplayName = "Beta" });

            var result = db.FindItemsByName("Alp");
            
            Assert.HasCount(1, result);
            Assert.AreEqual("Alpha", result[0].DisplayName);
        }
    }
}
