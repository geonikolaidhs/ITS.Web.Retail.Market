using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    public class CustomSerialPort : SerialPort
    {

        public CustomSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {

        }

        public new int Read(byte[] buffer, int offset, int count)
        {
            Trace.WriteLine("Receiving Start: " + DateTime.Now);
            for (int i = 0; i < count; i++)
            {
                Trace.WriteLine(buffer[offset + i]);
            }
            int result = base.Read(buffer, offset, count);
            Trace.WriteLine("Receiving End: " + DateTime.Now);

            return result;
        }

        public new int ReadByte()
        {
            Trace.WriteLine("Receiving Start: " + DateTime.Now);
            int result = base.ReadByte();
            Trace.WriteLine(result);
            Trace.WriteLine("Receiving End: " + DateTime.Now);
            return result;
        }

        public new void Write(string text)
        {
            Trace.WriteLine("Sending '" + text + "' Start: " + DateTime.Now);
            base.Write(text);
            Trace.WriteLine("Sending End: " + DateTime.Now);
        }

        public new void Write(byte[] buffer, int offset, int count)
        {
            Trace.WriteLine("Sending Start: " + DateTime.Now);
            for (int i = 0; i < count; i++)
            {
                Trace.WriteLine(buffer[offset + i]);
            }
            base.Write(buffer, offset, count);

            Trace.WriteLine("Sending End: " + DateTime.Now);
        }
    }
}
