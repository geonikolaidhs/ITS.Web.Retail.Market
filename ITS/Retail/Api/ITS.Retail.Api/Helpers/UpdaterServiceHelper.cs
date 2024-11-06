using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.ViewModel;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ITS.Retail.Api.Helpers
{
    public class UpdaterServiceHelper
    {
        public static BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        public UpdaterServiceHelper() { }
        private SyncCriteriaHelper SyncCriteriaHelper = new SyncCriteriaHelper();

        /// <summary>
        /// Αν είναι απαραίτητο επιστρέφει τις επιπλέον εγγραφές για download που έχουν ίδιο UpdatedOnTicks με τη version που μας έστειλε ο client
        /// αλλά δεν επιλέχθηκαν λόγω του Download Packet Limit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="version"></param>
        /// <param name="sameVersionCriteria"></param>
        /// <param name="direction"></param>
        /// <param name="alreadyInList"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        private List<string> HandleSameVersion<T>(DateTime maxVrsion, eIdentifier identifier, eUpdateDirection direction, List<Guid> alreadyInList) where T : BasicObj
        {
            List<string> list = new List<string>();
            try
            {
                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    CriteriaOperator sameVersionCriteria = SyncCriteriaHelper.GetResponseCriteria<T>(maxVrsion, BinaryOperatorType.Equal, identifier, direction);
                    XPCursor extraItems = new XPCursor(uow, typeof(T), sameVersionCriteria, new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                    foreach (T itm in extraItems)
                    {
                        if (!alreadyInList.Contains(itm.Oid))
                            list.Add(itm.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, direction));
                    }
                }
            }
            catch (Exception ex)
            {
                // MvcApplication.WRMLogModule.Log(ex.GetFullMessage() + Environment.NewLine + "StackTrace:" + ex.GetFullStackTrace() + "POSUpdateService - HandleSameVersion", KernelLogLevel.Error);
            }
            return list;
        }

        /// <summary>
        /// Επιλέγει τα data που θα στείλει ο server ως response 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="version"></param>
        /// <param name="deviceID"></param>
        /// <param name="identifier"></param>
        /// <param name="direction"></param>
        /// <param name="packetSize"></param>
        /// <param name="totalChanges"></param>
        /// <param name="serviceResult"></param>
        /// <returns></returns>
        public List<object> GetDBObjects<T>(DateTime version, int packetSize, eIdentifier identifier, eUpdateDirection direction) where T : BasicObj
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            string result = string.Empty;
            List<string> list = new List<string>();
            List<object> returnList = new List<object>();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                int totalChanges = 0;
                long maxVersion = -1;

                CriteriaOperator crop = SyncCriteriaHelper.GetResponseCriteria<T>(version, BinaryOperatorType.Greater, identifier, direction);
                List<Guid> alreadyAddedToList = new List<Guid>();

                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    XPCursor objects = new XPCursor(uow, typeof(T), crop, new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending))
                    {
                        SelectDeleted = true,
                        TopReturnedObjects = packetSize
                    };
                    foreach (T obj in objects)
                    {
                        maxVersion = obj.UpdatedOnTicks > maxVersion ? obj.UpdatedOnTicks : maxVersion;
                        alreadyAddedToList.Add(obj.Oid);
                        string s = obj.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, direction);
                        JObject jobj = JObject.Parse(s);
                        list.Add(obj.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, direction));
                        returnList.Add(obj);
                    }

                    list.AddRange(maxVersion != -1 ? HandleSameVersion<T>(new DateTime(maxVersion), identifier, direction, alreadyAddedToList) : new List<string>());
                    totalChanges = list.Count;

                    if (totalChanges == 1 && maxVersion == version.Ticks)
                    {
                        totalChanges = 0;
                    }

                    result = totalChanges > 0 ? JsonConvert.SerializeObject(list, PlatformConstants.JSON_SERIALIZER_SETTINGS) : result;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                }

            }
            catch (Exception e)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            }
            return returnList;
        }



        //private long GetMaxTableVersion<T>(UnitOfWork uow, long version) where T : BasicObj
        //{
        //    XPCollection<T> max = new XPCollection<T>(uow, SyncCriteriaHelper.GetMaxTableVersionCriteria<T>(version), new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending))
        //    {
        //        TopReturnedObjects = 1
        //    };
        //    if (max.Count == 0)
        //    {
        //        max = new XPCollection<T>(uow, SyncCriteriaHelper.GetMaxTableVersionCriteria<T>(0), new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending))
        //        {
        //            TopReturnedObjects = 1
        //        };
        //    }
        //    return max.Count > 0 ? max[0].UpdatedOnTicks : 0;
        //}

        public long GetMaxTableVersionByDevice<T>(UnitOfWork uow, long version, string createdBydevice, eIdentifier deviceIdentifier) where T : BasicObj
        {
            XPCollection<T> max = new XPCollection<T>(uow, SyncCriteriaHelper.GetMaxTableVersionByDeviceCriteria<T>(version, createdBydevice, deviceIdentifier),
                                                                              new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending))
            {
                TopReturnedObjects = 1
            };
            if (max.Count == 0)
            {
                max = new XPCollection<T>(uow, SyncCriteriaHelper.GetMaxTableVersionCriteria<T>(0), new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending))
                {
                    TopReturnedObjects = 1
                };
            }
            return max.Count > 0 ? max[0].UpdatedOnTicks : 0;
        }

        public long GetMaxTableVersion<T>() where T : BasicObj
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    XPCollection<T> max = new XPCollection<T>(uow, SyncCriteriaHelper.GetMaxTableVersionCriteria<T>(0), new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending))
                    {
                        TopReturnedObjects = 1
                    };
                    return max.Count > 0 ? max[0].UpdatedOnTicks : 0;
                }
            }
            catch (Exception ex)
            { }
            return 0;
        }







        /// <summary>
        /// Searches for a usable coupon with the given code.
        /// </summary>
        /// <param name="code">The code to search with</param>
        /// <param name="storeGuid">The current store in order to filter with its owner</param>
        /// <returns>Null if no coupon found or found but is not possible to use it, otherwise it returns the coupon that has been found as a view model.</returns>
        public CouponViewModel CreateCouponViewModel(Coupon coupon)
        {
            CouponViewModel couponViewModel = new CouponViewModel()
            {
                Oid = coupon.Oid,
                Code = coupon.Code,
                Owner = coupon.Owner.Oid,
                Description = coupon.Description,
                IsActiveUntilDate = coupon.IsActiveUntilDate,
                IsActiveFromDate = coupon.IsActiveFromDate,
                NumberOfTimesUsed = coupon.NumberOfTimesUsed,
                IsUnique = coupon.IsUnique,
                CouponAppliesOn = coupon.CouponAppliesOn,
                CouponAmountType = coupon.CouponAmountType,
                CouponAmountIsAppliedAs = coupon.CouponAmountIsAppliedAs,
                Amount = coupon.Amount,
                DiscountType = coupon.DiscountType == null ? Guid.Empty : coupon.DiscountType.Oid,
                PaymentMethod = coupon.PaymentMethod == null ? Guid.Empty : coupon.PaymentMethod.Oid,
                CouponCategory = coupon.CouponCategory == null ? Guid.Empty : coupon.CouponCategory.Oid,
                CouponCategoryDescription = coupon.CouponCategory == null ? string.Empty : coupon.CouponCategory.Description,
                CouponMaskOid = coupon.CouponMask != null ? coupon.CouponMask.Oid : Guid.Empty
            };
            return couponViewModel;
        }


        /// <summary>
        /// Internal της public GetExistingUsableCouponByMask ,δημιουργεί το  CouponViewModel από Coupon η GeneratedCoupon
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        private CouponViewModel CreateCouponViewModel(GeneratedCoupon generatedCoupon)
        {
            CouponViewModel couponViewModel = new CouponViewModel()
            {
                Oid = Guid.Empty,
                Code = generatedCoupon.Code,
                Owner = generatedCoupon.Owner.Oid,
                Description = generatedCoupon.Description,
                IsActiveUntilDate = generatedCoupon.CouponMask.IsActiveUntilDate,
                IsActiveFromDate = generatedCoupon.CouponMask.IsActiveFromDate,
                NumberOfTimesUsed = generatedCoupon.Status == GeneratedCouponStatus.Used ? 1 : 0,
                IsUnique = generatedCoupon.CouponMask.IsUnique,
                CouponAppliesOn = generatedCoupon.CouponMask.CouponAppliesOn,
                CouponAmountType = generatedCoupon.CouponMask.CouponAmountType,
                CouponAmountIsAppliedAs = generatedCoupon.CouponMask.CouponAmountIsAppliedAs,
                //Amount = CouponHelper.GetMaskAmount(generatedCoupon.Code, generatedCoupon.CouponMask),
                DiscountType = generatedCoupon.CouponMask.DiscountType == null ? Guid.Empty : generatedCoupon.CouponMask.DiscountType.Oid,
                PaymentMethod = generatedCoupon.CouponMask.PaymentMethod == null ? Guid.Empty : generatedCoupon.CouponMask.PaymentMethod.Oid,
                CouponCategory = generatedCoupon.CouponMask.CouponCategory == null ? Guid.Empty : generatedCoupon.CouponMask.CouponCategory.Oid,
                CouponCategoryDescription = generatedCoupon.CouponMask.CouponCategory == null ? string.Empty : generatedCoupon.CouponMask.CouponCategory.Description,
                CouponMaskOid = generatedCoupon.CouponMask != null ? generatedCoupon.CouponMask.Oid : Guid.Empty
            };
            return couponViewModel;
        }

        /// <summary>
        /// Δημιουργεί το Xml που κατεβάζει το POS για τα report settings(εκτύπωση παραστατικού σε windows printer)
        /// </summary>
        /// <param name="posid"></param>
        /// <returns></returns>
        public XmlDocument CreateReportSettingsXmlFile(int posid)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                if (pos == null)
                {
                    XmlNode errorNode = xml.CreateElement("Error");
                    errorNode.InnerText = "Pos Id Not Found";
                    xml.AppendChild(errorNode);
                    return xml;
                }
                XmlNode settingsNode = xml.CreateElement("Settings");
                pos.POSReportSettings.ToList().ForEach(reportSettings =>
                {
                    XmlNode reportSettingsNode = xml.CreateElement("ReportSetting");
                    XmlNode documentTypeNode = xml.CreateElement("DocumentType");
                    documentTypeNode.InnerText = reportSettings.DocumentType.Oid.ToString();
                    reportSettingsNode.AppendChild(documentTypeNode);
                    XmlNode printerNode = xml.CreateElement("Printer");
                    printerNode.InnerText = reportSettings.Printer.Name;
                    reportSettingsNode.AppendChild(printerNode);
                    XmlNode xmlCustomReportNode = xml.CreateElement("CustomReport");
                    xmlCustomReportNode.InnerText = reportSettings.Report == null ? Guid.Empty.ToString() : reportSettings.Report.Oid.ToString();
                    reportSettingsNode.AppendChild(xmlCustomReportNode);
                    XmlNode xmlPrintFormatNode = xml.CreateElement("XMLPrintFormat");
                    if (reportSettings.PrintFormat != null)
                    {
                        xmlPrintFormatNode.InnerXml = reportSettings.PrintFormat.Oid.ToString();
                        reportSettingsNode.AppendChild(xmlPrintFormatNode);
                    }

                    settingsNode.AppendChild(reportSettingsNode);
                });
                xml.AppendChild(settingsNode);
            }
            return xml;
        }



        public List<T> DeserializeCompressedData<T>(string compressedData)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(Convert.ToString(ZipLZMA.DecompressLZMA(compressedData))) ?? new List<T>();
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }



        public string GetPOSVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision;
        }

        public bool GetForceReload<T>(string deviceID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString()),
                                                                                            new BinaryOperator("CreatedByDevice", deviceID)));
                if (obj != null)
                {
                    return obj.ForceReload;
                }
                return true;
            }
        }

        public void SetForceReload<T>(bool reload, string deviceID)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString()),
                                                                                     new BinaryOperator("CreatedByDevice", deviceID)));
                if (obj != null)
                {
                    obj.ForceReload = reload;
                    obj.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
        }

        public void SetVersion<T>(DateTime ver, string createdByDevice)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal),
                    new BinaryOperator("CreatedByDevice", createdByDevice, BinaryOperatorType.Equal)));
                if (obj == null)
                {
                    obj = new TableVersion(uow) { EntityName = typeof(T).ToString() };
                }
                obj.Number = ver.Ticks;
                obj.CreatedByDevice = createdByDevice;
                obj.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        public byte[] GetDllLibrary(int posid, string dllPath)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ITS.Retail.Model.POS pos = uow.FindObject<ITS.Retail.Model.POS>(new BinaryOperator("ID", posid));
                    if (pos == null)
                    {
                        return GetEnumBytes(eServiceResponce.INVALID_INPUT);
                    }
                    if (pos.POSReportSettings.Count > 0)
                    {
                        byte[] tempDllBytes = File.ReadAllBytes(dllPath);
                        return tempDllBytes;
                    }
                    return GetEnumBytes(eServiceResponce.EMPTY_RESPONCE);
                }
            }
            catch (Exception exception)
            {
                return GetEnumBytes(eServiceResponce.EXCEPTION_HAS_BEEN_THROWN);
            }
        }

        private byte[] GetEnumBytes(eServiceResponce iNVALID_INPUT)
        {
            throw new NotImplementedException();
        }

        public void FailureOnDesirialise<T>(string error, JObject jsonItem)
        {
            throw new Exception("ERROR ON DESERIALISE TYPE " + typeof(T).ToString());
        }

        public void FromJson<T>(JObject jsonItem, T obj) where T : BasicObj
        {
            string error;
            var jsonResult = obj.FromJson(jsonItem, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, true, out error);
            if (jsonResult == false)
                FailureOnDesirialise<T>(error, jsonItem);
        }

        public bool InvokeGenericMethod(object invoker, MethodInfo genericMethod, object parameters)
        {
            bool result;
            return bool.TryParse(Convert.ToString(genericMethod.Invoke(invoker, new object[] { parameters })), out result) ? result : false;
        }

        public MethodInfo GetGenericMethod(Type classType, Type[] args, string methodName)
        {
            return classType.GetMethod(methodName, Flags).MakeGenericMethod(args);
        }

        public Type[] GetGenericMethodArgsRetailModel(string typeName)
        {
            return new Type[] { System.Type.GetType(typeof(Item).AssemblyQualifiedName.ToString().Replace("Item", typeName)) };

        }


        /// <summary>
        /// Invokes a generic by reflection
        /// </summary>R= The return type
        /// <typeparam name="R"> The return type from the invoked method </typeparam>
        /// <typeparam name="C"> The ClassType the method belongs </typeparam>
        /// <param name="methodName"> The name of the invoked method </param>
        /// <param name="parameters"> Parameters for invoked method as object array (nothing = new object[0]) </param>
        /// <param name="args">The generic type of invoked method etc T </param>
        /// <param name="invoker">if required the instance of object that will call the method -> null for static methods</param>
        /// <returns>R if R is not Convertible attempts an unsafe Cast </returns>
        public R InvokeGeneric<R, C>(string methodName, object[] parameters, Type[] args, object invoker)
        {
            if (!typeof(R).IsAssignableFrom(typeof(IConvertible)))
            {
                return (R)(typeof(C).GetMethod(methodName, Flags).MakeGenericMethod(args).Invoke(invoker, parameters));
            }
            else
            {
                var a = typeof(C).GetMethod(methodName, Flags);
                var b = a.MakeGenericMethod(args);
                var d = b.Invoke(invoker, parameters);
                return (R)Convert.ChangeType(typeof(C).GetMethod(methodName, Flags).MakeGenericMethod(args).Invoke(invoker, parameters),
                                                               typeof(R).IsEnum ? typeof(R).GetEnumUnderlyingType() : typeof(R));
            }

        }


        /// <summary>
        /// Invokes a generic by reflection
        /// </summary>R= The return type
        /// <typeparam name="R"> The return type from the invoked method </typeparam>
        /// <typeparam name="C"> The ClassType the method belongs </typeparam>
        /// <param name="methodName"> The name of the invoked method </param>
        /// <param name="parameters"> Parameters for invoked method as object array (nothing = new object[0]) </param>
        /// <param name="args">The generic type of invoked method etc T </param>
        /// <param name="invoker">if required the instance of object that will call the method -> null for static methods</param>
        /// <returns>R</returns>
        public R InvokeGeneric<R, C>(string methodName, object[] parameters, Type type, object invoker)
        {
            if (!typeof(R).IsAssignableFrom(typeof(IConvertible)))
            {
                return (R)(typeof(C).GetMethod(methodName, Flags).MakeGenericMethod(new Type[] { type }).Invoke(invoker, parameters));
            }
            else
            {
                return (R)Convert.ChangeType(typeof(C).GetMethod(methodName, Flags).MakeGenericMethod(new Type[] { type }).Invoke(invoker, parameters),
                                                                                               typeof(R).IsEnum ? typeof(R).GetEnumUnderlyingType() : typeof(R));
            }

        }



        public List<string> GetDatabaseBObjects<T>(CriteriaOperator crit) where T : BasicObj
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            List<string> list = new List<string>();
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    XPCursor objects = new XPCursor(uow, typeof(T), crit, new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending))
                    {
                        SelectDeleted = true,
                        TopReturnedObjects = 10000
                    };
                    foreach (T obj in objects)
                    {
                        list.Add(obj.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, eUpdateDirection.MASTER_TO_STORECONTROLLER));
                    }

                }
            }
            catch (Exception e)
            {
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            return list;
        }



    }


}