using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient;
using ITS.Retail.WebClient.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ITS.Retail.WebClient.Helpers.WebServicesHelpers
{
    public class SyncCriteriaHelper
    {
        public SyncCriteriaHelper() { }

        /// <summary>
        /// Επιστρέφει το σύνολο τω κριτηρίων με τα αποία επιλέγονταi τα οι εγγραφές για Upload 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <param name="createdBydevice"></param>
        /// <param name="latestVersion"></param>
        /// <returns>CriteriaOperator</returns>
        public CriteriaOperator GetPostCriteria<T>(eIdentifier device, string createdBydevice, long latestVersion)
        {

            CriteriaOperator deviceCrop;
            CriteriaOperator versionCrop;
            CriteriaOperator restriciontsCrop = typeof(T).GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                    .Invoke(null, new object[] { eUpdateDirection.STORECONTROLLER_TO_MASTER,  StoreControllerAppiSettings.CurrentStore.Owner,
                                                    StoreControllerAppiSettings.CurrentStore, "" }) as CriteriaOperator;

            CriteriaOperator typeCrop = CriteriaOperator.And(CriteriaOperator.Parse("IsExactType(This,?)", typeof(T).FullName));
            CriteriaOperator gcRecordCrop = CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull());
            versionCrop = CriteriaOperator.And(new BinaryOperator("UpdatedOnTicks", latestVersion, BinaryOperatorType.Greater));

            deviceCrop = device == eIdentifier.POS ? CriteriaOperator.And(new BinaryOperator("CreatedByDevice", createdBydevice)) :
                                                     CriteriaOperator.Or(new BinaryOperator("CreatedByDevice", createdBydevice),
                                                                         new BinaryOperator("CreatedByDevice", ""), new NullOperator("CreatedByDevice"));

            return CriteriaOperator.And(deviceCrop, versionCrop, typeCrop, gcRecordCrop);
        }

        /// <summary>
        /// Επιστρέφει το σύνολο τω κριτηρίων με τα αποία επιλέγει ο server τις εγγραφές για Download
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceID"></param>
        /// <param name="version"></param>
        /// <param name="op"></param>
        /// <param name="identifier"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public CriteriaOperator GetResponseCriteria<T>(DateTime version, BinaryOperatorType op, eIdentifier identifier, eUpdateDirection direction)
        {
            CriteriaOperator posManualUpdateCriteria = null;
            CriteriaOperator versionCriteria = GetVersionCriteria<T>(version, op);
            //if (direction == eUpdateDirection.STORECONTROLLER_TO_POS)
            //{
            //    posManualUpdateCriteria = GetPOSUpdaterManualVersionCriteria<T>(deviceID);
            //    versionCriteria = !ReferenceEquals(posManualUpdateCriteria, null) ? CriteriaOperator.And(versionCriteria, posManualUpdateCriteria) : versionCriteria;

            //}
            CriteriaOperator restrictionCriteria = GetRestrictionsCriteria<T>();
            CriteriaOperator typeCriteria = GetTypeCriteria<T>();

            CriteriaOperator crop = CriteriaOperator.And(restrictionCriteria, versionCriteria, typeCriteria);
            return crop;
        }


        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα αποία επιλέγονταi τα οι εγγραφές για από τον ResendObjectRequest για επαναποστολή 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>CriteriaOperator</returns>
        public CriteriaOperator GetResendObjectPostCriteria<T>()
        {

            return new JoinOperand("ResendObjectRequest", CriteriaOperator.And(new OperandProperty("^.Oid") == new OperandProperty("EntityOid"),
                                                                                          new BinaryOperator("EntityName", typeof(T).FullName),
                                                                                          new BinaryOperator("TargetDeviceOid", Guid.Empty)));

        }

        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα όποία μετά την εισαγωγή πελάτη από συγχρονισμό γίνονται update σε συγκεκριμένα παραστατικά
        /// </summary>
        /// <param name="customerOid"></param>
        /// <param name="traderTaxcode"></param>
        /// <returns>CriteriaOperator</returns>
        public CriteriaOperator GetCustomerCriteria(Guid customerOid, string traderTaxcode)
        {
            return CriteriaOperator.And(new BinaryOperator("Customer", customerOid, BinaryOperatorType.NotEqual),
                                                                       new BinaryOperator("CustomerLookUpTaxCode", traderTaxcode),
                                                                       new NotOperator(new NullOperator("DenormalizedCustomer")));
        }


        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα όποία το συγκεκριμένο device για συγκεκριμενο direction και οντότητα 
        /// επιτρέπεται ή οχι να συγχρονίσει.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceID"></param>
        /// <param name="identifier"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        //public CriteriaOperator GetRestrictionsCriteria<T>(string deviceID, eIdentifier identifier, eUpdateDirection direction, eApplicationInstance applicationInstance)
        //{
        //    CriteriaOperator crit = CriteriaOperator.And(SynchronizationInfoHelper.GetUpdaterRestrictionsForDevice(applicationInstance, typeof(T), deviceID, identifier, direction));

        //    return CriteriaOperator.Or(CriteriaOperator.And(crit, new OperandProperty("GCRecord").IsNull()),
        //                                                new OperandProperty("GCRecord").IsNotNull());
        //}




        public CriteriaOperator GetRestrictionsCriteria<T>()
        {
            return CriteriaOperator.Or(new OperandProperty("GCRecord").IsNull(), new OperandProperty("GCRecord").IsNotNull());
        }

        /// <summary>
        /// Κρητήρια type class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private CriteriaOperator GetTypeCriteria<T>()
        {
            return CriteriaOperator.Parse("IsExactType(This,?)", typeof(T).FullName);
        }


        /// <typeparam name="T"></typeparam>
        /// <param name="version"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public CriteriaOperator GetVersionCriteria<T>(DateTime version, BinaryOperatorType op)
        {
            return new BinaryOperator("UpdatedOnTicks", version.Ticks, op);
        }

        /// <summary>
        /// Επιστρέφει τα κριτήρια για το αν το POS για συγκεκριμενη οντότητα είναι σε manual mode,αν πρεπει να κατεβασει αυτόματα ή οχι
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        private CriteriaOperator GetPOSUpdaterManualVersionCriteria<T>(string deviceID)
        {
            long maxVersion = 0;
            CriteriaOperator posCriteria = null;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string typeName = typeof(T).Name.Replace(typeof(T).Namespace + ".", "");
                UpdaterMode manualUpdates = uow.FindObject<UpdaterMode>(CriteriaOperator.And(new BinaryOperator("Mode", eUpdaterMode.MANUAL),
                                                                                             new BinaryOperator("EntityName", typeName)));
                if (manualUpdates != null)
                {
                    Guid deviceGuid;
                    if (Guid.TryParse(deviceID, out deviceGuid))
                    {
                        POSUpdaterManualVersion maxVersionObject = uow.FindObject<POSUpdaterManualVersion>(
                                        CriteriaOperator.And(
                                            new BinaryOperator("EntityName", typeName),
                                            new BinaryOperator("POS.Oid", deviceGuid)
                                        ));
                        maxVersion = maxVersionObject == null ? 0 : maxVersionObject.MaxUpdatedOnTicks;
                        posCriteria = new BinaryOperator("UpdatedOnTicks", maxVersion, BinaryOperatorType.LessOrEqual);
                    }
                }
            }
            return posCriteria;
        }

        public CriteriaOperator GetAfterUploadCriteria(string typeName)
        {
            return CriteriaOperator.And(new BinaryOperator("EntityName", typeName));
        }

        /// <summary>
        /// Επιστρέφει τα κριτήρια για το αν το κουπόνι είναι έγκυρο
        /// </summary>
        /// <param name="code"></param>
        /// <param name="nowTicks"></param>
        /// <returns></returns>
        public CriteriaOperator GetCouponCriteria(string code, long nowTicks)
        {
            return CriteriaOperator.And(new BinaryOperator("Code", code),
                                          new BinaryOperator("IsActiveFrom", nowTicks, BinaryOperatorType.LessOrEqual),
                                          new BinaryOperator("IsActiveUntil", nowTicks, BinaryOperatorType.GreaterOrEqual));
        }

        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα όποία τα οποία ψάχνουμε το Max UpdatedOnTicks ενός πίνακα φιλτράροντας από ένα version μεγαλύτερο
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CriteriaOperator GetMaxTableVersionCriteria<T>(long ticks)
        {
            CriteriaOperator gcRecordCrop = CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull());
            return CriteriaOperator.And(GetTypeCriteria<T>(), GetVersionCriteria<T>(new DateTime(ticks), BinaryOperatorType.GreaterOrEqual), gcRecordCrop);
        }

        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα όποία τα οποία ψάχνουμε το Max UpdatedOnTicks ενός πίνακα 
        /// για ένα συγκεκριμενο device φιλτράροντας από ένα version μεγαλύτερο
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CriteriaOperator GetMaxTableVersionByDeviceCriteria<T>(long ticks, string createdByDevice, eIdentifier deviceIdentifier)
        {
            CriteriaOperator deviceCrop = deviceIdentifier == eIdentifier.POS ? CriteriaOperator.And(new BinaryOperator("CreatedByDevice", createdByDevice)) :
                                                        CriteriaOperator.Or(new BinaryOperator("CreatedByDevice", createdByDevice),
                                                                            new BinaryOperator("CreatedByDevice", ""), new NullOperator("CreatedByDevice"));

            CriteriaOperator gcRecordCrop = CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull());
            return CriteriaOperator.And(GetTypeCriteria<T>(), GetVersionCriteria<T>(new DateTime(ticks), BinaryOperatorType.GreaterOrEqual), gcRecordCrop);
        }

        public static CriteriaOperator GetSynchronizationInfoSearchCriteria(JObject jsonObj)
        {
            eIdentifier DeviceType;
            eSyncInfoEntityDirection SyncInfoEntityDirection;
            Guid DeviceOid;
            Enum.TryParse(jsonObj.Property("DeviceType").Value.ToString(), out DeviceType);
            Enum.TryParse(jsonObj.Property("SyncInfoEntityDirection").Value.ToString(), out SyncInfoEntityDirection);
            Guid.TryParse(jsonObj.Property("DeviceOid").Value.ToString(), out DeviceOid);

            return CriteriaOperator.And(new BinaryOperator("DeviceOid", DeviceOid, BinaryOperatorType.Equal), new BinaryOperator("DeviceType", DeviceType, BinaryOperatorType.Equal),
                                                                                    new BinaryOperator("SyncInfoEntityDirection", SyncInfoEntityDirection, BinaryOperatorType.Equal),
                                                                                    new BinaryOperator("EntityName", jsonObj.Property("EntityName").Value.ToString(), BinaryOperatorType.Equal),
                                                                                    new OperandProperty("GCRecord").IsNull());
        }


        /// <summary>
        /// Επιστρέφει τα κριτήρια με τα αποία θα επιλεχτουν οι εγγραφές που θα στείλουμε για έλεγχο στο Master
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>CriteriaOperator</returns>
        public CriteriaOperator GetResendCriteria<T>(long ticksToLookBack)
        {

            CriteriaOperator classCriteria =
                typeof(T).GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[]
                                            { eUpdateDirection.STORECONTROLLER_TO_MASTER, StoreControllerAppiSettings.Owner, StoreControllerAppiSettings.CurrentStore, "" }) as CriteriaOperator;



            return CriteriaOperator.Or(new JoinOperand("ResendObjectRequest", CriteriaOperator.And(new OperandProperty("EntityOid") == new OperandProperty("^.Oid"),
                                                                              new BinaryOperator("EntityName", typeof(T).FullName))), CriteriaOperator.And(classCriteria,
                                                                              new BinaryOperator("UpdatedOnTicks", ticksToLookBack, BinaryOperatorType.GreaterOrEqual)));
        }

        public CriteriaOperator GetTypeDirectionCriteria(eUpdateDirection direction, Type classType)
        {
            return CriteriaOperator.And(new BinaryOperator("EntityName", classType.FullName), new BinaryOperator("UpdateDirection", direction));
        }


    }
}