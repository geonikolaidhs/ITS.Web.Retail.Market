using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using ITS.MobileAtStore.ObjectModel;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace ITS.MobileAtStore.Common.ApplicationExportSettings
{
    public class ReflexisExportSettings : ApplicationExportSettings
    {
        private string CREATE_ORDER_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot+".\\ApplicationExportScripts\\Reflexis\\CreateOrder.sql";
            }
        }

        private string CREATE_ORDER_LINE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot+".\\ApplicationExportScripts\\Reflexis\\CreateOrderLine.sql";
            }
        }

        private string CREATE_TRANSACTION_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateTransaction.sql";
            }
        }

        private string CREATE_TRANSACTION_LINE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateTransactionLine.sql";
            }
        }

        private string CREATE_INVOICE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateInvoice.sql";
            }
        }

        private string CREATE_INVOICE_LINE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateInvoiceLine.sql";
            }
        }

        private string CREATE_LABEL_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateLabel.sql";
            }
        }

        private string CREATE_LABEL_LINE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateLabelLine.sql";
            }
        }


        private string CREATE_INVENTORY_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateInventory.sql";
            }
        }

        private string CREATE_INVENTORY_LINE_STATEMENT_FILE
        {
            get
            {
                return this.ServerMapPathRoot + ".\\ApplicationExportScripts\\Reflexis\\CreateInventoryLine.sql";
            }
        }

        public ReflexisExportSettings()
            : base()
        {            
        }

        public ReflexisExportSettings(string serverMapPathRoot, ConnectionSettings connectionSettings,Dictionary<string,object> applicationSpecificSettings) :
            base(serverMapPathRoot, connectionSettings,applicationSpecificSettings)
        {
        }
                       

        public override void Initialise()
        {            
            this.Application = eApplication.REFLEXIS;
            this.ConnectionSettings.ConnectionMode = ConnectionMode.MYSQL;
        }

        public override bool PerformExport(Header document, string remoteIP, out string logMessage)
        {
            try
            {
                logMessage = "";
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                string transactionQuery = "START TRANSACTION;";
                string createDocumentStatement = CreateDocumentStatement(document,remoteIP);
                if (String.IsNullOrEmpty(createDocumentStatement))
                {
                    return false;
                }
                transactionQuery += createDocumentStatement;

                string createLineStatement = "";
                int lineCounter = 0;
                foreach (Line line in document.Lines)
                {
                    lineCounter++;
                    createLineStatement = CreateLineStatement(line,document.DocType,lineCounter);
                    if (String.IsNullOrEmpty(createLineStatement))
                    {
                        return false;
                    }
                    transactionQuery += createLineStatement;
                }

                transactionQuery += "COMMIT;";
                this.ExecuteQuery(transactionQuery,out logMessage);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return true;
        }

        private MySqlConnection CreateConnection()
        {
            String connectionString = String.Format("server={0};user id={1}; password={2}; database={3};Allow User Variables=True",
                this.ConnectionSettings.Server,this.ConnectionSettings.Username,
                this.ConnectionSettings.Password,this.ConnectionSettings.DatabaseName
                );

            return new MySqlConnection(connectionString);
        }

        private bool ExecuteQuery(string query,out string logMessage)
        {
            logMessage = "";
            try
            {
                using (DbConnection connection = this.CreateConnection())
                {
                    connection.Open();
                    logMessage = "Method ExecuteQuery: Beginning execution of query :" + query;
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandTimeout = -1;
                    int result = command.ExecuteNonQuery();
                    logMessage += "Method ExecuteQuery: Execution Completed Succesfully";
                    connection.Close();
                    return true;
                }

                /*logMessage = "Method ExecuteQuery: Beginning execution of query :"+query;
                MySqlScript script = new MySqlScript(this.CreateConnection(), query);
                script.Delimiter = "$$";
                
                script.Execute();
                logMessage += "Method ExecuteQuery: Execution Completed Succesfully";
                return true;*/
            }
            catch(Exception exception)
            {
                string errorMessage = exception.Message;
                if(exception.InnerException!=null && !String.IsNullOrEmpty(exception.InnerException.Message))
                {
                    errorMessage += exception.InnerException.Message;
                }
                logMessage += "Error in ExecuteQuery" + errorMessage;
                throw exception;
            }
            finally
            {
            }
        }

        private string CreateLineStatement(Line line,DOC_TYPES docType,int lineCounter)
        {
            string filename = "";
            List<string> arguments = null;
            switch (docType)
            {
                case DOC_TYPES.ORDER:
                    arguments = new List<string>() { String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode, 
                                                              String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                                line.Qty1.ToString() 
                                                            };
                    filename = CREATE_ORDER_LINE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVOICE:
                    arguments = new List<string>() { String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode, 
                                                              String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                                line.Qty1.ToString(),
                                                                "'"+line.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")+"'"
                                                            };
                    filename = CREATE_INVOICE_LINE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVOICE_SALES:
                    arguments = new List<string>() { String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode, 
                                                              String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                                line.Qty1.ToString(),
                                                                "'"+line.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")+"'"
                                                            };
                    filename = CREATE_INVOICE_LINE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.RECEPTION:                
                //case DOC_TYPES.MATCHING:// Ghost in the shell...
                case DOC_TYPES.PICKING:
                //case DOC_TYPES.TRANSFER: //We do not export this type
                case DOC_TYPES.TAG: //TODO
                    arguments = new List<string>() { String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode, 
                                                              String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                                line.Qty1.ToString()
                                                            };
                    filename = CREATE_LABEL_LINE_STATEMENT_FILE;
                    break;
                //case DOC_TYPES.PRICE_CHECK: //We do not export this type
                case DOC_TYPES.COMPETITION:
                    arguments = new List<string>() { String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode, 
                                                              String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                                line.Qty1.ToString() 
                                                            };
                    filename = CREATE_TRANSACTION_LINE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVENTORY://We don't want these to export in the application.It CAN ONLY export to specific table!!!
                    arguments = new List<string>() {
                                                    lineCounter.ToString(),
                                                    String.IsNullOrEmpty(line.ProdCode) ? "''" : line.ProdCode,
                                                    String.IsNullOrEmpty(line.ProdBarcode) ? "''" : line.ProdBarcode,
                                                    line.Qty1.ToString()
                                                   };
                    filename = CREATE_INVENTORY_LINE_STATEMENT_FILE;
                    break;
                default :
                    throw new Exception(docType + " is not supported for Reflexis Export");
            }

            if (File.Exists(filename) && arguments!=null)
            {
                string createDocumentStatement = String.Format(File.ReadAllText(filename), arguments.ToArray());
                return createDocumentStatement;
            }
            else
            {
                throw new Exception("File not found: " + filename);
            }
        }

        private string CreateDocumentStatement(Header document,string remoteIP)
        {
            string filename = "";
            List<string> arguments = null;
            switch (document.DocType)
            {
                case DOC_TYPES.ORDER:
                    arguments = new List<string>() { Settings.DatabaseViewSettings.WarehousesView, document.CustomerCode };
                    filename = CREATE_ORDER_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVOICE:
                    arguments = new List<string>() { Settings.DatabaseViewSettings.WarehousesView, document.CustomerCode };
                    filename = CREATE_INVOICE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVOICE_SALES:
                    arguments = new List<string>() { Settings.DatabaseViewSettings.WarehousesView, document.CustomerCode };
                    filename = CREATE_INVOICE_STATEMENT_FILE;
                    break;
                case DOC_TYPES.RECEPTION:                
                //case DOC_TYPES.MATCHING:// Ghost in the shell...
                case DOC_TYPES.PICKING:
                //case DOC_TYPES.TRANSFER: //We do not export this type
                case DOC_TYPES.TAG: //TODO
                    arguments = new List<string>() { Settings.DatabaseViewSettings.WarehousesView, document.TerminalID.ToString(),"'"+remoteIP+"'" };
                    filename = CREATE_LABEL_STATEMENT_FILE;
                    break;
                //case DOC_TYPES.PRICE_CHECK: //We do not export this type
                case DOC_TYPES.COMPETITION:
                    arguments = new List<string>() { Settings.DatabaseViewSettings.WarehousesView, document.CustomerCode };
                    filename = CREATE_TRANSACTION_STATEMENT_FILE;
                    break;
                case DOC_TYPES.INVENTORY://We don't want these to export in the application.It CAN ONLY export to specific table!!!
                    arguments = new List<string>() { 
                                                    Settings.DatabaseViewSettings.WarehousesView,
                                                    document.TerminalID.ToString(),
                                                    "'"+document.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")+"'"
                                                   };
                    filename = CREATE_INVENTORY_STATEMENT_FILE;
                    break;
                default:
                    throw new Exception(document.DocType + " is not supported for Reflexis Export");
            }

            if (File.Exists(filename) && arguments!=null)
            {                
                string createDocumentStatement = String.Format(File.ReadAllText(filename), arguments.ToArray());
                return createDocumentStatement;
            }
            else
            {
                throw new Exception("File not found: " + filename);
            } 
        }

    }
}
