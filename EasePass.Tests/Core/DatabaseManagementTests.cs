using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Linq;
using EasePass.Core.Database;
using EasePass.Settings;
using EasePass.Models;
using System.Threading.Tasks;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class DatabaseManagementTests
    {

        [TestInitialize]
        public void Setup()
        { 
            //setup code if needed
        }

        [TestCleanup]
        public void Cleanup()
        {
            //I clear databases directly in the finally branch
        }

        [TestMethod]
        public void TestAddDatabasePath()
        {
             string newPath = DatabaseTestHelper.GetTempDatabasePath();
             File.WriteAllText(newPath, "dummy");
             try
             {
                 Database.AddDatabasePath(newPath);
                 var paths = Database.GetAllDatabasePaths();
                 Assert.IsTrue(paths.Contains(newPath), "Database path should be added.");
             }
             finally
             {
                 if(File.Exists(newPath)) File.Delete(newPath);
             }
        }

        [TestMethod]
        public void TestRemoveDatabasePath()
        {
             string newPath = DatabaseTestHelper.GetTempDatabasePath();
             File.WriteAllText(newPath, "dummy");
             try
             {
                 Database.AddDatabasePath(newPath);
                 var paths = Database.GetAllDatabasePaths();
                 Assert.IsTrue(paths.Contains(newPath));

                 Database.RemoveDatabasePath(newPath);
                 paths = Database.GetAllDatabasePaths();
                 Assert.IsFalse(paths.Contains(newPath), "Database path should be removed.");
             }
             finally
             {
                 if(File.Exists(newPath)) File.Delete(newPath);
             }
        }
        
        [TestMethod]
        public async Task TestCreateNewDatabase()
        {
            string dbPath = DatabaseTestHelper.GetTempDatabasePath();
            var password = DatabaseTestHelper.ToSecureString("password123");
            
            try
            {
                var db = await Database.CreateNewDatabase(dbPath, password);
                Assert.IsNotNull(db);
                Assert.AreEqual(dbPath, (db.DatabaseSource as NativeDatabaseSource)?.Path);
                Assert.IsTrue(File.Exists(dbPath));
            }
            finally
            {
                if(File.Exists(dbPath)) File.Delete(dbPath);
            }
        }

        [TestMethod]
        public void TestHasDatabasePath()
        {
             // Ensure clean state
             AppSettings.DatabasePaths = "";
             AppSettings.LoadedDatabaseName = null;
             Assert.IsFalse(Database.HasDatabasePath());

             string newPath = DatabaseTestHelper.GetTempDatabasePath();
             File.WriteAllText(newPath, "dummy");
             try
             {
                 Database.AddDatabasePath(newPath);
                 // Need to mimic loaded database behavior
                 AppSettings.LoadedDatabaseName = "dummy";
                 Assert.IsTrue(Database.HasDatabasePath());
             }
             finally
             {
                 if(File.Exists(newPath)) File.Delete(newPath);
             }
        }

        [TestMethod]
        public void TestLoadedInstanceUpdatesSettings()
        {
             string dbPath = DatabaseTestHelper.GetTempDatabasePath();
             File.WriteAllText(dbPath, "dummy");
             
             try 
             {
                 DatabaseItem db = new DatabaseItem(dbPath);
                 Database.LoadedInstance = db;

                Assert.AreEqual(db.Name, AppSettings.LoadedDatabaseName);
             }
             finally
             {
                 if(File.Exists(dbPath)) File.Delete(dbPath);
             }
        }
    }
}
