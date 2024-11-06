using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;


namespace ITS.Retail.WebClient.ViewModel
{
    public class DocumentDetailDiscountViewModel : IPersistableViewModel
    {


        public Guid DocumentDetail { get; set; }
        public Guid Type { get; set; }

        public eDiscountSource DiscountSource { get; set; }
        public int Priority { get; set; }
        public eDiscountType DiscountType { get; set; }
        public bool DiscardsOtherDiscounts { get; set; }
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }

        public Guid CustomEnumerationValue1 { get; set; }
        public Guid CustomEnumerationValue2 { get; set; }
        public Guid CustomEnumerationValue3 { get; set; }
        public Guid CustomEnumerationValue4 { get; set; }
        public Guid CustomEnumerationValue5 { get; set; }

        public DateTime DateField5 { get; set; }
        public DateTime DateField4 { get; set; }
        public DateTime DateField3 { get; set; }
        public DateTime DateField2 { get; set; }
        public DateTime DateField1 { get; set; }
        public decimal DecimalField5 { get; set; }
        public decimal DecimalField4 { get; set; }
        public decimal DecimalField3 { get; set; }
        public decimal DecimalField2 { get; set; }
        public decimal DecimalField1 { get; set; }
        public string StringField5 { get; set; }
        public string StringField4 { get; set; }
        public string StringField3 { get; set; }
        public string StringField2 { get; set; }
        public string StringField1 { get; set; }
        public int IntegerField5 { get; set; }
        public int IntegerField4 { get; set; }
        public int IntegerField3 { get; set; }
        public int IntegerField2 { get; set; }
        public int IntegerField1 { get; set; }


        public Guid Oid { get; set; }

        public Type PersistedType
        {
            get { return typeof(DocumentDetailDiscount); }
        }

        public bool IsDeleted { get; set; }

        public void UpdateModel(Session uow)
        {
            this.UpdateProperties(uow);
        }

        public bool Validate(out string message)
        {
            throw new NotImplementedException();
        }
    }
}
