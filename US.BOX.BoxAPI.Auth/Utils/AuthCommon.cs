using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace US.BOX.BoxAPI.Auth.Utils
{
    public static class AuthCommon
    {

        public static string ProtectData(string input) {
            return Base64UrlEncoder.Encode(ProtectedData.Protect(Encoding.UTF8.GetBytes(input), null, DataProtectionScope.LocalMachine));
        }
    }
}
