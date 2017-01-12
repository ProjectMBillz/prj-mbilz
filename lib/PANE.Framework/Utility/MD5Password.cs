using System;
using System.Text;
using System.Security.Cryptography;

namespace CBC.Framework.Utility
{
    /// <summary>
    /// Summary description for MD5Password
    /// </summary>
    public class MD5Password
    {
        public MD5Password()
        {

        }

        /// <summary>
        /// Creates the secure password.
        /// </summary>
        /// <param name="clear">The clear.</param>
        /// <returns></returns>
        public string CreateSecurePassword(string clear)
        {
            byte[] clearBytes;
            byte[] computedHash;

            clearBytes = ASCIIEncoding.ASCII.GetBytes(clear);
            computedHash = new MD5CryptoServiceProvider().ComputeHash(clearBytes);

            return ByteArrayToString(computedHash);
        }
        /// <summary>
        /// Gets the password in clear.
        /// </summary>
        /// <param name="secure">The secure.</param>
        /// <returns></returns>
        public string GetPasswordInClear(string secure)
        {
            throw new Exception("One way encryption service");
        }

        /// <summary>
        /// Converts ByteArray to string.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] array)
        {

            StringBuilder output = new StringBuilder(array.Length);

            for (int index = 0; index < array.Length; index++)
            {
                output.Append(array[index].ToString("X2"));
            }
            return output.ToString();
        }
    }
}