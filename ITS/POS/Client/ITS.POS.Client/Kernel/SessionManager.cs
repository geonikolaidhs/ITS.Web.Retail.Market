using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Xpo;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Versions;
using ITS.Retail.Platform;
using System.IO;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.POS.Client.Exceptions;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Helpers;


namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides database access
    /// </summary>
    public class SessionManager : ISessionManager
    {
        private UnitOfWork _sesMaster;
        private UnitOfWork _sesSettings;
        private UnitOfWork _sesTrasactions;
        private UnitOfWork _sesMemory;
        private UnitOfWork _sesVersions;

        public SessionManager()
        {
            TransactionConnectionHelper.SetTransactionFile("PosTransactions");
        }

        private UnitOfWork Master
        {
            get
            {
                if (_sesMaster == null)
                    _sesMaster =  CreateSession(typeof(Item));
                return _sesMaster;
            }
        }

        private UnitOfWork Settings
        {
            get
            {
                if (_sesSettings == null)
                    _sesSettings = CreateSession(typeof(VatCategory));

                return _sesSettings;
            }
        }

        private UnitOfWork Transactions
        {
            get
            {
                if (_sesTrasactions == null || !_sesTrasactions.IsConnected)
                    _sesTrasactions = CreateSession(typeof(DocumentHeader));
                return _sesTrasactions;
            }
        }

        public UnitOfWork MemorySettings
        {
            get
            {
                if (_sesMemory == null)
                    _sesMemory = MemoryConnectionHelper.GetNewUnitOfWork();
                return _sesMemory;
            }
        }
        private UnitOfWork Versions
        {
            get
            {
                if (_sesVersions == null)
                    _sesVersions = CreateSession(typeof(TableVersions));
                return _sesVersions;
            }
        }

        public UnitOfWork GetSession(Type type)
        {
            Assembly TAssembly = type.Assembly;

            if (TAssembly == typeof(Item).Assembly)
            {
                return Master;
            }
            else if (TAssembly == typeof(VatCategory).Assembly)
            {
                return Settings;
            }
            else if (TAssembly == typeof(DocumentHeader).Assembly)
            {
                return Transactions;
            }
            else if (TAssembly == typeof(TableVersions).Assembly)
            {
                return Versions;
            }

            throw new POSException("UnitOfWork not found for provided Assembly.");
        }

        public UnitOfWork GetSession<T>() where T : BasicObj
        {
            return GetSession(typeof(T));
        }

        public T GetObjectByKey<T>(Guid? id) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.GetObjectByKey<T>(id);
            return result;
        }

        public T GetObjectByKey<T>(Guid? id, bool alwaysGetFromDb) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.GetObjectByKey<T>(id, alwaysGetFromDb);
            return result;
        }

        public T FindObject<T>(CriteriaOperator criteria) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.FindObject<T>(criteria);
            return result;
        }

        public T FindObject<T>(CriteriaOperator criteria,bool selectDeleted) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.FindObject<T>(criteria,selectDeleted);
            return result;
        }

        public T FindObject<T>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.FindObject<T>(criteriaEvaluationBehavior, criteria);
            return result;
        }

        public void CommitTransactionsChanges()
        {
            Transactions.CommitChanges();
        }

        public void ClearMasterSession()
        {
            if (_sesMaster != null)
            {
                _sesMaster.Dispose();
                _sesMaster = null;
            }
        }
        public void ClearSettingsSession()
        {
            if (_sesSettings != null)
            {
                _sesSettings.Dispose();
                _sesSettings = null;
            }
        }
        public void ClearTransactionsSession()
        {
            if (_sesTrasactions != null)
            {
                _sesTrasactions.Dispose();
                _sesTrasactions = null;
            }
        }
        /*
         * _sesMaster.Dispose();
            _sesSettings.Dispose();
            _sesTrasactions.Dispose();
            _sesMemory.Dispose();

            _sesMaster = null;
            _sesSettings = null;
            _sesTrasactions = null;
            _sesMemory = null;
         */


        public void ClearAllSessions()
        {
            ClearMasterSession();
            ClearSettingsSession();
            ClearTransactionsSession();
        }

        public void UpdateDatabase()
        {
            Assembly asm1 = Assembly.GetAssembly(typeof(Item));
            Master.UpdateSchema(asm1);
            Master.CreateObjectTypeRecords(asm1);
            Master.FlushChanges();
            Master.CommitChanges();
            //Master.ExecuteNonQuery("ANALYZE");

            Assembly asm2 = Assembly.GetAssembly(typeof(OwnerApplicationSettings));
            Settings.UpdateSchema(asm2);
            Settings.CreateObjectTypeRecords(asm2);
            Settings.FlushChanges();
            Settings.CommitChanges();
            //Settings.ExecuteNonQuery("ANALYZE");

            Assembly asm3 = Assembly.GetAssembly(typeof(DocumentHeader));
            Transactions.UpdateSchema(asm3);
            Transactions.CreateObjectTypeRecords(asm3);
            Transactions.FlushChanges();
            Transactions.CommitChanges();
            //Transactions.ExecuteNonQuery("ANALYZE");

            Assembly asm4 = Assembly.GetAssembly(typeof(TableVersions));
            Versions.UpdateSchema(asm4);
            Versions.CreateObjectTypeRecords(asm4);
            Versions.FlushChanges();
            Versions.CommitChanges();
            //Versions.ExecuteNonQuery("ANALYZE");

        }


        private UnitOfWork GetUnitOfWorkByAssembly(Assembly assembly)
        {
            if (assembly == typeof(Item).Assembly)
            {
                return this.Master;
            }
            else if (assembly == typeof(VatCategory).Assembly)
            {
                return this.Settings;
            }
            else if (assembly == typeof(DocumentHeader).Assembly)
            {
                return this.Transactions;
            }
            else if (assembly == typeof(TableVersions).Assembly)
            {
                return this.Versions;
            }
            else
            {
                return null;
            }
        }

        Dictionary<Type, List<PropertyInfo>> _cacheProperties = new Dictionary<Type, List<PropertyInfo>>();

        Dictionary<PropertyInfo, List<DenormalizedFieldAttribute>> _cacheAttributes = new Dictionary<PropertyInfo, List<DenormalizedFieldAttribute>>();

        public void FillDenormalizedFields(BasicObj obj)
        {
            Type type = obj.GetType();
            List<PropertyInfo> objProps;
            if (_cacheProperties.ContainsKey(type))
            {
                objProps = _cacheProperties[type];
            }
            else
            {
                objProps = obj.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(DenormalizedFieldAttribute), true).Count() > 0).ToList();
                _cacheProperties[type] = objProps;
            }
            foreach (PropertyInfo currentProperty in objProps)
            {
                List<DenormalizedFieldAttribute> atts;
                if(_cacheAttributes.ContainsKey(currentProperty))
                {
                    atts = _cacheAttributes[currentProperty];
                }
                else
                {
                    atts = currentProperty.GetCustomAttributes(typeof(DenormalizedFieldAttribute), true).Cast<DenormalizedFieldAttribute>().ToList();
                    _cacheAttributes.Add(currentProperty, atts);
                }
                foreach(DenormalizedFieldAttribute attribute in atts)
                {
                    if (currentProperty.PropertyType == typeof(Guid) || currentProperty.PropertyType == typeof(Guid?))
                    {
                        Type localEntityType = attribute.LocalEntityType;
                        if (localEntityType != null)
                        {
                            UnitOfWork uow = GetUnitOfWorkByAssembly(localEntityType.Assembly);
                            BasicObj currentPropertyObject = uow.GetObjectByKey(localEntityType, currentProperty.GetValue(obj, null)) as BasicObj;
                            if (currentPropertyObject != null)
                            {
                                PropertyInfo objDenormalizedField = obj.GetType().GetProperty(attribute.DenormalizedField);
                                if (objDenormalizedField == null)
                                {
                                    throw new POSException("SessionHelper: Property '" + attribute.DenormalizedField + "' not found in type '" + obj.GetType().FullName + "'");
                                }
                                object lookupDenormalizedFieldValue = currentPropertyObject.GetType().GetProperty(attribute.RemotePropertyName).GetValue(currentPropertyObject, null);

                                objDenormalizedField.SetValue(obj, lookupDenormalizedFieldValue, null);
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("SessionHelper: DenormalizedFieldAttribute: Only guids are supported");
                    }
                }
            }

        }

        public void ReconnectToNewFile(IDataLayer fDataLayer, IEnumerable<IDisposable> _objectsToDisposeOnDisconnect)
        {
            UnitOfWork uow = this.Transactions;
            uow.Dispose();
            IDataLayer layer = TransactionConnectionHelper.DataLayer;
            layer.Dispose();
            //fDataLayer = null;
            foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
            {
                disp.Dispose();
            }

            File.Move("PosTransactions", String.Format("PosTransactions-{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now));
            _sesTrasactions = null;
            TransactionConnectionHelper.ClearDataLayer();
        }


        public UnitOfWork CreateSession(Type type)
        {
            if (type.Assembly == typeof(Item).Assembly)
            {
                return MasterConnectionHelper.GetNewUnitOfWork();
            }
            else if (type.Assembly == typeof(VatCategory).Assembly)
            {
                return SettingsConnectionHelper.GetNewUnitOfWork();
            }
            else if (type.Assembly == typeof(DocumentHeader).Assembly)
            {
                return TransactionConnectionHelper.GetNewUnitOfWork();
            }
            else if (type.Assembly == typeof(TableVersions).Assembly)
            {
                return VersionsConnectionHelper.GetNewUnitOfWork();
            }
            else
            {
                return null;
            }
        }
    }
}
