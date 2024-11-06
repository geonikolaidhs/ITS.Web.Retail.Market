using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ITS.POS.Client.Helpers
{
    public class DatabaseCommand
    {
        public string Command { get; set; }
        public string ConnectionString { get; set; }
        public string SelectColumn { get; set; }
        public string ApplyOn { get; set; }
        public string ApplyTime { get; set; }
        public DBType DbType { get; set; }
        private DataAdapter DataAdapter { get; set; }
        private DbConnection DatabaseConnection { get; set; }

        public Dictionary<string, string> Parameters;

        public DatabaseCommand()
        {
            this.Parameters = new Dictionary<string, string>();
        }

        public void ExecuteCommand(ref ITS.Retail.Model.DocumentHeader document)
        {
            string commandText = String.Empty;
            string queryValue = String.Empty;
            try
            {
                PropertyInfo prop = document.GetType().GetProperty(this.ApplyOn, BindingFlags.Public | BindingFlags.Instance);

                if (null != prop && prop.CanWrite)
                {
                    foreach (var p in this.Parameters)
                    {
                        if (p.Value.Contains("@"))
                        {
                            string replace = p.Value.Replace("@", "").ToString();
                            queryValue = GetPropertyValue(document, replace).ToString();
                        }
                        else
                        {
                            queryValue = p.Value;
                        }
                        commandText = this.Command.Replace(p.Key, "'" + queryValue.ToString() + "'");
                    }
                }

                if (prop.PropertyType.IsValueType || prop.GetType().IsNested == false)
                {
                    Type resultType = prop.PropertyType;
                    var commandResult = ExecuteCommand(commandText);
                    if (resultType == typeof(string))
                    {
                        prop.SetValue(document, commandResult, null);
                    }
                    else
                    {
                        var result = Activator.CreateInstance(prop.PropertyType);
                        if (!String.IsNullOrEmpty(commandResult.ToString()))
                        {
                            result = Convert.ChangeType(commandResult, resultType);
                            prop.SetValue(document, result, null);
                        }
                    }

                    if (this.ApplyOn == "PreviousBalance")
                    {
                        SetBalancePrintedValues(commandResult, document);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
            }
        }

        private string ExecuteCommand(string commandText)
        {
            string rowValue = string.Empty;
            try
            {
                switch (this.DbType)
                {
                    case DBType.SQLServer:
                        this.DatabaseConnection = new SqlConnection(this.ConnectionString);
                        this.DataAdapter = new SqlDataAdapter(commandText, ConnectionString);
                        break;

                }

                try
                {
                    DataSet ds = new DataSet();
                    DatabaseConnection.Open();
                    DataAdapter.Fill(ds);
                    DataTable dt = new DataTable();
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        DataRow row = dt.Rows[0];
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (column.ColumnName == this.SelectColumn)
                                {
                                    rowValue = row[column.ColumnName].ToString();
                                    DatabaseConnection.Close();
                                    return rowValue;
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    string errorMessage = ex.GetFullMessage();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
            }
            finally
            {
                DatabaseConnection.Close();
            }

            return rowValue;
        }




        private static object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }


        private static Type GetPropertyType(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyType(GetPropertyType(src, temp[0]), temp[1]);

            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.PropertyType : null;
            }
        }


        private void SetBalancePrintedValues(string previousBalance, ITS.Retail.Model.DocumentHeader document)
        {
            try
            {
                if (!String.IsNullOrEmpty(previousBalance.ToString()))
                {
                    decimal amount = document.DocumentPayments.Where(x => x.PaymentMethod.PaymentMethodType == ePaymentMethodType.CREDIT).Sum(y => y.Amount);
                    document.Balance = document.PreviousBalance + amount;
                    document.PreviousBalance = document.Balance - amount;
                    document.PrintedPreviousBalance = document.PreviousBalance.ToString("F") + " €";
                    document.PrintedBalance = document.Balance.ToString("F") + " €";
                }
                else
                {
                    document.PrintedPreviousBalance = "NA";
                    document.PrintedBalance = "NA";
                }
            }
            catch (Exception ex)
            {
                string erroMessage = ex.GetFullMessage();
            }
        }



    }
}
