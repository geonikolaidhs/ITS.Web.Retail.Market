using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model;
using System.Reflection;
using System.Collections;

namespace ITS.Retail.Common.ViewModel
{
    public class PersistableViewModelAttribute : Attribute
    {
        public string PersistantObjectPropertyName { get; set; }
        public bool NotPersistant { get; set; }
    }
}
