using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.XtraReports.UI;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Common
{
    public class SingleObjectXtraReport : XtraReportBaseExtension
    {
        public static DataBaseConnectionType RemoteDBType { get; set; }
        public Guid ObjectOid { get; set; }
        public Type ObjectType { get; set; }

        public object Object { get; set; }

        protected override void XtraReportBaseExtension_DataSourceDemanded(object sender, EventArgs e)
        {
            if ( Object != null )
            {
                this.DataSource = this.Object;
            }
            else
            {
                CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                string filterStringWithValues = ReplaceParametersWithValues(this.FilterString);
                DevExpress.Data.Filtering.CriteriaOperator getFilterCriteria = this.GetFilterCriteria(filterStringWithValues);
                getFilterCriteria = CriteriaOperator.And(getFilterCriteria, new BinaryOperator(ModelProperty.KeyExpression, ObjectOid));
                foreach (CalculatedField calcField in this.CalculatedFields)
                {
                    getFilterCriteria = CriteriaHelper.RemoveCriteriaByFieldName(calcField.Name, getFilterCriteria);
                }
                if (OriginalQueryableSource == null)
                {
                    OriginalQueryableSource = this.ModelProperty.QueryableSource.AppendWhere(converter, null);
                }
                this.ModelProperty.QueryableSource = OriginalQueryableSource.AppendWhere(converter, getFilterCriteria);
                this.DataSource = this.ModelProperty;
            }
        }

        private string _LinqCode;
        public override string LinqCode
        {
            get
            {
                return _LinqCode;
            }
            set
            {
                DataBaseConnectionType DataBaseType;
                if (XpoHelper.databasetype== DBType.postgres)
                {
                    DataBaseType = DataBaseConnectionType.PostGreSql;
                }
                else if (XpoHelper.databasetype== DBType.Remote)
                {
                    DataBaseType = RemoteDBType;
                }
                else
                {
                    DataBaseType = DataBaseConnectionType.MSSql;
                }
                _LinqCode = value;
                this.ModelQuery = new SingleObjectLinqQuery(this.ObjectType, DataBaseType);
            }
        }

        protected override void SerializeProperties(DevExpress.XtraReports.Serialization.XRSerializer serializer)
        {
            base.SerializeProperties(serializer);
            serializer.SerializeValue("ObjectOid", this.ObjectOid, typeof(Guid)); //Required for previewing
            serializer.SerializeValue("ObjectType", this.ObjectType, typeof(Type));
            serializer.SerializeValue("CurrentDuplicate", this.CurrentDuplicate, typeof(int));
            serializer.SerializeValue("Duplicates", this.Duplicates, typeof(int));
        }

        protected override void DeserializeProperties(DevExpress.XtraReports.Serialization.XRSerializer serializer)
        {
            this.ObjectType = serializer.DeserializeValue("ObjectType", typeof(Type), null) as Type;
            base.DeserializeProperties(serializer);
            this.ObjectOid = (Guid)serializer.DeserializeValue("ObjectOid", typeof(Guid), Guid.Empty); //Required for previewing            
            this.CurrentDuplicate = (int)serializer.DeserializeValue("CurrentDuplicate", typeof(int), 1);
            this.Duplicates = (int)serializer.DeserializeValue("Duplicates", typeof(int), 1);
        }


        public override void CreateDocument(bool buildPagesInBackground)
        {
            if (this.Pages.Count == 0)
            {
                base.CreateDocument(buildPagesInBackground);
            }
        }

    }
}
