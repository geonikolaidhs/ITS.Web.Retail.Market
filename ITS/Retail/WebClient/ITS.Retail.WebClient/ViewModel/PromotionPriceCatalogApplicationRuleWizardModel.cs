using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{

    public class PromotionPriceCatalogApplicationRuleWizardModel: PromotionApplicationRuleWizardModel
    {
        public override string Description
        {
            get
            {
                if(this.ActivePriceCatalogObjects == null
                || this.ActivePriceCatalogObjects.Count == 0
                  )
                {
                    return string.Empty;
                }

                string description = Resources.PriceCatalogIn;
                description += this.ActivePriceCatalogObjects.Select(priceCatalog => priceCatalog.Description).Aggregate((first, second) => String.Concat(first, ",", second));
                return description;
            }
        }

        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "PriceCatalog")]
        public override Type PersistedType { get { return typeof(PromotionPriceCatalogApplicationRule); } }


        public PromotionPriceCatalogApplicationRuleWizardModel()
        {
            ActivePriceCatalogs = new List<Guid>();
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            ActivePriceCatalogObjects = uow.Query<PriceCatalog>().Where(x => this.ActivePriceCatalogs.Contains(x.Oid)).ToList();
        }

        [Binding("PriceCatalogsString_selected")]
        public string PriceCatalogs
        {
            get
            {
                if (this.ActivePriceCatalogs == null || this.ActivePriceCatalogs.Count <= 0)
                {
                    return string.Empty;
                }
                if( this.ActivePriceCatalogs.Count == 1 )
                {
                    return this.ActivePriceCatalogs.First().ToString();
                }
                return this.ActivePriceCatalogs.Select(priceCatalog => priceCatalog.ToString()).Aggregate((first, second) => String.Concat(first, ",", second));
            }
            set
            {
                try
                {
                    string oids = value;
                    this.ActivePriceCatalogs = oids.Split(',').ToList().Select(oid => Guid.Parse(oid)).ToList();
                }
                catch(Exception exception)
                {
                    string errorMessage = exception.GetFullMessage();
                    this.ActivePriceCatalogs = new List<Guid>();
                }
            }
        }


        public List<Guid> ActivePriceCatalogs
        {
            get; set;
        }


        public List<PriceCatalog> ActivePriceCatalogObjects
        {
            get; set;
        }


    }
}