using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ITS.Retail.Platform.Common.AuxilliaryClasses
{
    public static class ZipLZMA
    {
        public static string CompressLZMA(string inFile)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, inFile);
            byte[] inbyt = ms.ToArray();
            byte[] b = SevenZip.Compression.LZMA.SevenZipHelper.Compress(inbyt);
            return Convert.ToBase64String(b);
        }

        public static object DecompressLZMA(string value)
        {
            object retval = null;
            try
            {
                byte[] byt = Convert.FromBase64String(value);

                byte[] outByt = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(byt);

                MemoryStream outMs = new MemoryStream(outByt);
                outMs.Seek(0, 0);
                BinaryFormatter bf = new BinaryFormatter();
                retval = (object)bf.Deserialize(outMs, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }
    }
}
