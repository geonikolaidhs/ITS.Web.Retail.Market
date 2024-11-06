using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public static class SHA1Util
    {
        /// <summary>
        /// Compute hash for string
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <param name="encoding">Encoding to be used</param>
        /// <returns>40-character hex string</returns>
        public static string SHA1HashStringForString(string s, Encoding encoding = null)
        {
            return HexStringFromBytes(SHA1HashBytesForString(s,encoding));
        }


        /// <summary>
        /// Compute hash for byte array
        /// </summary>
        /// <param name="s">byte array to be hashed</param>
        /// <returns>40-character hex string</returns>
        public static string SHA1HashStringForBytes(byte[] s)
        {
            return HexStringFromBytes(SHA1HashBytesForBytes(s));
        }
        /// <summary>
        /// Compute hash for string
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <param name="encoding">Encoding to be used</param>
        /// <returns>40 bytes</returns>
        public static byte[] SHA1HashBytesForString(string s, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = encoding.GetBytes(s);

            byte[] hashBytes = SHA1HashBytesForBytes(bytes);
 
            return hashBytes;
        }

        public static byte[] SHA1HashBytesForBytes(byte[] bytes)
        {
            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            return hashBytes;
        }
 
        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
