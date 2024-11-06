//-----------------------------------------------------------------------
// <copyright file="BasicObj.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;
using System.Drawing;
using System.IO;
using DevExpress.Xpo.Metadata.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Globalization;
using ITS.Retail.Platform.Kernel.Model;
using ITS.WRM.Model.Interface;
using System.Linq.Expressions;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public abstract class BasicObj : XPCustomObject, IXPObject, IComparable, IPersistentObject
    {
        public static readonly int year = 2017;
        public static readonly int month = 8;
        public static readonly int day = 24;
        public static readonly int order = 1;

        public static readonly long schemaVersion = CalculateVersion(year, month, day, order);//year * 100000000L + month * 100000L + day * 10000L + order * 1L;
        public static long CalculateVersion(int year, int month, int day, int order)
        {
            return year * 100000000L + month * 1000000L + day * 10000L + order * 1L;
        }

        public delegate void ExecuteActionTypesEvent(IEnumerable<ActionTypeEntity> actionTypeEntities, Guid oid, Type type);

        public static event ExecuteActionTypesEvent ExecuteActionTypes;

        // this delegate is just, so you don't have to pass an object array. _(params)_
        public delegate object ConstructorDelegate(params object[] args);

        public static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
        {
            // Get the constructor info for these parameters
            var constructorInfo = type.GetConstructor(parameters);

            // define a object[] parameter
            var paramExpr = Expression.Parameter(typeof(Object[]));

            // To feed the constructor with the right parameters, we need to generate an array 
            // of parameters that will be read from the initialize object array argument.
            var constructorParameters = parameters.Select((paramType, index) =>
                // convert the object[index] to the right constructor parameter type.
                Expression.Convert(
                    // read a value from the object[index]
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            // just call the constructor.
            var body = Expression.New(constructorInfo, constructorParameters);

            var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
            return constructor.Compile();
        }


        public BasicObj()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BasicObj(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public static Type GetTypeStatic()
        {
            var stack = new System.Diagnostics.StackTrace();

            if (stack.FrameCount < 2)
                return null;

            return (stack.GetFrame(1).GetMethod() as System.Reflection.MethodInfo).DeclaringType;
        }

        public static CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            return null;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            Oid = Guid.NewGuid();
            CreatedOnTicks = DateTime.Now.Ticks;
            //CreatedBy = GlobalSettings.User;
            _RowDeleted = false;
            IsActive = true;
            _IsSynchronized = true;
            //MasterObjOid = null;
            this.SkipOnSavingProcess = false;
        }

        private Guid _MasterObjOid;
        private string _MLValues;
        private Guid _Oid;
        private MemberInfoCollection _ChangedMembers;
        private static Dictionary<Type, ReflectionModelDescription> _cachedReflectionModelDescription = new Dictionary<Type, ReflectionModelDescription>();
        private static object lockobject = new object();
        private string _ObjectSignature;
        private bool _IsActive;
        private bool _IsSynchronized;
        private bool _RowDeleted;
        private long _UpdatedOnTicks;
        private User _CreatedBy;
        private string _CreatedByDevice;
        private string _UpdateByDevice;
        private User _UpdatedBy;
        private long _CreatedOnTicks;
        private string _ReferenceId;
        private bool _SkipOnSavingProcess;
        private string _ThirdPartNum;

        [Key]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        public DateTime CreatedOn
        {
            get
            {
                return new DateTime(_CreatedOnTicks);
            }
        }

        public long CreatedOnTicks
        {
            get
            {
                return _CreatedOnTicks;
            }
            set
            {
                SetPropertyValue("CreatedOnTicks", ref _CreatedOnTicks, value);
            }
        }


        public DateTime UpdatedOn
        {
            get
            {
                return new DateTime(_UpdatedOnTicks);
            }
        }

        public long UpdatedOnTicks
        {
            get
            {
                return _UpdatedOnTicks;
            }
            set
            {
                SetPropertyValue("UpdatedOnTicks", ref _UpdatedOnTicks, value);
            }
        }

        public User CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                SetPropertyValue("CreatedBy", ref _CreatedBy, value);
            }
        }


        public User UpdatedBy
        {
            get
            {
                return _UpdatedBy;
            }
            set
            {
                SetPropertyValue("UpdatedBy", ref _UpdatedBy, value);
            }
        }

        public string CreatedByDevice
        {
            get
            {
                return _CreatedByDevice;
            }
            set
            {
                SetPropertyValue("CreatedByDevice", ref _CreatedByDevice, value);
            }
        }


        public string UpdateByDevice
        {
            get
            {
                return _UpdateByDevice;
            }
            set
            {
                SetPropertyValue("UpdateByDevice", ref _UpdateByDevice, value);
            }
        }


        public bool RowDeleted
        {
            get
            {
                return _RowDeleted;
            }
            set
            {
                SetPropertyValue("RowDeleted", ref _RowDeleted, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }


        public bool IsSynchronized
        {
            get
            {
                return _IsSynchronized;
            }
            set
            {
                SetPropertyValue("IsSynchronized", ref _IsSynchronized, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string MLValues
        {
            get
            {
                return _MLValues;
            }
            set
            {
                SetPropertyValue("MLValues", ref _MLValues, value);
            }
        }


        public Guid MasterObjOid
        {
            get
            {
                return _MasterObjOid;
            }
            set
            {
                SetPropertyValue("MasterObjOid", ref _MasterObjOid, value);
            }
        }


        [Indexed(Unique = false)]
        [Size(1000)]
        public string ReferenceId
        {
            get
            {
                return _ReferenceId;
            }
            set
            {
                SetPropertyValue("ReferenceId", ref _ReferenceId, value);
            }
        }

        public string ThirdPartNum
        {
            get
            {
                return _ThirdPartNum;
            }
            set
            {
                SetPropertyValue("ThirdPartNum", ref _ThirdPartNum, value);
            }
        }

        #region ChangedMembers

        public MemberInfoCollection ChangedMembers
        {
            get
            {
                if (_ChangedMembers == null)
                {
                    _ChangedMembers = new MemberInfoCollection(ClassInfo);
                }
                return _ChangedMembers;
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (newValue != null)
            {
                base.OnChanged(propertyName, oldValue, newValue);
                if (!this.IsLoading)
                {
                    XPMemberInfo Member = ClassInfo.GetPersistentMember(propertyName);
                    if (Member != null && !ChangedMembers.Contains(Member))
                    {
                        ChangedMembers.Add(ClassInfo.GetMember(propertyName));
                    }
                }
            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();

            List<MemberValuesDifference> memberValuesDifferences = null;
            bool triggersActionTypes = this.GetType().GetCustomAttributes(typeof(TriggerActionTypeAttribute), true).Count() >= 1;
            if (triggersActionTypes && ExecuteActionTypes != null)
            {
                memberValuesDifferences = this.ChangedMembers.Select(xpMemberInfo => new MemberValuesDifference(xpMemberInfo.Name, xpMemberInfo.GetOldValue(this), xpMemberInfo.GetValue(this))).ToList();
            }

            if (triggersActionTypes && ExecuteActionTypes != null)
            {
                //DatabaseAction databaseAction = DatabaseAction.UPDATE;
                //if (this.Session.IsNewObject(this))
                //{
                //    databaseAction = DatabaseAction.INSERT;
                //}
                //else if (this.GetPropertyValue("GCRecord") != null)
                //{
                //    databaseAction = DatabaseAction.DELETE;
                //}

                XPCollection<ActionTypeEntity> actionTypeEntities = null;

                PropertyInfo actionTypeEntitiesPropertyInfo = this.GetType().GetProperty("ActionTypeEntities");
                if (actionTypeEntitiesPropertyInfo != null)
                {
                    actionTypeEntities = (XPCollection<ActionTypeEntity>)actionTypeEntitiesPropertyInfo.GetValue(this, null);
                    ExecuteActionTypes(actionTypeEntities, Oid, this.GetType());
                }
            }

            ChangedMembers.Clear();
        }
        #endregion


        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            PropertyInfo ownerProperty = this.GetType().GetProperty("Owner");

            if (typeof(IRequiredOwner).IsAssignableFrom(this.GetType()) && ownerProperty.CanWrite && ownerProperty.GetValue(this, null) == null && this.IsDeleted == false)
            {
                throw new Exception("Owner for object " + this.GetType().Name + " cannot be null!!!");
            }

            int count = this.ChangedMembers.Where(x => x.Name == "UpdatedOnTicks").Count();

            if (count == 0 && this.SkipOnSavingProcess == false)
            {
                UpdatedOnTicks = DateTime.Now.Ticks;
            }


            //UpdatedBy = GlobalSettings.User;

            if (!String.IsNullOrEmpty(this.CreatedByDevice) && this.MasterObjOid == Guid.Empty)
            {
                UniqueFieldsAttribute[] uniqueFields = this.GetType().GetCustomAttributes(typeof(UniqueFieldsAttribute), true) as UniqueFieldsAttribute[];

                if (uniqueFields.Length > 1)
                {
                    throw new Exception("Please Redesign Object " + this.GetType().FullName);
                }
                else if (uniqueFields.Length == 1)
                {
                    CriteriaOperator uniqueFieldsCriteria = null;

                    foreach (String uniqueField in uniqueFields.First().UniqueFields)
                    {
                        PropertyInfo property = this.GetType().GetProperty(uniqueField);
                        if (property == null)
                        {
                            throw new Exception("Please Redesign Object " + this.GetType().FullName + " and check property " + uniqueField);
                        }
                        uniqueFieldsCriteria = CriteriaOperator.And(uniqueFieldsCriteria, new BinaryOperator(uniqueField, property.GetValue(this, null)));
                    }

                    CriteriaOperator crop = CriteriaOperator.And(uniqueFieldsCriteria, CriteriaOperator.Or(
                        new NotOperator(new BinaryOperator("CreatedByDevice", this.CreatedByDevice)),
                        new NullOperator("CreatedByDevice")
                        ));
                    XPCollection duplicateEntries = new XPCollection(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, this.GetType(), crop);

                    if (duplicateEntries.Count == 0)
                    {

                    }
                    else if (duplicateEntries.Count == 1)
                    {
                        this.MasterObjOid = ((BasicObj)duplicateEntries[0]).Oid;
                    }
                    else//>1
                    {
                        duplicateEntries.Filter = new NullOperator("MasterObjOid");
                        if (duplicateEntries.Count != 1)
                        {
                            throw new Exception("To many entries created by Master Retail Server");
                        }
                        this.MasterObjOid = ((BasicObj)duplicateEntries[0]).Oid;
                    }
                }

                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    if (property.PropertyType.IsSubclassOf(typeof(BasicObj)))
                    {
                        BasicObj propertyObj = property.GetValue(this, null) as BasicObj;
                        if (propertyObj != null && propertyObj.MasterObjOid != Guid.Empty)
                        {
                            BasicObj parentMasterObject = this.Session.GetObjectByKey(property.PropertyType, propertyObj.MasterObjOid) as BasicObj;
                            property.SetValue(this, parentMasterObject, null);
                        }
                    }
                }
            }

            this.GenerateObjectSignature();

            base.OnSaving();
        }

        protected void Activate(bool act)
        {
            IsActive = act;
        }

        public void GetData(BasicObj item, List<string> ignoreProperties = null)
        {
            if (ignoreProperties == null)
            {
                ignoreProperties = new List<string>();
                ignoreProperties.Add("UpdatedOnTicks");
                ignoreProperties.Add("UpdatedOn");
                ignoreProperties.Add("Owner");
            }


            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties().Where(g => g.CanWrite == true && typeof(BasicObj).IsAssignableFrom(g.DeclaringType));
            foreach (PropertyInfo prop in properties)
            {
                if (ignoreProperties.Contains(prop.Name))
                {
                    continue;
                }

                if (prop.PropertyType.IsSubclassOf(typeof(BasicObj)))
                {
                    BasicObj newVariable = ((BasicObj)prop.GetValue(item, null));
                    if (newVariable != null)
                    {
                        Guid otherObjectOid = newVariable.Oid;
                        prop.SetValue(this, this.Session.GetObjectByKey(prop.PropertyType, otherObjectOid), null);
                    }
                    else
                    {
                        prop.SetValue(this, null, null);
                    }
                }
                else
                {
                    prop.SetValue(this, prop.GetValue(item, null), null);
                }
            }

        }


        public virtual bool FromJson(JObject jsonItem, JsonSerializerSettings settings, bool copyOid, bool errorOnOidNotFound, out string error, bool deserializeChild = true)
        {
            CultureInfo cultureInfoToBeRestored = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = Platform.PlatformConstants.DefaultCulture;
            IEnumerable<JProperty> itemValues = jsonItem.Properties();
            error = "";

            foreach (JProperty itmValue in itemValues)
            {
                try
                {
                    if (itmValue.Name == "GCRecord")
                    {
                        int value;
                        int? finalValue;
                        if (Int32.TryParse(itmValue.Value.ToString(), out value))
                        {
                            finalValue = value;
                        }
                        else
                        {
                            finalValue = null;
                        }

                        this.SetMemberValue("GCRecord", finalValue);
                    }
                    else
                    {

                        if (itmValue.Name == "Oid" && copyOid == false)
                        {
                            continue;
                        }
                        Type itemPropertyType = this.GetType().GetProperty(itmValue.Name).PropertyType;
                        object fValue = GetDefault(itemPropertyType);
                        if (itemPropertyType.IsSubclassOf(typeof(BaseObj)))
                        {
                            if (!String.IsNullOrWhiteSpace(itmValue.Value.ToString()))
                            {
                                Guid oid = Guid.Empty;

                                if (Guid.TryParse(itmValue.Value.ToString(), out oid) && oid != Guid.Empty)
                                {
                                    fValue = this.Session.GetObjectByKey(itemPropertyType, oid);
                                    if (fValue == null)
                                    {
                                        fValue = this.Session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, itemPropertyType, new BinaryOperator("Oid", oid));
                                        if (fValue == null && errorOnOidNotFound)
                                        {
                                            error = this.GetType().Name + ": Could not find object of type \"" + itemPropertyType.Name + "\" with Oid \"" + oid + "\"";
                                            Thread.CurrentThread.CurrentCulture = cultureInfoToBeRestored;
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        JObject v = JsonConvert.DeserializeObject(itmValue.Value.ToString()) as JObject;
                                        string key = v.Property("Oid").Value.ToString();
                                        Guid guidKey;
                                        if (Guid.TryParse(key, out guidKey) && deserializeChild)
                                        {
                                            BasicObj childObject = this.Session.GetObjectByKey(itemPropertyType, guidKey) as BasicObj;
                                            if (childObject == null)
                                            {
                                                childObject = Activator.CreateInstance(itemPropertyType, new object[] { this.Session }) as BasicObj;
                                            }
                                            childObject.FromJson(v, settings, copyOid, errorOnOidNotFound, out error);
                                            fValue = childObject;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string errorMessage = ex.Message;
                                    }
                                }

                            }
                        }
                        else if (itemPropertyType == typeof(Guid) || itemPropertyType == typeof(Guid?))
                        {
                            if (!String.IsNullOrWhiteSpace(itmValue.Value.ToString()))
                            {
                                fValue = new Guid(itmValue.Value.ToString());
                            }
                        }
                        else if (itemPropertyType.IsEnum)
                        {
                            try
                            {
                                fValue = Enum.Parse(itemPropertyType, itmValue.Value.ToString());
                            }
                            catch (Exception)
                            {
                                fValue = GetDefault(itemPropertyType);
                            }
                        }
                        else if (typeof(XPBaseCollection).IsAssignableFrom(itemPropertyType) && itemPropertyType.IsGenericType && itmValue.Value.Type == JTokenType.Array)
                        {
                            Type xpType = itemPropertyType.GetGenericArguments().First();
                            if (xpType.IsSubclassOf(typeof(BasicObj)))
                            {
                                JArray array = (JArray)itmValue.Value;
                                var methodAdd = itemPropertyType.GetMethods().Where(g => g.Name == "Add").First();
                                var thisDetailCollection = GetType().GetProperty(itmValue.Name).GetValue(this, null);
                                List<Guid> includedDetails = new List<Guid>();
                                foreach (var v in array)
                                {
                                    JObject json = (v is JObject) ? (v as JObject) : JObject.Parse(v.ToString());

                                    string key = json.Property("Oid").Value.ToString();
                                    Guid keyGuid = new Guid(key);

                                    BasicObj detail = (BasicObj)Session.GetObjectByKey(xpType, keyGuid);
                                    if (detail == null)
                                    {
                                        detail = (BasicObj)Activator.CreateInstance(xpType, this.Session);
                                        if (!copyOid)
                                        {
                                            keyGuid = detail.Oid;
                                        }
                                    }
                                    includedDetails.Add(keyGuid);
                                    bool detailResult = detail.FromJson(json, settings, copyOid, errorOnOidNotFound, out error);
                                    if (detailResult == false)
                                    {
                                        Thread.CurrentThread.CurrentCulture = cultureInfoToBeRestored;
                                        return false;
                                    }
                                    methodAdd.Invoke(thisDetailCollection, new object[] { detail });
                                }
                                List<BasicObj> objectsToDelete = (thisDetailCollection as IEnumerable).Cast<BasicObj>().Where(
                                    x => includedDetails.Contains(x.Oid) == false
                                    ).ToList();
                                this.Session.Delete(objectsToDelete);
                            }
                        }
                        else if (typeof(XPBaseCollection).IsAssignableFrom(itemPropertyType))
                        {
                            continue;
                        }
                        else if (itemPropertyType.IsArray)
                        {
                            fValue = Convert.ChangeType(itmValue.Value, itemPropertyType);
                        }
                        else if (itemPropertyType == typeof(Image))
                        {
                            byte[] imageBytes = Convert.FromBase64String(itmValue.Value.ToString());
                            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            Image image = Image.FromStream(ms, true);
                            fValue = image;
                        }
                        else if (itemPropertyType == typeof(DateTime) || itemPropertyType == typeof(DateTime?))
                        {
                            try
                            {
                                if (itmValue.Value != null && !String.IsNullOrWhiteSpace(itmValue.Value.ToString()))
                                {
                                    fValue = new DateTime(long.Parse(itmValue.Value.ToString()));
                                }
                            }
                            catch
                            {
                                fValue = Convert.ChangeType(itmValue.Value.ToString(), itemPropertyType);
                            }
                        }
                        else
                        {
                            fValue = Nullable.GetUnderlyingType(itemPropertyType) != null ? Convert.ChangeType(itmValue.Value.ToString(), Nullable.GetUnderlyingType(itemPropertyType))
                                                                                                                              : Convert.ChangeType(itmValue.Value.ToString(), itemPropertyType);
                        }
                        this.GetType().GetProperty(itmValue.Name).SetValue(this, fValue, null);
                    }
                }
                catch (Exception)
                {
                    continue;
                }

            }
            Thread.CurrentThread.CurrentCulture = cultureInfoToBeRestored;
            return true;
        }
        /// <summary>
        /// Needs testing
        /// </summary>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        public bool FromJson(string json, JsonSerializerSettings settings, bool copyOid, bool errorOnNotFoundOid, out string error)
        {
            JObject jsonItem = JObject.Parse(json);
            return FromJson(jsonItem, settings, copyOid, errorOnNotFoundOid, out error);
        }


        public virtual Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails,
            eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            CultureInfo cultureInfoToBeRestored = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = Platform.PlatformConstants.DefaultCulture;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            Type thisType = this.GetType();
            if (_cachedReflectionModelDescription.ContainsKey(thisType) == false)
            {
                lock (lockobject)
                {
                    if (_cachedReflectionModelDescription.ContainsKey(thisType) == false)
                    {
                        _cachedReflectionModelDescription.Add(thisType, new ReflectionModelDescription(thisType));
                    }
                }
            }
            ReflectionModelDescription modelDescription = _cachedReflectionModelDescription[thisType];
            foreach (PropertyInfo prop in modelDescription.GetProperties(direction))
            {
                string propName = prop.Name;

                object propValue = prop.GetValue(this, null);
                {
                    if (propValue is PersistentBase && propValue != null)
                    {
                        dict.Add(propName, ((BasicObj)propValue).Oid.ToString());
                    }
                    else if (propValue != null && propValue.GetType().IsSubclassOf(typeof(Enum)))
                    {
                        dict.Add(propName, ((int)propValue).ToString());
                    }
                    else if (propValue != null && propValue.GetType().IsArray)
                    {
                        dict.Add(propName, propValue);
                    }
                    else if (propValue != null && propValue is Image)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ((Image)propValue).Save(ms, ((Image)propValue).RawFormat);
                            byte[] imageBytes = ms.ToArray();
                            string base64String = Convert.ToBase64String(imageBytes);
                            dict.Add(propName, base64String);
                        }
                    }
                    else if (propValue != null && (propValue.GetType() == typeof(DateTime) || propValue.GetType() == typeof(DateTime?)))
                    {
                        dict.Add(propName, ((DateTime)propValue).Ticks);
                    }
                    else
                    {
                        dict.Add(propName, propValue != null ? propValue.ToString() : "");
                    }
                }
            }
            dict.Add("GCRecord", this.GetPropertyValue("GCRecord") == null ? "null" : this.GetPropertyValue("GCRecord").ToString());
            if (includeType)
            {
                if (!dict.ContainsKey("Type"))
                {
                    dict.Add("Type", thisType.Name);
                }
            }

            Thread.CurrentThread.CurrentCulture = cultureInfoToBeRestored;
            return dict;
        }

        public string ToJson(JsonSerializerSettings settings, eUpdateDirection direction, bool includeType = false, bool includeDetails = true)
        {
            Dictionary<string, object> dict = GetDict(settings, includeType, includeDetails, direction);
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }

        public string ToJson(JsonSerializerSettings settings, bool includeType = false, bool includeDetails = true)
        {
            Dictionary<string, object> dict = GetDict(settings, includeType, includeDetails);
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }


        public virtual bool CollectionsHasData(Type myType)
        {
            //IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            //foreach (PropertyInfo prop in props)
            //{
            //    if (prop.PropertyType is XPBaseCollection)
            //    {
            //        object propValue = prop.GetValue(this, null);
            //        if (propValue == null)
            //        {
            //            continue;
            //        }
            //        if ((propValue as XPBaseCollection).Count > 0)
            //        {
            //            return false;
            //        }
            //    }
            //}
            return false;
        }

        protected override void OnDeleting()
        {
            NonPersistentAttribute npa = this.GetType().GetCustomAttributes(typeof(NonPersistentAttribute), false).Cast<NonPersistentAttribute>().FirstOrDefault();
            if (npa == null)
            {
                ICollection objs = Session.CollectReferencingObjects(this);
                if (objs.Count > 0)
                {
                    foreach (XPMemberInfo mi in ClassInfo.CollectionProperties)
                    {
                        if (!mi.IsAggregated && mi.IsCollection && mi.IsAssociation)
                        {
                            foreach (IXPObject obj in objs)
                            {
                                dynamic newVariable = ((XPBaseCollection)mi.GetValue(this));
                                if (newVariable.GetType().FullName.IndexOf(obj.GetType().FullName) > 0)
                                {
                                    if (newVariable.BaseIndexOf(obj) >= 0)
                                    {
                                        throw new InvalidOperationException(String.Format("The object cannot be deleted. Other objects ({0}) have references to it.", obj.GetType().FullName.Replace("ITS.Retail.Model.", "")));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            base.OnDeleting();
        }

        protected T GetLookupObject<T>(Session session, T getObj) where T : BasicObj
        {
            if (getObj != null)
            {
                return session.GetObjectByKey<T>((getObj as BaseObj).Oid);
            }
            return null;
        }

        public override string ToString()
        {
            PropertyInfo descriptionField = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), false).Count() > 0).FirstOrDefault();
            if (descriptionField == null)
            {
                descriptionField = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), true).Count() > 0).FirstOrDefault();
            }

            if (descriptionField == null)
            {
                return base.ToString();
            }
            else
            {
                object value = descriptionField.GetValue(this, null);
                return value == null ? "" : value.ToString();
            }
        }


        public TemporaryObject GetTemporaryObject(Session session = null)
        {
            Session s = session == null ? this.Session : session;
            return s.FindObject<TemporaryObject>(
                CriteriaOperator.And(
                        new BinaryOperator("EntityOid", this.Oid),
                        new BinaryOperator("EntityType", this.GetType().FullName)
                    ));
        }

        [NonPersistent]
        public bool TempObjExists
        {
            get
            {
                return GetTemporaryObject(this.Session) != null;
            }
        }


        [Size(SizeAttribute.Unlimited)]
        [JsonIgnore]
        //[Indexed("Oid;GCRecord",Unique = false)]
        [UpdaterIgnoreField]
        public string ObjectSignature
        {
            get
            {
                return _ObjectSignature;
            }
            set
            {
                SetPropertyValue("ObjectSignature", ref _ObjectSignature, value);
            }
        }

        private void GenerateObjectSignature()
        {
            string source = this.ToJson(Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS);//TODO Modify string
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, source);
                this.ObjectSignature = hash;
            }
        }

        /// <summary>
        /// Microsoft provided
        /// https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public bool SkipOnSavingProcess
        {
            get
            {
                return _SkipOnSavingProcess;
            }
            set
            {
                SetPropertyValue("SkipOnSavingProcess", ref _SkipOnSavingProcess, value);
            }
        }

        public static void ChangeToAnonymity()
        {


        }
        public virtual List<string> IgnorePropertiesOnJsonDeserialise()
        {
            return new List<string>();
        }

        public void GetDataFromDetailObjects(BasicObj item, List<string> ignoreProperties = null)
        {
            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties().Where(g => g.CanRead
                                                                                          //&& g.CanWrite
                                                                                          && g.PropertyType.IsGenericType
                                                                                          && g.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>));
            try
            {
                foreach (PropertyInfo prop in properties)
                {
                    if (ignoreProperties != null && ignoreProperties.Contains(prop.Name))
                    {
                        continue;
                    }
                    MethodInfo addMethod = prop.PropertyType.GetMethod("Add");
                    if (addMethod == null)
                    {
                        continue;
                    }
                    IEnumerable<BasicObj> details = prop.GetValue(item, null) as IEnumerable<BasicObj>;
                    if (details == null)
                    {
                        continue;
                    }
                    List<BasicObj> toDelete = new List<BasicObj>();
                    foreach (BasicObj currentDetail in details)
                    {
                        if (currentDetail == null)
                        {
                            continue;
                        }
                        BasicObj newDetail = Activator.CreateInstance(currentDetail.GetType(), new object[] { this.Session }) as BasicObj;
                        newDetail.GetData(currentDetail);
                        addMethod.Invoke(prop.GetValue(this, null), new object[] { newDetail });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
