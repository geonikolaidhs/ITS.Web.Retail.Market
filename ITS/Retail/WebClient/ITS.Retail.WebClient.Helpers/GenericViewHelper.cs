using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.Attributes;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class GenericViewHelper
    {
        public static GenericViewModelMaster CreateGenericViewModel(string entityOid, Type entityType, GenericViewRuleset ruleset)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            GenericViewModelMaster viewModel = new GenericViewModelMaster();
            viewModel.Ruleset = ruleset;

            BasicObj obj = null;
            if (entityOid != null)
            {
                Guid entityGuid = Guid.Parse(entityOid);
                obj = uow.GetObjectByKey(entityType, entityGuid) as BasicObj;
            }

            viewModel.Object = obj;
            viewModel.PropertiesToShow.AddRange(GetFilteredProperties(entityType, ruleset.PropertiesToIgnore.Concat(ruleset.DetailedPropertiesToShow), false, null));

            foreach (string detailedProperty in viewModel.Ruleset.DetailedPropertiesToShow)
            {
                BasicObj detailObj = entityType.GetProperty(detailedProperty).GetValue(obj, null) as BasicObj;
                if(detailObj != null)
                {
                    Type detailedPropType = detailObj.GetType();
                    List<string> detailedPropertiesPropToIgnore = new List<string>();

                    if (ruleset.DetailedPropertiesPropToIgnore.ContainsKey(detailedPropType))
                    {
                        detailedPropertiesPropToIgnore = ruleset.DetailedPropertiesPropToIgnore[detailedPropType];
                    }

                    viewModel.DetailedProperties.Add(new GenericViewModelDetailedProperty()
                    {
                        PropertyName = detailedProperty,
                        Object = detailObj,
                        Master = viewModel,
                        PropertiesToShow = GetFilteredProperties(detailedPropType, detailedPropertiesPropToIgnore, false, entityType).ToList(),
                        Ruleset = ruleset
                    });
                }
            }

            if (ruleset.ShowDetails)
            {
                IEnumerable<string> detailsToShow = GetFilteredDetails(entityType, ruleset.DetailsToIgnore);
                foreach (string detailName in detailsToShow)
                {
                    Type detailType = entityType.GetProperty(detailName).PropertyType.GetGenericArguments()[0];
                    List<string> detailPropertiesToIgnore = new List<string>();

                    if (ruleset.DetailPropertiesToIgnore.ContainsKey(detailType))
                    {
                        detailPropertiesToIgnore = ruleset.DetailPropertiesToIgnore[detailType];
                    }

                    viewModel.Details.Add(new GenericViewModelDetail()
                    {
                        DetailType = detailType,
                        PropertyName = detailName,
                        Master = viewModel,
                        PropertiesToShow = GetFilteredProperties(detailType, detailPropertiesToIgnore, true, entityType).ToList()
                    });
                }
            }
            return viewModel;
        }

        private static IEnumerable<string> GetFilteredProperties(Type entityType, IEnumerable<string> propertiesToIgnore, bool isDetail, Type parentType)
        {
            List<PropertyInfo> allProperties = entityType.GetProperties().Where(g => typeof(BasicObj).IsAssignableFrom(g.DeclaringType)).ToList();
            IEnumerable<PropertyInfo> filteredProperties = allProperties.Where(x => propertiesToIgnore.Contains(x.Name) == false
                                                        && GenericViewRuleset.AlwaysIgnoredProperies.Contains(x.Name) == false
                                                        && x.PropertyType.IsSubclassOf(typeof(XPBaseCollection)) == false
                                                        && typeof(IList).IsAssignableFrom(x.PropertyType) == false)
                                                        .Select(x => new
                                                        {
                                                            Property = x,
                                                            Order = (DisplayOrderAttribute)x.GetCustomAttributes(typeof(DisplayOrderAttribute), true).FirstOrDefault()
                                                        })
                                                        .Select(x => new { Property = x.Property, OrderNumber = x.Order == null ? Int32.MaxValue : x.Order.Order })
                                                        .OrderBy(x => x.OrderNumber).ThenBy(x => x.Property.Name)
                                                        .Select(x => x.Property);
            List<string> result = new List<string>();
            if (isDetail)
            {
                string parentProperyName = null;
                if (parentType != null)
                {
                    PropertyInfo parentProperty = entityType.GetProperties().Where(x => x.PropertyType == parentType).FirstOrDefault();
                    if (parentProperty != null)
                    {
                        parentProperyName = parentProperty.Name;
                    }
                }

                foreach (PropertyInfo property in filteredProperties)
                {
                    if (property.Name == parentProperyName)
                        continue;
                    if (property.PropertyType.IsSubclassOf(typeof(BaseObj)))
                    {
                        PropertyInfo descriptionProperty = property.PropertyType.GetProperties().
                            FirstOrDefault(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), false).Count() > 0);

                        if (descriptionProperty == null)
                        {
                            descriptionProperty = property.PropertyType.GetProperties().
                                FirstOrDefault(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), true).Count() > 0);
                        }

                        if (descriptionProperty != null)
                        {
                            result.Add(property.Name + "." + descriptionProperty.Name);
                        }
                    }
                    else
                    {
                        result.Add(property.Name);
                    }
                }
            }
            else
            {
                result = filteredProperties.Select(x => x.Name).ToList();
            }
            return result;
        }

        private static IEnumerable<string> GetFilteredDetails(Type entityType, IEnumerable<string> detailsToIgnore)
        {
            IEnumerable<PropertyInfo> properties = entityType.GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(XPBaseCollection)) &&
                detailsToIgnore.Contains(x.Name) == false);
            IEnumerable<string> filteredDetails = properties
                .Select(x => new
                {
                    Property = x,
                    Order = (DisplayOrderAttribute)x.GetCustomAttributes(typeof(DisplayOrderAttribute), true).FirstOrDefault()
                })
                .Select(x => new { Property = x.Property, OrderNumber = x.Order == null ? Int32.MaxValue : x.Order.Order })
                .OrderBy(x => x.OrderNumber).ThenBy(x => x.Property.Name)
                .Select(x => x.Property.Name);
            return filteredDetails;
        }

    }
}
