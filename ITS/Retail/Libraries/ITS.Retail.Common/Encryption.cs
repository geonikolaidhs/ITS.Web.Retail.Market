using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ITS.Retail.Common
{
    public class Encryption
    {

        public static string EncryptString(string Message, string Passphrase)
        {
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            byte[] salt = UTF8.GetBytes(Passphrase);
            byte[] EncryptedData = new byte[DataToEncrypt.Length];
            for (int i = 0; i < DataToEncrypt.Length; i++)
            {
                EncryptedData[i] = (byte)(DataToEncrypt[i] ^ Passphrase[i % Passphrase.Length]);
            }
            return Convert.ToBase64String(EncryptedData);
        }

        public static string DecryptString(string Message, string Passphrase)
        {
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            byte[] DataToEncrypt = Convert.FromBase64String(Message);
            byte[] salt = UTF8.GetBytes(Passphrase);
            byte[] EncryptedData = new byte[DataToEncrypt.Length];
            for (int i = 0; i < DataToEncrypt.Length; i++)
            {
                EncryptedData[i] = (byte)(DataToEncrypt[i] ^ Passphrase[i % Passphrase.Length]);
            }
            return UTF8.GetString(EncryptedData);
        }

    }
}
