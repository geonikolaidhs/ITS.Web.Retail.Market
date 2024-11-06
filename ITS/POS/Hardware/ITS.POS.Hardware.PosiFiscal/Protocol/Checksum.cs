using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class Checksum
    {
        public int Value { get; set; }

        public Checksum(List<Field> fields)
        {
            Value = CalculateChecksum(fields);
        }

        protected byte CalculateChecksum(List<Field> fields)
        {
            //TODO Test it
            int sum = 0;
            foreach (Field field in fields)
            {
                foreach (char ch in field.Value)
                {
                    sum += (int)(ch);
                }
                sum += (int)'/';
            }
            return (byte)(sum % 100);
        }

        protected byte CalculateChecksum(string packetDataString)
        {
            //TODO Test it
            int sum = 0;
            foreach (char ch in packetDataString)
            {
                    sum += (int)(ch);
            }
            return (byte)(sum % 100);
        }
    }
}
