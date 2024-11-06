//-----------------------------------------------------------------------
// <copyright file="Permission.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class Permission : BaseObj
    {
        public Permission()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Permission(Session session)
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

        private bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                SetPropertyValue("Visible", ref _Visible, value);
            }
        }

        private bool _CanInsert;
        public bool CanInsert
        {
            get
            {
                return _CanInsert;
            }
            set
            {
                SetPropertyValue("CanInsert", ref _CanInsert, value);
            }
        }

        private bool _CanUpdate;
        public bool CanUpdate
        {
            get
            {
                return _CanUpdate;
            }
            set
            {
                SetPropertyValue("CanUpdate", ref _CanUpdate, value);
            }
        }

        private bool _CanDelete;
        public bool CanDelete
        {
            get
            {
                return _CanDelete;
            }
            set
            {
                SetPropertyValue("CanDelete", ref _CanDelete, value);
            }
        }

		private bool _CanExport;
		public bool CanExport
		{
			get
			{
				return _CanExport;
			}
			set
			{
				SetPropertyValue("CanExport", ref _CanExport, value);
			}
		}


    }

}