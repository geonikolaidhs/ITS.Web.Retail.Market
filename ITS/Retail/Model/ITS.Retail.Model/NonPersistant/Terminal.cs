//-----------------------------------------------------------------------
// <copyright file="Terminal.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{

    public abstract class Terminal : LookupField
    {
        public Terminal()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Terminal(Session session)
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

        private string _Name;
        private string _IPAddress;

        [DescriptionField]
        public string Name {
            get {
                return _Name;
            }
            set {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

		private int _ID;
        [Indexed("Store;GCRecord;ObjectType", Unique = true)]
		public int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				SetPropertyValue("ID", ref _ID, value);
			}
		}
        
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                SetPropertyValue("IPAddress", ref _IPAddress, value);
            }
        }
        private string _Remarks;
        [Size(2048)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private Store _Store;
        [Association("Store-Terminals")]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }
        

        [Association("Terminal-TerminalDeviceAssociations"),Aggregated]
        public XPCollection<TerminalDeviceAssociation> TerminalDeviceAssociations {
            get {
				return GetCollection<TerminalDeviceAssociation>("TerminalDeviceAssociations");
            }
        }

        //public override void GetData(Session myses, object item)
        //{
        //    base.GetData(myses, item);
        //    
        //    Terminal t = item as Terminal;
        //    Name = t.Name;
        //    IPAddress = t.IPAddress;
        //    Remarks = t.Remarks;
        //    Store = GetLookupObject<Store>(myses, t.Store) as Store;

        //}


    }

}