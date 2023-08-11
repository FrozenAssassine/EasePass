using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace EasePass.Helper
{
    internal class AuthenticationHelper
    {
        public static async Task<bool> AuthenticateAsync()
        {
            bool supported = await KeyCredentialManager.IsSupportedAsync();
            if (!supported)
                return false;

            KeyCredentialRetrievalResult result =
                await KeyCredentialManager.RequestCreateAsync("login",
                KeyCredentialCreationOption.ReplaceExisting);

            return result.Status == KeyCredentialStatus.Success;
        }
    }
}
