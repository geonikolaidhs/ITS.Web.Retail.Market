using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 415, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class CustomReport : LookupField, IRequiredOwner
    {


        public CustomReport()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public CustomReport(Session session)
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
                    Type thisType = typeof(CustomReport);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (String.IsNullOrEmpty(deviceID))
                    {
                        throw new Exception(typeof(CustomReport).Name + ".GetUpdaterCriteria(); POS: Oid is empty");
                    }
                    //CriteriaOperator joinCriteria = new OperandProperty("^.Report") == new OperandProperty("Oid");
                    //JoinOperand joinOperand = new JoinOperand(typeof(POSReportSetting).Name, joinCriteria, Aggregate.Exists, new BinaryOperator("POS.Oid", deviceID));
                    //crop = joinOperand;
                    break;
            }
            return crop;
        }

        private string _Code;
        private string _Title;
        private byte[] _ReportFile;
        private string _FileName;
        private eCultureInfo _CultureInfo;
        private CompanyNew _Owner;

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
        [DisplayOrder(Order = 7)]
        public eCultureInfo CultureInfo
        {
            get
            {
                return _CultureInfo;
            }
            set
            {
                SetPropertyValue("CultureInfo", ref _CultureInfo, value);
            }
        }

        [DisplayOrder(Order = 4)]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                SetPropertyValue("FileName", ref _FileName, value);
            }
        }
        [DisplayOrder(Order = 3)]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                SetPropertyValue("Title", ref _Title, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public Byte[] ReportFile
        {
            get
            {
                return _ReportFile;
            }
            set
            {
                SetPropertyValue("ReportFile", ref _ReportFile, value);
            }
        }

        [Indexed("Owner;GCRecord", Unique = true)]
        [DisplayOrder (Order= 1 )]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }
        [DisplayOrder(Order = 5)]
        public string ReportType
        {
            get
            {
                return _ReportType;
            }
            set
            {
                SetPropertyValue("ReportType", ref _ReportType, value);
            }
        }
        [DisplayOrder(Order = 6)]
        public string ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                SetPropertyValue("ObjectType", ref _ObjectType, value);
            }
        }



        private ReportCategory _ReportCategory;
        private string _ObjectType;
        private string _ReportType;
        [Association("ReportCategory-CustomReports"), Indexed(Unique = false)]
        [DisplayOrder(Order = 8)]
        public ReportCategory ReportCategory
        {
            get
            {
                return _ReportCategory;
            }
            set
            {
                SetPropertyValue("ReportCategory", ref _ReportCategory, value);
            }
        }

        [Association("CustomReport-ReportRoles"), Aggregated]
        public XPCollection<ReportRole> ReportRoles
        {
            get
            {
                return GetCollection<ReportRole>("ReportRoles");
            }
        }

        [Aggregated, Association("CustomReport-DocumentTypeCustomReports")]
        public XPCollection<DocumentTypeCustomReport> DocumentTypeCustomReports
        {
            get
            {
                return GetCollection<DocumentTypeCustomReport>("DocumentTypeCustomReports");
            }
        }
    }
}
