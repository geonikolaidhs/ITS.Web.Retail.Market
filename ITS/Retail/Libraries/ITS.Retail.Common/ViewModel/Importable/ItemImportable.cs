using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel.Importable
{
    public class ItemImportable : ImportableViewModel<Item>
    {
        public ItemImportable()
        {
            Barcodes = new List<BarcodeImportable>();
        }

        public override string ImportObjectUniqueKey
        {
            get { return Code; }
        }


        public override bool HasNewChildren
        {
            get
            {
                return this.Barcodes.Any(x => x.IsNew);
            }
        }

        private void PadCode(OwnerApplicationSettings appSetings)
        {
            this.PaddedCode = (appSetings.PadItemCodes) ? this.Code.PadLeft(appSetings.ItemCodeLength, appSetings.ItemCodePaddingCharacter[0]) : this.Code;
        }

        private void CreateCode(UnitOfWork uow, Guid owner)
        {
            if(String.IsNullOrEmpty(this.Code))
            {
                object maxCode = uow.Evaluate<Item>(CriteriaOperator.Parse("Max(Code)"), null);
            }
        }

        public override void CheckWithDatabase(UnitOfWork uow, Guid owner)
        {
            CompanyNew Owner = uow.GetObjectByKey<CompanyNew>(owner);
            OwnerApplicationSettings appSetings = Owner.OwnerApplicationSettings;
            PadCode(appSetings);
            base.CheckWithDatabase(uow, owner);
            this.Barcodes.ForEach(x => x.CheckWithDatabase(uow, owner));
            if(this.IsNew == false && this.Barcodes.Any(x=>x.IsNew))
            {
                this.IsNew = true;                
            }

        }

        public override Item GetPersistentObjectGeneric(UnitOfWork uow, Guid owner)
        {
            if (string.IsNullOrWhiteSpace(this.Code) == false)
            {
                return base.GetPersistentObjectGeneric(uow, owner);
            }
            else if (string.IsNullOrWhiteSpace(this.SupplierCode) == false)
            {
                string supplierCodeInDatabase = SupplierPrefix + SupplierCode;
                return uow.FindObject<Item>(
                    CriteriaOperator.And(
                        new BinaryOperator("IsActive", true),
                        new ContainsOperator("ItemBarcodes",new BinaryOperator("Barcode.Code", supplierCodeInDatabase))
                        )
                    );                
            }
            return null;
        }


        public string SupplierCode { get; set; }

        public string SupplierPrefix { get; set; }


        public string Code { get; set; }

        [KeyImportable("Code")]
        [Display(AutoGenerateField = false)]
        public string PaddedCode { get; set; }

        public string Name { get; set; }

        [LookupImportable(typeof(MeasurementUnit), "Code", "MeasurementUnit")]
        public string MeasurementUnitCode { get; set; }

        [Display(AutoGenerateField = false)]        
        public Guid MeasurementUnit { get; set; }

        [LookupImportable(typeof(VatCategory), "Code", "VatCategory")]
        public string VatLevelCode { get; set; }

        [Display(AutoGenerateField = false)]
        public Guid VatCategory { get; set; }


        [MasterDetailImportable(MasterProperty = "Code", DetailProperty = "ItemCode")]
        public List<BarcodeImportable> Barcodes { get; set; }

        public override bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store)
        {
            CompanyNew Owner = uow.GetObjectByKey<CompanyNew>(owner);
            OwnerApplicationSettings appSetings = Owner.OwnerApplicationSettings;


            Item item = this.GetPersistentObjectGeneric(uow, owner);
            MeasurementUnit mm = uow.GetObjectByKey<MeasurementUnit>(MeasurementUnit);
            if (item == null)
            {

                item = new Item(uow)
                {
                    Code = this.PaddedCode,
                    Name = this.Name,
                    Owner = Owner,
                    VatCategory = uow.GetObjectByKey<VatCategory>(VatCategory)
                };
                //Φ-Κ Barcode
                string paddedBarcode = (appSetings.PadBarcodes) ? this.Code.PadLeft(appSetings.BarcodeLength, appSetings.BarcodePaddingCharacter[0]) : this.Code;
                Barcode fkBarcode = uow.FindObject<Barcode>(new BinaryOperator("Code", paddedBarcode));
                if (fkBarcode == null)
                {
                    fkBarcode = new Barcode(uow) { Code = paddedBarcode };
                }
                ItemBarcode ibc = new ItemBarcode(uow)
                {
                    Item = item,
                    Barcode = fkBarcode,
                    Owner = Owner,
                    MeasurementUnit = mm
                };
            }

            Barcodes.ForEach(importedBarcode =>
            {
                ItemBarcode ibc = item.ItemBarcodes.FirstOrDefault(x => x.Barcode.Code == importedBarcode.Code);
                if (ibc == null)
                {
                    Barcode barcode = uow.FindObject<Barcode>(new BinaryOperator("Code", importedBarcode.Code));
                    if (barcode == null)
                    {
                        barcode = new Barcode(uow) { Code = importedBarcode.Code };
                    }
                    ibc = new ItemBarcode(uow)
                    {
                        Item = item,
                        Barcode = barcode,
                        Owner = Owner,
                        MeasurementUnit = mm,
                    };
                }
            });

            return true;
        }

        [Display(AutoGenerateField = false)]
        public override string EntityName
        {
            get { return "Item"; }
        }


    }
}
