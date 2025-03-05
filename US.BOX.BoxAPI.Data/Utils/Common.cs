using System;
using System.Security.Cryptography;
using System.Text;

namespace US.BOX.BoxAPI.Data
{
    /// <summary>
    /// Provides common utility methods.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Creates an MD5 hash and returns a Base64 URL-encoded string.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>A Base64 URL-safe encoded MD5 hash string.</returns>
        public static string CreateMD5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Base64UrlEncode(bytes);
        }

        /// <summary>
        /// Encodes the given byte array into a Base64 URL-safe string.
        /// </summary>
        /// <param name="inputBytes">The byte array to encode.</param>
        /// <returns>A URL-safe Base64 string.</returns>
        private static string Base64UrlEncode(byte[] inputBytes)
        {
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <summary>
        /// Gets the debtor signature based on amount and key.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="key">The key used in the signature calculation.</param>
        /// <returns>A signature string.</returns>
        public static string GetDebtorSignature(decimal amount, string key)
        {
            string formattedAmount = ((int)(amount * 100)).ToString("G29");
            string sig = CreateMD5($"{key}&{formattedAmount}");
            return sig;
        }
    }
}
