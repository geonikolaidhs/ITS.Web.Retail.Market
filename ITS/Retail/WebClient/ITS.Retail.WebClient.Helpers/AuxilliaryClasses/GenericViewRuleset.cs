using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{
    public class GenericViewRuleset
    {
        public List<string> PropertiesToIgnore { protected set; get; }       
        public List<string> DetailsToIgnore { get; set; }
        public List<string> DetailedPropertiesToShow { get; set; }
        public Dictionary<Type, List<string>> DetailPropertiesToIgnore { protected set; get; }
        public Dictionary<Type, List<string>> DetailedPropertiesPropToIgnore { protected set; get; }
        public Dictionary<Type, int> DetailedPropertiesNumOfCol { protected set; get; }
        public bool ShowDetails { get; set; }
        public int NumberOfColumns { get; set; }

        public static readonly List<string> AlwaysIgnoredProperies = new List<string>()
        {
                "Oid",
                "ChangedMembers" , 
                "CreatedBy",
                "CreatedByDevice",
                "CreatedOnTicks",
                "IsDeleted",
                "IsSynchronized",
                "MasterObjOid",
                "MLValues",
                "RowDeleted",
                "UpdateByDevice",
                "UpdatedBy",
                "UpdatedOnTicks",
                "TempObjExists",
                "CreatedOn",
                "UpdatedOn",
                "ReferenceCode",
                "ShouldResetMenu",
                "Owner",
                "ObjectSignature"
        };

        public GenericViewRuleset()
        {
            this.PropertiesToIgnore = new List<string>();
            this.DetailedPropertiesToShow = new List<string>();
            this.DetailedPropertiesPropToIgnore = new Dictionary<Type, List<string>>();
            this.DetailPropertiesToIgnore = new Dictionary<Type, List<string>>();
            this.DetailedPropertiesNumOfCol = new Dictionary<Type, int>();
            this.DetailsToIgnore= new List<string>();
            this.ShowDetails = true;
            this.NumberOfColumns = 4;
        }
    }
}