using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;
namespace ITS.Retail.Common.ViewModel
{
    public class CustomerPointTransaction: BasicViewModel
    {
        // Fields...
        private decimal _Points;
        private DateTime _DateTime;
        private string _Remarks;
        private Guid _User;
        private Guid? _Customer;
        private Guid _Oid;
        
        [System.ComponentModel.DataAnnotations.Required]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType=typeof(Resources), ErrorMessageResourceName="PleaseSelectACustomer")]
        [Binding("Customer_VI")]
        public Guid? Customer
        {
            get
            {                
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Required]
        public Guid User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Required]
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

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime DateTime
        {
            get
            {
                return _DateTime;
            }
            set
            {
                SetPropertyValue("DateTime", ref _DateTime, value);
            }
        }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.RegularExpression("^(-10000|10000|-?1[0-9]|-?[1-9][0-9]?|-?[1-9][0-9][0-9]?|-?[1-9][0-9][0-9][0-9]?|-?[1-9][0-9][0-9][0-9][0-9]?)$", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "InvalidValue")]
        public decimal Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Points", ref _Points, value);
            }
        }

    }
}
