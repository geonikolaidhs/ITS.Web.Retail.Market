﻿using System;

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ITS.MobileAtStore;
using System.IO;

namespace ITS.Mobile.Updater 
{
    public static class AppSettings
    {
        static string localItemsDBLocation = ""; //The location of the items database
        static volatile string serverIP = ""; //The web service url
        static OPERATION_MODE operationMode = 0; //The operation mode
        static bool advancedPriceCheckingActive = false;
        static string compcode = "-1";
        static int priceCatalogPolicy = -1;
        public static String configurationLocation;
        public static ITS.MobileAtStore.TerminalSettings Terminal { get; set; }

#if (DEBUG)
        static string configurationFile = "dataLoggerConfigDebug.xml"; //The debug configuration file
#else
        static string configurationFile = "dataLoggerConfig.xml"; //The configuration file
#endif
        /// <summary>
        /// Reads teh settings from the configuration file
        /// </summary>
        public static void ReadSettings()
        {
            XmlDocument xml;
            XmlElement element;
            try
            {
                string terminalFile = configurationLocation + "\\TerminalSettings.xml";
                XmlSerializer ser = new XmlSerializer(typeof(TerminalSettings));
                using (StreamReader stream = new StreamReader(terminalFile))
                {
                    Terminal = ser.Deserialize(stream) as TerminalSettings;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
                Terminal = new TerminalSettings();
                Terminal.ID = 0;
            }
            try
            {
                string terminalFile = configurationLocation + "\\TerminalSettings.xml";
                XmlSerializer ser = new XmlSerializer(typeof(TerminalSettings));
                using (StreamReader stream = new StreamReader(terminalFile))
                {
                    Terminal = ser.Deserialize(stream) as TerminalSettings;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message + "\r\n" + ex.StackTrace;
                Terminal = new TerminalSettings();
                Terminal.ID = 0;
            }

            try
            {
                string config = configurationLocation + "\\" + configurationFile;
                xml = new XmlDocument();
                xml.Load(config);
                element = xml["Settings"]["LocalItemsDBLocation"];
                localItemsDBLocation = element.InnerXml;

                element = xml["Settings"]["ServerIP"];
                serverIP = element.InnerXml;

                element = xml["Settings"]["OperationMode"];
                operationMode = (OPERATION_MODE)int.Parse(element.InnerXml);

                element = xml["Settings"]["AdvancedPriceChecking"];
                advancedPriceCheckingActive = element.Attributes["Active"].InnerText == "True" ? true : false;               

            }
            catch (Exception e)
            {
                throw new Exception("Λάθος κατά την ανάγνωση του <" + configurationFile + ">.\r\n" + e.Message);
            }
        }

        /// <summary>
        /// Saves the settings in the xml file
        /// </summary>
        /// <returns></returns>
        

        #region Enumerations
        public enum OPERATION_MODE
        {
            BATCH = 1,
            ONLINE = 2
        }
        #endregion

        #region Properties
        public static string LocalItemsDBLocation
        {
            get
            {
                return localItemsDBLocation;
            }
            set
            {
                localItemsDBLocation = value;
            }
        }

        public static OPERATION_MODE OperationMode
        {
            get
            {
                return operationMode;
            }

            set
            {
                operationMode = value;
            }
        }

        public static string ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                serverIP = value;
            }
        }

        public static bool AdvancedPriceCheckingActive
        {
            get
            {
                return advancedPriceCheckingActive;
            }
            set
            {
                advancedPriceCheckingActive = value;
            }
        }

        public static string CompCode
        {
            get
            {
                return compcode;
            }
            set
            {
                compcode = value;
            }
        }

        public static int PriceList
        {
            get
            {
                return priceCatalogPolicy;
            }
            set
            {
                priceCatalogPolicy = value;
            }
        }

        private static volatile bool _connectedToServiceService = false;
        public static bool ConnectedToWebService
        {
            get
            {
                return _connectedToServiceService;
            }
            set
            {
                _connectedToServiceService = value;
            }
        }
        #endregion
    }
}