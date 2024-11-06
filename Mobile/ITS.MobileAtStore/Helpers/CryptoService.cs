using System;

using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Xml;

namespace ITS.MobileAtStore.Helpers
{
    public static class CryptoService
    {
       private static byte[] Key = new byte[] { 35, 237, 202, 112, 40, 216, 126, 253, 66, 189, 92, 160, 108, 176, 90, 154, 199, 232, 2, 236, 104, 207, 141, 58, 236, 136, 174, 115, 52, 68, 197, 59 };
       private static byte[] IV = new byte[] { 168, 170, 107, 148, 18, 124, 73, 199, 150, 186, 20, 119, 219, 173, 53, 206 };

       public static byte[] EncryptStringToBytes(string plainText)
       {           
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText"); 
   
            byte[] encrypted;    
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;           
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);    
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

       public static string EncryptStringToBase64(string plainText)
       {
           if (plainText == null || plainText.Length <= 0)
               throw new ArgumentNullException("plainText");

           byte[] bytes = EncryptStringToBytes(plainText);
           return Convert.ToBase64String(bytes);          
       }

        public static string DecryptStringFromBytes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
   
            string plaintext = null;   
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;           
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);  
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        public static string DecryptStringFromBase64String(string base64ChiperText)
        {
            byte[] bytes = Convert.FromBase64String(base64ChiperText);
            return DecryptStringFromBytes(bytes);
        }


        public static XmlDocument DecryptXmlFromBase64String(string base64ChiperText)
        {
            byte[] bytes = Convert.FromBase64String(base64ChiperText);
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Position = 0;
                using (Rijndael RijndaelAlg = Rijndael.Create())
                {
                    using (CryptoStream cStream = new CryptoStream(memStream, RijndaelAlg.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                    {
                        XmlReader xmlreader = XmlReader.Create(cStream);
                        XmlDocument xml = new XmlDocument();
                        xml.Load(xmlreader);
                        return xml;
                    }
                }
            }
        }

    }
}
