using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.AuxilliaryClasses;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class TransactionHelper
    {
        public static string PosTransactionsPrefix = "PosTransactions-";

        public static List<POSTransactionFile> GetTransactionFiles(out string errorMessage)
        {
            errorMessage = string.Empty;
            if ( StoreControllerAppiSettings.TransactionFilesFolder == null || string.IsNullOrEmpty(StoreControllerAppiSettings.TransactionFilesFolder) )
            {
                errorMessage = ResourcesLib.Resources.PleaseDefineFolderForTransactionFiles;
                return new List<POSTransactionFile>();
            }
            List<POSTransactionFile> files = new List<POSTransactionFile>();
            foreach (string file in Directory.GetFiles(StoreControllerAppiSettings.TransactionFilesFolder, PosTransactionsPrefix+"*").ToList()){
                POSTransactionFile tempFile = new POSTransactionFile(file);
                if (tempFile.IsValid)
                {
                    files.Add(tempFile);
                }
            }
            return files;
        }

        public static List<POSTransactionFile> GetSelectedTransactionFiles(List<string> POSTransactionFileNames)
        {
            List<POSTransactionFile> files = new List<POSTransactionFile>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                foreach (string file in POSTransactionFileNames)
                {
                    POSTransactionFile POSTransactionFile = new POSTransactionFile(file);
                    if (POSTransactionFile.IsValid)
                    {
                        using (UnitOfWork posUow = TransactionConnectionHelper.OpenFile(file))
                        {
                            XPCollection<POS.Model.Transactions.DocumentHeader> posDocuments = new XPCollection<POS.Model.Transactions.DocumentHeader>(posUow);
                            foreach (POS.Model.Transactions.DocumentHeader posDocument in posDocuments)
                            {
                                POSFileDocumentHeader POSFileDocumentHeader = new POSFileDocumentHeader(posDocument,uow);
                                POSTransactionFile.POSFileDocumentHeaders.Add(POSFileDocumentHeader);
                            }
                        }
                        TransactionConnectionHelper.ReleaseResources();
                        files.Add(POSTransactionFile);
                    }
                }
            }
            return files;
        }

    }
}
