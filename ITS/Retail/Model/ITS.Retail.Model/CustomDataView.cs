using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{
    [Updater(Order = 1160, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class CustomDataView : BaseObj, IRequiredOwner
    {
        private string _Description;
        private string _Query;
        private CustomDataViewCategory _Category;
        private CompanyNew _Owner;
        public CustomDataView()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomDataView(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:

                    Type thisType = typeof(CustomDataView);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("ShowSettings", ShowSettings.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("Parameters", Parameters.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("Roles", Roles.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }

        [DisplayOrder(Order = 1)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [DisplayOrder(Order = 1)]
        public string Query
        {
            get
            {
                return _Query;
            }
            set
            {
                SetPropertyValue("Query", ref _Query, value);
            }
        }

        [Association("DataViewCategory-DataViews")]
        [DisplayOrder(Order = 2)]
        public CustomDataViewCategory Category
        {
            get
            {
                return _Category;
            }
            set
            {
                SetPropertyValue("Category", ref _Category, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        [Association("CustomDataViews-Roles")]
        [DisplayOrder(Order = 5)]
        public XPCollection<Role> Roles
        {
            get
            {
                return GetCollection<Role>("Roles");
            }
        }

        [Aggregated, Association("CustomDataView-ShowSettings")]
        [DisplayOrder(Order = 3)]
        public XPCollection<CustomDataViewShowSettings> ShowSettings
        {
            get
            {
                return GetCollection<CustomDataViewShowSettings>("ShowSettings");
            }
        }

        [Aggregated, Association("CustomDataView-Parameters")]
        [DisplayOrder(Order = 4)]
        public XPCollection<CustomDataViewParameter> Parameters
        {
            get
            {
                return GetCollection<CustomDataViewParameter>("Parameters");
            }
        }

        public SelectStatementResult[] CreateDataView(Session uow, string oids = null, Dictionary<string, string> parameters = null)
        {
            SelectStatementResult[] selectedData;
            string SQLquery = this.Query;

            if (!String.IsNullOrEmpty(oids))
            {
                SQLquery = SQLquery.Replace("{OIDS}", oids);
            }
            if (parameters != null)
            {
                foreach (CustomDataViewParameter param in this.Parameters)
                {
                    if (SQLquery.Contains(param.Name))
                    {
                        if(parameters.ContainsKey(param.Name))
                        {
                            string paramValue = parameters[param.Name];
                            switch (param.ParameterType)
                            { 
                                case "DateTime":
                                    DateTime datetime = DateTime.Parse(paramValue);
                                    SQLquery = SQLquery.Replace(param.Name, datetime.ToString("yyyy-MM-dd"));
                                    break;
                                case "Boolean":
                                    bool boolValue = Boolean.Parse(paramValue);
                                    SQLquery = SQLquery.Replace(param.Name, boolValue ? "1" : "0");
                                    break;
                                default:
                                    SQLquery = SQLquery.Replace(param.Name, paramValue.ToString());
                                    break;
                            }
                        }
                    }
                }
            }
            selectedData = uow.ExecuteQueryWithMetadata(SQLquery).ResultSet;
            return selectedData;
        }     

        public Dictionary<string,string> GetDataViewColumnNames()
        {
            Dictionary<string, string> parameters = null;

            if (this.Parameters.Count > 0)
            {
                parameters = new Dictionary<string, string>();

                foreach (CustomDataViewParameter customDataViewParameter in this.Parameters)
                {
                    string val = string.Empty;
                    switch (customDataViewParameter.ParameterType)
                    {
                        case "decimal":
                            val = 0.ToString();
                            break;
                        case "string":
                            val = string.Empty;
                            break;
                        case "DateTime":
                            val = DateTime.Now.ToString();
                            break;
                        case "bool":
                            val = false.ToString();
                            break;
                        default:
                            val = Guid.Empty.ToString();
                            break;
                    }
                    parameters.Add(customDataViewParameter.Name, val);
                }

            }

            SelectStatementResult[] selectedData = this.CreateDataView(this.Session, Guid.Empty.ToString(), parameters);

            Dictionary<string,string> columnNames = new Dictionary<string, string>();

            if (selectedData.Length > 0)
            {
                int rowsLength = selectedData[1].Rows.Length + 1;
                int columnsLength = selectedData[0].Rows.Length;


                for (int column = 0; column < columnsLength; column++)
                {
                    object keyValue = selectedData[0].Rows[column].Values[0];
                    string keyStringValue = keyValue == null ? string.Empty : keyValue.ToString();
                    object typeValue = selectedData[0].Rows[column].Values[2];
                    string typeStringValue = typeValue == null ? string.Empty : typeValue.ToString();
                    columnNames.Add(keyStringValue, typeStringValue);
                }
            }
            return columnNames;
        }
    }
}
