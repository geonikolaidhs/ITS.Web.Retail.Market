using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class CustomDataViewRecord
    {
        public CustomDataView CustomDataView;
        public string OidString;
        public Dictionary<string, string> parametersArray;

        List<string> _columnNames = null;
        private List<List<object>> _MultipleValues = null;

        public CustomDataViewRecord(CustomDataView dataView, Guid oid, Dictionary<string, string> parameters = null)             
        {
            Guid[] oids = new Guid[1];
            oids[0] = oid;
            InitializeCustomDataViewRecord(dataView, oids, parameters);
        }

        public CustomDataViewRecord(CustomDataView dataView, Guid[] oids, Dictionary<string, string> parameters = null)
        {
            InitializeCustomDataViewRecord(dataView, oids, parameters);
        }

        private void InitializeCustomDataViewRecord(CustomDataView dataView, Guid[] oids, Dictionary<string, string> parameters)
        {
            this.CustomDataView = dataView;
            this.OidString = oids.Select(x => "'" + x.ToString() + "'").Aggregate((f, s) => f + "," + s);
            this.parametersArray = parameters;
        }
    
        public List<string> ColumnNames
        {
            get
            {
                if (_columnNames == null)
                {
                    _columnNames = new List<string>();
                    SelectStatementResult[] resultset = this.CustomDataView.CreateDataView(this.CustomDataView.Session, this.OidString, parametersArray);
                    foreach (SelectStatementResultRow row in resultset[1].Rows)
                    {
                        for (int i = 0; i < resultset[0].Rows.Count(); i++)
                        {
                            _columnNames.Add(resultset[0].Rows[i].Values[0].ToString());
                        }
                    }
                }
                return _columnNames;
            }
        }

        public List<List<object>> MultipleValues
        {
            get
            {
                if (_MultipleValues == null)
                {
                    _MultipleValues = new List<List<object>>();
                    SelectStatementResult[] resultset = this.CustomDataView.CreateDataView(this.CustomDataView.Session, this.OidString, parametersArray);
                    foreach (SelectStatementResultRow row in resultset[1].Rows)
                    {
                        List<object> temp = new List<object>();
                        foreach (object obj in row.Values)
                        {
                            temp.Add(obj);
                        }
                        _MultipleValues.Add(temp);
                    }
                }
                return _MultipleValues;
            }
        }
    }
}
