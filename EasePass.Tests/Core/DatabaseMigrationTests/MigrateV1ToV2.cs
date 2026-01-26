using EasePass.Core.Database;
using EasePass.Helper.Database;
using EasePass.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Tests.Core.DatabaseMigrationTests
{
    [TestClass]
    public class MigrateV1ToV2
    {
        [TestMethod]
        public async Task TestMigrateDb1()
        {
            var pw = DatabaseTestHelper.ToSecureString("supersecretPW!");
            var v1DB = MigrateV1ToV2_Helper.CreateV2DB(pw);

            for (int i = 0; i < 20; i++)
            {
                v1DB.AddItem(new V1PasswordManagerItem()
                {
                    DisplayName = "Item" + i,
                    Email = "Email" + i,
                    Notes = "Notes\n" + i,
                    Password = "Password" + i * i,
                    Username = "Username" + i + 5,
                    Website = "https://website" + i + ".com",
                    Algorithm = "SHA256",
                    Interval = "30",
                    Digits = "6",
                    Secret = "asdasd",
                });
            }
            v1DB.Save();

            Assert.IsTrue(v1DB.Unlock(pw));
            Assert.IsTrue(File.Exists(v1DB.Path));

            //migrate the db
            DatabaseItem migratedDB = new DatabaseItem(new NativeDatabaseSource(v1DB.Path));
            bool unlocked = await migratedDB.Unlock(pw);
            Assert.IsTrue(unlocked);
            await migratedDB.ForceSaveAsync();
            migratedDB.Dispose();

            //load again to verify
            migratedDB = new DatabaseItem(new NativeDatabaseSource(v1DB.Path));
            await migratedDB.Load(pw, false);

            Assert.IsTrue(await migratedDB.Unlock(pw));
            Assert.HasCount(v1DB.Items.Count, migratedDB.Items);
            Assert.AreEqual(v1DB.Name, migratedDB.Name);
            for (int i = 0; i < v1DB.Items.Count; i++)
            {
                var v1Item = v1DB.Items[i];
                var v2Item = migratedDB.Items[i];
                Assert.AreEqual(v1Item.DisplayName, v2Item.DisplayName);
                Assert.AreEqual(v1Item.Email, v2Item.Email);
                Assert.AreEqual(v1Item.Notes, v2Item.Notes);
                Assert.AreEqual(v1Item.Password, v2Item.Password);
                Assert.AreEqual(v1Item.Username, v2Item.Username);
                Assert.AreEqual(v1Item.Website, v2Item.Website);
                Assert.AreEqual(v1Item.Algorithm, v2Item.Algorithm);
                Assert.AreEqual(v1Item.Interval, v2Item.Interval);
                Assert.AreEqual(v1Item.Digits, v2Item.Digits);
                Assert.AreEqual(v1Item.Secret, v2Item.Secret);
            }

            var versionTagRes = DatabaseVersionTagHelper.GetVersionTag(await migratedDB.DatabaseSource.GetDatabaseFileBytes());
            Assert.AreEqual(DatabaseVersionTag.EpdbV2DbVersion, versionTagRes.versionTag);
        }
    }
}