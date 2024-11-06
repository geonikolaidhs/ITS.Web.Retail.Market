using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ITS.Retail.WebClient.AuxilliaryClasses
{
    public class POSTransactionFile
    {
        [JsonIgnore]
        public long? id { get; protected set; }
        public string Filepath { get; protected set; }
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return id != null;
            }
        }

        public POSTransactionFile(string filepath)
        {
            POSFileDocumentHeaders = new List<POSFileDocumentHeader>();
            Filepath = filepath;
            string timestampstr = filepath.Replace(TransactionHelper.PosTransactionsPrefix, "");
            MatchCollection matches = Regex.Matches(timestampstr, @"\d+");
            try
            {
                id = (new DateTime(int.Parse(matches[0].Groups[0].Value), int.Parse(matches[1].Groups[0].Value), int.Parse(matches[2].Groups[0].Value),
                             int.Parse(matches[3].Groups[0].Value), int.Parse(matches[4].Groups[0].Value), int.Parse(matches[5].Groups[0].Value))).Ticks;
            }
            catch (Exception )
            {
                id = null;
            }
        }

        public POSTransactionFile(long idTicks)
        {
            try
            {
                id = (long)idTicks;
                DateTime timestamp = new DateTime((long)id);
                Filepath = String.Format("{0}{1}{2:yyyy-MM-dd_HH-mm-ss}", StoreControllerAppiSettings.TransactionFilesFolder,TransactionHelper.PosTransactionsPrefix, timestamp);
            }
            catch (Exception )
            {
                id = null;
            }
        }

        public List<POSFileDocumentHeader> POSFileDocumentHeaders { get; set; }
    }
}
