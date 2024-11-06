using System;
using System.Collections.Generic;
using System.Text;
using Common;
using System.Net;
using System.IO;

namespace Retail.Mobile
{
    public static class ServiceProvider
    {

        public static string passphrase = "@#kdlsklασδαΰωkk";
        //public static string globalurl = @"http://localhost:40386/";
        public static string globalurl = @"http://169.254.2.2:40386/";
        public static Boolean hasErros = false;

        enum Mode
        {
            Get = 0, Create = 1, Update = 2, Delete = 3
        }


          /// <summary>
        /// Methodos gia na paro dedomena me Post method
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string GetDataFromServiceWithPostMethod(string Model, string query, string filter)
        {
            try
            {
                hasErros = false;
                XmlCreator newxml = new XmlCreator("ITS");

                List<string> xmldata = new List<string>();
                xmldata.Add("Mode|" + "0");
                xmldata.Add("Query|" + query);
                xmldata.Add("Filter|" + filter);
                newxml.CreateNodes("settings", xmldata.ToArray());
                newxml.Xmlclose();


                byte[] dataByte = Encoding.UTF8.GetBytes(Encryption.EncryptString(newxml.MyXml,passphrase));

                HttpWebRequest POSTRequest = (HttpWebRequest)WebRequest.Create(globalurl + Model + @"/");
                POSTRequest.Method = "POST";
                POSTRequest.ContentType = "text/xml";
                POSTRequest.KeepAlive = false;
                POSTRequest.Timeout = 5000;
                POSTRequest.ContentLength = dataByte.Length;

                using (Stream POSTstream = POSTRequest.GetRequestStream())
                {
                    POSTstream.Write(dataByte, 0, dataByte.Length);
                }

                HttpWebResponse POSTResponse = (HttpWebResponse)POSTRequest.GetResponse();
                using (StreamReader reader = new StreamReader(POSTResponse.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                hasErros = true;
                return ex.Message;
            }
        }


        /// <summary>
        /// Methodos gia na postaro dedomena
        /// </summary>
        /// <param name="xml2send"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static string Send2Service(string xml2send, string Model)
        {
            try
            {
                hasErros = false;
                byte[] dataByte = Encoding.UTF8.GetBytes(Encryption.EncryptString(xml2send, passphrase));
                HttpWebRequest POSTRequest = (HttpWebRequest)WebRequest.Create(globalurl + Model + @"/");
                POSTRequest.Method = "POST";
                POSTRequest.ContentType = "text/xml";
                POSTRequest.KeepAlive = false;
                POSTRequest.Timeout = 5000;
                POSTRequest.ContentLength = dataByte.Length;

                using (Stream POSTstream = POSTRequest.GetRequestStream())
                {
                    POSTstream.Write(dataByte, 0, dataByte.Length);
                }

                HttpWebResponse POSTResponse = (HttpWebResponse)POSTRequest.GetResponse();
                using (StreamReader reader = new StreamReader(POSTResponse.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                hasErros = true;
                return ex.Message;
            }
        }


        /// <summary>
        /// Methodos Get gia na paro dedomena
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDataFromServiceWithGetMethod(string url)
        {
            try
            {
                //string url = @"http://localhost:2503/Cook/?Code='11'";
                HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
                GETRequest.Method = "GET";
                HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
                Stream GETResponseStream = GETResponse.GetResponseStream();
                using (StreamReader sr = new StreamReader(GETResponseStream))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
    }
}
