using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;

namespace ITS.Retail.Bridge.Service
{

    public static class CompressionHelper
    {

        public static byte[] CompressLZMAToBytes(string inFile)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, inFile);
                byte[] inbyt = ms.ToArray();
                byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
                return b;
            }
            catch (Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message, ex);
            }
        }



        public static byte[] CompressLZMAToBytes(Image inFile)
        {
            try
            {
                ImageConverter converter = new ImageConverter();
                byte[] imageBytes = (byte[])converter.ConvertTo(inFile, typeof(byte[]));
                byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(imageBytes);
                return b;
            }
            catch
            {
                throw new Exception("General compress error");
            }
        }

        public static string CompressLZMA(Image inFile)
        {
            try
            {
                ImageConverter converter = new ImageConverter();
                byte[] imageBytes = (byte[])converter.ConvertTo(inFile, typeof(byte[]));
                byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(imageBytes);
                return Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message, ex);
            }
        }

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
            catch
            {
                throw new Exception("General compress error");
            }
        }

        /// <summary>
        /// Detects the byte order mark of a file and returns
        /// an appropriate encoding for the file.
        /// </summary>
        /// <param name="srcFile"></param>
        /// <returns></returns>
        public static Encoding GetFileEncoding(string srcFile)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            file.Read(buffer, 0, 5);
            file.Close();

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;

            return enc;
        }

        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(fullFilePath);
            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            finally
            {
                fs.Close();
            }

        }
    }
}
