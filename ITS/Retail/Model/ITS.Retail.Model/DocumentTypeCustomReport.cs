using System.Linq;
//-----------------------------------------------------------------------
// <copyright file="Barcode.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 620,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class DocumentTypeCustomReport : BaseObj
    {
        		
		public DocumentTypeCustomReport()
			: base()
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

        public DocumentTypeCustomReport(Session session)
			: base(session)
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

		public override void AfterConstruction()
		{
			base.AfterConstruction();
			// Place here your initialization code.
            this.Duplicates = 1;
		}

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    Type thisType = typeof(DocumentTypeCustomReport);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("DocumentType.Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        // Fields...
        private DocumentType _DocumentType;
        private CustomReport _Report;
        private UserType _UserType;
        private int _Duplicates;

        [Association("CustomReport-DocumentTypeCustomReports")]
        public CustomReport Report
        {
            get
            {
                return _Report;
            }
            set
            {
                SetPropertyValue("Report", ref _Report, value);
            }
        }

        public UserType UserType
        {
            get
            {
                return _UserType;
            }
            set
            {
                SetPropertyValue("UserType", ref _UserType, value);
            }
        }

        [Association("DocumentType-DocumentTypeCustomReports")]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        public int Duplicates
        {
            get
            {
                return _Duplicates;
            }
            set
            {
                SetPropertyValue("Duplicates", ref _Duplicates, value);
            }
        }

        public string Description
        {
            get
            {
                string reportDescr = this.Report != null ? this.Report.Description : "";
                return reportDescr + " - " + ResourcesLib.Resources.Duplicates + ": " + Duplicates;
            }
        
        }
    }
}
