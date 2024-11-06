using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Transactions
{
        public class ElectronicJournalFilePackage : BaseObj
        {
            public ElectronicJournalFilePackage()
                : base()
            {
                // This constructor is used when an object is loaded from a persistent storage.
                // Do not place any code here.
            }

            public ElectronicJournalFilePackage(Session session)
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

            private Guid _Store;
            public Guid Store
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

            private Guid _POS;
            public Guid POS
            {
                get
                {
                    return _POS;
                }
                set
                {
                    SetPropertyValue("POS", ref _POS, value);
                }
            }

            private int _ZReportNumber;
            public int ZReportNumber
            {
                get
                {
                    return _ZReportNumber;
                }
                set
                {
                    SetPropertyValue("ZReportNumber", ref _ZReportNumber, value);
                }
            }

            private DateTime _Date;
            public DateTime Date
            {
                get
                {
                    return _Date;
                }
                set
                {
                    SetPropertyValue("Date", ref _Date, value);
                }
            }

            //byte[] savedDataFile;
            //public byte[] SavedDataFile
            //{
            //    get { return savedDataFile; }
            //    set { SetPropertyValue&lt;byte[]&gt;("SavedDataFile", ref savedDataFile, value); }
            //}

            private byte[] _PackageData;
            [Size(SizeAttribute.Unlimited)]
            public byte[] PackageData
            {
                get
                {
                    return _PackageData;
                }
                set
                {
                    SetPropertyValue("PackageData", ref _PackageData, value);
                }
            }


        }
    }
