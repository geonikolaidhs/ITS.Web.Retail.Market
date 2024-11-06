using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform.Kernel.Model;


namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public abstract class BasicObj : XPCustomObject, IXPObject, IComparable, IPersistentObject
    {
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
        }

        private Guid _Oid;
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
        private long _CreatedOnTicks;
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
                return new DateTime(UpdatedOnTicks);
            }
        }

        private string _UpdatedOnTicksStr;
        [Persistent("UpdatedOnTicks")]
        [Indexed("GCRecord")]
        /// <summary>
        /// SQLite Fix ....
        /// </summary>
        public string UpdatedOnTicksStr
        {
            get
            {
                return _UpdatedOnTicksStr;
            }
            set
            {
                SetPropertyValue("UpdatedOnTicksStr", ref _UpdatedOnTicksStr, value);
            }
        }

        [NonPersistent]
        public long UpdatedOnTicks
        {
            get
            {
                long parsed = 0;
                long.TryParse(this.UpdatedOnTicksStr, out parsed);
                return parsed;
            }
            set
            {
                this.UpdatedOnTicksStr = value.ToString();
            }
        }

        private Guid _CreatedBy;
        public Guid CreatedBy
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


        private Guid _UpdatedBy;
        public Guid UpdatedBy
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

        private string _CreatedByDevice;
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

        private string _UpdateByDevice;
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

        private bool _RowDeleted;
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

        private bool _IsActive;
        [Indexed("GCRecord")]
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

        private bool _IsSynchronized;
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

        private DevExpress.Xpo.Metadata.Helpers.MemberInfoCollection _ChangedMembers;
        public DevExpress.Xpo.Metadata.Helpers.MemberInfoCollection ChangedMembers
        {
            get
            {
                if (_ChangedMembers == null)
                {
                    _ChangedMembers = new DevExpress.Xpo.Metadata.Helpers.MemberInfoCollection(ClassInfo);
                }
                return _ChangedMembers;
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!this.IsLoading)
            {
                DevExpress.Xpo.Metadata.XPMemberInfo Member = ClassInfo.GetPersistentMember(propertyName);
                if (Member != null && !ChangedMembers.Contains(Member))
                {
                    ChangedMembers.Add(ClassInfo.GetMember(propertyName));
                }
            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            ChangedMembers.Clear();
        }

        protected virtual bool UpdateTicksOnSaving
        {
            get
            {
                return true;
            }
        }

        protected override void OnSaving()
        {
            int count = this.ChangedMembers.Where(x => x.Name == "UpdatedOnTicks" || x.Name == "UpdatedOnTicksStr").Count();

            if (count == 0 && UpdateTicksOnSaving)
            {
                UpdatedOnTicks = DateTime.Now.Ticks;
            }

            base.OnSaving();
        }

        protected void Activate(bool act)
        {
            IsActive = act;
        }

        public void GetData(BasicObj item)
        {
            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties().Where(g => g.CanWrite == true && typeof(BasicObj).IsAssignableFrom(g.DeclaringType));
            foreach (PropertyInfo prop in properties)
            {
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


        public virtual bool CollectionsHasData(Type myType)
        {
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                if (typeof(XPBaseCollection).IsAssignableFrom(prop.PropertyType))
                {
                    object propValue = prop.GetValue(this, null);
                    if (propValue == null)
                    {
                        continue;
                    }
                    if ((propValue as XPBaseCollection).Count > 0)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        protected override void OnDeleting()
        {
            if (!CollectionsHasData(this.GetType()))
            {
                base.OnDeleting();
            }
        }
        protected object GetLookupObject<T>(Session session, T getObj)
        {
            if (getObj != null)
            {
                return session.GetObjectByKey<T>((getObj as BaseObj).Oid);
            }
            return null;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Reads the properties of the object from the specified json object
        /// </summary>
        /// <param name="jsonItem">JSON Object</param>
        /// <param name="settings">Serilization Settings</param>
        /// <param name="copyOid">Copy Oid</param>
        public virtual void FromJson(JObject jsonItem, JsonSerializerSettings settings, bool copyOid)
        {
            IEnumerable<JProperty> itemValues = jsonItem.Properties();
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

                        SetMemberValue("GCRecord", finalValue);
                    }
                    else
                    {

                        if (itmValue.Name == "Oid" && copyOid == false)
                        {
                            continue;
                        }
                        Type itemPropertyType = GetType().GetProperty(itmValue.Name).PropertyType;
                        object fValue = GetDefault(itemPropertyType);
                        if (itemPropertyType.IsSubclassOf(typeof(BaseObj)))
                        {
                            if (!String.IsNullOrWhiteSpace(itmValue.Value.ToString()))
                            {
                                fValue = Session.GetObjectByKey(itemPropertyType, new Guid(itmValue.Value.ToString()));
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
                                foreach (var v in array)
                                {
                                    string key = (v as JObject).Property("Oid").Value.ToString();
                                    BasicObj detail = (BasicObj)Session.GetObjectByKey(xpType, new Guid(key));
                                    if (detail == null)
                                    {
                                        detail = (BasicObj)Activator.CreateInstance(xpType, this.Session);
                                    }
                                    detail.FromJson(v as JObject, settings, copyOid);

                                    methodAdd.Invoke(thisDetailCollection, new object[] { detail });
                                }
                            }
                        }
                        else if (typeof(XPBaseCollection).IsAssignableFrom(itemPropertyType))
                        {
                            continue;
                        }
                        else if (itemPropertyType == typeof(DateTime) || itemPropertyType == typeof(DateTime?))
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(itmValue.Value.ToString()))
                                {
                                    fValue = new DateTime(long.Parse(itmValue.Value.ToString()));
                                }
                            }
                            catch
                            {
                                fValue = Convert.ChangeType(itmValue.Value.ToString(), itemPropertyType);
                            }
                        }
                        else if (itemPropertyType == typeof(byte[]))
                        {
                            fValue = Convert.FromBase64String(itmValue.Value.ToString());
                        }
                        else
                        {
                            if (itmValue.Name.ToUpper() == "CANINSERT" || itmValue.Name.ToUpper() == "CANUPDATE" || itmValue.Name.ToUpper() == "CANDELETE")
                            {
                                if (itmValue.Value.ToString().ToUpper() == "FALSE")
                                {
                                    fValue = 0;
                                }
                                else
                                {
                                    fValue = 1;
                                }
                            }
                            else
                            {
                                fValue = Convert.ChangeType(itmValue.Value.ToString(), itemPropertyType);
                            }
                        }
                        this.GetType().GetProperty(itmValue.Name).SetValue(this, fValue, null);
                    }
                }
                catch (Exception exception)
                {
                    continue;
                }

            }
        }
        /// <summary>
        /// Reads the properties of the object from the specified json string
        /// </summary>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        public virtual void FromJson(string json, JsonSerializerSettings settings, bool copyOid)
        {
            JObject jsonItem = JObject.Parse(json);
            FromJson(jsonItem, settings, copyOid);
        }

        public virtual Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties());
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                //Τα Non persistent ειναι υπολογιζόμενα συνήθως. Σε κάθε περίπτωση δεν χρειάζεται να μεταφερθούν
                if (prop.GetCustomAttributes(typeof(NonPersistentAttribute), false).Count() > 0)
                {
                    if (!prop.CanWrite)
                        continue;
                }


                //Attributes που χρειάζονται στο desing time
                if (prop.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Count() > 0)
                    continue;
                //Συγκεκριμένα Attribute που προκαλούν διάφορα προβλήματα
                if (propName == "Session" || propName == "ClassInfo" || propName == "IsLoading")
                    continue;
                //Computed Attributes. Αυτά δεν μπορούν να γίνουν set. Τζάμπα θα μεταφερθούν!
                if (!prop.CanWrite)
                    continue;

                //Για όλα τα υπόλοιπα....
                object propValue = prop.GetValue(this, null);
                if (!(propValue is XPBaseCollection))
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
                dict.Add("Type", this.GetType().Name);
            }
            return dict;
        }

        public string ToJson(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, object> dict = GetDict(settings, includeType);
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }


        private string _ThirdPartNum;
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
    }
}
