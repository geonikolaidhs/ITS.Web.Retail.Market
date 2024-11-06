using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel.Importable
{
    public class BarcodeImportable : ImportableViewModel<Barcode>
    {
        public override string ImportObjectUniqueKey
        {
            get { return Code; }
        }


        public string Code { get; set; }

        [KeyImportable("Code")]
        [Display(AutoGenerateField = false)]
        public string PaddedCode { get; set; }


        public string ItemCode { get; set; }

        public override string EntityName
        {
            get { return "Barcode"; }
        }

        public override bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store)
        {
            return false;
        }
        
        public override bool HasNewChildren { get { return false; } }

        public override void CheckWithDatabase(UnitOfWork uow, Guid owner)
        {
            CompanyNew Owner = uow.GetObjectByKey<CompanyNew>(owner);
            OwnerApplicationSettings appSetings = Owner.OwnerApplicationSettings;
            this.PaddedCode = (appSetings.PadBarcodes) ? this.Code.PadLeft(appSetings.BarcodeLength, appSetings.BarcodePaddingCharacter[0]) : this.Code;
            base.CheckWithDatabase(uow, owner);
        }
    }
}
