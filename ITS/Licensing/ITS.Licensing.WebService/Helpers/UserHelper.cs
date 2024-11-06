using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace ITS.Licensing.Web.Helpers
{
    public class UserHelper
    {
        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            Byte[] doubleEncodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            using (md5 = new MD5CryptoServiceProvider())
            {
                originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
                encodedBytes = md5.ComputeHash(originalBytes);
            }

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                doubleEncodedBytes = sha1.ComputeHash(encodedBytes);
            }
            return BitConverter.ToString(doubleEncodedBytes);
        }
    }
}