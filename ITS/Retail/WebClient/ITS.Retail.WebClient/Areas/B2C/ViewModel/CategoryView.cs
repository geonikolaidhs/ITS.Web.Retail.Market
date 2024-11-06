using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.WebClient.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.Areas.B2C.ViewModel
{
    public class CategoryView : IPersistableViewModel
    {
        public CategoryView()
        {
            items = new List<CategoryView>();
        }

        public string title { get; set; }

        public string name { get; set; }

        public string icon { get; set; }

        public string link { get; set; }

        public List<CategoryView> items { get; set; }

        [JsonIgnore]
        public bool HasChild { get; set; }

        [JsonIgnore]
        public string Description { get; set; }

        [JsonIgnore]
        public Guid Oid { get; set; }

        [JsonIgnore]
        public Type PersistedType { get { return typeof(ItemCategory); } }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public void UpdateModel(Session uow)
        {            
            link = "#" + Oid;
            if (HasChild)
            {
                CategoryView childView = (CategoryView)this.MemberwiseClone();
                childView.items = new List<CategoryView>();
                childView.title = Description;
                childView.icon = "fa fa-eye";
                childView.name = null;
                this.items.Add(childView);
            }
            else
            {
                icon = "fa fa-eye";
                items = null;
            }
            
            name = Description;
        }

        public bool Validate(out string message)
        {
            message = "";
            return true;
        }
    }
}