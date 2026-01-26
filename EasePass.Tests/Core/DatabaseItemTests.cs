using EasePass.Core.Database;
using EasePass.Extensions;
using EasePass.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
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
             DatabaseItem db = new DatabaseItem("test.epdb");
             PasswordManagerItem item = new PasswordManagerItem() { DisplayName = "Test Item" };
             db.AddItem(item);

             Assert.HasCount(1, db.Items);
             Assert.AreEqual(item, db.Items[0]);
        }
        
        [TestMethod]
        public void TestDeleteItem()
        {
             DatabaseItem db = new DatabaseItem("test.epdb");
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
                DatabaseItem db = await Database.CreateNewDatabase(dbPath, password);
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
                DatabaseItem db = await Database.CreateNewDatabase(dbPath, oldPass);
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
             DatabaseItem db = new DatabaseItem("t.epdb");
             db.AddItem(new PasswordManagerItem());
             
             Database.LoadedInstance = db;
             db.Dispose();
             
             Assert.IsEmpty(db.Items);
             Assert.IsNull(db.MasterPassword);
        }

        [TestMethod]
        public void TestFindItems()
        {
            DatabaseItem db = new DatabaseItem("t.epdb");
            db.AddItem(new PasswordManagerItem() { DisplayName = "Alpha" });
            db.AddItem(new PasswordManagerItem() { DisplayName = "Beta" });

            var result = db.FindItemsByName("Alp");
            
            Assert.HasCount(1, result);
            Assert.AreEqual("Alpha", result[0].DisplayName);
        }

        [TestMethod]
        public async Task SaveDatabase()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var pw = "mysuperlongpassword!".ConvertToSecureString();

            for (int i = 0; i < 20; i++)
            {
                DatabaseItem db = new DatabaseItem(dbPath);
                if(i == 0)
                    db.MasterPassword = pw;
                else
                    await db.Load(pw);
                db.AddItem(new PasswordManagerItem() { 
                    DisplayName = "Item1" + i * i,
                    Email = "emailAddress@emailprovider.com", 
                    Password = "mycoolpassword" + i *i,
                    Notes = "These are some notes\nLine2\nLine3",
                    Username = "myusername" + i,
                    Website = "https://mywebsite" + i + ".com",
                    Tags = ["tag1", "tag2"],
                });
                await db.ForceSaveAsync();
                db.Dispose();
                await Task.Delay(new Random().Next(10, 200));
            }

            Assert.IsTrue(File.Exists(dbPath), "Database file was not created.");
            DatabaseItem finalDb = new DatabaseItem(dbPath);
            await finalDb.Load(pw, false);
            Assert.HasCount(20, finalDb.Items);
        }

        [TestMethod]
        public async Task SaveDatabaseScheduled()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var pw = "mysuperlongpassword!".ConvertToSecureString();

            DatabaseItem db = new DatabaseItem(dbPath);
            db.MasterPassword = pw;;
            
            for (int i = 0; i < 20; i++)
            {
                db.AddItem(new PasswordManagerItem()
                {
                    DisplayName = "Item1" + i * i,
                    Email = "emailAddress@emailprovider.com",
                    Password = "mycoolpassword" + i * i,
                    Notes = "These are some notes\nLine2\nLine3",
                    Username = "myusername" + i,
                    Website = "https://mywebsite" + i + ".com",
                    Tags = ["tag1", "tag2"],
                });
                await db.SaveAsync();
                await Task.Delay(new Random().Next(100, 2000));
            }

            //simulate app close
            if (db.deferredSaver.SaveScheduled)
                await db.ForceSaveAsync();

            Assert.IsTrue(File.Exists(dbPath), "Database file was not created.");
            DatabaseItem finalDb = new DatabaseItem(dbPath);
            await finalDb.Load(pw, false);
            Assert.HasCount(20, finalDb.Items);
        }

        [TestMethod]
        public async Task DatabaseReadonlyTests()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var pw = "mysuperlongpassword!".ConvertToSecureString();

            DatabaseItem db = new DatabaseItem(dbPath, isReadOnly: false);
            db.MasterPassword = pw;

            //create a database with some data
            for (int i = 0; i < 20; i++)
            {
                db.AddItem(new PasswordManagerItem()
                {
                    DisplayName = "Item1" + i * i,
                    Email = "emailAddress@emailprovider.com",
                    Password = "mycoolpassword" + i * i,
                    Notes = "These are some notes\nLine2\nLine3",
                    Username = "myusername" + i,
                    Website = "https://mywebsite" + i + ".com",
                    Tags = ["tag1", "tag2"],
                });
            }
            Assert.HasCount(20, db.Items);
            await db.ForceSaveAsync();
            var saveTime = File.GetLastWriteTime(dbPath);

            //now the readonly database
            DatabaseItem readOnlyDB = new DatabaseItem(dbPath, isReadOnly: true);
            await readOnlyDB.Load(pw, false);

            Debug.WriteLine(readOnlyDB.DatabaseSource.IsReadOnly);
            Debug.WriteLine(readOnlyDB.IsReadonlyDatabase);

            //now try to edit. Should not work!
            for (int i = 0; i < 10; i++)
            {
                readOnlyDB.DeleteItem(readOnlyDB.Items[i]);
            }

            for (int i = 0; i < 5; i++)
            {
                readOnlyDB.AddItem(new PasswordManagerItem()
                {
                    DisplayName = "Item1" + i * i,
                    Email = "emailAddress@emailprovider.com",
                    Username = "myusername" + i,
                    Password = "mycoolpassword" + i * i,
                });
            }

            readOnlyDB.AddRange(new PasswordManagerItem[] { new PasswordManagerItem()
            {
                DisplayName = "Item1",
                Email = "emailAddress@emailprovider.com",
                Username = "myusername",
                Password = "mycoolpassword",
            }});

            readOnlyDB.AddRange(new ObservableCollection<PasswordManagerItem> { new PasswordManagerItem()
            {
                DisplayName = "Item1",
                Email = "emailAddress@emailprovider.com",
                Username = "myusername",
                Password = "mycoolpassword",
            }});

            readOnlyDB.SetNewPasswords(new ObservableCollection<PasswordManagerItem> { new PasswordManagerItem()
            {
                DisplayName = "Item1",
                Email = "emailAddress@emailprovider.com",
                Username = "myusername",
                Password = "mycoolpassword",
            }});

            readOnlyDB.SetNewPasswords(new PasswordManagerItem[] { new PasswordManagerItem()
            {
                DisplayName = "Item1",
                Email = "emailAddress@emailprovider.com",
                Username = "myusername",
                Password = "mycoolpassword",
            }});

            await readOnlyDB.ForceSaveAsync();
            var readonlyDBSaveTime = File.GetLastWriteTime(dbPath);

            //no edits should have happened
            Assert.AreEqual(saveTime.ToFileTime(), readonlyDBSaveTime.ToFileTime());
            Assert.HasCount(20, readOnlyDB.Items);
        }
    }
}
