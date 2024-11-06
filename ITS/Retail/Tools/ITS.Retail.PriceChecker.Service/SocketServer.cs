using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ITS.Retail.PriceChecker.Service
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }


    public class SocketServer
    {

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public SocketServer()
        {
        }

        public static void StartListening()
        {
            byte[] bytes = new Byte[1024];
            IPAddress ipAddress = IPAddress.Parse(Settings.getInstance().Ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Settings.getInstance().Port);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                Program.WriteToWindowsEventLog("Listening on port " + Settings.getInstance().Port + " and Ip " + Settings.getInstance().Ip, EventLogEntryType.Information);
                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
        }

        public static async void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
                    content = state.sb.ToString();
                    string code = TrimBarcodeInvalidChars(content);
                    SG15Result result = await GetItemPrice(code);
                    Send(handler, result.PrepareSG15Text());
                }
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
        }

        private static void Send(Socket handler, String data)
        {
            try
            {
                byte[] byteData = Settings.getInstance().FromEncoding.GetBytes(data);
                byte[] newBytes = Encoding.Convert(Settings.getInstance().FromEncoding, Settings.getInstance().ToEncoding, byteData);
                handler.BeginSend(newBytes, 0, newBytes.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
        }

        private static async Task<SG15Result> GetItemPrice(string code)
        {
            SG15Result result = new SG15Result();
            try
            {
                string path = Settings.getInstance().StoreControllerURL + "/PriceChecker/SG15CheckPriceTcp/?searchcode=" + code;
                HttpClient client = new HttpClient();
                String response = await client.GetStringAsync(path);
                result = JsonConvert.DeserializeObject<SG15Result>(response);
            }
            catch (SocketException e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); }
            return result;
        }


        private static string TrimBarcodeInvalidChars(string barcode)
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                if (barcode.ToCharArray()[0] == 'F')
                {
                    barcode = barcode.Replace("F", "");
                }

                if (barcode.Length > 1 && barcode.ToCharArray()[1] == 'F')
                {
                    barcode = barcode.Replace("F", "");
                }

            }
            return barcode;
        }

    }
}
