using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Model;
namespace ITS.Retail.Common.ViewModel
{
    public class StoreViewModel : BasePersistableViewModel
    {
        // Fields...
        private string _Name;

        public override Type PersistedType
        {
            get { return typeof(Store); }
        } 

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }
    }
}
