using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace ITS.Retail.Common.ViewModel
{
    public class BarcodeViewModel : BasePersistableViewModel
    {
        public override Type PersistedType { get { return typeof(Barcode); } }

        // Fields...
        private string _Code;
        
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }
    }
}
