using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System.Windows.Forms;
using System.Reflection;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Versions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using DevExpress.Data.Filtering;
using System.Collections.Concurrent;


namespace ITS.POS.Client.Helpers
{
    public class UtilsHelper
    {
        public static ConcurrentDictionary<string, POSKeyMapping> hKeyMap;
        public static Hashtable hKeyCharacters;
        public UtilsHelper()
        {
        }

        /// <summary>
        /// The MapVirtualKey function translates (maps) a virtual-key code into a scan
        /// code or character value, or translates a scan code into a virtual-key code    
        /// </summary>
        /// <param name="uCode">[in] Specifies the virtual-key code or scan code for a key.
        /// How this value is interpreted depends on the value of the uMapType parameter
        /// </param>
        /// <param name="uMapType">[in] Specifies the translation to perform. The value of this
        /// parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>Either a scan code, a virtual-key code, or a character value, depending on
        /// the value of uCode and uMapType. If there is no translation, the return value is zero
        /// </returns>
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

        public static decimal RoundValue(decimal value, OwnerApplicationSettings settings,MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, (int)settings.ComputeDigits, midpointRounding); 
        }

        public static decimal RoundDisplayValue(decimal value, OwnerApplicationSettings settings, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, (int)settings.DisplayDigits, midpointRounding);
        }

        /// <summary>
        /// InitKeyMaps() initialize KeyMap reading from POSKeyMapping Table 
        /// </summary>
        public static void InitKeyMaps(Guid currentPOSOid)
        {

            if (hKeyMap == null) hKeyMap = new ConcurrentDictionary<string, POSKeyMapping>();
            ITS.POS.Model.Settings.POS currentPos = SessionHelper.GetObjectByKey<ITS.POS.Model.Settings.POS>(currentPOSOid);
            if (currentPos != null)
            {
                hKeyMap.Clear();
                XPCollection<POSKeyMapping> storedKeys = new XPCollection<POSKeyMapping>(SessionHelper.GetSession<POSKeyMapping>(), new BinaryOperator("POSKeysLayout", currentPos.POSKeysLayout));
                foreach (POSKeyMapping item in storedKeys)
                {
                    hKeyMap.TryAdd(item.KeyData.ToString(),item);
                }
            }
        }

        /// <summary>
        /// void InitKeyCharacters() initialize printable character hash table
        /// </summary>
        public static void InitKeyCharacters()
        {
            if (hKeyCharacters == null) hKeyCharacters = new Hashtable();
            hKeyCharacters.Clear();

            for (int i = 48; i <= 57; i++)       //0-9
            {
                hKeyCharacters[((Keys)i).ToString()] = (i - 48).ToString();
            }

            for (int i = 96; i < 106; i++)      //Numpad 0-9
            {
                hKeyCharacters[((Keys)i).ToString()] = (i - 96).ToString(); ;
            }
            
            for (int i = 65; i < 90; i++)
            {
                hKeyCharacters[((Keys)i).ToString()] = ((Keys)i).ToString();
            }

            //hKeyCharacters[Keys.None.ToString()] = "";
            hKeyCharacters[Keys.Subtract.ToString()] = "-";
            //hKeyCharacters[Keys.Add.ToString()] = "+";
            hKeyCharacters[Keys.Decimal.ToString()] = ",";
            //hKeyCharacters[Keys.Divide.ToString()] = "/";
            //hKeyCharacters[Keys.Multiply.ToString()] = "*";
            //hKeyCharacters[Keys.OemCloseBrackets.ToString()] = "]";
            //hKeyCharacters[Keys.OemOpenBrackets.ToString()] = "[";
            hKeyCharacters[Keys.OemMinus.ToString()] = "-";
            //hKeyCharacters[Keys.Oemplus.ToString()] = "+";
            hKeyCharacters[Keys.OemPeriod.ToString()] = ".";
            hKeyCharacters[Keys.Oemcomma.ToString()] = ",";
            //hKeyCharacters[Keys.Oemtilde.ToString()] = "~";
            //hKeyCharacters[Keys.OemQuotes.ToString()] = "\"";
            //hKeyCharacters[Keys.OemPipe.ToString()] = "|";
            //hKeyCharacters[Keys.OemBackslash.ToString()] = "\\";
            //hKeyCharacters[Keys.Oem1.ToString()] = ";";
            //hKeyCharacters[Keys.Oem5.ToString()] = "\\";
            //hKeyCharacters[Keys.Oem7.ToString()] = "'";
            //hKeyCharacters[Keys.Oem6.ToString()] = "\\";

        }

        /// <summary>
        /// string KeyaboardDecode(Keys key) : decode Key if this key is printable charcater
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string KeyboardDecode(Keys keys)
        {
            var key = hKeyCharacters[keys.ToString()];

            return !(key == null) ? key.ToString() : "";
        }

        /// <summary>
        /// Compress string with LZMA compression
        /// </summary>
        /// <param name="inFile">string to compression</param>
        /// <returns></returns>
        public static string CompressLZMA(string inFile)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, inFile);
                byte[] inbyt = ms.ToArray();
                byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
                return Convert.ToBase64String(b);
            }
            catch(Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message,ex);
            }
        }

        /// <summary>
        /// Decompress string with LZMA compression 
        /// </summary>
        /// <param name="value">string value to decompression</param>
        /// <returns></returns>
        public static object DecompressLZMA(string value)
        {
            object retval = null;
            //try
            {
                byte[] byt = Convert.FromBase64String(value);

                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(byt);


                MemoryStream outMs = new MemoryStream(outByt);
                outMs.Seek(0, 0);
                BinaryFormatter bf = new BinaryFormatter();
                retval = (object)bf.Deserialize(outMs, null);
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return retval;
        }

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

        public static int GetScanCode(Keys key)
        {
            uint keyCode = Convert.ToUInt32((int)key);
            return MapVirtualKey(keyCode, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
        }

        public static Keys GetKeyCode(uint scanCode)
        {
            return (Keys)MapVirtualKey(scanCode, MapVirtualKeyMapTypes.MAPVK_VSC_TO_VK_EX);
        }

        public static Type GetEntityByName(string name)
        {
            Type retval = typeof(Item).Assembly.GetType("ITS.POS.Model.Master." + name);
            if (retval == null)
            {
                retval = typeof(DocumentType).Assembly.GetType("ITS.POS.Model.Settings." + name);
            }

            return retval;
        }

        /// <summary>
        /// On x86 machines it's a hardware beep. On x64 it's through the audio driver
        /// </summary>
        public static void HardwareBeep()
        {
            try
            {
                Console.Beep(250, 500);
            }
            catch
            {
            }
        }
    }
}
