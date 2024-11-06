//-----------------------------------------------------------------------
// <copyright file="DataFileReceived.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;

namespace ITS.Retail.Model
{
    public class DataFileReceived : BasicObj, IRequiredOwner
    {
        public DataFileReceived()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataFileReceived(Session session)
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

        // Fields...
        private string _Filename;
        private string _StoreUserRoleDescription;
        private string _CustomerUserRoleDescription;
        private bool _CreateUsersForStores;
        private bool _CreateUsersForCustomers;
        private string _Encoding;
        private int _Tries;
        private Image _Image;
        private string _Email;
        private Guid _AgendOid;
        private bool _Decoded;
        private string _FileContext;
        private string _MD5;
        private string _Locale;
        private CompanyNew _Owner;
        private string _RootItemCategoryCode;
        private bool _MultipleItemCategoryTrees;
        private bool _MakeZeroPricesUserDefinable;
        private bool _SendEmailsForImages;
        private bool _OneStorePerCustomer;


        public String Description
        {
            get
            {
                return Filename;
            }
        }

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

        public string MD5
        {
            get
            {
                return _MD5;
            }
            set
            {
                SetPropertyValue("MD5", ref _MD5, value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        public string FileContext
        {
            get
            {
                return _FileContext;
            }
            set
            {
                SetPropertyValue("FileContext", ref _FileContext, value);
            }
        }
        public bool Decoded
        {
            get
            {
                return _Decoded;
            }
            set
            {
                SetPropertyValue("Decoded", ref _Decoded, value);
            }
        }

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                SetPropertyValue("Image", ref _Image, value);
            }
        }


        public Guid AgendOid
        {
            get
            {
                return _AgendOid;
            }
            set
            {
                SetPropertyValue("AgendOid", ref _AgendOid, value);
            }
        }
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                SetPropertyValue("Email", ref _Email, value);
            }
        }

        public int Tries
        {
            get
            {
                return _Tries;
            }
            set
            {
                SetPropertyValue("Tries", ref _Tries, value);

            }
        }

        public string Encoding
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

        public string RootItemCategoryCode
        {
            get
            {
                return _RootItemCategoryCode;
            }
            set
            {
                SetPropertyValue("RootItemCategoryCode", ref _RootItemCategoryCode, value);
            }
        }

        public bool MultipleItemCategoryTrees
        {
            get
            {
                return _MultipleItemCategoryTrees;
            }
            set
            {
                SetPropertyValue("MultipleItemCategoryTrees", ref _MultipleItemCategoryTrees, value);
            }
        }

        public bool SendEmailsForImagesOnlyOnError
        {
            get
            {
                return _SendEmailsForImages;
            }
            set
            {
                SetPropertyValue("SendEmailsForImages", ref _SendEmailsForImages, value);
            }
        }
        

        public bool CreateUsersForCustomers
        {
            get
            {
                return _CreateUsersForCustomers;
            }
            set
            {
                SetPropertyValue("CreateUsersForCustomers", ref _CreateUsersForCustomers, value);
            }
        }

        public bool MakeZeroPricesUserDefinable
        {
            get
            {
                return _MakeZeroPricesUserDefinable;
            }
            set
            {
                SetPropertyValue("MakeZeroPricesUserDefinable", ref _MakeZeroPricesUserDefinable, value);
            }
        }


        public bool OneStorePerCustomer
        {
            get
            {
                return _OneStorePerCustomer;
            }
            set
            {
                SetPropertyValue("OneStorePerCustomer", ref _OneStorePerCustomer, value);
            }
        }

        public bool CreateUsersForStores
        {
            get
            {
                return _CreateUsersForStores;
            }
            set
            {
                SetPropertyValue("CreateUsersForStores", ref _CreateUsersForStores, value);
            }
        }


        public string CustomerUserRoleDescription
        {
            get
            {
                return _CustomerUserRoleDescription;
            }
            set
            {
                SetPropertyValue("CustomerUserRoleDescription", ref _CustomerUserRoleDescription, value);
            }
        }


        public string StoreUserRoleDescription
        {
            get
            {
                return _StoreUserRoleDescription;
            }
            set
            {
                SetPropertyValue("StoreUserRoleDescription", ref _StoreUserRoleDescription, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Filename
        {
            get
            {
                return _Filename;
            }
            set
            {
                SetPropertyValue("Filename", ref _Filename, value);
            }
        }

        public string Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }



    }
}
