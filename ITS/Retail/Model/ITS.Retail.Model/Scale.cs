//-----------------------------------------------------------------------
// <copyright file="Supplier.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{    
    public class Scale : Lookup2Fields    
    {
        public Scale()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Scale(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            Encoding = System.Text.Encoding.UTF8.CodePage;
        }


        /// <summary>
        /// Gets or sets the format string of the scale export
        /// </summary>
        [DisplayOrder (Order= 3)]
        public string ExportFormatString
        {
            get
            {
                return _ExportFormatString;
            }
            set
            {
                SetPropertyValue("ExportFormatString", ref _ExportFormatString, value);
            }
        }

        /// <summary>
        /// Gets or sets the full path of the file to be saved for the scale export.
        /// </summary>
        [DisplayOrder (Order= 4)]
        public string ExportFullFilePath
        {
            get
            {
                return _ExportFullFilePath;
            }
            set
            {
                SetPropertyValue("ExportFullFilePath", ref _ExportFullFilePath, value);
            }
        }


        public int Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                SetPropertyValue("Encoding", ref _Encoding, value);
            }
        }



        public bool UseDirectSQL
        {
            get
            {
                return _UseDirectSQL;
            }
            set
            {
                SetPropertyValue("UseDirectSQL", ref _UseDirectSQL, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string DirectSQL
        {
            get
            {
                return _DirectSQL;
            }
            set
            {
                SetPropertyValue("DirectSQL", ref _DirectSQL, value);
            }
        }

        public DateTime ExportVersion
        {
            get
            {
                return _ExportVersion;
            }
            set
            {
                SetPropertyValue("ExportVersion", ref _ExportVersion, value);
            }
        }

        // Fields...
        private bool _UseDirectSQL;
        private string _DirectSQL;
        private int _Encoding;
        private string _ExportFullFilePath;
        private string _ExportFormatString;
        private DateTime _ExportVersion;
    }
}
