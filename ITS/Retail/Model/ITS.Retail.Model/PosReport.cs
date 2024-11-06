using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System.Web.Mvc;
using ITS.Retail.Model.Attributes;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Model
{
    [Updater(Order = 59,
      Permissions = eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class PosReport : BaseObj
    {


        public PosReport()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PosReport(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop =
                    CriteriaOperator.And(
                   new ContainsOperator("PosOposReportSettings", new BinaryOperator("POS.Oid", deviceID))
                   );
                    break;
            }



            return crop;
        }


        // Fields...
        private string _Format;
        private string _Code;
        private string _Description;


        [Size(SizeAttribute.Unlimited)]
        [AllowHtml]
        public string Format
        {
            get
            {
                return _Format;
            }
            set
            {
                SetPropertyValue("Format", ref _Format, value);
            }
        }

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

        [DescriptionField]
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


        [Association("PosReport-PosOposReportSettings")]
        public XPCollection<PosOposReportSettings> PosOposReportSettings
        {
            get
            {
                return GetCollection<PosOposReportSettings>("PosOposReportSettings");
            }
        }

    }
}
