using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;

namespace ITS.Retail.WebClient.Helpers
{
    public static class CompressionHelper
    {

        public static byte[] CompressLZMAToBytes(string inFile)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, inFile);
                    byte[] inbyt = ms.ToArray();
                    byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
                    return b;
                }
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
            catch (Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message, ex);
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
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, inFile);
                    byte[] inbyt = ms.ToArray();
                    byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
                    return Convert.ToBase64String(b);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("General compress error:" + ex.Message, ex);
            }
        }


        public static object DecompressLZMA(string value)
        {
            object retval = null;
            try
            {
                byte[] byt = Convert.FromBase64String(value);
                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(byt);
                using (MemoryStream outMs = new MemoryStream(outByt))
                {
                    outMs.Seek(0, 0);
                    BinaryFormatter bf = new BinaryFormatter();
                    retval = bf.Deserialize(outMs, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }

        public static object DecompressLZMAFromBytes(byte[] value)
        {
            object retval = null;
            try
            {

                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(value);
                using (MemoryStream outMs = new MemoryStream(outByt))
                {
                    outMs.Seek(0, 0);
                    BinaryFormatter bf = new BinaryFormatter();
                    retval = bf.Deserialize(outMs, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }


        public static Image DecompressImageLZMA(string value)
        {
            Image retval = null;
            try
            {
                byte[] byt = Convert.FromBase64String(value);
                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(byt);
                //must not dispose stream or the image will be disposed as well
                MemoryStream outMs = new MemoryStream(outByt);
                retval = Image.FromStream(outMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }

        public static Image DecompressImageLZMAFromBytes(byte[] value)
        {
            Image retval = null;
            try
            {
                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(value);
                //must not dispose stream or the image will be disposed as well
                MemoryStream outMs = new MemoryStream(outByt);
                retval = Image.FromStream(outMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }

    }
}