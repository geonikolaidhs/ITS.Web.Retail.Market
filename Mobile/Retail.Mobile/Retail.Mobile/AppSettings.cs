using System;

using System.Collections.Generic;
using System.Text;

namespace Retail.Mobile
{
    public static class AppSettings
    {
        public enum OPERATION_MODE
        {
            BATCH = 1,
            ONLINE = 2
        }

        static OPERATION_MODE operationMode = OPERATION_MODE.BATCH; //The operation mode
        public static decimal limit = (decimal)99999.999;
        public static string Title = "Retail Mobile";
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

        //private static volatile bool _connectedToServiceService = false;
        //public static bool ConnectedToWebService
        //{
        //    get
        //    {
        //        return _connectedToServiceService;
        //    }
        //    set
        //    {
        //        _connectedToServiceService = value;
        //    }
        //}


        public static string IP = "";
        public static string executingPath = "";
        public static string Pda_ID = "";
        public static string databasePath = "";

        public static string Username = "";
        public static string Password = "";
    }
}
