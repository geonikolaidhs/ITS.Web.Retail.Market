using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal static class DialogHelper
    {
        public static readonly byte SOH = 0x01;
        public static readonly byte STX = 0x02;
        public static readonly byte ETX = 0x03;
        public static readonly byte EOT = 0x04;
        public static readonly byte ENQ = 0x05;

        public static readonly byte ACK = 0x06;
        public static readonly byte NAK = 0x15;
        public static readonly byte ESC = 0x1b;

        private static int Hex2Digit(byte ch)
        {
            switch (ch)
            {
                case (byte)'0':
                    return 0;
                case (byte)'1':
                    return 1;
                case (byte)'2':
                    return 2;
                case (byte)'3':
                    return 3;
                case (byte)'4':
                    return 4;
                case (byte)'5':
                    return 5;
                case (byte)'6':
                    return 6;
                case (byte)'7':
                    return 7;
                case (byte)'8':
                    return 8;
                case (byte)'9':
                    return 9;
                case (byte)'A':
                    return 10;
                case (byte)'B':
                    return 11;
                case (byte)'C':
                    return 12;
                case (byte)'D':
                    return 13;
                case (byte)'E':
                    return 14;
                case (byte)'F':
                    return 15;
            };
            return 0;
        }

        private static int Hex2Dec(byte[] str)
        {
            int hex;
            hex = ((int)16 * Hex2Digit(str[0]));
            hex += Hex2Digit(str[1]);
            return hex;
        }

        static List<LogNums> list = new List<LogNums>(256);
        private static void ComputeLogNums()
        {

            using (Stream stream = typeof(Scale).Assembly.GetManifestResourceStream("ITS.POS.Hardware.BizerbaLogNums.DAT"))
            using (BinaryReader br = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length - 1)
                {
                    LogNums record = new LogNums();
                    record.Id = br.ReadInt16();
                    for (int i = 0; i < 16; i++)
                    {
                        record.Data[i] = br.ReadByte();
                    }
                    list.Add(record);
                }
            }
        }

        public static byte[] GetUnlockVal(byte[] str)
        {
            //FILE* hf;
            if (list.Count == 0)
            {
                ComputeLogNums();
            }
            int hex;
            LogNums ln;


            hex = Hex2Dec(str);

            ln = list[hex];
            byte[] returnValue;// = new byte[16];
            returnValue = ln.Data.Clone() as byte[];

            return returnValue;
        }

        public static IScaleRecord GetScaleRecord(IEnumerable<byte> response)
        {
            if(response.Count()==1)
            {
                byte responsebyte = response.First();
                if(responsebyte == DialogHelper.ACK)
                {
                    return new ScaleRecordACK(response);
                }
                if (responsebyte == DialogHelper.NAK)
                {
                    return new ScaleRecordNAK(response);
                }
            }
            if (response.Count() > 2)
            {
                while(response.First() != DialogHelper.STX)
                {
                    response = response.Skip(1);
                }
                string type = Encoding.UTF8.GetString(response.Skip(1).Take(2).ToArray());
                switch (type)
                {
                    case "11":
                        return new ScaleRecord11(response);
                    case "02":
                        return new ScaleRecord02(response);
                    case "09":
                        return new ScaleRecord09(response);
                    default:
                        throw new NotImplementedException("record type " + type);
                }
            }
            throw new NotImplementedException();
        }
    }
}
