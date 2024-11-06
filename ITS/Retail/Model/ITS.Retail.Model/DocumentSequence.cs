//-----------------------------------------------------------------------
// <copyright file="DocumentSequence.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 910,Permissions = eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER)]
    public class DocumentSequence : LookupField, IOwner
    {
        public DocumentSequence()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSequence(Session session)
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

        public DocumentSequence(Session session, DocumentSeries series)
            : base(session)
        {
            this.DocumentSeries = series;
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CompanyNew Owner
        {
            get
            {
                if (DocumentSeries == null)
                    return null;
                return DocumentSeries.Owner;
            }
        }

        private DocumentSeries _DocumentSeries;
        [Indexed("GCRecord", Unique =true)]
        public DocumentSeries DocumentSeries
        {
            get
            {
                return _DocumentSeries;
            }
            set
            {
                if (_DocumentSeries == value)
                    return;

                // Store a reference to the person's former house. 
                DocumentSeries docseries = _DocumentSeries;
                _DocumentSeries = value;

                if (IsLoading) return;

                // Remove a reference to the house's owner, if the person is its owner. 
                if (docseries != null && docseries.DocumentSequence == this)
                    docseries.DocumentSequence = null;

                // Specify the person as a new owner of the house. 
                if (_DocumentSeries != null)
                    _DocumentSeries.DocumentSequence = this;

                OnChanged("DocumentSeries");
            }
        }

        private int _DocumentNumber;
        public int DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }

        protected override void OnSaving()
        {
            if ( this.DocumentSeries != null && this.DocumentSeries.DocumentSequence == null )
            {
                this.DocumentSeries.DocumentSequence = this;
            }

            base.OnSaving();
        }
    }

}
