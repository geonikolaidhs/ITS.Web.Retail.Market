using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList.Data;
using ITS.Retail.Model;
using Mono.CSharp;
using System.Drawing;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.XtraEditors.Controls;

namespace ITS.Retail.ReportDesigner
{
    public class RetailReportExtension : ReportDesignExtension
    {
        //XPCollection<PayGrade> payGrades = new XPCollection<PayGrade>();
        UnitOfWork uow;

        public RetailReportExtension(UnitOfWork session)
        {
            uow = session;
        }

        public override void AddParameterTypes(IDictionary<Type, string> dictionary)
        {
            base.AddParameterTypes(dictionary);
            dictionary.Add(typeof(Guid), "Guid");
        }

        public override Type[] GetEditableDataTypes()
        {
            return typeof(Address).Assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(LookupField)) &&
                g.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() == 0).ToArray();
        }

        // repository item to show in params/filterstring
        private RepositoryItem CreateRepositoryItem(Type type)
        {
            RepositoryItemLookUpEdit item = new RepositoryItemLookUpEdit();
            item.NullText = "[Select " + type.Name + "]";
            item.DataSource = new XPCollection(XpoHelper.GetNewUnitOfWork(), type);
            item.DisplayMember = "Description";

            item.ValueMember = "Oid";

            return item;
        }

        protected override RepositoryItem CreateRepositoryItem(DevExpress.Data.DataColumnInfo dataColumnInfo, Type dataType, XtraReport report)
        {
            return CreateRepositoryItem(dataType);
        }

        protected override RepositoryItem CreateRepositoryItem(DevExpress.XtraReports.Parameters.Parameter parameter, Type dataType, XtraReport report)
        {
            return CreateRepositoryItem(dataType);
        }

        public override Type[] GetSerializableDataTypes()
        {
            return GetEditableDataTypes();
        }

        // when serializing report params/filterstring data
        protected override string SerializeData(object data, XtraReport report)
        {
            if (data is BaseObj)
            {
                return ((BaseObj)data).Oid.ToString();
            }
            else if (data is Guid)
            {
                return ((Guid)data).ToString();
            }
            else
            {
                return base.SerializeData(data, report);
            }
        }

        // for deserializing report params/filterstring data
        protected override object DeserializeData(string value, Type destType, XtraReport report)
        {
            if (destType.IsSubclassOf(typeof(BaseObj)))
            {
                return uow.GetObjectByKey(destType, Guid.Parse(value));
            }
            else if (destType == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            else
            {
                return base.DeserializeData(value, destType, report);
            }

        }


    }
}
