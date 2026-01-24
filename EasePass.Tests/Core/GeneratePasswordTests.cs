using EasePass.Helper.Security.Generator;
using EasePass.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class GeneratePasswordTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            PasswordHelper.Init();
        }

        [TestInitialize]
        public void Init()
        {
            AppSettings.DisableLeakedPasswords = true;
            AppSettings.PasswordLength = 15;
            AppSettings.PasswordChars = DefaultSettingsValues.passwordChars;
        }

        [TestMethod]
        public async Task GeneratePassword_Success()
        {
            string password = await PasswordHelper.GeneratePassword();
            Assert.IsNotNull(password);
            Assert.AreEqual(15, password.Length);
        }

        [TestMethod]
        public async Task GeneratePassword_RespectsLength()
        {
            AppSettings.PasswordLength = 32;
            string password = await PasswordHelper.GeneratePassword();
            Assert.AreEqual(32, password.Length);

            AppSettings.PasswordLength = 8;
            password = await PasswordHelper.GeneratePassword();
            Assert.AreEqual(8, password.Length);
        }

        [TestMethod]
        public async Task GeneratePassword_RespectsCharacters()
        {
            AppSettings.PasswordChars = "abc";
            AppSettings.PasswordLength = 10;
            string password = await PasswordHelper.GeneratePassword();
            Assert.IsTrue(password.All(c => c == 'a' || c == 'b' || c == 'c'));
        }

        [TestMethod]
        public async Task GeneratePassword_IsSecure()
        {
            AppSettings.PasswordLength = 20;
            AppSettings.PasswordChars = DefaultSettingsValues.passwordChars;

            string password = await PasswordHelper.GeneratePassword();

            StringBuilder sb = new StringBuilder(password);
            Assert.IsTrue(PasswordHelper.IsSecure(sb, 20), "Generated password should be secure");
        }

        [TestMethod]
        public void IsSecure_DetectsCommonSequences()
        {
            // Note: "password"
            StringBuilder sb = new StringBuilder("password");

            // If we disable all complexity requirements, IsSecure should only check for CommonSequences
            // bool includeUpper = false means we don't require Upper.
            // In IsSecure implementation: bool upper = !includeUpper; (so upper is true/satisfied)
            // If all are satisfied, the loop returns true immediately, UNLESS ContainsCommonSequences returns true before the loop.
            bool secure = PasswordHelper.IsSecure(sb, sb.Length, false, false, false, false);
            Assert.IsFalse(secure, "'password' is a common sequence");

            sb = new StringBuilder("xfpr492!_"); // Random gibberish
            secure = PasswordHelper.IsSecure(sb, sb.Length, false, false, false, false);
            Assert.IsTrue(secure, "Gibberish should not be common sequence");
        }

        [TestMethod]
        public void GetAllowedIncludes_CalculatesCorrectly()
        {
            var res = PasswordHelper.GetAllowedIncludes("a");
            Assert.IsTrue(res.includeLower);
            Assert.IsFalse(res.includeUpper);

            res = PasswordHelper.GetAllowedIncludes("A");
            Assert.IsFalse(res.includeLower);
            Assert.IsTrue(res.includeUpper);

            res = PasswordHelper.GetAllowedIncludes("1");
            Assert.IsTrue(res.includeNumber);

            res = PasswordHelper.GetAllowedIncludes("!");
            Assert.IsTrue(res.includeSpecial);
        }
    }
}
