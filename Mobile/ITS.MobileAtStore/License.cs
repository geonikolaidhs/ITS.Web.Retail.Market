//#define EMULATOR
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using OpenNETCF.Win32;
using OpenNETCF.Security;
using OpenNETCF.Security.Cryptography;
using OpenNETCF.Diagnostics;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// Encapsulates the logic of verifying whether or not there is a licence and its valid for this machine
    /// </summary>
    public class License
    {
        #region Data Members
        private static byte[] cryptoKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6 };
        private static byte[] cryptoIV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private byte[] deviceID;
        private string deviceSN;
        private string keyFile;
        private bool isValid;
        #endregion

        #region DLL Imports
        [DllImport("sernumber.dll", EntryPoint = "GetSerialNumber", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetSerialNumber(char[] buffer, int len);
        #endregion

        #region Constructors
        public License()
            : this(null)
        {
        }

        /// <summary>
        /// Validates the given key file and sets whether is valid or not
        /// </summary>
        /// <param name="key_file"></param>
        public License(string key_file)
        {
            try
            {
                char[] buffer = new char[16];
                int len = GetSerialNumber(buffer, 16);
                this.deviceSN = new string(buffer, 0, len);
                buffer = null;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                this.deviceSN = Device.GetDeviceID();
            }
            this.deviceSN = this.deviceSN.ToUpper();
            this.deviceID = Encoding.Unicode.GetBytes(this.deviceSN);
            //MessageBox.Show("Device SN : " + this.deviceSN);
            if (keyFile == null)
            {
                keyFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\terminal.lic";
            }
            if (File.Exists(keyFile))
            {
                FileStream fin = File.Open(keyFile, FileMode.Open, FileAccess.Read, FileShare.None);
                byte[] encryptedKey = new byte[fin.Length];
                byte[] decryptedKey;
                fin.Read(encryptedKey, 0, (int)fin.Length);
                fin.Close();

                decryptedKey = this.UnScrable(encryptedKey);
                string[] keys = Encoding.ASCII.GetString(decryptedKey, 0, decryptedKey.Length).ToUpper().Split(new char[] { '\r', '\n' });
                if (keys.Length > 0)
                {
                    for (int i = 0 ; i < keys.Length ; i++)
                    {
                        if (keys[i] == this.deviceSN)
                        {
                            this.isValid = true;
                            break;
                        }
                    }
                }
                else
                {
                    this.isValid = false;
                }
            }
            else
            {
                this.isValid = false;
            }
            if (this.isValid == false)
            {
                StreamWriter fout = File.CreateText(@"\Windows\deviceid.txt");
                fout.WriteLine(this.deviceSN);
                fout.Close();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Scrables a byte array thus encrypting it
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public byte[] Scrable(byte[] buffer)
        {
            RC2CryptoServiceProvider crypto = new RC2CryptoServiceProvider();
            crypto.Key = License.cryptoKey;
            crypto.IV = License.cryptoIV;

            ICryptoTransform encryptor = crypto.CreateEncryptor();
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(buffer, 0, buffer.Length);
            csEncrypt.FlushFinalBlock();

            byte[] encrypted = msEncrypt.ToArray();
            return encrypted;
        }

        /// <summary>
        /// Unscrables a given byte array thus decrypting it
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public byte[] UnScrable(byte[] buffer)
        {
            RC2CryptoServiceProvider crypto = new RC2CryptoServiceProvider();

            crypto.Key = License.cryptoKey;
            crypto.IV = License.cryptoIV;

            ICryptoTransform decryptor = crypto.CreateDecryptor();
            MemoryStream msDecrypt = new MemoryStream(buffer);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[buffer.Length];

            try
            {
                csDecrypt.Read(fromEncrypt, 0, buffer.Length);
            }
            catch
            {
                // do nothing.
                // This exception occurres only in Datalogic black.
                // The buffer contains the value.
            }

            return fromEncrypt;
        }
        #endregion

        #region Properties
        public string SerialNumber
        {
            get
            {
#if EMULATOR
				return string.Empty;
#else
                return this.deviceSN;
#endif
            }
        }

        public byte[] DeviceID
        {
            get
            {
#if EMULATOR
				return new byte[] {0};
#else
                return this.deviceID;
#endif
            }
        }

        public bool IsValid
        {
            get
            {
#if EMULATOR
				return true;
#elif DEMO
				return true;
#elif DEBUG
                return true;
#else
                return this.isValid;
#endif
            }
        }
        #endregion
    }
}
