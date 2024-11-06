using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using OpenNETCF.Windows.Forms;
using DevExpress.Xpo;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo.DB;
using ITS.Common.Keyboards.Compact;
using ITS.Common.Utilities.Compact;
using OpenNETCF.Threading;
using System.Runtime.InteropServices;
using System.Data;
using System.Threading;
using App;
using System.Net;
using DevExpress.Xpo.Metadata;
using ITS.MobileAtStore.WRMMobileAtStore;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// Summary description for Warehouse2.
    /// </summary>
    public static class MobileAtStore
    {
        private static NamedMutex _mutex;

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern uint FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern uint SetForegroundWindow(uint hwnd);


        static void ActivateWindow(string __wname)
        {
            uint hwnd = FindWindow(null, __wname);
            SetForegroundWindow(hwnd);
        }

        public static IDataLayer TransactionsDL; //The database for our transactions
        public static IDataLayer ItemsDL; //The database for our mobile product database

        static void Main()
        {
            string title = "Mobile@Store"; 
            _mutex = new NamedMutex(false, "its_mobile@store_mutex");

            if (_mutex.WaitOne(1, false))
            {
                
                MessageForm.Initialize(MessageForm.SoundModes.SYSTEM_SOUNDS);
                License myLicense = new License();            

                if (!myLicense.IsValid)
                {
                    MessageForm.Execute("Σφάλμα", "Μη έγκυρη άδεια χρήσης.{0}Η εφαρμογή θα τερματιστεί.", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }
                else
                {
                    try
                    {
                        LockDown.Execute(true);
                        myLicense = null;
                        //CreateSimpleDataLayer();
                        KeyboardGateway.Initialize(ObjectModel.Common.CultureInfo, true);
                        AppSettings.ReadSettings();
                        SideBar80x320 sideBar80x320 = new SideBar80x320();
                        Main main = new Main(sideBar80x320);
                        main.Text = title;
                        sideBar80x320.MainForm = main;

                        int screenW = (Screen.PrimaryScreen.WorkingArea.Width / 10) * 10;
                        int screenH = (Screen.PrimaryScreen.WorkingArea.Height / 10) * 10;
                        if ((screenW > 240) && (screenW <= 320))
                        {
                            sideBar80x320.Show();
                        }
                        else
                            if (screenW > 320)
                            {
                                sideBar80x320.Hide();
                            }
                            else
                            {
                                sideBar80x320.Hide();
                            }
                        System.Windows.Forms.Application.Run(main);
                        sideBar80x320.Close();
                    }
                    catch(Exception e)
                    {
                        string exceptionMessage = e.Message + "\r\n" + e.StackTrace;
                    }
                    finally
                    {
                        try
                        {
                            if (TransactionsDL != null)
                                TransactionsDL.Dispose();
                            if (ItemsDL != null)
                                ItemsDL.Dispose();
                            LockDown.Execute(false);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                ActivateWindow(title);
            }
        }

        private static void CreateSimpleDataLayer()
        {
            XpoDefault.DataLayer = new SimpleDataLayer(new InMemoryDataStore(new DataSet(), AutoCreateOption.DatabaseAndSchema));
        }

        /// <summary>
        /// Returns a static value of the expected web service version so we can check for incompatibilities
        /// </summary>
        /// <returns></returns>
        public static string GetExpectedWebServiceVersion()
        {
            return "ver2.0";
        }

        /// <summary>
        /// Returns a web service instance of the datalogger web services. This method checks only once for incompatible versions
        /// </summary>
        /// <returns></returns>
        public static WRMMobileAtStore.WRMMobileAtStore GetWebService(int timeout)
        {
            WRMMobileAtStore.WRMMobileAtStore service = new WRMMobileAtStore.WRMMobileAtStore();
            string url = string.Format(@"http://{0}/WRMMobileAtStore.svc", AppSettings.ServerIP);           
            service.Url = url;
            service.Timeout = timeout == 0 ? 300000 : timeout; //5 minutes            
            return service;
        }
        
        public static void ConnectDataLayers()
        {
            try
            {
                if (TransactionsDL == null)
                {
                    string transactionsDBPath = Path.Combine(Application2.StartupPath, ObjectModel.Common.fname_trans);
                    string transactionsDBConnString = SQLiteConnectionProvider.GetConnectionString(transactionsDBPath);
                    XPDictionary dict = new ReflectionDictionary();
                    dict.GetDataStoreSchema(typeof(ITS.MobileAtStore.ObjectModel.Header).Assembly);
                    TransactionsDL = XpoDefault.GetDataLayer(transactionsDBConnString, dict,AutoCreateOption.DatabaseAndSchema);
                    XPClassInfo headerClassInfo = dict.GetClassInfo(typeof(ITS.MobileAtStore.ObjectModel.Header));
                    headerClassInfo.AddAttribute(new PersistentAttribute("ITS_DATALOGGER_HEADER"));
                    XPClassInfo lineClassInfo = dict.GetClassInfo(typeof(ITS.MobileAtStore.ObjectModel.Line));
                    lineClassInfo.AddAttribute(new PersistentAttribute("ITS_DATALOGGER_LINE"));
                }

                if (ItemsDL == null)
                {
                    string itemsDBPath = Path.Combine(AppSettings.LocalItemsDBLocation, ObjectModel.Common.fname_items);
                    string itemsDBConnString = SQLiteConnectionProvider.GetConnectionString(itemsDBPath);
                    ItemsDL = XpoDefault.GetDataLayer(itemsDBConnString, AutoCreateOption.DatabaseAndSchema);
                }
            }
            catch (Exception ex)
            {
                TransactionsDL = null;
                ItemsDL = null;
                MessageForm.Execute("Σφάλμα", "H σύνδεση με τις mobile βάσεις δεδομένων db δεν ήταν επιτυχής, παρακαλώ ελέγξτε το αρχείο ρυθμίσεων\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                Application2.Exit();
            }
        }

        public static void DisconnectDataLayers()
        {
            if (TransactionsDL != null)
            {
                TransactionsDL.Dispose();
                TransactionsDL = null;
            }
            if (ItemsDL != null)
            {
                ItemsDL.Dispose();
                ItemsDL = null;
            }
        }
    }
}

