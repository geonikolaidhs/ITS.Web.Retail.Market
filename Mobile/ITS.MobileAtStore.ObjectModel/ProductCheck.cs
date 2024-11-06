using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.MobileAtStore.ObjectModel
{
    [Persistent("ITS_DATALOGGER_PRODUCT_CHECK")]
    public class ProductCheck : BaseXPObject
    {
        public ProductCheck()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ProductCheck(Session session)
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

        private string _code = string.Empty;
        //[Size(20)]
        //[DbType("VARCHAR2(20)")]
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                SetPropertyValue("Code", ref _code, value);
            }
        }

        private string _compCode = string.Empty;
        //[Size(20)]
        //[DbType("VARCHAR2(4)")]
        public string CompCode
        {
            get
            {
                return _compCode;
            }
            set
            {
                SetPropertyValue("CompCode", ref _compCode, value);
            }
        }

        public static ProductCheck Find(UnitOfWork uow, string code, string compcode)
        {
            if(!string.IsNullOrEmpty(compcode))
                return uow.FindObject<ProductCheck>(CriteriaOperator.Parse("Code = ? AND CompCode = ?", code, compcode));
            else
                return uow.FindObject<ProductCheck>(CriteriaOperator.Parse("Code = ?", code));
        }

        public static ProductCheck Find(UnitOfWork uow, string code)
        {
            return ProductCheck.Find(uow, code, null);
        }

        public static ProductCheck Add(UnitOfWork uow, string code, string compcode)
        {            
            ProductCheck pc = ProductCheck.Find(uow, code, compcode);
            if (pc == null)
                pc = new ProductCheck(uow) { Code = code, CompCode = compcode };
            return pc;
        }
    }
}
