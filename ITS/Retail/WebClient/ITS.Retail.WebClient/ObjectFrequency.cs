using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient
{
    public class ObjectFrequency<T> where T:BasicObj
    {
        public int Frequency { get; set; }
        public T Item { get; set; }
        public decimal Qty { get; set; }
        public string Quantity {
            get
            {
                return ITS.Retail.WebClient.Helpers.BusinessLogic.RoundAndStringify(Qty, OwnerObject);
            }
        }
        public decimal Sum { get; set; }
        public string Summary
        {
            get
            {
                return ITS.Retail.WebClient.Helpers.BusinessLogic.RoundAndStringify(Sum, OwnerObject);
            }
        }
        Guid _owner;
        public Guid Owner 
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
            }
        }
        public CompanyNew OwnerObject
        {
            get
            {
                return Item.Session.GetObjectByKey<CompanyNew>(Owner);
            }
        }

        public ObjectFrequency(T item, Guid owner)
        {
            Item = item;
            Frequency = 0;
            this.Owner = owner;
        }

        public ObjectFrequency()
        {
            
            Frequency = 0;
        }

    }

    public class ItemStatistics
    {
        public List<ObjectFrequency<Item>> Items { get; set; }
        public List<ObjectFrequency<CategoryNode>> ItemCategories { get; set; }
    }

    public static class FrequencyHelpers<T> where T:BasicObj
    {
        public static IEnumerable<ObjectFrequency<T>> NormalizeFrequencies(IEnumerable<ObjectFrequency<T>> inputCollection, int minValue, int maxValue, CompanyNew owner)
        {
            if (inputCollection.Count() == 0)
            {
                return inputCollection;
            }
            int realMinValue = inputCollection.Min(g => g.Frequency);
            int realMaxValue = inputCollection.Max(g => g.Frequency);
            
            return inputCollection.Select(g => new ObjectFrequency<T>(g.Item, owner.Oid) { Frequency = minValue + (g.Frequency - realMinValue) * (maxValue - minValue) / (realMaxValue - realMinValue) });
        }

        
    }


}