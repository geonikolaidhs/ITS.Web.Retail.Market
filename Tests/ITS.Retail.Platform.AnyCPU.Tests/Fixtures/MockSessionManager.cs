using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Platform.Tests.Fixtures
{
    public class MockSessionManager : ISessionManager
    {
        private UnitOfWork _sesMaster;
        private UnitOfWork _sesSettings;
        private UnitOfWork _sesTrasactions;
        private UnitOfWork _sesVersions;

        private UnitOfWork Master
        {
            get
            {
                if (_sesMaster == null)
                    _sesMaster = MemoryXpoHelper.GetNewUnitOfWork<ITS.POS.Model.Master.Item>();
                return _sesMaster;
            }
        }

        private UnitOfWork Settings
        {
            get
            {
                if (_sesSettings == null)
                    _sesSettings = MemoryXpoHelper.GetNewUnitOfWork<ITS.POS.Model.Settings.DocumentSeries>();

                return _sesSettings;
            }
        }

        private UnitOfWork Transactions
        {
            get
            {
                if (_sesTrasactions == null || !_sesTrasactions.IsConnected)
                    _sesTrasactions = MemoryXpoHelper.GetNewUnitOfWork<ITS.POS.Model.Transactions.DocumentHeader>();
                return _sesTrasactions;
            }
        }

        public UnitOfWork MemorySettings
        {
            get
            {
                return Settings;
            }
        }

        private UnitOfWork Versions
        {
            get
            {
                if (_sesVersions == null)
                    _sesVersions = MemoryXpoHelper.GetNewUnitOfWork<ITS.POS.Model.Versions.TableVersions>();
                return _sesVersions;
            }
        }

        public UnitOfWork GetSession<T>() where T : BasicObj
        {
            Assembly TAssembly = typeof(T).Assembly;

            if (TAssembly == typeof(Item).Assembly)
            {
                return Master;
            }
            else if (TAssembly == typeof(BarcodeType).Assembly)
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

        public T FindObject<T>(CriteriaOperator criteria, bool selectDeleted) where T : BasicObj
        {
            UnitOfWork uow = GetSession<T>();
            T result = uow.FindObject<T>(criteria, selectDeleted);
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

            MemoryXpoHelper.ClearDataLayer<Item>();
        }
        public void ClearSettingsSession()
        {
            if (_sesSettings != null)
            {
                _sesSettings.Dispose();
                _sesSettings = null;
            }

            MemoryXpoHelper.ClearDataLayer<OwnerApplicationSettings>();
        }
        public void ClearTransactionsSession()
        {
            if (_sesTrasactions != null)
            {
                _sesTrasactions.Dispose();
                _sesTrasactions = null;
            }

            MemoryXpoHelper.ClearDataLayer<DocumentHeader>();
        }

        public void ClearTableVersionsSession()
        {

            if (_sesTrasactions != null)
            {
                _sesTrasactions.Dispose();
                _sesTrasactions = null;
            }

            MemoryXpoHelper.ClearDataLayer<TableVersions>();
        }


        public void ClearAllSessions()
        {
            ClearMasterSession();
            ClearSettingsSession();
            ClearTransactionsSession();
            ClearTableVersionsSession();
        }

        public void UpdateDatabase()
        {
            Assembly asm1 = Assembly.GetAssembly(typeof(Item));
            Master.UpdateSchema(asm1);
            Master.CreateObjectTypeRecords(asm1);
            Master.FlushChanges();
            Master.CommitChanges();

            Assembly asm2 = Assembly.GetAssembly(typeof(OwnerApplicationSettings));
            Settings.UpdateSchema(asm2);
            Settings.CreateObjectTypeRecords(asm2);
            Settings.FlushChanges();
            Settings.CommitChanges();

            Assembly asm3 = Assembly.GetAssembly(typeof(DocumentHeader));
            Transactions.UpdateSchema(asm3);
            Transactions.CreateObjectTypeRecords(asm3);
            Transactions.FlushChanges();
            Transactions.CommitChanges();

            Assembly asm4 = Assembly.GetAssembly(typeof(TableVersions));
            Versions.UpdateSchema(asm4);
            Versions.CreateObjectTypeRecords(asm4);
            Versions.FlushChanges();
            Versions.CommitChanges();

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

        public void FillDenormalizedFields(BasicObj obj)
        {

        }

        public void ReconnectToNewFile(IDataLayer fDataLayer, IEnumerable<IDisposable> _objectsToDisposeOnDisconnect)
        {

        }
    }
}
