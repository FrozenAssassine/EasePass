using EasePass.Helper.Security.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Tests.Core
{
    [TestClass]
    public class TwoFactorAuthTests
    {
        [TestMethod]
        public void TestTotpGenerator()
        {
            string secret = "JBSWY3DPEHPK3PXP";
            int numOfDigits = 6;
            int periodsSec = 30;
            string expectedTOTP = "693453";
            DateTime dt = new DateTime(2026, 1, 24, 15, 47, 35, DateTimeKind.Utc);

            string token = TOTP.GenerateTOTPToken(dt, secret, numOfDigits, periodsSec);

            Assert.AreEqual(expectedTOTP, token);
        }
    }
}
