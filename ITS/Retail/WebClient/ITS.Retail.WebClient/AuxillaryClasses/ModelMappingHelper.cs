using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Transactions;
using ITS.Retail.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using ITS.POS.Model.Settings;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public static class ModelMappingHelper
    {

        private const int cursorPageSize = 150;
        //private const int uowCommitSize = 25000;

        public static void TransferObjects(Type sourceType, Type destinationType, UnitOfWork sourceUow,CriteriaOperator criteria)
        {
            eUpdateDirection updateDirection = eUpdateDirection.POS_TO_STORECONTROLLER;

            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            IEnumerable<PropertyInfo> denormalisedProperties = sourceType.GetProperties().Where(property => property.GetCustomAttributes(typeof(DenormalizedFieldAttribute), false).Count() > 0).AsEnumerable<PropertyInfo>();


            ///TODO
            ///Πρέπει να διορθωθεί το συγκεκριμένο τμήμα για τα σημεία που αφορούν 
            ///τις πολλαπλές ιδιότητες

            List<string> excludeProperties = new List<string>()
            {
                "Session",
                "ClassInfo",
                "IsLoading"
            };

            Dictionary<PropertyInfo, PropertyInfo> propertyDictionary = sourceProperties.Where(
                        g => g.CanWrite == true && !excludeProperties.Contains(g.Name)
                    && ( g.GetCustomAttributes(typeof(ITS.Retail.Model.UpdaterIgnoreFieldAttribute), false).Count() == 0
                        || (g.GetCustomAttributes(typeof(ITS.Retail.Model.UpdaterIgnoreFieldAttribute), false).FirstOrDefault() as ITS.Retail.Model.UpdaterIgnoreFieldAttribute).IgnoreWhenDirection.HasFlag(updateDirection) == false
                        )
                    && g.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Count() == 0
                    && destinationProperties.Where(x => x.Name == g.Name).Count() > 0
            ).ToDictionary(g => g, g => destinationProperties.Where(x => x.Name == g.Name).First());

            IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> propertyDictionaryWhithoutDenormalisation = propertyDictionary.Where(p => denormalisedProperties.Contains(p.Key) == false);

            UnitOfWork destinationUow = XpoHelper.GetNewUnitOfWork();

            XPCursor cursor = new XPCursor(sourceUow, sourceType, criteria);
            cursor.PageSize = cursorPageSize;
            int counter = 0;
            foreach (object from in cursor)
            {
                object dest = null;

                if (denormalisedProperties.Count() > 0)
                {
                    #region Check Object Existance in destination
                    Guid objOid = (from as ITS.POS.Model.Settings.BaseObj).Oid;
                    dest = destinationUow.GetObjectByKey(destinationType,objOid);
                    if (dest == null)
                    {
                        dest = Activator.CreateInstance(destinationType, destinationUow);
                    }
                    
                    foreach (PropertyInfo sourceProperty in denormalisedProperties)
                    {
                        
                        object sourceOriginalValue = sourceProperty.GetValue(from, null);
                        if (sourceOriginalValue != null)
                        {
                            Guid? searchguid = Guid.Empty;
                            if (sourceProperty.PropertyType.Equals(typeof(Guid)))
                            {
                                searchguid = sourceOriginalValue as Guid?;
                            }
                            else if (sourceProperty.PropertyType.Equals(typeof(ITS.POS.Model.Settings.BaseObj)))
                            {
                                searchguid = (sourceOriginalValue as ITS.POS.Model.Settings.BaseObj).Oid;
                            }
                            else
                            {
                                throw new Exception();
                            }
                            PropertyInfo destinationProperty = propertyDictionary[sourceProperty];
                            DenormalizedFieldAttribute sourceDenormalizedFieldAttribute = sourceProperty.GetCustomAttributes(typeof(DenormalizedFieldAttribute), false).First() as DenormalizedFieldAttribute;
                            object relativeObject = destinationUow.GetObjectByKey(destinationProperty.PropertyType, searchguid);
                            if (relativeObject == null)
                            {
                                PropertyInfo lookUpProperty = from.GetType().GetProperty(sourceDenormalizedFieldAttribute.DenormalizedField);
                                object lookUpValue = lookUpProperty.GetValue(from, null);
                                CriteriaOperator findCriteria = new BinaryOperator(sourceDenormalizedFieldAttribute.RemotePropertyName, lookUpValue);
                                
                                relativeObject = destinationUow.FindObject(destinationProperty.PropertyType, findCriteria);
                                if (relativeObject == null)
                                {
                                    if (destinationProperty.PropertyType.Equals(typeof(Guid)))
                                    {
                                        destinationProperty.SetValue(dest, Guid.Empty, null);
                                    }
                                    else if (destinationProperty.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                                    {
                                        destinationProperty.SetValue(dest, null, null);
                                    }
                                }
                                else
                                {
                                    if (destinationProperty.PropertyType.Equals(typeof(Guid)))
                                    {
                                        destinationProperty.SetValue(dest, (relativeObject as ITS.Retail.Model.BaseObj ).Oid , null);
                                    }
                                    else if (destinationProperty.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                                    {
                                        destinationProperty.SetValue(dest, relativeObject, null);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                //else
                {
                    #region Simple Data Copy Paste
                    if (dest == null)
                    {
                        dest = Activator.CreateInstance(destinationType, destinationUow);
                    }

                    foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyDictionaryWhithoutDenormalisation)
                    {
                        try
                        {
                            object prop = pair.Key.GetValue(from, null);
                            if (prop == null)
                            {
                                pair.Value.SetValue(dest, null, null);
                            }
                            else if (pair.Key.PropertyType.IsSubclassOf(typeof(ITS.POS.Model.Settings.BaseObj)))
                            {
                                if (pair.Value.PropertyType == typeof(Guid))
                                {
                                    pair.Value.SetValue(dest, ((ITS.POS.Model.Settings.BaseObj)pair.Key.GetValue(from, null)).Oid, null);
                                }
                                else if (pair.Value.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                                {
                                    Guid objOid = ((ITS.POS.Model.Settings.BaseObj)pair.Key.GetValue(from, null)).Oid;
                                    pair.Value.SetValue(dest, destinationUow.GetObjectByKey(pair.Value.PropertyType, objOid), null);
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            else if (pair.Key.PropertyType == pair.Value.PropertyType)
                            {
                                pair.Value.SetValue(dest, pair.Key.GetValue(from, null), null);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        catch (Exception )
                        {
                        }
                    }
                    #endregion
                }
                counter++;
                //if (counter % uowCommitSize == 0)
                //{
                //    XpoHelper.CommitChanges(destinationUow);
                //    System.GC.Collect();
                //}
            }
            XpoHelper.CommitChanges(destinationUow);
            destinationUow.Dispose();
            System.GC.Collect();
        }   
    }
}