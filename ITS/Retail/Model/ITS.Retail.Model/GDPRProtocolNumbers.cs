//-----------------------------------------------------------------------
// <copyright file="Barcode.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;

namespace ITS.Retail.Model
{
    [Serializable]
    [EntityDisplayName("GDPRProtocolNumbers", typeof(Resources))]
    public class GDPRProtocolNumbers : BaseObj
    {
        public GDPRProtocolNumbers()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public GDPRProtocolNumbers(Session session)
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
        #region "ExportProtocolNumber"
        private int _ExportProtocolNumber;
        public int ExportProtocolNumber
        {
            get { return _ExportProtocolNumber; }
            set { SetPropertyValue("ExportProtocolNumber", ref _ExportProtocolNumber, value); }
        }
        #endregion
        #region "AnonymizationProtocolNumber"
        private int _AnonymizationProtocolNumber;
        public int AnonymizationProtocolNumber
        {
            get { return _AnonymizationProtocolNumber; }
            set { SetPropertyValue("AnonymizationProtocolNumber", ref _AnonymizationProtocolNumber, value); }
        }
        #endregion
    }

}