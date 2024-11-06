using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.WebClient.Helpers
{
    public static class SynchronizationInfoHelper
    {
        public static long GetEntityExpectedCountOfRemoteDevice(eApplicationInstance currentApplicationInstance, string entityName, long deviceMinVersion, Guid deviceID, eIdentifier deviceType, eSyncInfoEntityDirection syncInfoDirection)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string fullTypeName = typeof(Item).AssemblyQualifiedName.ToString().Replace("Item", entityName);
                Type entityType = Type.GetType(fullTypeName);
                CriteriaOperator criteria = null;
                if (syncInfoDirection == eSyncInfoEntityDirection.DOWN)
                {
                    eUpdateDirection direction = currentApplicationInstance == eApplicationInstance.RETAIL ? eUpdateDirection.MASTER_TO_STORECONTROLLER : eUpdateDirection.STORECONTROLLER_TO_POS;
                    criteria = GetUpdaterRestrictionsForDevice(currentApplicationInstance, entityType, deviceID.ToString(), deviceType, direction);
                    criteria = CriteriaOperator.And(criteria, new BinaryOperator("IsActive", true));
                }
                else
                {
                    CriteriaOperator minVersionCriteria = new BinaryOperator("UpdatedOnTicks", deviceMinVersion, BinaryOperatorType.GreaterOrEqual);
                    if (currentApplicationInstance == eApplicationInstance.RETAIL)
                    {
                        StoreControllerSettings sc = uow.GetObjectByKey<StoreControllerSettings>(deviceID);
                        XPCollection<Model.POS> scPoses = new XPCollection<Model.POS>(uow, new BinaryOperator("Store.Oid", sc.Store.Oid));
                        List<string> deviceIDs = new List<string>();
                        deviceIDs.Add(deviceID.ToString());
                        deviceIDs.AddRange(scPoses.Select(pos => pos.Oid.ToString()));
                        criteria = CriteriaOperator.And(
                            new InOperator("CreatedByDevice", deviceIDs),
                            CriteriaOperator.Parse("IsExactType(This,?)", entityType.FullName),
                            CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                            );
                    }
                    else
                    {
                        criteria = CriteriaOperator.And(minVersionCriteria,
                              new BinaryOperator("CreatedByDevice", deviceID.ToString()),
                              CriteriaOperator.Parse("IsExactType(This,?)", entityType.FullName),
                              CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                              );
                    }
                }


                long entityCount = (int)uow.Evaluate(entityType, CriteriaOperator.Parse("Count"), criteria);
                return entityCount;
            }
        }

        public static long GetEntityExpectedVersionOfRemoteDevice(eApplicationInstance currentApplicationInstance, string entityName, Guid deviceID, eIdentifier deviceType, eSyncInfoEntityDirection syncInfoDirection)
        {

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string fullTypeName = typeof(Item).AssemblyQualifiedName.ToString().Replace("Item", entityName);
                Type entityType = Type.GetType(fullTypeName);
                CriteriaOperator criteria = null;
                if (syncInfoDirection == eSyncInfoEntityDirection.DOWN)
                {
                    eUpdateDirection direction = currentApplicationInstance == eApplicationInstance.RETAIL ? eUpdateDirection.MASTER_TO_STORECONTROLLER : eUpdateDirection.STORECONTROLLER_TO_POS;
                    criteria = GetUpdaterRestrictionsForDevice(currentApplicationInstance, entityType, deviceID.ToString(), deviceType, direction);
                }
                else
                {
                    if (currentApplicationInstance == eApplicationInstance.RETAIL)
                    {
                        StoreControllerSettings sc = uow.GetObjectByKey<StoreControllerSettings>(deviceID);
                        XPCollection<Model.POS> scPoses = new XPCollection<Model.POS>(uow, new BinaryOperator("Store.Oid", sc.Store.Oid));
                        List<string> deviceIDs = new List<string>();
                        deviceIDs.Add(deviceID.ToString());
                        deviceIDs.AddRange(scPoses.Select(pos => pos.Oid.ToString()));
                        criteria = CriteriaOperator.And(new InOperator("CreatedByDevice", deviceIDs),
                            CriteriaOperator.Parse("IsExactType(This,?)", entityType.FullName),
                            CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                            );
                    }
                    else
                    {
                        criteria = CriteriaOperator.And(new BinaryOperator("CreatedByDevice", deviceID.ToString()),
                              CriteriaOperator.Parse("IsExactType(This,?)", entityType.FullName),
                              CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                              );
                    }
                }
                object value = uow.Evaluate(entityType, CriteriaOperator.Parse("Max(UpdatedOnTicks)"), criteria);
                return value == null ? 0 : (long)value;
            }
        }

        public static CriteriaOperator GetUpdaterRestrictionsForDevice(eApplicationInstance currentApplicationInstance, Type type, string deviceID, eIdentifier identifier, eUpdateDirection direction)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CompanyNew supplier = null;
                Store store = null;

                if (currentApplicationInstance == eApplicationInstance.RETAIL)
                {
                    StoreControllerSettings sc = uow.FindObject<StoreControllerSettings>(new BinaryOperator("Oid", Guid.Parse(deviceID)));
                    if (sc == null)
                    {
                        throw new Exception("StoreController is null. Oid:" + deviceID);  
                    }

                    store = sc.Store;
                    if (store == null)
                    {
                        throw new Exception("Store is null. Oid:" + StoreControllerAppiSettings.CurrentStoreOid); 
                    }

                    supplier = sc.Owner;
                    if (supplier == null)
                    {
                        throw new Exception("Supplier of Store '" + store.Code + "' is null.");  
                }
                }
                else
                {
                    store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                    if (store == null)
                    {
                        throw new Exception("Store is null. Oid:" + StoreControllerAppiSettings.CurrentStoreOid);  
                    }

                    supplier = store.Owner;
                    if (supplier == null)
                    {
                        throw new Exception("Supplier of Store '" + store.Code + "' is null.");  
                    }
                }

                CriteriaOperator objectFilters = type.GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { direction, supplier, store, deviceID }) as CriteriaOperator;

                if (direction != eUpdateDirection.STORECONTROLLER_TO_POS && typeof(IOwner).IsAssignableFrom(type))
                {
                    if (objectFilters == null)
                    {
                        throw new Exception("Owner Criteria has not been applied in object type " + type.ToString());
                    }
                    if (objectFilters.LegacyToString().Contains("Owner") == false)
                    {
                        throw new Exception("Owner Criteria has not been applied in object type " + type.ToString());
                    }
                }
                return objectFilters;
            }
        }
    }
}
