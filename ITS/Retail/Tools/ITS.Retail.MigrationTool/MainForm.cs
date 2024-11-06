using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using ITS.Retail.Common;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.MigrationTool.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ITS.Retail.MigrationTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            if (File.Exists("Retail.StoreController.dll"))
            {
                storeControllerRadioButton.Checked = true;
            }

        }

        private readonly string filePath = Application.StartupPath + "\\..\\..\\Configuration\\config.xml";
        private readonly string serviceName = "RetailService.asmx";

        private DbCommand sqlCommand = null;
        private DbConnection sqlConnection;
        private LogHelper logger;

        protected WebConfigurationSettings Settings = new WebConfigurationSettings();

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text, database = txtDatabase.Text,
                db_username = txtDBUsername.Text, db_password = txtDBPassword.Text;
            switch (this.Settings.DatabaseType)
            {
                case DBType.SQLServer:
                    sqlConnection = new SqlConnection() { ConnectionString = string.Format("Server={0};Database={1};User Id={2};Password={3}", server, database, db_username, db_password) };
                    break;
                case DBType.postgres:
                    sqlConnection = new NpgsqlConnection() { ConnectionString = string.Format("Server={0};Port=5432;Database={1};User Id={2};Password={3}", server, database, db_username, db_password) };
                    break;
                case DBType.Oracle:
                    sqlConnection = new OracleConnection() { ConnectionString = string.Format(@"User Id={2};Password={3};Data Source=" + @" (DESCRIPTION = (ADDRESS = (PROTOCOL = tcp)(HOST = {0})(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME = {1})))", server, database, db_username, db_password) };
                    break;
            }

            object result;
            try
            {
                sqlConnection.Open();

                switch (this.Settings.DatabaseType)
                {
                    case DBType.SQLServer:
                        sqlCommand = new SqlCommand()
                        {
                            CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES"
                            //WHERE TABLE_SCHEMA = 'Retail' AND  TABLE_NAME = 'SchemaMigration'"
                        };
                        break;
                    case DBType.postgres:
                        sqlCommand = new NpgsqlCommand()
                        {
                            CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES"
                            //WHERE TABLE_SCHEMA = 'Retail' AND  TABLE_NAME = 'SchemaMigration'"
                        };
                        break;
                    case DBType.Oracle:
                        sqlCommand = new OracleCommand()
                        {
                            CommandText = "SELECT * FROM all_tables WHERE owner='SYS'"
                            //WHERE TABLE_SCHEMA = 'Retail' AND  TABLE_NAME = 'SchemaMigration'"
                        };
                        break;
                }
                sqlCommand.Connection = sqlConnection;
                result = sqlCommand.ExecuteScalar();
                string message = "Connection Test Passed!" + Environment.NewLine;
                logger.Success(message);
            }
            catch (Exception ex)
            {
                string error_message = ex.Message + Environment.NewLine;
                logger.Error(error_message);
                return;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public void InitializationRetailDual(eCultureInfo currentCulture, bool showForm = true)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                #region Company and Default Customer
                string ownerFirstName = null;
                string ownerLastName = null;
                string ownerTaxCode = null;
                TaxOffice ownerTaxOffice = null;
                string ownerStoreName = null;
                string ownerStoreCity = null;
                string ownerStoreStreet = null;
                string ownerCompanyName = null;
                string ownerStoreCode = null;

                CreateMinistryTaxOffices(uow, currentCulture);

                if (showForm)
                {
                    using (OwnerInfoForm ownerInfoForm = new OwnerInfoForm(true, uow)) //always request store info
                    {
                        if (ownerInfoForm.ShowDialog() == DialogResult.OK)
                        {
                            ownerFirstName = ownerInfoForm.FirstName;
                            ownerLastName = ownerInfoForm.LastName;
                            ownerTaxCode = ownerInfoForm.TaxCode;
                            ownerTaxOffice = ownerInfoForm.TaxOffice;
                            ownerStoreName = ownerInfoForm.StoreName;
                            ownerStoreCity = ownerInfoForm.StoreCity;
                            ownerStoreStreet = ownerInfoForm.StoreStreet;
                            ownerCompanyName = ownerInfoForm.OwnerCompanyName;
                            ownerStoreCode = ownerInfoForm.StoreCode;
                        }
                        else
                        {
                            throw new Exception("User Canceled.");
                        }
                    }
                }
                else
                {
                    ownerFirstName = "TDD";
                    ownerLastName = "TDD";
                    ownerTaxCode = "TDD";
                    ownerTaxOffice = null;
                    ownerStoreName = "TDD";
                    ownerStoreCity = "TDD";
                    ownerStoreStreet = "TDD";
                    ownerCompanyName = "TDD";
                    ownerStoreCode = "TDD";
                }

                CompanyNew owner = new CompanyNew(uow);
                owner.Trader = new Trader(uow);

                owner.CompanyName = ownerCompanyName;
                owner.Trader.FirstName = ownerFirstName;
                owner.Trader.LastName = ownerLastName;
                owner.Trader.TaxCode = ownerTaxCode;
                owner.Trader.TaxOfficeLookUp = ownerTaxOffice;
                owner.Code = "1";

                OwnerApplicationSettings oaps = new OwnerApplicationSettings(uow);
                oaps.Owner = owner;
                oaps.PadBarcodes = false;
                oaps.PadItemCodes = false;
                oaps.BarcodeLength = 14;
                oaps.ItemCodeLength = 7;
                oaps.BarcodePaddingCharacter = "0";
                oaps.ItemCodePaddingCharacter = "0";
                oaps.ComputeDigits = 2;
                oaps.DisplayDigits = 2;
                oaps.UseBarcodeRelationFactor = true;

                oaps.DisplayValueDigits = 4;


                Store store = new Store(uow);
                store.Address = new Address(uow);
                store.Name = ownerStoreName;
                store.Code = ownerStoreCode;
                store.Address.City = ownerStoreCity;
                store.Address.Street = ownerStoreStreet;
                store.IsCentralStore = true;
                store.Owner = owner;


                owner.Stores.Add(store);
                owner.Trader.Addresses.Add(store.Address);
                owner.DefaultAddress = store.Address;

                Customer customer = uow.FindObject<Customer>(new BinaryOperator("Trader.FirstName", "Default Customer"));
                Address defaultCustAddress = uow.FindObject<Address>(new BinaryOperator("Street", Resources.InitializationDefaultAddress));

                if (defaultCustAddress == null)
                {
                    defaultCustAddress = new Address(uow);
                    defaultCustAddress.Street = Resources.InitializationDefaultAddress;
                    defaultCustAddress.POBox = Resources.InitializationPOBox;
                    defaultCustAddress.PostCode = Resources.InitializationPostCode;
                }

                if (customer == null)
                {
                    customer = new Customer(uow);
                    customer.Trader = new Trader(uow);
                    customer.CompanyName = Resources.InitializationDemoCompanyName;
                    customer.CompanyBrandName = Resources.InitializationDemoCompanyName;
                    customer.Code = "1";
                    customer.Profession = Resources.InitializationProffesion;
                    customer.Trader.FirstName = Resources.InitializationDefaultCustomer;
                    customer.Trader.LastName = Resources.InitializationDefaultCustomer;
                    customer.Trader.TaxCode = "000000001";
                    customer.Trader.Addresses.Add(defaultCustAddress);
                    customer.DefaultAddress = defaultCustAddress;
                    customer.Owner = owner;
                }

                #endregion

                #region Roles and Permitions
                /*
                    0 -> No permissions
                    1 -> Only View
                    2 -> View and Edit
                */
                Dictionary<Dictionary<string, int>, string> UserPermissions = new Dictionary<Dictionary<string, int>, string>()
                {
                    {
                        new Dictionary<string, int>()
                        {
                            //Customer
                            {"Company", 0},
                            {"Customer", 0},
                            {"Supplier", 0},
                            {"Item", 1},
                            {"ItemBarcode", 1},
                            {"ItemCategory", 1},
                            {"CustomerCategory", 0},
                            {"PriceCatalog", 1},
                            {"Seasonality", 0},
                            {"Offer", 1},
                            {"Buyer", 0},
                            {"InformationSheet", 0},
                            {"AddressType", 0},
                            {"PhoneType", 0},
                            {"PaymentMethod", 0},
                            {"MeasurementUnit", 0},
                            {"BarcodeType", 0},
                            {"TerminalType", 0},
                            {"VatLevel", 0},
                            {"ApplicationSettings", 0},
                            {"DocumentType", 0},
                            {"DocumentSeries", 0},
                            {"StoreDocumentSeries", 0},
                            {"StoreDocumentSeriesType", 0},
                            {"DocumentStatus", 0},
                            {"Role", 0},
                            {"Tools", 0},
                            {"User", 0},
                            {"ApplicationLog", 0},
                            {"DataFileRecordHeader", 0},
                            {"TransformationRule", 0},
                            {"FormMessage", 0},
                            {"CustomReport", 0},
                            {"DocumentTypeCustomReport", 0},
                            {"StoreControllerSettings", 0},
                            {"CustomEnumeration", 0},
                            {"MesurmentUnits", 0},
                            {"TaxOffice", 0},
                            {"FiscalDevices", 0},
                            {"StoreDailyReport", 0},
                            {"PrintLabelSettings", 0},
                            {"CustomActionCode", 0},
                            {"Store", 0},
                            {"SpecialItem", 0},
                            {"DeficiencySettings", 0},
                            {"OwnerApplicationSettings", 0},
                            {"Division", 0},
                            {"DiscountType", 0},
                            {"TransferPurpose", 0},
                            {"ScannedDocumentHeader", 0},
                            {"ElectronicJournalFilePackage", 0},
                            {"POS", 0},
                            {"POSDevice", 0},
                            {"POSKeysLayout", 0},
                            {"POSLayout", 0},
                            {"POSPrintFormat", 0},
                            {"Labels", 0},
                            {"LabelDesign", 0},
                            {"POSActionLevelsSet", 0},
                            {"UpdaterMode", 0},
                            {"Scale", 0},
                            {"Promotion", 0},
                            {"SupplierImportFilesSet", 0},
                            {"SynchronizationInfo", 0},
                            {"ActionType",0 },
                            {"CustomDataView",0 },
                            {"CustomDataViewCategory",0 },
                            {"VariableValuesDisplay",0 },
                            {"Variable",0 }
                        }, Resources.Customer
                    },
                    {
                        new Dictionary<string, int>()
                        {
                            //CompanyAdmin
                            {"Company", 0},
                            {"ApplicationLog", 0},
                            {"ApplicationSettings", 0},
                        },Resources.CompanyAdmin
                    },
                    {
                        new Dictionary<string, int>()
                        {
                            //StoreManager
                            {"Company", 0},
                            {"Customer", 2},
                            {"Supplier", 2},
                            {"Item", 1},
                            {"ItemBarcode", 1},
                            {"ItemCategory", 1},
                            {"PriceCatalog", 1},
                            {"Seasonality", 1},
                            {"Offer", 1},
                            {"Buyer", 1},
                            {"AddressType", 1},
                            {"PhoneType", 1},
                            {"PaymentMethod", 1},
                            {"MeasurementUnit", 1},
                            {"BarcodeType", 1},
                            {"TerminalType", 0},
                            {"VatLevel", 0},
                            {"ApplicationSettings", 0},
                            {"DocumentType", 1},
                            {"DocumentSeries", 1},
                            {"DocumentStatus", 1},
                            {"Role", 1},
                            {"Tools", 0},
                            {"User", 0},
                            {"ApplicationLog", 0},
                            {"DataFileRecordHeader", 1},
                            {"TransformationRule", 1},
                            {"FormMessage", 1},
                            {"CustomReport", 1},
                            {"CustomEnumeration", 1},
                            {"MesurmentUnits", 1},
                            {"TaxOffice", 1},
                            {"Store", 1},
                            {"SpecialItem", 1},
                            {"DeficiencySettings", 1},
                            {"OwnerApplicationSettings", 1},
                            {"Division", 1},
                            {"DiscountType", 1},
                            {"LabelDesign", 1},
                            {"TransferPurpose", 1},
                            {"ScannedDocumentHeader", 1},
                            {"ElectronicJournalFilePackage", 0},
                            {"DocumentTypeCustomReport", 1},
                            {"Promotion", 1},
                            {"FiscalDevices", 1},
                            {"POS", 1},
                            {"POSDevice", 1},
                            {"POSKeysLayout", 1},
                            {"POSLayout", 1},
                            {"POSPrintFormat", 1},
                            {"Labels", 1},
                            {"PrintLabelSettings", 1},
                            {"POSActionLevelsSet", 1},
                            {"UpdaterMode", 1},
                            {"Scale", 1},
                            {"StoreDailyReport", 1},
                            {"CustomActionCode", 1},
                            {"CustomerCategory", 1},
                            {"ActionType",2},
                            {"CustomDataView",2},
                            {"CustomDataViewCategory",2},
                            {"VariableValuesDisplay",2},
                            {"Variable",2}
                        }, Resources.StoreManager
                    },
                    {
                        new Dictionary<string, int>()
                        {
                            //StoreEmployee
                            {"Company", 0},
                            {"Customer", 2},
                            {"Supplier", 2},
                            {"Item", 1},
                            {"ItemBarcode", 1},
                            {"ItemCategory", 1},
                            {"PriceCatalog", 1},
                            {"Seasonality", 1},
                            {"Offer", 1},
                            {"Buyer", 1},
                            {"AddressType", 0},
                            {"PhoneType", 0},
                            {"PaymentMethod", 0},
                            {"MeasurementUnit", 0},
                            {"BarcodeType", 0},
                            {"TerminalType", 0},
                            {"VatLevel", 0},
                            {"ApplicationSettings", 0},
                            {"DocumentType", 0},
                            {"DocumentSeries", 0},
                            {"DocumentStatus", 0},
                            {"Role", 0},
                            {"Tools", 0},
                            {"User", 0},
                            {"ApplicationLog", 0},
                            {"DataFileRecordHeader", 0},
                            {"TransformationRule", 0},
                            {"FormMessage", 0},
                            {"CustomReport", 0},
                            {"CustomEnumeration", 0},
                            {"MesurmentUnits", 0},
                            {"TaxOffice", 0},
                            {"Store", 0},
                            {"SpecialItem", 0},
                            {"DeficiencySettings", 0},
                            {"OwnerApplicationSettings", 0},
                            {"Division", 0},
                            {"DiscountType", 0},
                            {"LabelDesign", 0},
                            {"TransferPurpose", 0},
                            {"ScannedDocumentHeader", 1},
                            {"ElectronicJournalFilePackage", 0},
                            {"DocumentTypeCustomReport", 0},
                            {"Promotion", 1},
                            {"FiscalDevices", 0},
                            {"POS", 0},
                            {"POSDevice", 0},
                            {"POSKeysLayout", 0},
                            {"POSLayout", 0},
                            {"POSPrintFormat", 0},
                            {"Labels", 0},
                            {"PrintLabelSettings", 0},
                            {"POSActionLevelsSet", 0},
                            {"UpdaterMode", 0},
                            {"Scale", 0},
                            {"StoreDailyReport", 0},
                            {"CustomActionCode", 0},
                            {"CustomerCategory", 0},
                            {"SupplierImportFilesSet", 0},
                            {"ActionType", 0},
                            {"CustomDataView", 0},
                            {"CustomDataViewCategory", 0},
                            {"VariableValuesDisplay", 0},
                            {"Variable", 0}
                        }, Resources.StoreEmployee
                    }
                };

                //Admin Role
                Role roleAdmin = uow.FindObject<Role>(new BinaryOperator("Description", Resources.SystemAdmin));
                if (roleAdmin == null)
                {
                    roleAdmin = new Role(uow);
                    roleAdmin.Owner = null;//!!!
                    roleAdmin.Description = Resources.SystemAdmin;
                    roleAdmin.Type = eRoleType.SystemAdministrator;
                }

                //Customer Role
                Role roleCustomer = uow.FindObject<Role>(new BinaryOperator("Description", Resources.Customer));
                if (roleCustomer == null)
                {
                    roleCustomer = new Role(uow);
                    roleCustomer.Owner = owner;
                    roleCustomer.Type = eRoleType.Customer;
                    roleCustomer.Description = Resources.Customer;

                    foreach (KeyValuePair<string, int> pair in UserPermissions.Where(user => user.Value == roleCustomer.Description).FirstOrDefault().Key)
                    {
                        RoleEntityAccessPermision permission = new RoleEntityAccessPermision(uow);
                        permission.EnityAccessPermision = new EntityAccessPermision(uow);
                        permission.EnityAccessPermision.EntityType = pair.Key;
                        permission.EnityAccessPermision.Visible = pair.Value != 0;
                        permission.EnityAccessPermision.CanInsert = permission.EnityAccessPermision.CanUpdate = permission.EnityAccessPermision.CanDelete = pair.Value == 2;
                        roleCustomer.RoleEntityAccessPermisions.Add(permission);
                    }
                }

                //Company Admnin Role
                Role roleCompanyAdmin = uow.FindObject<Role>(new BinaryOperator("Description", Resources.CompanyAdmin));
                if (roleCompanyAdmin == null)
                {
                    roleCompanyAdmin = new Role(uow);
                    roleCompanyAdmin.Owner = owner;
                    roleCompanyAdmin.Description = Resources.CompanyAdmin;
                    roleCompanyAdmin.Type = eRoleType.CompanyAdministrator;

                    foreach (KeyValuePair<string, int> pair in UserPermissions.Where(user => user.Value == roleCompanyAdmin.Description).FirstOrDefault().Key)
                    {
                        RoleEntityAccessPermision permission = new RoleEntityAccessPermision(uow);
                        permission.EnityAccessPermision = new EntityAccessPermision(uow);
                        permission.EnityAccessPermision.EntityType = pair.Key;
                        permission.EnityAccessPermision.Visible = pair.Value != 0;
                        permission.EnityAccessPermision.CanInsert = permission.EnityAccessPermision.CanUpdate = permission.EnityAccessPermision.CanDelete = pair.Value == 2;
                        roleCompanyAdmin.RoleEntityAccessPermisions.Add(permission);
                    }
                }

                //Store Manager Role
                Role roleStoreManager = uow.FindObject<Role>(new BinaryOperator("Description", Resources.StoreManager));
                if (roleStoreManager == null)
                {
                    roleStoreManager = new Role(uow);
                    roleStoreManager.Owner = owner;
                    roleStoreManager.Description = Resources.StoreManager;
                    roleStoreManager.Type = eRoleType.CompanyUser;

                    foreach (KeyValuePair<string, int> pair in UserPermissions.Where(user => user.Value == roleStoreManager.Description).FirstOrDefault().Key)
                    {
                        RoleEntityAccessPermision permission = new RoleEntityAccessPermision(uow);
                        permission.EnityAccessPermision = new EntityAccessPermision(uow);
                        permission.EnityAccessPermision.EntityType = pair.Key;
                        permission.EnityAccessPermision.Visible = pair.Value != 0;
                        permission.EnityAccessPermision.CanInsert = permission.EnityAccessPermision.CanUpdate = permission.EnityAccessPermision.CanDelete = pair.Value == 2;
                        roleStoreManager.RoleEntityAccessPermisions.Add(permission);
                    }
                }

                //Store Employee Role
                Role roleStoreEmployee = uow.FindObject<Role>(new BinaryOperator("Description", Resources.StoreEmployee));
                if (roleStoreEmployee == null)
                {
                    roleStoreEmployee = new Role(uow);
                    roleStoreEmployee.Owner = owner;
                    roleStoreEmployee.Description = Resources.StoreEmployee;
                    roleStoreEmployee.Type = eRoleType.CompanyUser;

                    foreach (KeyValuePair<string, int> pair in UserPermissions.Where(user => user.Value == roleStoreEmployee.Description).FirstOrDefault().Key)
                    {
                        RoleEntityAccessPermision permission = new RoleEntityAccessPermision(uow);
                        permission.EnityAccessPermision = new EntityAccessPermision(uow);
                        permission.EnityAccessPermision.EntityType = pair.Key;
                        permission.EnityAccessPermision.Visible = pair.Value != 0;
                        permission.EnityAccessPermision.CanInsert = permission.EnityAccessPermision.CanUpdate = permission.EnityAccessPermision.CanDelete = pair.Value == 2;
                        roleStoreEmployee.RoleEntityAccessPermisions.Add(permission);
                    }
                }
                #endregion

                #region Default Users
                //default user customer
                User userCustomer = uow.FindObject<User>(new BinaryOperator("UserName", "customer"));
                if (userCustomer == null)
                {
                    userCustomer = new User(uow);
                    userCustomer.UserName = "customer";
                    userCustomer.Password = UserHelper.EncodePassword("customer");
                    //userCustomer.TaxCode = customer.Trader.TaxCode;
                    userCustomer.FullName = "Demo Customer";
                    userCustomer.IsApproved = true;
                    UserTypeAccess utaCustomer = new UserTypeAccess(uow);
                    utaCustomer.User = userCustomer;
                    utaCustomer.EntityOid = customer.Oid;
                    utaCustomer.EntityType = customer.GetType().ToString();
                    roleCustomer.Users.Add(userCustomer);
                }

                //default user supplier
                User userCompAdmin = uow.FindObject<User>(new BinaryOperator("UserName", "compAdmin"));
                if (userCompAdmin == null)
                {
                    userCompAdmin = new User(uow);
                    userCompAdmin.UserName = "compAdmin";
                    userCompAdmin.Password = UserHelper.EncodePassword("compAdmin");
                    //userCompAdmin.TaxCode = owner.Trader.TaxCode;
                    userCompAdmin.FullName = "Company Administrator";
                    userCompAdmin.IsApproved = true;
                    UserTypeAccess utaSupplier = new UserTypeAccess(uow);
                    utaSupplier.User = userCompAdmin;
                    utaSupplier.EntityOid = owner.Oid;
                    utaSupplier.EntityType = owner.GetType().ToString();
                    UserTypeAccess utaSupplierStore = new UserTypeAccess(uow);
                    utaSupplierStore.User = userCompAdmin;
                    utaSupplierStore.EntityOid = store.Oid;
                    utaSupplierStore.EntityType = store.GetType().ToString();
                    roleCompanyAdmin.Users.Add(userCompAdmin);
                }

                //default user admin
                User userAdmin = uow.FindObject<User>(new BinaryOperator("UserName", "admin"));
                if (userAdmin == null)
                {
                    userAdmin = new User(uow);
                    userAdmin.UserName = "admin";
                    userAdmin.Password = UserHelper.EncodePassword("admin");
                    userAdmin.FullName = "System Administrator";
                    userAdmin.TermsAccepted = true;
                    userAdmin.IsApproved = true;
                    roleAdmin.Users.Add(userAdmin);
                }

                //default pos user
                User posuser = uow.FindObject<User>(new BinaryOperator("UserName", "posuser"));
                if (posuser == null)
                {
                    posuser = new User(uow);
                    posuser.UserName = "posuser";
                    posuser.Password = UserHelper.EncodePassword("posuser");
                    //posuser.TaxCode = owner.Trader.TaxCode;
                    posuser.FullName = "Demo POS User";
                    posuser.IsApproved = true;
                    UserTypeAccess utaSupplier = new UserTypeAccess(uow);
                    utaSupplier.User = posuser;
                    utaSupplier.EntityOid = owner.Oid;
                    utaSupplier.EntityType = owner.GetType().ToString();
                    UserTypeAccess utaSupplierStore = new UserTypeAccess(uow);
                    utaSupplierStore.User = posuser;
                    utaSupplierStore.EntityOid = store.Oid;
                    utaSupplierStore.EntityType = store.GetType().ToString();

                    roleStoreEmployee.Users.Add(posuser);

                    posuser.POSUserName = "1234";
                    posuser.POSPassword = "1234";
                    posuser.POSUserLevel = ePOSUserLevel.LEVEL0;
                }

                User posadmin = uow.FindObject<User>(new BinaryOperator("UserName", "posadmin"));
                if (posadmin == null)
                {
                    posadmin = new User(uow);
                    posadmin.UserName = "posadmin";
                    posadmin.Password = UserHelper.EncodePassword("posadmin");
                    //posadmin.TaxCode = owner.Trader.TaxCode;
                    posadmin.FullName = "Demo POS Admin";
                    posadmin.IsApproved = true;
                    UserTypeAccess utaSupplier = new UserTypeAccess(uow);
                    utaSupplier.User = posadmin;
                    utaSupplier.EntityOid = owner.Oid;
                    utaSupplier.EntityType = owner.GetType().ToString();
                    UserTypeAccess utaSupplierStore = new UserTypeAccess(uow);
                    utaSupplierStore.User = posadmin;
                    utaSupplierStore.EntityOid = store.Oid;
                    utaSupplierStore.EntityType = store.GetType().ToString();

                    roleStoreManager.Users.Add(posadmin);

                    posadmin.POSUserName = "5678";
                    posadmin.POSPassword = "5678";
                    posadmin.POSUserLevel = ePOSUserLevel.LEVEL4;
                }

                #endregion

                #region Initial Basic data
                //Vatlevels
                VatLevel vatlevelnormal = uow.FindObject<VatLevel>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.InitializationNormalVatLevel)));
                if (vatlevelnormal == null)
                {
                    vatlevelnormal = new VatLevel(uow);
                    vatlevelnormal.Owner = owner;
                    vatlevelnormal.Code = "1";
                    vatlevelnormal.Description = Resources.InitializationNormalVatLevel;
                    vatlevelnormal.IsDefault = true;
                }
                VatLevel vatlevelreduced = uow.FindObject<VatLevel>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.InitializationReducedVatLevel)));
                if (vatlevelreduced == null)
                {
                    vatlevelreduced = new VatLevel(uow);
                    vatlevelreduced.Owner = owner;
                    vatlevelreduced.Code = "2";
                    vatlevelreduced.Description = Resources.InitializationReducedVatLevel;
                }
                customer.VatLevel = vatlevelnormal;
                store.Address.VatLevel = vatlevelnormal;

                //Address Types
                AddressType addressType = uow.FindObject<AddressType>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.CentralStore)));
                if (addressType == null)
                {
                    addressType = new AddressType(uow);
                    addressType.Code = "1";
                    addressType.Description = Resources.CentralStore;
                    addressType.IsDefault = true;
                    addressType.Owner = owner;
                }

                AddressType addressType2 = uow.FindObject<AddressType>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.Branch)));
                if (addressType2 == null)
                {
                    addressType2 = new AddressType(uow);
                    addressType2.Code = "2";
                    addressType2.Description = Resources.Branch;
                    addressType2.Owner = owner;
                }

                store.Address.AddressType = addressType2;

                //Payment Methods
                PaymentMethod paymentMethod = uow.FindObject<PaymentMethod>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.Cash)));
                if (paymentMethod == null)
                {
                    paymentMethod = new PaymentMethod(uow);
                    paymentMethod.Owner = owner;
                    paymentMethod.Code = "1";
                    paymentMethod.Description = Resources.Cash;
                    paymentMethod.IsDefault = true;
                    paymentMethod.GiveChange = true;
                    paymentMethod.CanExceedTotal = true;
                    paymentMethod.OpensDrawer = true;
                    paymentMethod.PaymentMethodType = ePaymentMethodType.CASH;
                    paymentMethod.IncreasesDrawerAmount = true;
                }

                PaymentMethod paymentMethod2 = uow.FindObject<PaymentMethod>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.Credit)));
                if (paymentMethod2 == null)
                {
                    paymentMethod2 = new PaymentMethod(uow);
                    paymentMethod2.Code = "2";
                    paymentMethod2.Description = Resources.Credit;
                    paymentMethod2.Owner = owner;
                    paymentMethod2.CanExceedTotal = false;
                    paymentMethod2.PaymentMethodType = ePaymentMethodType.CREDIT;
                }

                //Measurement Units
                MeasurementUnit mesurementUnit = uow.FindObject<MeasurementUnit>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.Parts)));
                if (mesurementUnit == null)
                {
                    mesurementUnit = new MeasurementUnit(uow);
                    mesurementUnit.Code = "1";
                    mesurementUnit.Description = Resources.Parts;
                    mesurementUnit.IsDefault = true;
                    mesurementUnit.Owner = owner;
                }

                //Discount Types
                DiscountType DiscountType1 = uow.FindObject<DiscountType>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.PercentageOfTotal)));
                if (DiscountType1 == null)
                {
                    DiscountType1 = new DiscountType(uow);
                    DiscountType1.Code = "1";
                    DiscountType1.Description = Resources.PercentageOfTotal;
                    DiscountType1.eDiscountType = eDiscountType.PERCENTAGE;
                    DiscountType1.Priority = 1;
                    DiscountType1.IsHeaderDiscount = true;
                    DiscountType1.Owner = owner;
                }

                DiscountType DiscountType2 = uow.FindObject<DiscountType>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.PercentageOfDetail)));
                if (DiscountType2 == null)
                {
                    DiscountType2 = new DiscountType(uow);
                    DiscountType2.Code = "2";
                    DiscountType2.Description = Resources.PercentageOfDetail;
                    DiscountType2.eDiscountType = eDiscountType.PERCENTAGE;
                    DiscountType2.Priority = 0;
                    DiscountType2.Owner = owner;
                }

                DiscountType DiscountType3 = uow.FindObject<DiscountType>(CriteriaOperator.Or(new BinaryOperator("Code", "3"), new BinaryOperator("Description", Resources.IsOfValuesTotal)));
                if (DiscountType3 == null)
                {
                    DiscountType3 = new DiscountType(uow);
                    DiscountType3.Code = "3";
                    DiscountType3.Description = Resources.IsOfValuesTotal;
                    DiscountType3.eDiscountType = eDiscountType.VALUE;
                    DiscountType3.Priority = 1;
                    DiscountType3.IsHeaderDiscount = true;
                    DiscountType3.Owner = owner;
                }

                DiscountType DiscountType4 = uow.FindObject<DiscountType>(CriteriaOperator.Or(new BinaryOperator("Code", "4"), new BinaryOperator("Description", Resources.IsOfValuesDetail)));
                if (DiscountType4 == null)
                {
                    DiscountType4 = new DiscountType(uow);
                    DiscountType4.Code = "4";
                    DiscountType4.Description = Resources.IsOfValuesDetail;
                    DiscountType4.eDiscountType = eDiscountType.VALUE;
                    DiscountType4.Priority = 0;
                    DiscountType4.Owner = owner;
                }

                //Barcode Types
                BarcodeType barcodeType = uow.FindObject<BarcodeType>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.Barcode)));
                if (barcodeType == null)
                {
                    barcodeType = new BarcodeType(uow);
                    barcodeType.Code = "1";
                    barcodeType.Description = Resources.Barcode;
                    barcodeType.IsDefault = true;
                    barcodeType.Owner = owner;
                }

                //Barcode Types
                BarcodeType barcodeType2 = uow.FindObject<BarcodeType>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.SupplierCode)));
                if (barcodeType == null)
                {
                    barcodeType2 = new BarcodeType(uow);
                    barcodeType2.Code = "2";
                    barcodeType2.Description = Resources.SupplierCode;
                    barcodeType2.Owner = owner;
                }

                //phoneTypes
                PhoneType phoneTypeLandLine = uow.FindObject<PhoneType>(CriteriaOperator.Or(new BinaryOperator("Code", "1"), new BinaryOperator("Description", Resources.Landline)));
                if (phoneTypeLandLine == null)
                {
                    phoneTypeLandLine = new PhoneType(uow);
                    phoneTypeLandLine.Code = "1";
                    phoneTypeLandLine.Description = Resources.Landline;
                    phoneTypeLandLine.Owner = owner;
                }
                PhoneType phoneTypeCellphone = uow.FindObject<PhoneType>(CriteriaOperator.Or(new BinaryOperator("Code", "2"), new BinaryOperator("Description", Resources.Cellphone)));
                if (phoneTypeCellphone == null)
                {
                    phoneTypeCellphone = new PhoneType(uow);
                    phoneTypeCellphone.Code = "2";
                    phoneTypeCellphone.Description = Resources.Cellphone;
                    phoneTypeCellphone.Owner = owner;
                }

                PhoneType phoneTypeFax = uow.FindObject<PhoneType>(CriteriaOperator.Or(new BinaryOperator("Code", "3"), new BinaryOperator("Description", Resources.Fax)));
                if (phoneTypeFax == null)
                {
                    phoneTypeFax = new PhoneType(uow);
                    phoneTypeFax.Code = "3";
                    phoneTypeFax.Description = Resources.Fax;
                    phoneTypeFax.Owner = owner;
                }

                //vatcategories
                VatCategory vatcategory1 = uow.FindObject<VatCategory>(new BinaryOperator("Code", "6"));
                if (vatcategory1 == null)
                {
                    vatcategory1 = new VatCategory(uow);
                    vatcategory1.Description = Resources.VAT + " 6%";
                    vatcategory1.Code = "6";
                    vatcategory1.Owner = owner;
                    vatcategory1.MinistryVatCategoryCode = eMinistryVatCategoryCode.A;
                }

                VatCategory vatcategory2 = uow.FindObject<VatCategory>(new BinaryOperator("Code", "13"));
                if (vatcategory2 == null)
                {
                    vatcategory2 = new VatCategory(uow);
                    vatcategory2.Description = Resources.VAT + " 13%";
                    vatcategory2.Code = "13";
                    vatcategory2.Owner = owner;
                    vatcategory2.MinistryVatCategoryCode = eMinistryVatCategoryCode.B;
                }
                VatCategory vatcategory3 = uow.FindObject<VatCategory>(new BinaryOperator("Code", "24"));
                if (vatcategory3 == null)
                {
                    vatcategory3 = new VatCategory(uow);
                    vatcategory3.Description = Resources.VAT + " 24%";
                    vatcategory3.Code = "24";
                    vatcategory3.IsDefault = true;
                    vatcategory3.Owner = owner;
                    vatcategory3.MinistryVatCategoryCode = eMinistryVatCategoryCode.C;
                }
                VatCategory vatcategory4 = uow.FindObject<VatCategory>(new BinaryOperator("Code", "0"));
                if (vatcategory4 == null)
                {
                    vatcategory4 = new VatCategory(uow);
                    vatcategory4.Description = Resources.VAT + " 0%";
                    vatcategory4.Code = "0";
                    vatcategory4.Owner = owner;
                    vatcategory4.MinistryVatCategoryCode = eMinistryVatCategoryCode.E;
                }

                VatFactor vf0 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", .0));
                VatFactor vf1 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.06));
                VatFactor vf2 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.04));
                VatFactor vf3 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.13));
                VatFactor vf4 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.09));
                VatFactor vf5 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.24));
                VatFactor vf6 = uow.FindObject<VatFactor>(new BinaryOperator("Factor", 0.17));

                if (vf0 == null)
                {
                    vf0 = new VatFactor(uow);
                    vf0.VatCategory = vatcategory4;
                    vf0.VatLevel = vatlevelnormal;
                    vf0.Code = "0";
                    vf0.Factor = .0m;
                    vf0.Owner = owner;
                }
                if (vf1 == null)
                {
                    vf1 = new VatFactor(uow);
                    vf1.VatCategory = vatcategory1;
                    vf1.VatLevel = vatlevelnormal;
                    vf1.Code = "1";
                    vf1.Factor = 0.06m;
                    vf1.Owner = owner;
                }
                if (vf2 == null)
                {
                    vf2 = new VatFactor(uow);
                    vf2.VatCategory = vatcategory1;
                    vf2.VatLevel = vatlevelreduced;
                    vf2.Code = "2";
                    vf2.Factor = 0.04m;
                    vf2.Owner = owner;
                }
                if (vf3 == null)
                {
                    vf3 = new VatFactor(uow);
                    vf3.VatCategory = vatcategory2;
                    vf3.VatLevel = vatlevelnormal;
                    vf3.Code = "3";
                    vf3.Factor = 0.13m;
                    vf3.Owner = owner;
                }
                if (vf4 == null)
                {
                    vf4 = new VatFactor(uow);
                    vf4.VatCategory = vatcategory2;
                    vf4.VatLevel = vatlevelreduced;
                    vf4.Code = "4";
                    vf4.Factor = 0.09m;
                    vf4.Owner = owner;
                }
                if (vf5 == null)
                {
                    vf5 = new VatFactor(uow);
                    vf5.VatCategory = vatcategory3;
                    vf5.VatLevel = vatlevelnormal;
                    vf5.Code = "5";
                    vf5.Factor = 0.24m;
                    vf5.Owner = owner;
                }
                if (vf6 == null)
                {
                    vf6 = new VatFactor(uow);
                    vf6.VatCategory = vatcategory3;
                    vf6.VatLevel = vatlevelreduced;
                    vf6.Code = "6";
                    vf6.Factor = 0.17m;
                    vf6.Owner = owner;
                }
                ///Document Statuses
                DocumentStatus docstat1 = uow.FindObject<DocumentStatus>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Opened), new BinaryOperator("Code", "1")));
                DocumentStatus docstat2 = uow.FindObject<DocumentStatus>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Closed), new BinaryOperator("Code", "2")));
                if (docstat1 == null)
                {
                    docstat1 = new DocumentStatus(uow);
                    docstat1.Code = "1";
                    docstat1.Description = Resources.Opened;
                    docstat1.TakeSequence = false;
                    docstat1.IsDefault = true;
                    docstat1.Owner = owner;
                }
                if (docstat2 == null)
                {
                    docstat2 = new DocumentStatus(uow);
                    docstat2.Code = "2";
                    docstat2.Description = Resources.Closed;
                    docstat2.TakeSequence = true;
                    docstat2.Owner = owner;
                }

                ///Document Series
                DocumentSeries series1 = uow.FindObject<DocumentSeries>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Series + " 1"), new BinaryOperator("Code", "1")));
                if (series1 == null)
                {
                    series1 = new DocumentSeries(uow);
                    series1.Code = "1";
                    series1.Description = Resources.Series + " 1";
                    series1.IsDefault = true;
                    series1.Owner = owner;
                    series1.HasAutomaticNumbering = true;
                }

                DocumentSeries series2 = uow.FindObject<DocumentSeries>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Series + " 2"), new BinaryOperator("Code", "2")));
                if (series2 == null)
                {
                    series2 = new DocumentSeries(uow);
                    series2.Code = "2";
                    series2.Description = Resources.Series + " 2";
                    series2.IsDefault = false;
                    series2.Owner = owner;
                    series2.HasAutomaticNumbering = true;
                }

                ///Ministry Document Types
                CreateMinistryDocumentTypes(uow, currentCulture);


                ///Divisions
                Division divisionStore = new Division(uow);
                divisionStore.Description = Resources.Storage;
                divisionStore.Section = eDivision.Store;


                Division divisionPurchases = new Division(uow);
                divisionPurchases.Description = Resources.Purchases;
                divisionPurchases.Section = eDivision.Purchase;

                Division divisionSales = new Division(uow);
                divisionSales.Description = Resources.Sales;
                divisionSales.Section = eDivision.Sales;

                ///Document Types
                DocumentType doctypeOrder = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Order), new BinaryOperator("Code", "1")));
                if (doctypeOrder == null)
                {
                    doctypeOrder = new DocumentType(uow);
                    doctypeOrder.Code = "1";
                    doctypeOrder.DefaultDocumentStatus = docstat1;
                    doctypeOrder.Description = Resources.Order;
                    doctypeOrder.IsDefault = true;
                    doctypeOrder.UsesPrices = true;
                    doctypeOrder.IsForWholesale = true;
                    doctypeOrder.QuantityFactor = 1;
                    doctypeOrder.ValueFactor = 1;
                    doctypeOrder.Owner = owner;
                    doctypeOrder.Division = divisionSales;
                    switch (currentCulture)  //todo Add specific for each country
                    {
                        default:
                            doctypeOrder.MinistryDocumentType = uow.FindObject<MinistryDocumentType>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Code", "188"));
                            break;
                    }
                }

                DocumentType doctypeReceipt = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Receipt), new BinaryOperator("Code", "2")));
                if (doctypeReceipt == null)
                {
                    doctypeReceipt = new DocumentType(uow);
                    doctypeReceipt.Code = "2";
                    doctypeReceipt.DefaultDocumentStatus = docstat1;
                    doctypeReceipt.Description = Resources.Receipt;
                    doctypeReceipt.UsesPrices = true;
                    doctypeReceipt.TakesDigitalSignature = true;
                    doctypeReceipt.QuantityFactor = 1;
                    doctypeReceipt.ValueFactor = 1;
                    doctypeReceipt.Owner = owner;
                    doctypeReceipt.Division = divisionSales;
                    switch (currentCulture)  //todo Add specific for each country
                    {
                        default:
                            doctypeReceipt.MinistryDocumentType = uow.FindObject<MinistryDocumentType>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Code", "173"));
                            break;
                    }
                }

                DocumentType doctypeDeposit = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Deposit), new BinaryOperator("Code", "3")));
                if (doctypeDeposit == null)
                {
                    doctypeDeposit = new DocumentType(uow);
                    doctypeDeposit.Code = "3";
                    doctypeDeposit.DefaultDocumentStatus = docstat1;
                    doctypeDeposit.Description = Resources.Deposit;
                    doctypeDeposit.UsesPrices = true;
                    doctypeDeposit.QuantityFactor = 1;
                    doctypeDeposit.ValueFactor = 1;
                    doctypeDeposit.Owner = owner;
                    doctypeDeposit.Division = divisionStore;
                }

                DocumentType doctypeWithdrawal = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Withdrawal), new BinaryOperator("Code", "4")));
                if (doctypeWithdrawal == null)
                {
                    doctypeWithdrawal = new DocumentType(uow);
                    doctypeWithdrawal.Code = "4";
                    doctypeWithdrawal.DefaultDocumentStatus = docstat1;
                    doctypeWithdrawal.Description = Resources.Withdrawal;
                    doctypeWithdrawal.UsesPrices = true;
                    doctypeWithdrawal.QuantityFactor = 1;
                    doctypeWithdrawal.ValueFactor = -1;
                    doctypeWithdrawal.Owner = owner;
                    doctypeWithdrawal.Division = divisionStore;
                }

                DocumentType doctypeProforma = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.ProformaInvoice), new BinaryOperator("Code", "5")));
                if (doctypeProforma == null)
                {
                    doctypeProforma = new DocumentType(uow);
                    doctypeProforma.Code = "5";
                    doctypeProforma.DefaultDocumentStatus = docstat1;
                    doctypeProforma.Description = Resources.ProformaInvoice;
                    doctypeProforma.UsesPrices = true;
                    doctypeProforma.QuantityFactor = 1;
                    doctypeProforma.ValueFactor = 1;
                    doctypeProforma.Owner = owner;
                    doctypeProforma.Division = divisionSales;
                }

                DocumentType doctypeInvoice = uow.FindObject<DocumentType>(CriteriaOperator.Or(new BinaryOperator("Description", Resources.Invoice), new BinaryOperator("Code", "6")));
                if (doctypeInvoice == null)
                {
                    doctypeInvoice = new DocumentType(uow);
                    doctypeInvoice.Code = "6";
                    doctypeInvoice.DefaultDocumentStatus = docstat1;
                    doctypeInvoice.Description = Resources.Invoice;
                    doctypeInvoice.UsesPrices = true;
                    doctypeInvoice.TakesDigitalSignature = true;
                    doctypeInvoice.IsForWholesale = true;
                    doctypeInvoice.QuantityFactor = 1;
                    doctypeInvoice.ValueFactor = 1;
                    doctypeInvoice.Owner = owner;
                    doctypeInvoice.Division = divisionSales;
                    switch (currentCulture)  //todo Add specific for each country
                    {
                        default:
                            doctypeInvoice.MinistryDocumentType = uow.FindObject<MinistryDocumentType>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Code", "165"));
                            break;
                    }
                }

                ///Store Document Type Series
                series1.Store = store;
                StoreDocumentSeriesType sdst = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentSeries.Oid", series1.Oid),
                                                                                                            new BinaryOperator("DocumentType.Oid", doctypeOrder.Oid)));
                if (sdst == null)
                {
                    sdst = new StoreDocumentSeriesType(uow);
                    sdst.DocumentSeries = series1;
                    sdst.DocumentType = doctypeOrder;
                }

                series2.Store = store;
                StoreDocumentSeriesType sdst2 = uow.FindObject<StoreDocumentSeriesType>(CriteriaOperator.And(new BinaryOperator("DocumentSeries.Oid", series2.Oid),
                                                                                    new BinaryOperator("DocumentType.Oid", doctypeInvoice.Oid)));
                if (sdst2 == null)
                {
                    sdst2 = new StoreDocumentSeriesType(uow);
                    sdst2.DocumentSeries = series2;
                    sdst2.DocumentType = doctypeInvoice;

                }

                ///Price Catalogs
                PriceCatalog pc = uow.FindObject<PriceCatalog>(new BinaryOperator("Code", "1"));
                if (pc == null)
                {
                    pc = new PriceCatalog(uow);
                    pc.Code = "1";
                    pc.Description = Resources.DefaultPriceCatalog;
                    pc.Owner = owner;
                    pc.StartDate = DateTime.Now.AddYears(-10);
                    pc.EndDate = DateTime.Now.AddYears(10);
                }

                ///Store Price Lists
                StorePriceList spl = uow.FindObject<StorePriceList>(CriteriaOperator.And(new BinaryOperator("PriceList.Oid", pc.Oid), new BinaryOperator("Store.Oid", store.Oid)));
                if (spl == null)
                {
                    spl = new StorePriceList(uow);
                    spl.PriceList = pc;
                    spl.Store = store;
                }

                ///Customer Price Lists
                CustomerStorePriceList cspl = uow.FindObject<CustomerStorePriceList>(CriteriaOperator.And(new BinaryOperator("Customer.Oid", customer.Oid), new BinaryOperator("StorePriceList.Oid", spl.Oid)));
                if (cspl == null)
                {
                    cspl = new CustomerStorePriceList(uow);
                    cspl.Customer = customer;
                    cspl.StorePriceList = spl;
                    cspl.IsDefault = true;
                }

                ///Transformation Rules
                TransformationRule trans1 = new TransformationRule(uow);
                trans1.InitialType = doctypeProforma;
                trans1.DerrivedType = doctypeInvoice;
                trans1.QtyTransformationFactor = 1;
                trans1.ValueTransformationFactor = 1;

                TransformationRule trans2 = new TransformationRule(uow);
                trans2.InitialType = doctypeOrder;
                trans2.DerrivedType = doctypeReceipt;
                trans2.QtyTransformationFactor = 1;
                trans2.ValueTransformationFactor = 1;

                TransformationRule trans3 = new TransformationRule(uow);
                trans3.InitialType = doctypeOrder;
                trans3.DerrivedType = doctypeInvoice;
                trans3.QtyTransformationFactor = 1;
                trans3.ValueTransformationFactor = 1;

                //Deposit Item
                SpecialItem depositItem, withdrawalItem;
                depositItem = uow.FindObject<SpecialItem>(new BinaryOperator("Description", Resources.Deposit));
                withdrawalItem = uow.FindObject<SpecialItem>(new BinaryOperator("Description", Resources.Withdrawal));

                if (depositItem == null)
                {
                    depositItem = new SpecialItem(uow);
                    depositItem.Code = "1";
                    depositItem.Description = Resources.Deposit;
                    depositItem.Owner = owner;
                }
                if (withdrawalItem == null)
                {
                    withdrawalItem = new SpecialItem(uow);
                    withdrawalItem.Code = "2";
                    withdrawalItem.Description = Resources.Withdrawal;
                    withdrawalItem.Owner = owner;
                }

                try
                {
                    string reportsDirectory = Application.StartupPath + "\\Reports\\" + currentCulture.GetDescription();
                    if (Directory.Exists(reportsDirectory))
                    {
                        string[] dirPaths = Directory.GetDirectories(reportsDirectory);
                        int index = 1;
                        ///Reports Categories
                        foreach (String dirpath in dirPaths)
                        {
                            ReportCategory reportCategory = new ReportCategory(uow);
                            reportCategory.Owner = owner;
                            reportCategory.Description = Path.GetFileName(dirpath);
                            reportCategory.Save();
                            //CustomReports
                            string[] filePaths = Directory.GetFiles(dirpath);
                            foreach (string filepath in filePaths)
                            {
                                CreateReport(filepath, uow, owner, index.ToString(), reportCategory).Save();
                                index++;
                            }
                        }
                        string[] filePathsNoCat = Directory.GetFiles(reportsDirectory);
                        foreach (string filepath in filePathsNoCat)
                        {
                            CreateReport(filepath, uow, owner, index.ToString(), null).Save();
                            index++;
                        }
                    }
                    else
                    {
                        logger.Error("CustomReports: No source directory was found!" + reportsDirectory + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("CustomReports Error: " + ex.Message + Environment.NewLine);
                }

                #endregion

                #region Default Import Settings

                DataFileRecordHeader itemCatRecordHeader = new DataFileRecordHeader(uow);
                itemCatRecordHeader.EntityName = typeof(ItemCategory).Name;
                itemCatRecordHeader.HeaderCode = "02";
                itemCatRecordHeader.Order = 0;
                itemCatRecordHeader.IsTabDelimited = true;
                itemCatRecordHeader.TabDelimitedString = "\\t";
                itemCatRecordHeader.Position = 0;
                itemCatRecordHeader.Length = 2;
                itemCatRecordHeader.Owner = owner;

                DataFileRecordDetail icd1 = new DataFileRecordDetail(uow) { PropertyName = "Level1", Position = 1, Trim = true, Header = itemCatRecordHeader };
                DataFileRecordDetail icd2 = new DataFileRecordDetail(uow) { PropertyName = "Level2", Position = 2, Trim = true, Header = itemCatRecordHeader };
                DataFileRecordDetail icd3 = new DataFileRecordDetail(uow) { PropertyName = "Level3", Position = 3, Trim = true, Header = itemCatRecordHeader };
                DataFileRecordDetail icd4 = new DataFileRecordDetail(uow) { PropertyName = "Level4", Position = 4, Trim = true, Header = itemCatRecordHeader };
                DataFileRecordDetail icd5 = new DataFileRecordDetail(uow) { PropertyName = "Description", Position = 5, Trim = true, Header = itemCatRecordHeader };
                DataFileRecordDetail icd6 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 6, Trim = true, Header = itemCatRecordHeader };

                DataFileRecordHeader supplierRecordHeader = new DataFileRecordHeader(uow);
                supplierRecordHeader.EntityName = typeof(CompanyNew).Name;
                supplierRecordHeader.HeaderCode = "03";
                supplierRecordHeader.Order = 1;
                supplierRecordHeader.IsTabDelimited = true;
                supplierRecordHeader.TabDelimitedString = "\\t";
                supplierRecordHeader.Position = 0;
                supplierRecordHeader.Length = 2;
                supplierRecordHeader.Owner = owner;

                DataFileRecordDetail supD1 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 1, Trim = true, Header = supplierRecordHeader };
                DataFileRecordDetail supD2 = new DataFileRecordDetail(uow) { PropertyName = "CompanyName", Position = 2, Trim = true, Header = supplierRecordHeader };
                DataFileRecordDetail supD3 = new DataFileRecordDetail(uow) { PropertyName = "Address", Position = 3, Trim = true, Header = supplierRecordHeader };
                DataFileRecordDetail supD4 = new DataFileRecordDetail(uow) { PropertyName = "Profession", Position = 4, Trim = true, Header = supplierRecordHeader };
                DataFileRecordDetail supD5 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 5, Trim = true, Header = supplierRecordHeader };

                DataFileRecordHeader pcRecordHeader = new DataFileRecordHeader(uow);
                pcRecordHeader.EntityName = typeof(PriceCatalog).Name;
                pcRecordHeader.HeaderCode = "07";
                pcRecordHeader.Order = 2;
                pcRecordHeader.IsTabDelimited = true;
                pcRecordHeader.TabDelimitedString = "\\t";
                pcRecordHeader.Position = 0;
                pcRecordHeader.Length = 2;
                pcRecordHeader.Owner = owner;

                DataFileRecordDetail pcD1 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 1, Trim = true, Header = pcRecordHeader };
                DataFileRecordDetail pcD2 = new DataFileRecordDetail(uow) { PropertyName = "Description", Position = 2, Trim = true, Header = pcRecordHeader };
                DataFileRecordDetail pcD3 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 3, Trim = true, Header = pcRecordHeader };
                DataFileRecordDetail pcD4 = new DataFileRecordDetail(uow) { PropertyName = "PriceCatalog", Position = 4, Trim = true, Header = pcRecordHeader };

                DataFileRecordHeader storeRecordHeader = new DataFileRecordHeader(uow);
                storeRecordHeader.EntityName = typeof(Store).Name;
                storeRecordHeader.HeaderCode = "01";
                storeRecordHeader.Order = 3;
                storeRecordHeader.IsTabDelimited = true;
                storeRecordHeader.TabDelimitedString = "\\t";
                storeRecordHeader.Position = 0;
                storeRecordHeader.Length = 2;
                storeRecordHeader.Owner = owner;

                DataFileRecordDetail stD1 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 1, Trim = true, Header = storeRecordHeader };
                DataFileRecordDetail stD2 = new DataFileRecordDetail(uow) { PropertyName = "Description", Position = 2, Trim = true, Header = storeRecordHeader };
                DataFileRecordDetail stD3 = new DataFileRecordDetail(uow) { PropertyName = "City", Position = 3, Trim = true, Header = storeRecordHeader };
                DataFileRecordDetail stD4 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 4, Trim = true, Header = storeRecordHeader };


                DataFileRecordHeader itemRecordHeader = new DataFileRecordHeader(uow);
                itemRecordHeader.EntityName = typeof(Item).Name;
                itemRecordHeader.HeaderCode = "04";
                itemRecordHeader.Order = 4;
                itemRecordHeader.IsTabDelimited = true;
                itemRecordHeader.TabDelimitedString = "\\t";
                itemRecordHeader.Position = 0;
                itemRecordHeader.Length = 2;
                itemRecordHeader.Owner = owner;

                DataFileRecordDetail itD1 = new DataFileRecordDetail(uow) { PropertyName = "SupplierCode", Position = 1, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD2 = new DataFileRecordDetail(uow) { PropertyName = "Level1", Position = 2, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD3 = new DataFileRecordDetail(uow) { PropertyName = "Level2", Position = 3, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD4 = new DataFileRecordDetail(uow) { PropertyName = "Level3", Position = 4, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD5 = new DataFileRecordDetail(uow) { PropertyName = "Level4", Position = 5, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD6 = new DataFileRecordDetail(uow) { PropertyName = "MotherCode", Position = 6, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD7 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 7, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD8 = new DataFileRecordDetail(uow) { PropertyName = "Description", Position = 8, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD9 = new DataFileRecordDetail(uow) { PropertyName = "Buyer", Position = 9, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD10 = new DataFileRecordDetail(uow) { PropertyName = "Seasonality", Position = 10, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD11 = new DataFileRecordDetail(uow) { PropertyName = "VatCategory", Position = 11, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD12 = new DataFileRecordDetail(uow) { PropertyName = "MeasurmentUnit", Position = 12, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD13 = new DataFileRecordDetail(uow) { PropertyName = "InsertedDate", Position = 13, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD14 = new DataFileRecordDetail(uow) { PropertyName = "CentralStoreCode", Position = 14, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD15 = new DataFileRecordDetail(uow) { PropertyName = "MinOrderQty", Position = 15, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD16 = new DataFileRecordDetail(uow) { PropertyName = "MaxItemOrderQty", Position = 16, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD17 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 17, Trim = true, Header = itemRecordHeader };
                DataFileRecordDetail itD18 = new DataFileRecordDetail(uow) { PropertyName = "PackingQuantity", Position = 18, Trim = true, Header = itemRecordHeader };

                DataFileRecordHeader bcRecordHeader = new DataFileRecordHeader(uow);
                bcRecordHeader.EntityName = typeof(Barcode).Name;
                bcRecordHeader.HeaderCode = "05";
                bcRecordHeader.Order = 5;
                bcRecordHeader.IsTabDelimited = true;
                bcRecordHeader.TabDelimitedString = "\\t";
                bcRecordHeader.Position = 0;
                bcRecordHeader.Length = 2;
                bcRecordHeader.Owner = owner;

                DataFileRecordDetail bcD1 = new DataFileRecordDetail(uow) { PropertyName = "Item", Position = 1, Trim = true, Header = bcRecordHeader };
                DataFileRecordDetail bcD2 = new DataFileRecordDetail(uow) { PropertyName = "Barcode", Position = 2, Trim = true, Header = bcRecordHeader };
                DataFileRecordDetail bcD3 = new DataFileRecordDetail(uow) { PropertyName = "FactorQty", Position = 3, Trim = true, Header = bcRecordHeader };
                DataFileRecordDetail bcD4 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 4, Trim = true, Header = bcRecordHeader };

                DataFileRecordHeader liRecordHeader = new DataFileRecordHeader(uow);
                liRecordHeader.EntityName = typeof(LinkedItem).Name;
                liRecordHeader.HeaderCode = "06";
                liRecordHeader.Order = 6;
                liRecordHeader.IsTabDelimited = true;
                liRecordHeader.TabDelimitedString = "\\t";
                liRecordHeader.Position = 0;
                liRecordHeader.Length = 2;
                liRecordHeader.Owner = owner;

                DataFileRecordDetail liD1 = new DataFileRecordDetail(uow) { PropertyName = "Item", Position = 1, Trim = true, Header = liRecordHeader };
                DataFileRecordDetail liD2 = new DataFileRecordDetail(uow) { PropertyName = "SubItemCode", Position = 2, Trim = true, Header = liRecordHeader };
                DataFileRecordDetail liD3 = new DataFileRecordDetail(uow) { PropertyName = "SubItemQty", Position = 3, Trim = true, Header = liRecordHeader };
                DataFileRecordDetail liD4 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 4, Trim = true, Header = liRecordHeader };

                DataFileRecordHeader pcdRecordHeader = new DataFileRecordHeader(uow);
                pcdRecordHeader.EntityName = typeof(PriceCatalogDetail).Name;
                pcdRecordHeader.HeaderCode = "08";
                pcdRecordHeader.Order = 7;
                pcdRecordHeader.IsTabDelimited = true;
                pcdRecordHeader.TabDelimitedString = "\\t";
                pcdRecordHeader.Position = 0;
                pcdRecordHeader.Length = 2;
                pcdRecordHeader.Owner = owner;

                DataFileRecordDetail pcdD1 = new DataFileRecordDetail(uow) { PropertyName = "Item", Position = 1, Trim = true, Header = pcdRecordHeader };
                DataFileRecordDetail pcdD2 = new DataFileRecordDetail(uow) { PropertyName = "PriceCatalog", Position = 2, Trim = true, Header = pcdRecordHeader };
                DataFileRecordDetail pcdD3 = new DataFileRecordDetail(uow) { PropertyName = "GrossPrice", Position = 3, Trim = true, Header = pcdRecordHeader };
                DataFileRecordDetail pcdD4 = new DataFileRecordDetail(uow) { PropertyName = "NetPrice", Position = 4, Trim = true, Header = pcdRecordHeader };
                DataFileRecordDetail pcdD5 = new DataFileRecordDetail(uow) { PropertyName = "Discount", Position = 5, Trim = true, Header = pcdRecordHeader };
                DataFileRecordDetail pcdD6 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 6, Trim = true, Header = pcdRecordHeader };

                DataFileRecordHeader custRecordHeader = new DataFileRecordHeader(uow);
                custRecordHeader.EntityName = typeof(Customer).Name;
                custRecordHeader.HeaderCode = "09";
                custRecordHeader.Order = 8;
                custRecordHeader.IsTabDelimited = true;
                custRecordHeader.TabDelimitedString = "\\t";
                custRecordHeader.Position = 0;
                custRecordHeader.Length = 2;
                custRecordHeader.Owner = owner;

                DataFileRecordDetail custD1 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 1, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD2 = new DataFileRecordDetail(uow) { PropertyName = "TaxCode", Position = 2, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD3 = new DataFileRecordDetail(uow) { PropertyName = "CompanyName", Position = 3, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD4 = new DataFileRecordDetail(uow) { PropertyName = "Address", Position = 4, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD5 = new DataFileRecordDetail(uow) { PropertyName = "Region", Position = 5, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD6 = new DataFileRecordDetail(uow) { PropertyName = "City", Position = 6, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD7 = new DataFileRecordDetail(uow) { PropertyName = "ZipCode", Position = 7, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD8 = new DataFileRecordDetail(uow) { PropertyName = "Phone", Position = 8, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD9 = new DataFileRecordDetail(uow) { PropertyName = "Profession", Position = 9, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD10 = new DataFileRecordDetail(uow) { PropertyName = "TaxOffice", Position = 10, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD11 = new DataFileRecordDetail(uow) { PropertyName = "BaseStore", Position = 11, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD12 = new DataFileRecordDetail(uow) { PropertyName = "PriceCatalog", Position = 12, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD13 = new DataFileRecordDetail(uow) { PropertyName = "Firm", Position = 13, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD14 = new DataFileRecordDetail(uow) { PropertyName = "Fax", Position = 14, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD15 = new DataFileRecordDetail(uow) { PropertyName = "SplitOrder", Position = 15, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD16 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 16, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD17 = new DataFileRecordDetail(uow) { PropertyName = "IsLicenced", Position = 17, Trim = true, Header = custRecordHeader };
                DataFileRecordDetail custD18 = new DataFileRecordDetail(uow) { PropertyName = "VatCategory", Position = 18, Trim = true, Header = custRecordHeader };

                DataFileRecordHeader offerRecordHeader = new DataFileRecordHeader(uow);
                offerRecordHeader.EntityName = typeof(Offer).Name;
                offerRecordHeader.HeaderCode = "10";
                offerRecordHeader.Order = 9;
                offerRecordHeader.IsTabDelimited = true;
                offerRecordHeader.TabDelimitedString = "\\t";
                offerRecordHeader.Position = 0;
                offerRecordHeader.Length = 2;
                offerRecordHeader.Owner = owner;

                DataFileRecordDetail oD1 = new DataFileRecordDetail(uow) { PropertyName = "FromDate", Position = 1, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD2 = new DataFileRecordDetail(uow) { PropertyName = "EndDate", Position = 2, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD3 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 3, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD4 = new DataFileRecordDetail(uow) { PropertyName = "Description", Position = 4, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD5 = new DataFileRecordDetail(uow) { PropertyName = "Description2", Position = 5, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD6 = new DataFileRecordDetail(uow) { PropertyName = "PriceCatalog", Position = 6, Trim = true, Header = offerRecordHeader };
                DataFileRecordDetail oD7 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 7, Trim = true, Header = offerRecordHeader };

                DataFileRecordHeader offerDetailRecordHeader = new DataFileRecordHeader(uow);
                offerDetailRecordHeader.EntityName = typeof(OfferDetail).Name;
                offerDetailRecordHeader.HeaderCode = "11";
                offerDetailRecordHeader.Order = 10;
                offerDetailRecordHeader.IsTabDelimited = true;
                offerDetailRecordHeader.TabDelimitedString = "\\t";
                offerDetailRecordHeader.Position = 0;
                offerDetailRecordHeader.Length = 2;
                offerDetailRecordHeader.Owner = owner;

                DataFileRecordDetail odD1 = new DataFileRecordDetail(uow) { PropertyName = "Code", Position = 1, Trim = true, Header = offerDetailRecordHeader };
                DataFileRecordDetail odD2 = new DataFileRecordDetail(uow) { PropertyName = "Item", Position = 2, Trim = true, Header = offerDetailRecordHeader };
                DataFileRecordDetail odD3 = new DataFileRecordDetail(uow) { PropertyName = "IsActive", Position = 3, Trim = true, Header = offerDetailRecordHeader };

                #endregion

                #region Dual Mode Extra Data
                if (dualModeRadioButton.Checked)
                {

                    //Default POS Customer
                    Customer posCustomer = uow.FindObject<Customer>(new BinaryOperator("Trader.FirstName", Resources.RetailClient));
                    if (posCustomer == null)
                    {
                        posCustomer = new Customer(uow);
                        posCustomer.Owner = owner;
                        posCustomer.Trader = new Trader(uow);
                        posCustomer.CompanyName = Resources.RetailClient;
                        posCustomer.CompanyBrandName = Resources.RetailClient;
                        posCustomer.Code = "2";
                        posCustomer.Profession = Resources.RetailClient;
                        posCustomer.Trader.FirstName = Resources.RetailClient;
                        posCustomer.Trader.LastName = Resources.RetailClient;
                        posCustomer.Trader.TaxCode = "000000002";
                        posCustomer.VatLevel = vatlevelnormal;
                    }


                    CustomerStorePriceList cspl2 = uow.FindObject<CustomerStorePriceList>(CriteriaOperator.And(new BinaryOperator("Customer.Oid", posCustomer.Oid), new BinaryOperator("StorePriceList.Oid", spl.Oid)));
                    if (cspl2 == null)
                    {
                        cspl2 = new CustomerStorePriceList(uow);
                        cspl2.Customer = posCustomer;
                        cspl2.StorePriceList = spl;
                        cspl2.IsDefault = true;
                    }

                    //store
                    StoreControllerSettings scs = uow.FindObject<StoreControllerSettings>(null);
                    if (scs == null)
                    {
                        scs = new StoreControllerSettings(uow);
                        scs.DefaultCustomer = posCustomer;
                        scs.DefaultDocumentStatus = docstat2;
                        scs.DepositDocumentType = doctypeDeposit;
                        scs.DepositItem = depositItem;
                        scs.ServerName = "Default Store Controller";
                        scs.ServerUrl = "127.0.0.1";
                        scs.DefaultDocumentStatus = docstat2;
                        scs.ID = 1;
                        scs.ProformaDocumentType = doctypeProforma;
                        scs.ReceiptDocumentType = doctypeReceipt;
                        scs.Store = store;
                        scs.WithdrawalDocumentType = doctypeWithdrawal;
                        scs.WithdrawalItem = withdrawalItem;
                        scs.Owner = owner;
                    }
                }
                #endregion

                XpoHelper.CommitChanges(uow);
            }
        }

        public CustomReport CreateReport(string filepath, UnitOfWork uow, CompanyNew owner, string code, ReportCategory category)
        {
            CustomReport report = new CustomReport(uow)
            {
                ReportCategory = category,
                Owner = owner,
                Code = code,
                Description = Path.GetFileNameWithoutExtension(filepath),
                FileName = Path.GetFileName(filepath),
                ReportFile = File.ReadAllBytes(filepath)
            };
            report.Title = report.Description;
            Type reportType = XtraReportBaseExtension.GetReportTypeFromFile(report.ReportFile);
            Type singleObjectReportType = XtraReportBaseExtension.GetSingleObjectTypeFromFile(report.ReportFile);
            report.ReportType = reportType == typeof(XtraReportExtension) ? "General Report" : "Single Object Report";
            report.ObjectType = singleObjectReportType != null ? singleObjectReportType.Name : "";
            return report;
        }


        public void InitializationDualSc()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {

                #region Default devices and Print Formats

                POSDevice msrOPOS = new POSDevice(uow);
                msrOPOS.ConnectionType = ConnectionType.OPOS;
                msrOPOS.Name = "MSR OPOS";
                OPOSDeviceSettings msrOposDeviceSettings = new OPOSDeviceSettings(uow);
                msrOposDeviceSettings.DeviceType = DeviceType.MagneticStripReader;
                msrOposDeviceSettings.LogicalDeviceName = "msr";
                msrOPOS.DeviceSettings = msrOposDeviceSettings;

                POSDevice keylockIndirect = new POSDevice(uow);
                keylockIndirect.ConnectionType = ConnectionType.INDIRECT;
                keylockIndirect.Name = "WINCOR KEYLOCK INDIRECT (FROM KEYBOARD)";
                IndirectDeviceSettings keylockIndirectDeviceSettings = new IndirectDeviceSettings(uow);
                keylockIndirectDeviceSettings.DeviceType = DeviceType.KeyLock;
                keylockIndirectDeviceSettings.KeyPosition0CommandString = "131089,75,48";
                keylockIndirectDeviceSettings.KeyPosition1CommandString = "131089,75,49";
                keylockIndirectDeviceSettings.KeyPosition2CommandString = "131089,75,50";
                keylockIndirectDeviceSettings.KeyPosition3CommandString = "131089,75,51";
                keylockIndirectDeviceSettings.KeyPosition4CommandString = "131089,75,52";
                keylockIndirect.DeviceSettings = keylockIndirectDeviceSettings;

                POSDevice keylockOPOS = new POSDevice(uow);
                keylockOPOS.ConnectionType = ConnectionType.OPOS;
                keylockOPOS.Name = "KEYLOCK OPOS";
                OPOSDeviceSettings keylockOPOSDeviceSettings = new OPOSDeviceSettings(uow);
                keylockOPOSDeviceSettings.DeviceType = DeviceType.KeyLock;
                keylockOPOSDeviceSettings.LogicalDeviceName = "keylock";
                keylockOPOS.DeviceSettings = keylockOPOSDeviceSettings;

                POSDevice poleDisplayOPOS = new POSDevice(uow);
                poleDisplayOPOS.ConnectionType = ConnectionType.OPOS;
                poleDisplayOPOS.Name = "POLEDISPLAY OPOS";
                OPOSDeviceSettings poleDisplayOPOSDeviceSettings = new OPOSDeviceSettings(uow);
                poleDisplayOPOSDeviceSettings.DeviceType = DeviceType.PoleDisplay;
                poleDisplayOPOSDeviceSettings.LogicalDeviceName = "poledisplay";
                poleDisplayOPOS.DeviceSettings = poleDisplayOPOSDeviceSettings;

                POSDevice poleDisplayCOM = new POSDevice(uow);
                poleDisplayCOM.ConnectionType = ConnectionType.COM;
                poleDisplayCOM.Name = "POLEDISPLAY COM";
                COMDeviceSettings poleDisplayComDeviceSettings = new COMDeviceSettings(uow);
                poleDisplayComDeviceSettings.DeviceType = DeviceType.PoleDisplay;
                poleDisplayComDeviceSettings.PortName = "COM1";
                poleDisplayCOM.DeviceSettings = poleDisplayComDeviceSettings;

                POSDevice scannerCOM = new POSDevice(uow);
                scannerCOM.ConnectionType = ConnectionType.COM;
                scannerCOM.Name = "SCANNER COM";
                COMDeviceSettings scannerComDeviceSettings = new COMDeviceSettings(uow);
                scannerComDeviceSettings.DeviceType = DeviceType.Scanner;
                scannerComDeviceSettings.PortName = "COM1";
                scannerCOM.DeviceSettings = scannerComDeviceSettings;

                POSDevice scannerOPOS = new POSDevice(uow);
                scannerOPOS.ConnectionType = ConnectionType.OPOS;
                scannerOPOS.Name = "SCANNER OPOS";
                OPOSDeviceSettings scannerOposDeviceSettings = new OPOSDeviceSettings(uow);
                scannerOposDeviceSettings.DeviceType = DeviceType.Scanner;
                scannerOposDeviceSettings.LogicalDeviceName = "scanner";
                scannerOPOS.DeviceSettings = scannerOposDeviceSettings;

                POSDevice drawerOPOS = new POSDevice(uow);
                drawerOPOS.ConnectionType = ConnectionType.OPOS;
                drawerOPOS.Name = "DRAWER OPOS";
                OPOSDeviceSettings drawerOposDeviceSettings = new OPOSDeviceSettings(uow);
                drawerOposDeviceSettings.DeviceType = DeviceType.Drawer;
                drawerOposDeviceSettings.LogicalDeviceName = "drawer";
                drawerOPOS.DeviceSettings = drawerOposDeviceSettings;

                POSDevice epsonOpos = new POSDevice(uow);
                epsonOpos.ConnectionType = ConnectionType.OPOS;
                epsonOpos.Name = "THERMAL PRINTER OPOS GREEK";
                OPOSPrinterSettings epsonOposDeviceSettings = new OPOSPrinterSettings(uow);
                epsonOposDeviceSettings.DeviceType = DeviceType.Printer;
                epsonOposDeviceSettings.LineChars = 42;
                epsonOposDeviceSettings.CharacterSet = 1253;
                epsonOposDeviceSettings.LogicalDeviceName = "printer";
                epsonOpos.DeviceSettings = epsonOposDeviceSettings;

                POSDevice scaleCOM = new POSDevice(uow);
                scaleCOM.ConnectionType = ConnectionType.COM;
                scaleCOM.Name = "SCALE DIGI DS-980";
                COMScaleSettings scaleCOMDeviceSettings = new COMScaleSettings(uow);
                scaleCOMDeviceSettings.DeviceType = DeviceType.Scale;
                scaleCOMDeviceSettings.PortName = "COM2";
                scaleCOMDeviceSettings.Parity = System.IO.Ports.Parity.Odd;
                scaleCOMDeviceSettings.DataBits = 7;
                scaleCOMDeviceSettings.StopBits = System.IO.Ports.StopBits.One;
                scaleCOMDeviceSettings.BaudRate = 9600;
                scaleCOMDeviceSettings.CommunicationType = ScaleCommunicationType.CONTINUOUS;
                scaleCOMDeviceSettings.ScaleReadPattern = @"AB\r(\d{3}.\d{3})\r\d{3}.\d{3}\r";
                scaleCOM.DeviceSettings = scaleCOMDeviceSettings;

                POSDevice comPrinter = new POSDevice(uow);
                comPrinter.ConnectionType = ConnectionType.COM;
                comPrinter.Name = "THERMAL PRINTER COM GREEK";
                COMDeviceSettings comPrinterSettings = new COMDeviceSettings(uow);
                comPrinterSettings.DeviceType = DeviceType.Printer;
                comPrinterSettings.LineChars = 42;
                comPrinterSettings.CharacterSet = 737;
                comPrinterSettings.PortName = "COM1";
                comPrinter.DeviceSettings = comPrinterSettings;

                POSDevice emulatorPrinter = new POSDevice(uow);
                emulatorPrinter.ConnectionType = ConnectionType.EMULATED;
                emulatorPrinter.Name = "PRINTER EMULATOR";
                DeviceSettings deviceSettings = new DeviceSettings(uow);
                deviceSettings.DeviceType = DeviceType.Printer;
                deviceSettings.LineChars = 42;
                deviceSettings.CharacterSet = 1253;
                emulatorPrinter.DeviceSettings = deviceSettings;

                POSDevice drawerIndirect = new POSDevice(uow);
                drawerIndirect.ConnectionType = ConnectionType.INDIRECT;
                drawerIndirect.Name = "DRAWER INDIRECT (THROUGH PRINTER)";
                IndirectDeviceSettings drawerIndirectDeviceSettings = new IndirectDeviceSettings(uow);
                drawerIndirectDeviceSettings.DeviceType = DeviceType.Drawer;
                drawerIndirectDeviceSettings.OpenCommandString = "\x1Bp\x0" + "dd";
                drawerIndirectDeviceSettings.ParentDeviceName = epsonOpos.Name;
                drawerIndirect.DeviceSettings = drawerIndirectDeviceSettings;

                POSDevice wincorEJ320 = new POSDevice(uow);
                wincorEJ320.ConnectionType = ConnectionType.COM;
                wincorEJ320.Name = "WINCOR EJ-320 FISCAL PRINTER";
                COMDeviceSettings wincorEJ320Settings = new COMDeviceSettings(uow);
                wincorEJ320Settings.DeviceType = DeviceType.WincorFiscalPrinter;
                wincorEJ320Settings.PortName = "COM1";
                wincorEJ320Settings.BaudRate = 115200;
                wincorEJ320Settings.LineChars = 40;
                wincorEJ320Settings.Parity = System.IO.Ports.Parity.Odd;
                wincorEJ320.DeviceSettings = wincorEJ320Settings;

                POSDevice citizen = new POSDevice(uow);
                citizen.ConnectionType = ConnectionType.COM;
                citizen.Name = "ICS CITIZEN FISCAL PRINTER";
                COMDeviceSettings citizenSettings = new COMDeviceSettings(uow);
                citizenSettings.DeviceType = DeviceType.MicrelecFiscalPrinter;
                citizenSettings.PortName = "COM1";
                citizenSettings.BaudRate = 115200;
                citizenSettings.LineChars = 30;
                citizen.DeviceSettings = citizenSettings;

                POSDevice disign = new POSDevice(uow);
                disign.ConnectionType = ConnectionType.ETHERNET;
                disign.Name = "ITS DISIGN";
                EthernetDeviceSettings disignSettings = new EthernetDeviceSettings(uow);
                disignSettings.DeviceType = DeviceType.DiSign;
                disignSettings.Port = 200;
                disignSettings.IPAddress = "127.0.0.1";
                disign.DeviceSettings = disignSettings;

                POSPrintFormat receiptFormat = new POSPrintFormat(uow);
                receiptFormat.Description = "Receipt Format";
                receiptFormat.FormatType = eFormatType.Receipt;
                receiptFormat.Format = Resources.DefaultReceiptFormat;

                POSPrintFormat xFormat = new POSPrintFormat(uow);
                xFormat.Description = "X Format";
                xFormat.FormatType = eFormatType.X;
                xFormat.Format = Resources.DefaultXFormat;

                POSPrintFormat zFormat = new POSPrintFormat(uow);
                zFormat.Description = "Z Format";
                zFormat.FormatType = eFormatType.Z;
                zFormat.Format = Resources.DefaultZFormat;

                #endregion

                XpoHelper.CommitChanges(uow);
            }
        }

        private void btnInitialiseDB_Click(object sender, EventArgs e)
        {
            SetConnectionProperties();
            if (VersionHelper.MigrationVersionExists())
            {
                logger.Error("The selected database has been already initialised!" + Environment.NewLine);
                return;
            }

            eCultureInfo currentCulture = (eCultureInfo)comboBoxLanguages.SelectedItem;
            Resources.Culture = new CultureInfo(currentCulture.GetDescription());


            logger.Message("Initializing DB..." + Environment.NewLine);
            try
            {
                XpoHelper.ResetDataLayer();
                XpoHelper.UpdateDatabase();
            }
            catch (Exception exc)
            {
                logger.Error(exc.Message + Environment.NewLine);
                return;
            }

            if (this.Settings.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                try
                {
                    InitializationRetailDual(currentCulture);
                }
                catch (Exception exc)
                {
                    logger.Message(exc.Message + Environment.NewLine);
                    return;
                }
            }


            if (this.Settings.ApplicationInstance != eApplicationInstance.RETAIL)
            {
                try
                {
                    InitializationDualSc();
                }
                catch (Exception ex)
                {
                    logger.Message(ex.Message + Environment.NewLine);
                    return;
                }
            }

            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<ApplicationSettings> appSettings = new XPCollection<ApplicationSettings>(uow);
                    ApplicationSettings aps = appSettings.FirstOrDefault();
                    if (aps == null)
                    {
                        aps = new ApplicationSettings(uow);
                        aps.LogingLevel = Platform.Enumerations.LogLevel.Basic;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Message(ex.Message + Environment.NewLine);
                return;
            }


            try //set Migration Version table
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CreateVersionInfoTable(uow);
                }
            }
            catch (Exception exc)
            {
                logger.Message(exc.Message + Environment.NewLine);
                return;
            }

            logger.Success("Initializing DB has been Succesfully Completed" + Environment.NewLine);
        }

        private void CreateMinistryDocumentTypes(UnitOfWork uow, eCultureInfo currentCulture)
        {
            switch (currentCulture)
            {
                default:
                    MinistryDocumentType ministryDocumentType001 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "10§1 ΠΔ186/92 και2§11ν.3052/02 απο1-1-03", ShortTitle = "ΔΕΛΤ.ΠΟΣ.ΠΑΡ.", Title = "ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ", Code = "40" };
                    MinistryDocumentType ministryDocumentType002 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ.176/77", ShortTitle = "ΑΠΟΔ.ΑΣΦ.", Title = "ΑΠΟΔΕΙΞΗ ΑΣΦΑΛΙΣΤΡΩΝ", Code = "54" };
                    MinistryDocumentType ministryDocumentType003 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.176/77", ShortTitle = "ΑΠΟΔ.ΕΠ.ΑΣΦ.", Title = "ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ ΑΣΦΑΛΙΣΤΡΩΝ", Code = "56" };
                    MinistryDocumentType ministryDocumentType004 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 ΠΑΡ 5,  6", ShortTitle = "ΤΙΜ.ΑΓ.ΑΓΡ.", Title = "ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΑΓΡΟΤΕΣ)", Code = "58" };
                    MinistryDocumentType ministryDocumentType005 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 ΠΑΡ 5", ShortTitle = "ΤΙΜ.ΑΓ.ΙΔΙΩΤ.", Title = "ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΙΔΙΩΤΕΣ)", Code = "60" };
                    MinistryDocumentType ministryDocumentType006 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 ΠΑΡ. 5", ShortTitle = "ΤΙΜ.ΑΓΟΡ.ΑΡΝ.", Title = "ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ (ΑΠΟ ΑΡΝΟΥΜΕΝΟΥΣ)", Code = "62" };
                    MinistryDocumentType ministryDocumentType007 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "10 παρ 5 ηα", ShortTitle = "ΔΕΛ.ΕΙΣΑΓ.", Title = "ΔΕΛΤΙΟ ΕΙΣΑΓΩΓΗΣ", Code = "63" };
                    MinistryDocumentType ministryDocumentType008 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "10 παρ. 5ηα", ShortTitle = "ΤΙΤΛ. ΑΠΟΘ.", Title = "ΤΙΤΛΟΣ ΑΠΟΘΗΚΕΥΣΗΣ", Code = "65" };
                    MinistryDocumentType ministryDocumentType009 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Κέντρων διασκέδασης κ.λ.π. ΠΟΛ.1159/94", ShortTitle = "ΕΙΣ.Κ.ΔΙΑΣΚ", Title = "ΕΙΣΙΤΗΡΙΟ", Code = "137" };
                    MinistryDocumentType ministryDocumentType010 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Θεαμάτων", ShortTitle = "ΕΙΣ.ΘΕΑΜ", Title = "ΕΙΣΙΤΗΡΙΟ", Code = "138" };
                    MinistryDocumentType ministryDocumentType011 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Συγκοινωνιακών μέσων εκτός πλοίων", ShortTitle = "ΕΙΣ.Σ.ΜΕΣΩΝ", Title = "ΕΙΣΙΤΗΡΙΟ", Code = "139" };
                    MinistryDocumentType ministryDocumentType012 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "13 ΠΑΡ 1", ShortTitle = "ΕΙΣ.ΠΛΟΙΩΝ", Title = "ΕΙΣΙΤΗΡΙΟ ΠΛΟΙΩΝ", Code = "145" };
                    MinistryDocumentType ministryDocumentType013 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΕΓΚ. 3 ΠΑΡ 13.2.5", ShortTitle = "ΕΙΣ.ΥΔΡ.", Title = "ΕΙΣΙΤΗΡΙΟ ΥΔΡΟΘΕΡΑΠΕΙΑΣ", Code = "147" };
                    MinistryDocumentType ministryDocumentType014 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "11 παρ.1", ShortTitle = "Δ.Α", Title = "ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "158" };
                    MinistryDocumentType ministryDocumentType015 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "11 παρ.3", ShortTitle = "Σ.Δ.Α", Title = "ΣΥΓΚΕΝΤΡΩΤΙΚΟ ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "159" };
                    MinistryDocumentType ministryDocumentType016 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "11 παρ.4", ShortTitle = "Δ.Ε.Δ", Title = "ΔΕΛΤΙΟ ΕΣΩΤΕΡΙΚΗΣ ΔΙΑΚΙΝΗΣΗΣ", Code = "160" };
                    MinistryDocumentType ministryDocumentType017 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "12 παρ. 1, 2", ShortTitle = "Τ.Π.", Title = "ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών)", Code = "161" };
                    MinistryDocumentType ministryDocumentType018 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "12 παρ. 1, 2", ShortTitle = "Τ.Π.Υ", Title = "ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιών)", Code = "162" };
                    MinistryDocumentType ministryDocumentType019 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "12 παρ.3", ShortTitle = "ΤΙΜ 12 παρ.3", Title = "ΤΙΜΟΛΟΓΙΟ (Επιδοτ. Αποζημ. κ.λ.π.)", Code = "163" };
                    MinistryDocumentType ministryDocumentType020 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "12 μικτή χρήση", ShortTitle = "ΤΙΜ.", Title = "ΤΙΜΟΛΟΓΙΟ", Code = "165" };
                    MinistryDocumentType ministryDocumentType021 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 παρ.7, 8", ShortTitle = "ΕΚΚΑΘ", Title = "ΕΚΚΑΘΑΡΙΣΗ", Code = "168" };
                    MinistryDocumentType ministryDocumentType022 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 παρ.13", ShortTitle = "Π.Τ", Title = "ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ", Code = "169" };
                    MinistryDocumentType ministryDocumentType023 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "13 παρ. 1-3", ShortTitle = "Α.Λ.Π", Title = "ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ", Code = "173" };
                    MinistryDocumentType ministryDocumentType024 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "13 παρ.1-3", ShortTitle = "Α.Π.Υ", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "174" };
                    MinistryDocumentType ministryDocumentType025 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "13 παρ.1", ShortTitle = "Α.ΕΠΙΣΤΡ", Title = "ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ", Code = "175" };
                    MinistryDocumentType ministryDocumentType026 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Συνδρομών εφημερίδων και περιοδικών", ShortTitle = "Α.ΕΙΣ.ΣΥΝΔ", Title = "ΑΠΟΔΕΙΞΗ ΕΙΣΠΡΑΞΗΣ", Code = "176" };
                    MinistryDocumentType ministryDocumentType027 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "13α΄", ShortTitle = "Δ.Κ.Τ.Λ", Title = "ΔΕΛΤΙΟ ΚΙΝΗΣΗΣ ΤΟΥΡΙΣΤΙΚΩΝ ΛΕΩΦΟΡΕΙΩΝ", Code = "177" };
                    MinistryDocumentType ministryDocumentType028 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αρθ. 14 Π.Δ 186/92", ShortTitle = "Α.ΑΥΤΟΠΑΡ", Title = "ΑΠΟΔΕΙΞΗ ΑΥΤΟΠΑΡΑΔΟΣΗΣ", Code = "178" };
                    MinistryDocumentType ministryDocumentType029 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "Αρθ. 14 Π.Δ 186/92", ShortTitle = "Α.ΔΑΠΑΝΗΣ", Title = "ΑΠΟΔΕΙΞΗ ΔΑΠΑΝΗΣ", Code = "179" };
                    MinistryDocumentType ministryDocumentType030 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "1167, 1271/02", ShortTitle = "ΦΟΡΤΩΤΙΚΗ", Title = "ΦΟΡΤΩΤΙΚΗ", Code = "180" };
                    MinistryDocumentType ministryDocumentType031 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "16 παρ.6", ShortTitle = "ΚΑΤ.ΑΠΟΣΤ", Title = "ΚΑΤΑΣΤΑΣΗ ΑΠΟΣΤΟΛΗΣ", Code = "181" };
                    MinistryDocumentType ministryDocumentType032 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "16 παρ.10", ShortTitle = "ΔΙ.ΣΗΜ", Title = "ΔΙΟΡΘΩΤΙΚΟ ΣΗΜΕΙΩΜΑ", Code = "182" };
                    MinistryDocumentType ministryDocumentType033 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "16 παρ.9", ShortTitle = "ΑΠ.ΜΕΤΑΦ", Title = "ΑΠΟΔΕΙΞΗ ΜΕΤΑΦΟΡΑΣ", Code = "183" };
                    MinistryDocumentType ministryDocumentType034 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "Εστιατ.ζυθεστ.κ.λ.π. ΠΟΛ.1117/91, 1238/92", ShortTitle = "ΔΕΛ.ΠΑΡΑΓ.", Title = "ΔΕΛΤΙΟ ΠΑΡΑΓΓΕΛΙΑΣ", Code = "188" };
                    MinistryDocumentType ministryDocumentType035 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ.1102/93", ShortTitle = "Α.Π.Α.ΕΙΣ", Title = "ΑΠΟΔΕΙΞΗ ΠΩΛΗΣΗΣ (Αεροπ.Εισιτηρίων)", Code = "190" };
                    MinistryDocumentType ministryDocumentType036 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.1102/92", ShortTitle = "ΕΚΚ.Α.ΕΙΣ", Title = "ΕΚΚΑΘΑΡΙΣΗ (Αεροπορικών εισιτηρίων)", Code = "191" };
                    MinistryDocumentType ministryDocumentType037 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Εισιτήρια Αστικών Συγκοινωνιών", ShortTitle = "Α.ΠΑΡ.ΕΙΣ", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΑΔΟΘΕΝΤΩΝ ΕΙΣΙΤΗΡΙΩΝ", Code = "192" };
                    MinistryDocumentType ministryDocumentType038 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.1233/93", ShortTitle = "ΕΚΚ.ΕΙΣ.Α.Σ.", Title = "ΕΚΚΑΘΑΡΙΣΗ (Εισιτήρια Αστικών Συγκοινωνιών)", Code = "193" };
                    MinistryDocumentType ministryDocumentType039 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.1292/93", ShortTitle = "ΕΚΚ.ΕΦ.ΠΕΡ.", Title = "ΕΚΚΑΘΑΡΙΣΗ (Εφημερίδων και περιοδικών)", Code = "194" };
                    MinistryDocumentType ministryDocumentType040 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.1319/93", ShortTitle = "ΕΚΚ.Μ.Α.Κ", Title = "ΕΚΚΑΘΑΡΙΣΗ (Μεριδίων αμοιβ. Κεφαλαίων)", Code = "195" };
                    MinistryDocumentType ministryDocumentType041 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Στάθμευσης δημοσίων χώρων ΠΟΛ.1104/94", ShortTitle = "Α.ΠΑΡ.ΚΑΡΤ.", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΑΔΟΘΕΙΣΩΝ ΚΑΡΤΩΝ", Code = "196" };
                    MinistryDocumentType ministryDocumentType042 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ.1104/94", ShortTitle = "ΕΚΚ.Κ.ΣΤΑΘ", Title = "ΕΚΚΑΘΑΡΙΣΗ (Καρτών σταθμ. δημ. χωρ.)", Code = "197" };
                    MinistryDocumentType ministryDocumentType043 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "Πωλ.Ανθέων μέσω ΙΝΤΕΡΦΛΟΡΑ ΠΟΛ.1212/94", ShortTitle = "ΕΚΚ.Π.ΑΝΘ", Title = "ΕΚΚΑΘΑΡΙΣΗ", Code = "199" };
                    MinistryDocumentType ministryDocumentType044 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "Εισιτηρίων καλαθοσφ. - ΕΣΑΚ ΠΟΛ.1094/95", ShortTitle = "ΕΚΚ.ΕΙΣ.ΚΑΛ", Title = "ΕΚΚΑΘΑΡΙΣΗ", Code = "200" };
                    MinistryDocumentType ministryDocumentType045 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ 61/88", ShortTitle = "ΕΚΚ.ΚΤΕΛ", Title = "ΕΚΚΑΘΑΡΙΣΗ (ΚΤΕΛ)", Code = "209" };
                    MinistryDocumentType ministryDocumentType046 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "ΠΟΛ. 66/88", ShortTitle = "ΕΚΚ.ΙΜΕ", Title = "ΕΚΚΑΘΑΡΙΣΗ (ΙΜΕ)", Code = "210" };
                    MinistryDocumentType ministryDocumentType047 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "ΕΚΚ.ΑΣΦ.ΕΠ", Title = "ΕΚΚΑΘΑΡΙΣΗ (Ασφαλιστικών επιχειρήσεων)", Code = "211" };
                    MinistryDocumentType ministryDocumentType048 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "1215/96", ShortTitle = "ΕΚΚ.Κ.ΠΛΟΙΩΝ", Title = "ΕΚΚΑΘΑΡΙΣΗ (Κοινοπραξιών πλοίων)", Code = "212" };
                    MinistryDocumentType ministryDocumentType049 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "(Λοιπές Περιπτώσεις)", ShortTitle = "ΕΚΚ.Λ.Π.", Title = "ΕΚΚΑΘΑΡΙΣΗ", Code = "213" };
                    MinistryDocumentType ministryDocumentType050 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.REVERSE, Description = "ΡΘΡΟ 23 ΠΑΡ. 5", ShortTitle = "ΕΙΔ.ΑΚ.ΣΤ.", Title = "ΕΙΔΙΚΟ ΑΚΥΡΩΤΙΚΟ ΣΤΟΙΧΕΙΟ", Code = "215" };
                    MinistryDocumentType ministryDocumentType051 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Β.ΣΤ.-Α.Π.Υ.", Title = "ΒΙΒΛΙΟ ΣΤΑΘΜΕΥΣΗΣ (οχηματων) - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "217" };
                    MinistryDocumentType ministryDocumentType052 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Β.ΣΤ.ΣΚ-Α.Π.Υ.", Title = "ΒΙΒΛΙΟ ΣΤΑΘΜΕΥΣΗΣ -ΦΥΛΑΞΗΣ ΣΚΑΦΩΝ -ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩ", Code = "218" };
                    MinistryDocumentType ministryDocumentType053 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "-", ShortTitle = "Β.ΕΡΓ.-Δ.Α.", Title = "ΒΙΒΛΙΟ ΕΡΓΩΝ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "219" };
                    MinistryDocumentType ministryDocumentType054 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Τ-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ -ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "221" };
                    MinistryDocumentType ministryDocumentType055 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Τ.Π.-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "222" };
                    MinistryDocumentType ministryDocumentType056 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Τ.Π.Υ.-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ (Παροχης Υπηρεσιων) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "223" };
                    MinistryDocumentType ministryDocumentType057 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "Τ(ΑΓ)-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ (Αγοράς από ιδιώτη) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "224" };
                    MinistryDocumentType ministryDocumentType058 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "Τ(ΑΓ.ΑΓΡ.)-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ (Αγοράς αγροτικών προϊόντων) - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "225" };
                    MinistryDocumentType ministryDocumentType059 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΤΠ.Υ.-Τ.Π.", Title = "ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιων) - ΤΙΜΟΛΟΓΙΟ (Πώληση Αγαθών)", Code = "226" };
                    MinistryDocumentType ministryDocumentType060 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΤΠΥ.-Τ.Π-Δ.Α", Title = "ΤΙΜΟΛΟΓΙΟ(Παροχή Υπηρεσιων)-ΤΙΜΟΛΟΓΙΟ(Πώληση Αγαθών)-ΔΕΛ.ΑΠ.", Code = "227" };
                    MinistryDocumentType ministryDocumentType061 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "ΕΚΚ-Δ.Α.", Title = "ΕΚΚΑΘΑΡΙΣΗ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "228" };
                    MinistryDocumentType ministryDocumentType062 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "ΔΕΛ.ΠΟΣ.ΠΑΡ-Π.Τ", Title = "ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ - ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ", Code = "229" };
                    MinistryDocumentType ministryDocumentType063 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Α.Λ.Π-Δ.Α", Title = "ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "231" };
                    MinistryDocumentType ministryDocumentType064 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Α.Π.Υ-Δ.Α", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "232" };
                    MinistryDocumentType ministryDocumentType065 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Α.Π.Υ-Α.Λ.Π", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΑΠΟΔΕΙΞΗ ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ", Code = "233" };
                    MinistryDocumentType ministryDocumentType066 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "Α.Ε.Δ-Δ.Α", Title = "ΑΠΟΔΕΙΞΗ ΕΠΑΓΓΕΛΜΑΤΙΚΗΣ ΔΑΠΑΝΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "234" };
                    MinistryDocumentType ministryDocumentType067 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "Α.Π.ΕΠΙΣΤ-Δ.Α", Title = "ΑΠΟΔΕΙΞΗ ΕΠΙΣΤΡΟΦΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "235" };
                    MinistryDocumentType ministryDocumentType068 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "ΠΙΣΤ.Τ-Δ.Α", Title = "ΠΙΣΤΩΤΙΚΟ ΤΙΜΟΛΟΓΙΟ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "236" };
                    MinistryDocumentType ministryDocumentType069 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΦΟΡΤ-Τ.Π.Υ", Title = "ΦΟΡΤΩΤΙΚΗ - ΤΙΜΟΛΟΓΙΟ (Παροχή Υπηρεσιων)", Code = "237" };
                    MinistryDocumentType ministryDocumentType070 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "-", ShortTitle = "Β.ΑΠΟΘ-ΤΙΤ.ΑΠΟΘ", Title = "ΒΙΒΛΙΟ ΑΠΟΘΗΚΕΥΣΗΣ - ΤΙΤΛΟΣ ΑΠΟΘΗΚΕΥΣΗΣ", Code = "238" };
                    MinistryDocumentType ministryDocumentType071 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΕΙΣΙΤ.-ΑΠΟΔ.ΜΕΤ", Title = "ΕΙΣΙΤΗΡΙΟ - ΑΠΟΔΕΙΞΗ ΜΕΤΑΦΟΡΑΣ", Code = "239" };
                    MinistryDocumentType ministryDocumentType072 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "άρθρο 16 παρ. 13", ShortTitle = "ΣΥΓΚ. ΦΟΡΤ.", Title = "ΣΥΓΚΕΝΤΡΩΤΙΚΗ ΦΟΡΤΩΤΙΚΗ", Code = "240" };
                    MinistryDocumentType ministryDocumentType073 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Μονο για πλυντήρια", ShortTitle = "Β.Ε.Α. - Α.Π.Υ.", Title = "ΒΙΒΛΙΟ ΕΙΣΕΡΧΟΜΕΝΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "242" };
                    MinistryDocumentType ministryDocumentType074 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "Τ(ΑΓ.ΑΓΡ.)-ΕΚΚ", Title = "ΤΙΜΟΛΟΓΙΟ (Αγοράς Αγροτικών Προϊόντων) - ΕΚΚΑΘΑΡΙΣΗ", Code = "245" };
                    MinistryDocumentType ministryDocumentType075 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αρθ.16", ShortTitle = "ΑΠΟΔ & ΗΜ.ΜΕΤΑΦ", Title = "ΑΠΟΔΕΙΞΗ ΚΑΙ ΗΜΕΡΟΛΟΓΙΟ ΜΕΤΑΦΟΡΑΣ", Code = "246" };
                    MinistryDocumentType ministryDocumentType076 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "10 παρ. 5β,  13,  19 παρ. 4", ShortTitle = "ΜΗΤΡ.ΜΑΘ-Α.Π.Υ.", Title = "ΜΗΤΡΩΟ ΜΑΘΗΤΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "247" };
                    MinistryDocumentType ministryDocumentType077 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΑΠ.ΑΥΤΟΠΑΡ-Δ.Α.", Title = "ΑΠΟΔΕΙΞΗ ΑΥΤΟΠΑΡΑΔΟΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "251" };
                    MinistryDocumentType ministryDocumentType078 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αθεώρητο απο 1-1-03 (ΠΟΛ 1166/2002)", ShortTitle = "ΑΠΥ-ΜΙΣΘΩΤΗΡΙΟ", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ-ΜΙΣΘΩΤΗΡΙΟ ΣΥΜΒΟΛΑΙΟ ΑΥΤΟΚΙΝΗΤΩΝ", Code = "252" };
                    MinistryDocumentType ministryDocumentType079 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "ΔΑ - ΑΛΠ - ΑΠΥ", Title = "ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ-ΑΠΟΔ.ΛΙΑΝΙΚΗΣ ΠΩΛΗΣΗΣ-ΑΠΟΔ.ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩ", Code = "253" };
                    MinistryDocumentType ministryDocumentType080 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αρθ.10 παρ.5δ & Αρθ 19παρ.4(Διαγνωστικά)", ShortTitle = "Β.ΕΠΙΣΚ.ΑΣΘ-ΑΠΥ", Title = "ΒΙΒΛΙΟ ΕΠΙΣΚΕΨΗΣ ΑΣΘΕΝΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "254" };
                    MinistryDocumentType ministryDocumentType081 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Α.Π.Υ-Τ.Π.-Δ.Α.", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ-ΤΙΜΟΛ(Πώλησης)-ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "257" };
                    MinistryDocumentType ministryDocumentType082 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1102/93", ShortTitle = "ΑΠ.ΠΩΛ.ΕΙΣ.-ΑΠΥ", Title = "ΑΠΟΔΕΙΞΗ ΠΩΛΗΣΗΣ ΕΙΣΙΤΗΡΙΩΝ - ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "258" };
                    MinistryDocumentType ministryDocumentType083 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ιατροί, οδοντίατροι", ShortTitle = "Β.ΕΠΙΣΚ.ΑΣΘ-ΑΠΥ", Title = "ΒΙΒΛΙΟ ΕΠΙΣΚΕΨΗΣ ΑΣΘΕΝΩΝ-ΑΠΟΔ.ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ(ιατροί, οδον)", Code = "259" };
                    MinistryDocumentType ministryDocumentType084 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αρθ.10παρ.5ιζ & Αρθ.19παρ.4 & ΠΟΛ1219/96", ShortTitle = "Β.ΕΡΓ-ΤΙΜ-ΔΑ", Title = "ΒΙΒΛΙΟ ΕΡΓΩΝ-ΤΙΜΟΛΟΓΙΟ-ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "260" };
                    MinistryDocumentType ministryDocumentType085 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "-", ShortTitle = "Τ-Δ.Α.-ΣΥΝ.Δ.ΕΓ", Title = "ΤΙΜΟΛΟΓΙΟ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ -ΣΥΝΟΔΕΥΤΙΚΟ ΔΙΟΙΚΗΤΙΚΟ ΕΓΓΡΑΦΟ", Code = "267" };
                    MinistryDocumentType ministryDocumentType086 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "1070526/496/0015/30-7-1999 (Ελαιουργεία)", ShortTitle = "ΑΠΥ-ΔΕΛ.ΠΟΣ.ΠΑΡ", Title = "ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ", Code = "268" };
                    MinistryDocumentType ministryDocumentType087 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 παρ. 5", ShortTitle = "ΤΙΜ.ΑΓΟΡ", Title = "ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ", Code = "270" };
                    MinistryDocumentType ministryDocumentType088 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "12 παρ. 5,  11 παρ. 1", ShortTitle = "ΤΙΜ.ΑΓ.-Δ.Α.", Title = "ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ", Code = "272" };
                    MinistryDocumentType ministryDocumentType089 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1151/06-06-2001 (ελαιοτριβεία)", ShortTitle = "Α.Π.Υ-Δ.Π.Π-Δ.Α", Title = "ΑΠΟΔ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - ΔΕΛ ΠΟΣΟΤ ΠAΡΑΛΑΒΗΣ - ΔΕΛ ΑΠΟΣΤΟΛΗΣ", Code = "279" };
                    MinistryDocumentType ministryDocumentType090 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.MINUS, Description = "-", ShortTitle = "ΔΠΠ-ΤΙΜ ΑΓΟΡ", Title = "ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ-ΤΙΜΟΛΟΓΙΟ ΑΓΟΡΑΣ", Code = "295" };
                    MinistryDocumentType ministryDocumentType091 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠΠ/Δ ΕΚΜ DVD", Title = "ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ (ΕΚΜ DVD κ.λ.π.)", Code = "298" };
                    MinistryDocumentType ministryDocumentType092 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠΠ/Λ ΕΚΜ DVD", Title = "ΔΕΛΤΙΟ ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ (ΕΚΜ DVD κ.λ.π.)", Code = "299" };
                    MinistryDocumentType ministryDocumentType093 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠοσ.Παρ-ΑΠΥDVD", Title = "Δ.ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ-ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ (ΕΚΜ DVD κλπ)", Code = "301" };
                    MinistryDocumentType ministryDocumentType094 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠοσ Π/δ-ΑΠΥDVD", Title = "Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ-ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ (ΕΚΜ DVD κλπ)", Code = "302" };
                    MinistryDocumentType ministryDocumentType095 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠοσ.Παρ-ΔΑ DVD", Title = "Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΔΟΣΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ (ΕΚΜ DVD κλπ)", Code = "303" };
                    MinistryDocumentType ministryDocumentType096 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠοσ Παρ-ΔΑ DVD", Title = "Δ. ΠΟΣΟΤΙΚΗΣ ΠΑΡΑΛΑΒΗΣ - ΔΕΛΤΙΟ ΑΠΟΣΤΟΛΗΣ (ΕΚΜ DVD κλπ)", Code = "304" };
                    MinistryDocumentType ministryDocumentType097 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠΠ-ΔΑ-ΑΠΥ DVD", Title = "(ΕΚΜDVD) Δ.ΑΠΟΣΤΟΛΗΣ-Δ.ΠΟΣΟΤ ΠΑΡΑΛΑΒΗΣ-ΑΠΟΔ ΠΑΡ. ΥΠΗΡΕΣΙΩΝ", Code = "306" };
                    MinistryDocumentType ministryDocumentType098 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1063/13-04-05 και ΠΟΛ 1088/13-06-05", ShortTitle = "ΔΠΠΔ-ΔΑ-ΑΠΥ DVD", Title = "(ΕΚΜDVD) Δ.ΑΠΟΣΤΟΛΗΣ-Δ.ΠΟΣΟΤ ΠΑΡΑΔΟΣΗΣ-ΑΠΟΔ ΠΑΡ. ΥΠΗΡΕΣΙΩΝ", Code = "307" };
                    MinistryDocumentType ministryDocumentType099 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Αρθρο 10 παρ. 5θ, 5ι", ShortTitle = "ΑΠΥ-Β.ΣΤΑ-Β.ΕΙΣ", Title = "ΑΠΟΔ. ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ - Β. ΕΙΣΕΡΧΟΜΕΝΩΝ - Β. ΣΤΑΘΜΕΥΣΗΣ", Code = "308" };
                    MinistryDocumentType ministryDocumentType100 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "ΠΟΛ 1039/09-03-2006", ShortTitle = "ΠΑΡΑΔ.ΚΤΙΣΜΑΤΩΝ", Title = "ΠΑΡΑΔΟΣΗ ΚΤΙΣΜΑΤΩΝ", Code = "310" };
                    MinistryDocumentType ministryDocumentType101 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "αρθρο 10 παρ. 5ε", ShortTitle = "Β.ΠΕΛ-ΑΠΥ", Title = "ΒΙΒΛΙΟ ΠΕΛΑΤΩΝ-ΑΠΟΔΕΙΞΗ ΠΑΡΟΧΗΣ ΥΠΗΡΕΣΙΩΝ", Code = "313" };
                    MinistryDocumentType ministryDocumentType102 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "αρθρο 10 παρ. 5 περ ζ", ShortTitle = "ΔΕΛΤ. ΕΠΙΣΚΕΥΗΣ", Title = "ΔΕΛΤΙΟ ΕΠΙΣΚΕΥΗΣ", Code = "316" };
                    MinistryDocumentType ministryDocumentType103 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "αρθρο 29 παρ.3 του Ν 3522/06", ShortTitle = "Σ.Δ.Ε.", Title = "ΣΥΓΚΕΝΤΡΩΤΙΚΟ ΔΕΛΤΙΟ ΕΠΙΣΤΡΟΦΗΣ", Code = "317" };
                    MinistryDocumentType ministryDocumentType104 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.PLUS, Description = "Ν.3870/2010 άρθρο 1§5", ShortTitle = "ΕΙΣIΤ.ΕΚΔΗΛ", Title = "ΕΙΣΙΤΗΡΙΟ ΕΚΔΗΛΩΣΕΩΝ", Code = "328" };
                    MinistryDocumentType ministryDocumentType105 = new MinistryDocumentType(uow) { IsSupported = false, DocumentValueFactor = eDocumentValueFactor.COMPLICATED, Description = "-", ShortTitle = "ΤΠ-Τ(ΑΓ.ΑΓΡ)-ΔΑ", Title = "ΤΙΜΟΛΟΓΙΟ(Παροχ.Υπηρ.)-ΤΙΜ.(Αγοράς Αγρ.Προϊόντων)-ΔΕΛ. ΑΠΟΣΤ", Code = "329" };
                    MinistryDocumentType ministryDocumentType106 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "-", ShortTitle = "-", Title = "ΔΙΑΦΟΡΑ ΒΙΒΛΙΑ", Code = "500" };
                    MinistryDocumentType ministryDocumentType107 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "-", ShortTitle = "-", Title = "ΛΟΙΠΑ ΣΤΟΙΧΕΙΑ", Code = "501" };
                    MinistryDocumentType ministryDocumentType108 = new MinistryDocumentType(uow) { IsSupported = true, DocumentValueFactor = eDocumentValueFactor.NONE, Description = "-", ShortTitle = "-", Title = "ΑΝΤΙΤΥΠΟ", Code = "502" };

                    break;
            }

        }

        private void CreateMinistryTaxOffices(UnitOfWork uow, eCultureInfo currentCulture)
        {
            switch (currentCulture)  //todo Add specific for each country
            {
                default:
                    TaxOffice taxoffice001 = new TaxOffice(uow) { Code = "1101", Municipality = "ΑΘΗΝΩΝ", PostCode = "10010", Street = "ΑΝΑΞΑΓΟΡΑ 6-8", Description = "Α' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice002 = new TaxOffice(uow) { Code = "1104", Municipality = "ΑΘΗΝΩΝ", PostCode = "10681", Street = "ΚΩΛΕΤΗ 14A", Description = "Δ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice003 = new TaxOffice(uow) { Code = "1106", Municipality = "ΑΘΗΝΩΝ", PostCode = "10554", Street = "ΑΓΙΑΣ ΕΛΕΟΥΣΗΣ", Description = "ΣΤ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice004 = new TaxOffice(uow) { Code = "1112", Municipality = "ΑΘΗΝΩΝ", PostCode = "15771", Street = "ΓΡΗΓ. ΑΥΞΕΝΤΙΟΥ", Description = "ΙΒ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice005 = new TaxOffice(uow) { Code = "1113", Municipality = "ΑΘΗΝΩΝ", PostCode = "11362", Street = "ΕΥΕΛΠΙΔΩΝ", Description = "ΙΓ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice006 = new TaxOffice(uow) { Code = "1114", Municipality = "ΑΘΗΝΩΝ", PostCode = "11143", Street = "ΚΟΥΡΤΙΔΟΥ", Description = "ΙΔ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice007 = new TaxOffice(uow) { Code = "1117", Municipality = "ΑΘΗΝΩΝ", PostCode = "11632", Street = "ΔΑΜΑΡΕΩΣ", Description = "ΙΖ' ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice008 = new TaxOffice(uow) { Code = "1118", Municipality = "ΑΘΗΝΩΝ", PostCode = "15125", Street = "ΚΗΦΙΣΙΑΣ/ ΜΕΓΑΡΟ ΑΤΡΙΝΑ", Description = "ΚΕΝΤΡΟ ΕΛΕΓΧΟΥ ΜΕΓΑΛΩΝ ΕΠΙΧΕΙΡΗΣΕΩΝ (Κ.Ε.ΜΕ.ΕΠ)" };
                    TaxOffice taxoffice009 = new TaxOffice(uow) { Code = "1125", Municipality = "ΑΘΗΝΩΝ", PostCode = "10682", Street = "ΜΕΤΣΟΒΟΥ", Description = "ΚΑΤΟΙΚΩΝ ΕΞΩΤΕΡΙΚΟΥ" };
                    TaxOffice taxoffice010 = new TaxOffice(uow) { Code = "1129", Municipality = "ΑΘΗΝΩΝ", PostCode = "17342", Street = "ΑΡΓΟΣΤΟΛΙΟΥ", Description = "ΑΓΙΟΥ ΔΗΜΗΤΡΙΟΥ" };
                    TaxOffice taxoffice011 = new TaxOffice(uow) { Code = "1130", Municipality = "ΑΘΗΝΩΝ", PostCode = "17673", Street = "ΕΛ.ΒΕΝΙΖΕΛΟΥ", Description = "ΚΑΛΛΙΘΕΑΣ" };
                    TaxOffice taxoffice012 = new TaxOffice(uow) { Code = "1131", Municipality = "ΑΘΗΝΩΝ", PostCode = "14231", Street = "ΕΛ.ΒΕΝΙΖΕΛΟΥ", Description = "ΝΕΑΣ ΙΩΝΙΑΣ" };
                    TaxOffice taxoffice013 = new TaxOffice(uow) { Code = "1132", Municipality = "ΑΘΗΝΩΝ", PostCode = "17110", Street = "ΛΕΩΦ.ΣΥΓΓΡΟΥ", Description = "ΝΕΑΣ ΣΜΥΡΝΗΣ" };
                    TaxOffice taxoffice014 = new TaxOffice(uow) { Code = "1133", Municipality = "ΑΘΗΝΩΝ", PostCode = "17561", Street = "ΑΛΚΥΟΝΗΣ", Description = "ΠΑΛΑΙΟΥ ΦΑΛΗΡΟΥ" };
                    TaxOffice taxoffice015 = new TaxOffice(uow) { Code = "1134", Municipality = "ΑΘΗΝΩΝ", PostCode = "15210", Street = "ΑΓ. ΠΑΡΑΣΚΕΥΗΣ ΚΑΙ ΑΙΣΧΥΛΟΥ", Description = "ΧΑΛΑΝΔΡΙΟΥ" };
                    TaxOffice taxoffice016 = new TaxOffice(uow) { Code = "1135", Municipality = "ΑΘΗΝΩΝ", PostCode = "15124", Street = "ΑΓ. ΚΩΝΣΤΑΝΤΙΝΟΥ ΚΑΙ ΠΛΑΤΑΙΩΝ", Description = "ΑΜΑΡΟΥΣΙΟΥ" };
                    TaxOffice taxoffice017 = new TaxOffice(uow) { Code = "1136", Municipality = "ΑΘΗΝΩΝ", PostCode = "13510", Street = "ΔΗΜΟΚΡΑΤΙΑΣ & ΠΡΙΓΚ ΟΛΓΑΣ", Description = "ΑΓΙΩΝ ΑΝΑΡΓΥΡΩΝ" };
                    TaxOffice taxoffice018 = new TaxOffice(uow) { Code = "1137", Municipality = "ΑΘΗΝΩΝ", PostCode = "12241", Street = "ΑΛΑΤΣΑΤΩΝ 93 ΚΑΙ ΚΗΦΙΣΟΥ", Description = "ΑΙΓΑΛΕΩ" };
                    TaxOffice taxoffice019 = new TaxOffice(uow) { Code = "1138", Municipality = "ΑΘΗΝΩΝ", PostCode = "12136", Street = "ΤΖ. ΚΕΝΕΝΤΥ 146 ΚΑΙ ΑΙΓΑΙΟΥ", Description = "Α' ΠΕΡΙΣΤΕΡΙΟΥ" };
                    TaxOffice taxoffice020 = new TaxOffice(uow) { Code = "1139", Municipality = "ΑΘΗΝΩΝ", PostCode = "16674", Street = "ΓΟΥΝΑΡΗ", Description = "ΓΛΥΦΑΔΑΣ" };
                    TaxOffice taxoffice021 = new TaxOffice(uow) { Code = "1145", Municipality = "ΑΘΗΝΩΝ", PostCode = "14122", Street = "Λ ΗΡΑΚΛΕΙΟΥ", Description = "Ν.ΗΡΑΚΛΕΙΟΥ" };
                    TaxOffice taxoffice022 = new TaxOffice(uow) { Code = "1151", Municipality = "ΑΘΗΝΩΝ", PostCode = "15561", Street = "ΕΛ ΒΕΝΙΖΕΛΟΥ", Description = "ΧΟΛΑΡΓΟΥ" };
                    TaxOffice taxoffice023 = new TaxOffice(uow) { Code = "1152", Municipality = "ΑΘΗΝΩΝ", PostCode = "16121", Street = "ΑΝΔΡΙΑΝΟΥΠΟΛΕΩΣ", Description = "ΒΥΡΩΝΟΣ" };
                    TaxOffice taxoffice024 = new TaxOffice(uow) { Code = "1153", Municipality = "ΑΘΗΝΩΝ", PostCode = "14561", Street = "ΑΧΑΡΝΩΝ", Description = "ΚΗΦΙΣΙΑΣ" };
                    TaxOffice taxoffice025 = new TaxOffice(uow) { Code = "1157", Municipality = "ΑΘΗΝΩΝ", PostCode = "12132", Street = "ΧΡ.ΛΑΔΑ", Description = "Β' ΠΕΡΙΣΤΕΡΙΟΥ" };
                    TaxOffice taxoffice026 = new TaxOffice(uow) { Code = "1159", Municipality = "ΑΘΗΝΩΝ", PostCode = "17671", Street = "ΘΗΣΕΩΣ", Description = "Φ.Α.Ε. ΑΘΗΝΩΝ" };
                    TaxOffice taxoffice027 = new TaxOffice(uow) { Code = "1173", Municipality = "ΑΘΗΝΩΝ", PostCode = "16346", Street = "ΒΟΥΛΙΑΓΜΕΝΗΣ", Description = "ΗΛΙΟΥΠΟΛΗΣ" };
                    TaxOffice taxoffice028 = new TaxOffice(uow) { Code = "1175", Municipality = "ΑΘΗΝΩΝ", PostCode = "15410", Street = "ΚΗΦΙΣΙΑΣ", Description = "ΨΥΧΙΚΟΥ" };
                    TaxOffice taxoffice029 = new TaxOffice(uow) { Code = "1179", Municipality = "ΑΘΗΝΩΝ", PostCode = "11146", Street = "ΧΡΙΣΤΙΑΝΟΥΠΟΛΕΩΣ", Description = "ΓΑΛΑΤΣΙΟΥ" };
                    TaxOffice taxoffice030 = new TaxOffice(uow) { Code = "1201", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18535", Street = "2ΑΣ ΜΕΡΑΡΧΙΑΣ", Description = "Α' ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice031 = new TaxOffice(uow) { Code = "1203", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18510", Street = "ΚΟΛΟΚΟΤΡΩΝΗ", Description = "Γ' ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice032 = new TaxOffice(uow) { Code = "1204", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18510", Street = "Κ.ΜΑΥΡΟΜΙΧΑΛΗ", Description = "Δ' ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice033 = new TaxOffice(uow) { Code = "1205", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18648", Street = "ΣΩΚΡΑΤΟΥΣ 15-17", Description = "Ε' ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice034 = new TaxOffice(uow) { Code = "1206", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18510", Street = "ΝΟΤΑΡΑ 38-40", Description = "ΦΑΕ ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice035 = new TaxOffice(uow) { Code = "1207", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18538", Street = "ΑΚΤΗ ΜΙΑΟΥΛΗ", Description = "ΠΛΟΙΩΝ ΠΕΙΡΑΙΑ" };
                    TaxOffice taxoffice036 = new TaxOffice(uow) { Code = "1211", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18346", Street = "ΚΥΠΡΟΥ", Description = "ΜΟΣΧΑΤΟΥ" };
                    TaxOffice taxoffice037 = new TaxOffice(uow) { Code = "1220", Municipality = "ΠΕΙΡΑΙΑ", PostCode = "18450", Street = "ΚΑΙΣΑΡΕΙΑΣ", Description = "ΝΙΚΑΙΑΣ" };
                    TaxOffice taxoffice038 = new TaxOffice(uow) { Code = "1302", Municipality = "ΑΝΑΤΟΛΙΚΗΣ ΑΤΤΙΚΗΣ", PostCode = "13671", Street = "ΠΟΥΡΑΙΜΗ", Description = "ΑΧΑΡΝΩΝ" };
                    TaxOffice taxoffice039 = new TaxOffice(uow) { Code = "1303", Municipality = "ΔΥΤΙΚΗΣ ΑΤΤΙΚΗΣ", PostCode = "19200", Street = "ΕΘΝ.ΑΝΤΙΣΤΑΣΕΩΣ ΚΑΙ ΔΗΜΗΤΡΟΣ", Description = "ΕΛΕΥΣΙΝΑΣ" };
                    TaxOffice taxoffice040 = new TaxOffice(uow) { Code = "1304", Municipality = "ΑΝΑΤΟΛΙΚΗΣ ΑΤΤΙΚΗΣ", PostCode = "19400", Street = "Β.ΚΩΝ/ΝΟΥ", Description = "ΚΟΡΩΠΙΟΥ" };
                    TaxOffice taxoffice041 = new TaxOffice(uow) { Code = "1312", Municipality = "ΑΝΑΤΟΛΙΚΗΣ ΑΤΤΙΚΗΣ", PostCode = "15351", Street = "ΕΘΝΙΚΗΣ ΑΝΤΙΣΤΑΣΕΩΣ", Description = "ΠΑΛΛΗΝΗΣ" };
                    TaxOffice taxoffice042 = new TaxOffice(uow) { Code = "1411", Municipality = "ΒΟΙΩΤΙΑΣ", PostCode = "32200", Street = "ΕΠΑΜΕΙΝΩΝΔΑ", Description = "ΘΗΒΩΝ" };
                    TaxOffice taxoffice043 = new TaxOffice(uow) { Code = "1421", Municipality = "ΒΟΙΩΤΙΑΣ", PostCode = "32100", Street = "ΣΟΦΟΚΛΕΟΥΣ", Description = "ΛΙΒΑΔΕΙΑΣ" };
                    TaxOffice taxoffice044 = new TaxOffice(uow) { Code = "1531", Municipality = "ΑΙΤΩΛΟΑΚΑΡΝΑΝΙΑΣ", PostCode = "30200", Street = "ΛΙΜΑΝΙ ΜΕΣΟΛΟΓΓΙ", Description = "ΜΕΣΟΛΟΓΓΙΟΥ" };
                    TaxOffice taxoffice045 = new TaxOffice(uow) { Code = "1552", Municipality = "ΑΙΤΩΛΟΑΚΑΡΝΑΝΙΑΣ", PostCode = "30100", Street = "ΜΑΝΔΗΛΑΡΑ", Description = "ΑΓΡΙΝΙΟΥ" };
                    TaxOffice taxoffice046 = new TaxOffice(uow) { Code = "1611", Municipality = "ΕΥΡΥΤΑΝΙΑΣ", PostCode = "36100", Street = "Χ.ΤΡΙΚΟΥΠΗ", Description = "ΚΑΡΠΕΝΗΣΙΟΥ" };
                    TaxOffice taxoffice047 = new TaxOffice(uow) { Code = "1722", Municipality = "ΕΥΒΟΙΑΣ", PostCode = "34003", Street = "ΚΥΜΗ", Description = "ΚΥΜΗΣ" };
                    TaxOffice taxoffice048 = new TaxOffice(uow) { Code = "1732", Municipality = "ΕΥΒΟΙΑΣ", PostCode = "34100", Street = "ΔΗΜΑΡΧΟΥ ΣΚΟΥΡΑ-ΔΥΟ ΔΕΝΔΡΑ ΔΟΚΟΣ", Description = "ΧΑΛΚΙΔΑΣ" };
                    TaxOffice taxoffice049 = new TaxOffice(uow) { Code = "1832", Municipality = "ΦΘΙΩΤΙΔΟΣ", PostCode = "35100", Street = "ΑΝΘΗΛΗΣ & ΚΑΝΑΡΗ", Description = "ΛΑΜΙΑΣ" };
                    TaxOffice taxoffice050 = new TaxOffice(uow) { Code = "1912", Municipality = "ΦΩΚΙΔΑΣ", PostCode = "33100", Street = "ΑΘΑΝΑΣΙΟΥ ΔΙΑΚΟΥ ΚΑΙ ΑΝΔΡΟΥΤΣΟΥ", Description = "ΑΜΦΙΣΣΑΣ" };
                    TaxOffice taxoffice051 = new TaxOffice(uow) { Code = "2111", Municipality = "ΑΡΓΟΛΙΔΑΣ", PostCode = "21200", Street = "ΓΟΥΝΑΡΗ", Description = "ΑΡΓΟΥΣ" };
                    TaxOffice taxoffice052 = new TaxOffice(uow) { Code = "2131", Municipality = "ΑΡΓΟΛΙΔΑΣ", PostCode = "21100", Street = "ΗΡΑΚΛΕΟΥΣ", Description = "ΝΑΥΠΛΙΟΥ" };
                    TaxOffice taxoffice053 = new TaxOffice(uow) { Code = "2231", Municipality = "ΑΡΚΑΔΙΑΣ", PostCode = "22100", Street = "ΛΑΓΟΠΑΤΗ", Description = "ΤΡΙΠΟΛΗΣ" };
                    TaxOffice taxoffice054 = new TaxOffice(uow) { Code = "2311", Municipality = "ΑΧΑΙΑΣ", PostCode = "25100", Street = "ΚΑΝΕΛΛΟΠΟΥΛΩΝ", Description = "ΑΙΓΙΟΥ" };
                    TaxOffice taxoffice055 = new TaxOffice(uow) { Code = "2331", Municipality = "ΑΧΑΙΑΣ", PostCode = "26110", Street = "ΚΑΝΑΚΑΡΗ 84-86", Description = "Α' ΠΑΤΡΩΝ" };
                    TaxOffice taxoffice056 = new TaxOffice(uow) { Code = "2334", Municipality = "ΑΧΑΙΑΣ", PostCode = "26331", Street = "ΑΚΤΗ ΔΥΜΑΙΩΝ", Description = "Γ' ΠΑΤΡΩΝ" };
                    TaxOffice taxoffice057 = new TaxOffice(uow) { Code = "2411", Municipality = "ΗΛΕΙΑΣ", PostCode = "27200", Street = "ΔΕΛΗΓΙΑΝΝΗ", Description = "ΑΜΑΛΙΑΔΑΣ" };
                    TaxOffice taxoffice058 = new TaxOffice(uow) { Code = "2412", Municipality = "ΗΛΕΙΑΣ", PostCode = "27100", Street = "ΚΑΤΑΚΟΛΟΥ ΚΑΙ ΠΡΟΥΣΣΗΣ", Description = "ΠΥΡΓΟΥ" };
                    TaxOffice taxoffice059 = new TaxOffice(uow) { Code = "2513", Municipality = "ΚΟΡΙΝΘΙΑΣ", PostCode = "20100", Street = "ΠΑΤΡΩΝ", Description = "ΚΟΡΙΝΘΟΥ" };
                    TaxOffice taxoffice060 = new TaxOffice(uow) { Code = "2632", Municipality = "ΛΑΚΩΝΙΑΣ", PostCode = "23100", Street = "ΜΕΝΕΛΑΟΥ", Description = "ΣΠΑΡΤΗΣ" };
                    TaxOffice taxoffice061 = new TaxOffice(uow) { Code = "2711", Municipality = "ΜΕΣΣΗΝΙΑΣ", PostCode = "24101", Street = "ΕΥΑΓΓΕΛΙΣΤΡΙΑΣ", Description = "ΚΑΛΑΜΑΤΑΣ" };
                    TaxOffice taxoffice062 = new TaxOffice(uow) { Code = "3111", Municipality = "ΚΑΡΔΙΤΣΑΣ", PostCode = "43100", Street = "Α ΠΑΠΑΝΔΡΕΟΥ", Description = "ΚΑΡΔΙΤΣΑΣ" };
                    TaxOffice taxoffice063 = new TaxOffice(uow) { Code = "3231", Municipality = "ΛΑΡΙΣΗΣ", PostCode = "41110", Street = "ΠΑΠΑΝΑΣΤΑΣΙΟΥ", Description = "Α' ΛΑΡΙΣΑΣ" };
                    TaxOffice taxoffice064 = new TaxOffice(uow) { Code = "3232", Municipality = "ΛΑΡΙΣΗΣ", PostCode = "41110", Street = "ΠΑΤΡΟΚΛΟΥ", Description = "Β' ΛΑΡΙΣΑΣ" };
                    TaxOffice taxoffice065 = new TaxOffice(uow) { Code = "3321", Municipality = "ΜΑΓΝΗΣΙΑΣ", PostCode = "38333", Street = "ΞΕΝΟΦΩΝΤΟΣ", Description = "ΒΟΛΟΥ" };
                    TaxOffice taxoffice066 = new TaxOffice(uow) { Code = "3323", Municipality = "ΜΑΓΝΗΣΙΑΣ", PostCode = "38446", Street = "ΤΕΡΜΑ ΑΓ.ΝΕΚΤΑΡΙΟΥ", Description = "Ν.ΙΩΝΙΑΣ ΒΟΛΟΥ" };
                    TaxOffice taxoffice067 = new TaxOffice(uow) { Code = "3412", Municipality = "ΤΡΙΚΑΛΩΝ", PostCode = "42100", Street = "ΚΟΛΟΚΟΤΡΩΝΗ ΚΑΙ ΠΤΟΛΕΜΑΙΩΝ", Description = "ΤΡΙΚΑΛΩΝ" };
                    TaxOffice taxoffice068 = new TaxOffice(uow) { Code = "4112", Municipality = "ΗΜΑΘΙΑΣ", PostCode = "59100", Street = "ΛΕΩΦ.ΘΕΣΣΑΛΟΝΙΚΗΣ", Description = "ΒΕΡΟΙΑΣ" };
                    TaxOffice taxoffice069 = new TaxOffice(uow) { Code = "4211", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54629", Street = "ΤΑΝΤΑΛΟΥ", Description = "Α' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice070 = new TaxOffice(uow) { Code = "4214", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54623", Street = "ΒΑΣ.ΗΡΑΚΛΕΙΟΥ", Description = "Δ' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice071 = new TaxOffice(uow) { Code = "4215", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54626", Street = "ΚΡΥΣΤΑΛΛΗ", Description = "Ε' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice072 = new TaxOffice(uow) { Code = "4216", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54640", Street = "ΤΑΚΑΝΤΖΑ", Description = "ΣΤ' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice073 = new TaxOffice(uow) { Code = "4217", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54008", Street = "Ν.ΠΛΑΣΤΗΡΑ", Description = "Ζ' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice074 = new TaxOffice(uow) { Code = "4222", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "57200", Street = "ΛΟΥΤΡΩΝ", Description = "ΛΑΓΚΑΔΑ" };
                    TaxOffice taxoffice075 = new TaxOffice(uow) { Code = "4224", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54012", Street = "26ΗΣ ΟΚΤΩΒΡΙΟΥ", Description = "ΦΑΕ ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice076 = new TaxOffice(uow) { Code = "4228", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54008", Street = "ΒΑΣ.ΟΛΓΑΣ", Description = "Η' ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice077 = new TaxOffice(uow) { Code = "4232", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "55134", Street = "ΕΘΝΙΚΗΣ ΑΝΤΙΣΤΑΣΕΩΣ", Description = "ΚΑΛΑΜΑΡΙΑΣ" };
                    TaxOffice taxoffice078 = new TaxOffice(uow) { Code = "4233", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "54630", Street = "ΕΙΡΗΝΗΣ", Description = "ΑΜΠΕΛΟΚΗΠΩΝΥ" };
                    TaxOffice taxoffice079 = new TaxOffice(uow) { Code = "4234", Municipality = "ΘΕΣΣΑΛΟΝΙΚΗΣ", PostCode = "57008", Street = "ΒΑΣ.ΓΕΩΡΓΙΟΥ ΚΑΙ ΑΘ.ΔΙΑΚΟΥ", Description = "ΙΩΝΙΑΣ ΘΕΣΣΑΛΟΝΙΚΗΣ" };
                    TaxOffice taxoffice080 = new TaxOffice(uow) { Code = "4311", Municipality = "ΚΑΣΤΟΡΙΑΣ", PostCode = "52100", Street = "ΛΕΩΦ.ΚΥΚΝΩΝ", Description = "ΚΑΣΤΟΡΙΑΣ" };
                    TaxOffice taxoffice081 = new TaxOffice(uow) { Code = "4411", Municipality = "ΚΙΛΚΙΣ", PostCode = "61100", Street = "21ΗΣ ΙΟΥΝΙΟΥ", Description = "ΚΙΛΚΙΣ" };
                    TaxOffice taxoffice082 = new TaxOffice(uow) { Code = "4521", Municipality = "ΓΡΕΒΕΝΩΝ", PostCode = "51100", Street = "Κ ΤΑΛΙΑΔΟΥΡΗ", Description = "ΓΡΕΒΕΝΩΝ" };
                    TaxOffice taxoffice083 = new TaxOffice(uow) { Code = "4531", Municipality = "ΚΟΖΑΝΗΣ", PostCode = "50200", Street = "ΦΙΛΙΠΠΟΥ", Description = "ΠΤΟΛΕΜΑΙΔΑΣ" };
                    TaxOffice taxoffice084 = new TaxOffice(uow) { Code = "4541", Municipality = "ΚΟΖΑΝΗΣ", PostCode = "50100", Street = "ΓΚΕΡΤΣΟΥ", Description = "ΚΟΖΑΝΗΣ" };
                    TaxOffice taxoffice085 = new TaxOffice(uow) { Code = "4621", Municipality = "ΠΕΛΛΑΣ", PostCode = "58100", Street = "Κ.ΣΤΑΜΚΟΥ", Description = "ΓΙΑΝΝΙΤΣΩΝ" };
                    TaxOffice taxoffice086 = new TaxOffice(uow) { Code = "4631", Municipality = "ΠΕΛΛΑΣ", PostCode = "58200", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΕΔΕΣΣΑΣ" };
                    TaxOffice taxoffice087 = new TaxOffice(uow) { Code = "4711", Municipality = "ΠΙΕΡΙΑΣ", PostCode = "60100", Street = "ΧΑΝΤΖΟΓΛΟΥ", Description = "ΚΑΤΕΡΙΝΗΣ" };
                    TaxOffice taxoffice088 = new TaxOffice(uow) { Code = "4812", Municipality = "ΦΛΩΡΙΝΗΣ", PostCode = "53100", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΦΛΩΡΙΝΑΣ" };
                    TaxOffice taxoffice089 = new TaxOffice(uow) { Code = "4922", Municipality = "ΧΑΛΚΙΔΙΚΗΣ", PostCode = "63100", Street = "ΧΑΡΙΛΑΟΥ ΤΡΙΚΟΥΠΗ", Description = "ΠΟΛΥΓΥΡΟΥ" };
                    TaxOffice taxoffice090 = new TaxOffice(uow) { Code = "4923", Municipality = "ΧΑΛΚΙΔΙΚΗΣ", PostCode = "63200", Street = "ΚΟΥΤΣΑΝΤΩΝΗ", Description = "ΝΕΩΝ ΜΟΥΔΑΝΙΩΝ" };
                    TaxOffice taxoffice091 = new TaxOffice(uow) { Code = "5111", Municipality = "ΔΡΑΜΑΣ", PostCode = "66100", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΔΡΑΜΑΣ" };
                    TaxOffice taxoffice092 = new TaxOffice(uow) { Code = "5211", Municipality = "ΕΒΡΟΥ", PostCode = "68100", Street = "ΑΓ ΔΗΜΗΤΡΙΟΥ", Description = "ΑΛΕΞΑΝΔΡΟΥΠΟΛΗΣ" };
                    TaxOffice taxoffice093 = new TaxOffice(uow) { Code = "5231", Municipality = "ΕΒΡΟΥ", PostCode = "68200", Street = "ΙΠΠΟΚΡΑΤΟΥΣ", Description = "ΟΡΕΣΤΙΑΔΑΣ" };
                    TaxOffice taxoffice094 = new TaxOffice(uow) { Code = "5321", Municipality = "ΚΑΒΑΛΑΣ", PostCode = "65110", Street = "ΕΘΝ.ΑΝΤΙΣΤΑΣΕΩΣ", Description = "ΚΑΒΑΛΑΣ" };
                    TaxOffice taxoffice095 = new TaxOffice(uow) { Code = "5411", Municipality = "ΞΑΝΘΗΣ", PostCode = "67100", Street = "ΜΕΣΟΛΟΓΓΙΟΥ", Description = "ΞΑΝΘΗΣ" };
                    TaxOffice taxoffice096 = new TaxOffice(uow) { Code = "5511", Municipality = "ΡΟΔΟΠΗΣ", PostCode = "69100", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΚΟΜΟΤΗΝΗΣ" };
                    TaxOffice taxoffice097 = new TaxOffice(uow) { Code = "5621", Municipality = "ΣΕΡΡΩΝ", PostCode = "62125", Street = "ΜΕΡΑΡΧΙΑΣ", Description = "ΣΕΡΡΩΝ" };
                    TaxOffice taxoffice098 = new TaxOffice(uow) { Code = "6111", Municipality = "ΑΡΤΑΣ", PostCode = "47100", Street = "ΠΕΡΙΦΕΡΕΙΑΚΗ 56", Description = "ΑΡΤΑΣ" };
                    TaxOffice taxoffice099 = new TaxOffice(uow) { Code = "6211", Municipality = "ΘΕΣΠΡΩΤΙΑΣ", PostCode = "46100", Street = "ΕΛΕΥΘΕΡΙΑΣ", Description = "ΗΓΟΥΜΕΝΙΤΣΑΣ" };
                    TaxOffice taxoffice100 = new TaxOffice(uow) { Code = "6311", Municipality = "ΙΩΑΝΝΙΝΩΝ", PostCode = "45332", Street = "ΔΟΜΠΟΛΗ", Description = "ΙΩΑΝΝΙΝΩΝ" };
                    TaxOffice taxoffice101 = new TaxOffice(uow) { Code = "6411", Municipality = "ΠΡΕΒΕΖΗΣ", PostCode = "48100", Street = "ΠΟΛΥΤΕΧΝΕΙΟΥ", Description = "ΠΡΕΒΕΖΑΣ" };
                    TaxOffice taxoffice102 = new TaxOffice(uow) { Code = "7121", Municipality = "ΚΥΚΛΑΔΩΝ", PostCode = "84700", Street = "ΘΗΡΑ", Description = "ΘΗΡΑΣ" };
                    TaxOffice taxoffice103 = new TaxOffice(uow) { Code = "7151", Municipality = "ΚΥΚΛΑΔΩΝ", PostCode = "84300", Street = "ΧΩΡΑ ΝΑΞΟΥ", Description = "ΝΑΞΟΥ" };
                    TaxOffice taxoffice104 = new TaxOffice(uow) { Code = "7161", Municipality = "ΚΥΚΛΑΔΩΝ", PostCode = "84400", Street = "ΠΑΡΟΙΚΙΑ", Description = "ΠΑΡΟΥ" };
                    TaxOffice taxoffice105 = new TaxOffice(uow) { Code = "7171", Municipality = "ΚΥΚΛΑΔΩΝ", PostCode = "84100", Street = "ΝΙΚΗΦΟΡΟΥ ΜΑΝΔΗΛΑΡΑ", Description = "ΣΥΡΟΥ" };
                    TaxOffice taxoffice106 = new TaxOffice(uow) { Code = "7172", Municipality = "ΚΥΚΛΑΔΩΝ", PostCode = "84600", Street = "ΔΡΑΦΑΚΙ", Description = "ΜΥΚΟΝΟΥ" };
                    TaxOffice taxoffice107 = new TaxOffice(uow) { Code = "7231", Municipality = "ΛΕΣΒΟΥ", PostCode = "81100", Street = "Π.ΚΟΥΝΤΟΥΡΙΩΤΟΥ", Description = "ΜΥΤΙΛΗΝΗΣ" };
                    TaxOffice taxoffice108 = new TaxOffice(uow) { Code = "7322", Municipality = "ΣΑΜΟΥ", PostCode = "83100", Street = "ΣΑΜΟΣ", Description = "ΣΑΜΟΥ" };
                    TaxOffice taxoffice109 = new TaxOffice(uow) { Code = "7411", Municipality = "ΧΙΟΥ", PostCode = "82100", Street = "Μ.ΛΙΒΑΝΟΥ", Description = "ΧΙΟΥ" };
                    TaxOffice taxoffice110 = new TaxOffice(uow) { Code = "7531", Municipality = "ΔΩΔΕΚΑΝΗΣΟΥ", PostCode = "85300", Street = "Λ.ΚΟΥΝΤΟΥΡΙΩΤΟΥ", Description = "ΚΩ" };
                    TaxOffice taxoffice111 = new TaxOffice(uow) { Code = "7542", Municipality = "ΔΩΔΕΚΑΝΗΣΟΥ", PostCode = "85100", Street = "ΓΕΩΡΓΙΟΥ ΜΑΥΡΟΥ", Description = "ΡΟΔΟΥ" };
                    TaxOffice taxoffice112 = new TaxOffice(uow) { Code = "8110", Municipality = "ΗΡΑΚΛΕΙΟΥ", PostCode = "71409", Street = "Λ.ΚΝΩΣΣΟΥ & ΝΑΘΕΝΑ", Description = "ΗΡΑΚΛΕΙΟΥ" };
                    TaxOffice taxoffice113 = new TaxOffice(uow) { Code = "8221", Municipality = "ΛΑΣΙΘΙΟΥ", PostCode = "72100", Street = "ΕΠΙΜΕΝΙΔΟΥ", Description = "ΑΓΙΟΥ ΝΙΚΟΛΑΟΥ" };
                    TaxOffice taxoffice114 = new TaxOffice(uow) { Code = "8341", Municipality = "ΡΕΘΥΜΝΟΥ", PostCode = "74100", Street = "ΣΤΑΜΑΘΙΟΥΔΑΚΗ", Description = "ΡΕΘΥΜΝΟΥ" };
                    TaxOffice taxoffice115 = new TaxOffice(uow) { Code = "8431", Municipality = "ΧΑΝΙΩΝ", PostCode = "73134", Street = "ΤΖΑΝΑΚΑΚΗ", Description = "ΧΑΝΙΩΝ" };
                    TaxOffice taxoffice116 = new TaxOffice(uow) { Code = "9111", Municipality = "ΖΑΚΥΝΘΟΥ", PostCode = "29100", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΖΑΚΥΝΘΟΥ" };
                    TaxOffice taxoffice117 = new TaxOffice(uow) { Code = "9211", Municipality = "ΚΕΡΚΥΡΑΣ", PostCode = "49100", Street = "ΣΑΜΑΡΑ", Description = "ΚΕΡΚΥΡΑΣ" };
                    TaxOffice taxoffice118 = new TaxOffice(uow) { Code = "9311", Municipality = "ΚΕΦΑΛΛΗΝΙΑΣ", PostCode = "28100", Street = "ΔΙΟΙΚΗΤΗΡΙΟ", Description = "ΑΡΓΟΣΤΟΛΙΟΥ" };
                    TaxOffice taxoffice119 = new TaxOffice(uow) { Code = "9421", Municipality = "ΛΕΥΚΑΔΑΣ", PostCode = "31100", Street = "8ΗΣ ΜΕΡΑΡΧΙΑΣ", Description = "ΛΕΥΚΑΔΑΣ" };

                    break;
            }

        }

        private void CreateVersionInfoTable(UnitOfWork uow, bool goToLastVersion = true)
        {
            Type[] scripts = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Migration))).ToArray();

            List<RetailMigration> versions = new List<RetailMigration>();

            foreach (Type script in scripts)
            {
                RetailMigration versionInfo = script.GetCustomAttributes(typeof(RetailMigration), false).FirstOrDefault() as RetailMigration;
                versions.Add(versionInfo);
            }

            if (this.Settings.DatabaseType == DBType.SQLServer)
            {
                uow.ExecuteNonQuery(@"CREATE TABLE VersionInfo(Version bigint,AppliedOn datetime);");
                if (goToLastVersion)
                {
                    foreach (RetailMigration versionInfo in versions.OrderBy(x => x.Version))
                    {
                        long version = versionInfo.Version;
                        uow.ExecuteNonQuery(
                            String.Format(@"INSERT INTO VersionInfo (Version,AppliedOn) VALUES({0},GETDATE()) ;",
                            version)
                        );
                    }
                }
            }
            else if (this.Settings.DatabaseType == DBType.postgres)
            {
                uow.ExecuteNonQuery(@"CREATE TABLE ""VersionInfo""(""Version"" bigint,""AppliedOn"" timestamp);");
                if (goToLastVersion)
                {
                    foreach (RetailMigration versionInfo in versions.OrderBy(x => x.Version))
                    {
                        long version = versionInfo.Version;
                        uow.ExecuteNonQuery(
                            String.Format(@"INSERT INTO ""VersionInfo"" (""Version"",""AppliedOn"") VALUES({0},current_timestamp) ;",
                            version)
                        );
                    }
                }
            }
            else if (this.Settings.DatabaseType == DBType.Oracle)
            {
                uow.ExecuteNonQuery(@"CREATE TABLE ""VersionInfo""(
                                                             ""Version"" NUMBER(19,0),
                                                             ""AppliedOn"" DATE)
                                                             PCTFREE     10
                                                             INITRANS    1
                                                             MAXTRANS    255
                                                             NOCACHE
                                                             MONITORING
                                                             NOPARALLEL
                                                             LOGGING");
                if (goToLastVersion)
                {
                    foreach (RetailMigration versionInfo in versions.OrderBy(x => x.Version))
                    {
                        long version = versionInfo.Version;
                        uow.ExecuteNonQuery(
                            String.Format(@"INSERT INTO ""VersionInfo"" (""Version"",""AppliedOn"") VALUES({0},CURRENT_DATE)",
                            version)
                        );
                    }
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
        }

        private void LoadXMLConfig()
        {

            if (File.Exists(filePath))
            {
                logger.Message("Started reading config file :" + filePath + Environment.NewLine);
                ClearBindings();
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(WebConfigurationSettings));
                    using (StreamReader stream = new StreamReader(filePath))
                    {
                        this.Settings = serializer.Deserialize(stream) as WebConfigurationSettings;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error during parsing file :" + filePath + Environment.NewLine);
                    logger.Error(ex.Message + Environment.NewLine);
                }
            }
            SetBindings();
            Settings.ApplicationInstance = Program.ApplicationInstance;

        }

        private void SetBindings()
        {
            retailRadioButton.DataBindings.Add("Checked", this.Settings, "RetailMaster");
            storeControllerRadioButton.DataBindings.Add("Checked", this.Settings, "RetailStoreController");
            dualModeRadioButton.DataBindings.Add("Checked", this.Settings, "RetailDual");

            txtServer.DataBindings.Add("Text", this.Settings, "SqlServer");
            txtDatabase.DataBindings.Add("Text", this.Settings, "Database");
            txtDBUsername.DataBindings.Add("Text", this.Settings, "Username");
            txtDBPassword.DataBindings.Add("Text", this.Settings, "Password");
            cmbDBType.DataBindings.Add("SelectedItem", this.Settings, "DatabaseType");

            txtSMTPHost.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "SMTPHost");
            txtPort.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "Port");
            txtSMTPHostUsername.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "Username");
            txtSMTPHostPassword.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "Password");
            txtDomain.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "Domain");
            txtEmailFrom.DataBindings.Add("Text", this.Settings.RetailBridgeEmailSettings, "From");

            txtMasterURL.DataBindings.Add("Text", this.Settings.StoreControllerSettings, "MasterURL");
            txtStoreControllerID.DataBindings.Add("Text", this.Settings.StoreControllerSettings, "ID");
            txtStoreControllerUserName.DataBindings.Add("Text", this.Settings.StoreControllerSettings, "WebUsername");
            txtStoreControllerPassword.DataBindings.Add("Text", this.Settings.StoreControllerSettings, "WebPassword");

            txtOlapServer.DataBindings.Add("Text", this.Settings, "OlapServer");
            txtOlapPassword.DataBindings.Add("Text", this.Settings, "OlapPassword");
            txtOlapUsername.DataBindings.Add("Text", this.Settings, "OlapUsername");

            chkEnableSSL.DataBindings.Add("Checked", this.Settings.RetailBridgeEmailSettings, "EnableSSL");

            chkMiniProfiler.DataBindings.Add("Checked", this.Settings, "EnableMiniProfiler");
            txtIISCache.DataBindings.Add("Text", this.Settings, "IISCache");
            txtUpdaterBatchSize.DataBindings.Add("Text", this.Settings, "UpdaterBatchSize");
            txtLicenseServer.DataBindings.Add("Text", this.Settings, "LicenseServerURL");

        }

        private void ClearBindings()
        {
            retailRadioButton.DataBindings.Clear();
            storeControllerRadioButton.DataBindings.Clear();
            dualModeRadioButton.DataBindings.Clear();

            txtServer.DataBindings.Clear();
            txtDatabase.DataBindings.Clear();
            txtDBUsername.DataBindings.Clear();
            txtDBPassword.DataBindings.Clear();
            cmbDBType.DataBindings.Clear();

            txtSMTPHost.DataBindings.Clear();
            txtPort.DataBindings.Clear();
            txtSMTPHostUsername.DataBindings.Clear();
            txtSMTPHostPassword.DataBindings.Clear();
            txtDomain.DataBindings.Clear();
            txtEmailFrom.DataBindings.Clear();

            txtMasterURL.DataBindings.Clear();
            txtStoreControllerID.DataBindings.Clear();
            txtStoreControllerUserName.DataBindings.Clear();
            txtStoreControllerPassword.DataBindings.Clear();

            txtOlapServer.DataBindings.Clear();
            txtOlapPassword.DataBindings.Clear();
            txtOlapPassword.DataBindings.Clear();

            chkEnableSSL.DataBindings.Clear();

            chkMiniProfiler.DataBindings.Clear();
            txtIISCache.DataBindings.Clear();
            txtUpdaterBatchSize.DataBindings.Clear();
            txtLicenseServer.DataBindings.Clear();
        }

        private void btnResetConfiguration_Click(object sender, EventArgs e)
        {
            LoadXMLConfig();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            richtxtBoxLog.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSaveConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                SetConnectionProperties();
                if (storeControllerRadioButton.Checked)
                {
                    try
                    {
                        logger.Message("Trying to get Store from service: " + this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName + Environment.NewLine);
                        this.Settings.StoreControllerSettings.CurrentStore = GetStoreGuidFromService(this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName,
                            this.Settings.StoreControllerSettings.ID, this.Settings.StoreControllerSettings.WebUsername, this.Settings.StoreControllerSettings.WebPassword);
                        if (this.Settings.StoreControllerSettings.CurrentStore == Guid.Empty)
                        {
                            throw new Exception("ID/User/Supplier/Store not found.");
                        }
                        logger.Message("Success: " + this.Settings.StoreControllerSettings.CurrentStore.ToString() + Environment.NewLine);

                        logger.Message("Trying to get Default Customer from service: " + this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName + Environment.NewLine);
                        this.Settings.StoreControllerSettings.DefaultCustomer = GetDefaultCustomerGuidFromService(this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName,
                            this.Settings.StoreControllerSettings.ID, this.Settings.StoreControllerSettings.WebUsername, this.Settings.StoreControllerSettings.WebPassword);
                        if (this.Settings.StoreControllerSettings.DefaultCustomer == Guid.Empty)
                        {
                            throw new Exception("ID/User/Customer not found.");
                        }
                        logger.Message("Success: " + this.Settings.StoreControllerSettings.DefaultCustomer.ToString() + Environment.NewLine);

                        logger.Message("Trying to get Store Controller ID from service: " + this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName + Environment.NewLine);
                        this.Settings.StoreControllerSettings.StoreController = GetStoreControllerGuidFromService(this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName,
                            this.Settings.StoreControllerSettings.ID, this.Settings.StoreControllerSettings.WebUsername, this.Settings.StoreControllerSettings.WebPassword);
                        if (this.Settings.StoreControllerSettings.StoreController == Guid.Empty)
                        {
                            throw new Exception("ID/User/Customer not found.");
                        }
                        logger.Message("Success: " + this.Settings.StoreControllerSettings.StoreController.ToString() + Environment.NewLine);

                        logger.Message("Trying to get and create Owner from service: " + this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName + Environment.NewLine);

                        string result;
                        if (CreateOwnerFromService(this.Settings.StoreControllerSettings.MasterURL.TrimEnd('/') + "/" + serviceName,
                            this.Settings.StoreControllerSettings.ID, this.Settings.StoreControllerSettings.WebUsername, this.Settings.StoreControllerSettings.WebPassword, out result))
                        {
                            logger.Message("Success: Owner created." + Environment.NewLine);
                        }
                        else
                        {
                            throw new Exception("Error creating owner: " + Environment.NewLine + result);
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.Error("An error occured: " + ex.Message + Environment.NewLine);
                    }
                }
                else if (dualModeRadioButton.Checked)
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {

                        Store store = uow.FindObject<Store>(new BinaryOperator("Code", txtStoreCode.Text));
                        if (store == null)
                        {
                            throw new Exception("Store with Code: '" + txtStoreCode.Text + "' not found.");
                        }
                        this.Settings.StoreControllerSettings.CurrentStore = store.Oid;
                        StoreControllerSettings scs = uow.FindObject<StoreControllerSettings>(null);
                        this.Settings.StoreControllerSettings.DefaultCustomer = scs.DefaultCustomer.Oid;
                        this.Settings.StoreControllerSettings.StoreController = scs.Oid;
                        this.Settings.StoreControllerSettings.ID = scs.ID.ToString();
                    }
                }
                XmlSerializer serializer = new XmlSerializer(typeof(WebConfigurationSettings));
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (StreamWriter stream = new StreamWriter(filePath))
                {
                    serializer.Serialize(stream, this.Settings);
                }

                logger.Success("Configuration Saved Successfully");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + Environment.NewLine);
            }
        }

        public void UpgradeDatabase()
        {
            using (IDbConnection connection = ConnectionHelper.GetConnection())
            {
                connection.Open();
                IDbCommand command = connection.CreateCommand();
                try
                {
                    switch (XpoHelper.databasetype)
                    {
                        case DBType.SQLServer:
                            command.CommandText = "select * from VersionInfo";
                            break;
                        case DBType.postgres:
                            command.CommandText = @"select * from ""VersionInfo""";
                            break;
                        case DBType.Oracle:
                            command.CommandText = @"select * from ""VersionInfo""";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    IDataReader reader = command.ExecuteReader();
                }
                catch (NotImplementedException ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                    throw;
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                    switch (XpoHelper.databasetype)
                    {
                        case DBType.SQLServer:
                            command.CommandText = "CREATE TABLE VersionInfo(Version bigint,AppliedOn datetime);";
                            break;
                        case DBType.postgres:
                            command.CommandText = @"CREATE TABLE ""VersionInfo""(""Version"" bigint,""AppliedOn"" timestamp);";
                            break;
                        case DBType.Oracle:
                            command.CommandText = @"CREATE TABLE ""VersionInfo""(
                                                             ""Version"" NUMBER(19,0),
                                                             ""AppliedOn"" DATE)
                                                             PCTFREE     10
                                                             INITRANS    1
                                                             MAXTRANS    255
                                                             NOCACHE
                                                             MONITORING
                                                             NOPARALLEL
                                                             LOGGING";
                            break;
                        default:
                            throw;
                    }
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        string eMessage = e.GetFullMessage();
                        throw;
                    }
                }
            }
            string connectionString = ConnectionHelper.GetConnectionStringForIDbConnection();
            Announcer announcer = new TextWriterAnnouncer(s => Debug.WriteLine(s));
            announcer.ShowSql = true;

            Assembly assembly = Assembly.GetExecutingAssembly();
            IRunnerContext migrationContext = new RunnerContext(announcer);
            var options = new ProcessorOptions
            {
                PreviewOnly = false,  // set to true to see the SQL
                Timeout = 0
            };
            MigrationProcessorFactory factory;
            switch (XpoHelper.databasetype)
            {
                case DBType.postgres:
                    factory = new FluentMigrator.Runner.Processors.Postgres.PostgresProcessorFactory();
                    //new PostgreSqlProviderFactory();
                    break;
                case DBType.Oracle:
                    factory = new FluentMigrator.Runner.Processors.Oracle.OracleProcessorFactory();// FluentMigrator.Runner.Processors.Oracle.OracleProcessorFactory();
                    break;
                default:
                    factory = new SqlServer2008ProcessorFactory();
                    break;
            }

            if (XpoHelper.databasetype == DBType.Oracle)
            {
                FluentMigrator.Runner.Processors.Oracle.OracleProcessor oraProcessor = factory.Create(connectionString, announcer, options) as FluentMigrator.Runner.Processors.Oracle.OracleProcessor;
                oraProcessor.Connection.Open();
                var runner = new MigrationRunner(assembly, migrationContext, oraProcessor);
                runner.ListMigrations();
                runner.MigrateUp(true);
                XpoHelper.UpdateDatabase();
            }
            else
            {
                IMigrationProcessor processor = factory.Create(connectionString, announcer, options);
                var runner = new MigrationRunner(assembly, migrationContext, processor);
                runner.ListMigrations();
                runner.MigrateUp(true);
                XpoHelper.UpdateDatabase();
            }

        }

        public void btnUpgradeDB_Click(object sender, EventArgs e)
        {
            try
            {
                SetConnectionProperties();
                long modelVersion = BasicObj.schemaVersion;
                long migrationVersion = VersionHelper.GetMigrationVersion();
                if (XpoHelper.databasetype == DBType.Oracle)
                {
                    if (modelVersion > migrationVersion && migrationVersion > 1)
                    {

                        logger.Message("Migration started at " + DateTime.Now.ToString() + Environment.NewLine);
                        VersionHelper.FixVersionInfoTableName();
                        UpgradeDatabase();
                        logger.Success("Migration successfully finished at " + DateTime.Now.ToString() + Environment.NewLine);
                    }
                    else
                    {
                        SetConnectionProperties();
                        VersionHelper.FixVersionInfoTableName();
                        logger.Success("All Migrations has been executed" + Environment.NewLine);
                    }
                }
                else
                {
                    logger.Message("Migration started at " + DateTime.Now.ToString() + Environment.NewLine);
                    VersionHelper.FixVersionInfoTableName();
                    UpgradeDatabase();
                    logger.Success("Migration successfully finished at " + DateTime.Now.ToString() + Environment.NewLine);
                }
            }
            catch (Exception exc)
            {
                string error_message = Environment.NewLine + " Error: " + exc.Message + Environment.NewLine;
                if (exc.InnerException != null && exc.InnerException.Message != null && exc.InnerException.Message != "")
                {
                    error_message += exc.InnerException.Message + Environment.NewLine;
                }
                error_message += Environment.NewLine;
                error_message += exc.StackTrace;
                error_message += Environment.NewLine;
                if (logger != null)
                {
                    logger.Error(error_message);
                }
            }


        }

        public void MainForm_Load(object sender, EventArgs e)
        {
            cmbDBType.SelectedIndex = 0;
            logger = new LogHelper(richtxtBoxLog);
            comboBoxLanguages.DataSource = Enum.GetValues(typeof(eCultureInfo));
            cmbDBType.DataSource = new List<DBType>() { DBType.SQLServer, DBType.postgres, DBType.Oracle };
            //Enum.GetValues(typeof(DBType));            
            LoadXMLConfig();
        }

        private void btnCheckVersions_Click(object sender, EventArgs e)
        {
            SetConnectionProperties();
            VersionHelper.FixVersionInfoTableName();

            long modelVersion = BasicObj.schemaVersion;
            logger.Message("Model Version: " + modelVersion.ToString() + Environment.NewLine);
            try
            {
                long migrationVersion = VersionHelper.GetMigrationVersion();
                logger.Message("Migration Version: " + migrationVersion.ToString() + Environment.NewLine);

                if (migrationVersion == modelVersion)
                {
                    logger.Success("All migrations have already taken place." + Environment.NewLine);
                }
                else if (migrationVersion < modelVersion)
                {
                    logger.Warning("You should run migrations newer than " + migrationVersion + ".Please press Upgrade Database." + Environment.NewLine);
                }
                else if (migrationVersion > modelVersion)
                {
                    logger.Error("Migration Version " + migrationVersion + " is greater than Model Version :" + modelVersion + "!Please contact youw administrator!" + Environment.NewLine);
                }

            }
            catch (Exception exc)
            {
                string error_message = Environment.NewLine + " Error: " + exc.Message + Environment.NewLine;
                if (exc.InnerException != null && exc.InnerException.Message != null && exc.InnerException.Message != "")
                {
                    error_message += exc.InnerException.Message + Environment.NewLine;
                }
                error_message += Environment.NewLine;

                logger.Error(error_message);
            }
        }

        private Guid GetStoreGuidFromService(string url, string id, string username, string password)
        {
            RetailService.RetailService service = new RetailService.RetailService();
            service.Url = url;
            return service.GetStoreOfStoreController(id, username, password);
        }

        private Guid GetDefaultCustomerGuidFromService(string url, string id, string username, string password)
        {
            RetailService.RetailService service = new RetailService.RetailService();
            service.Url = url;
            return service.GetDefaultCustomerOfStoreController(id, username, password);
        }


        private bool CreateOwnerFromService(string url, string storeControllerID, string username, string password, out string result)
        {
            try
            {
                result = "success";
                RetailService.RetailService service = new RetailService.RetailService();
                service.Url = url;
                string json = service.GetOwner(storeControllerID, username, password);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CompanyNew owner = new CompanyNew(uow);
                    string error;
                    owner.FromJson(json, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    owner.UpdatedOnTicks--;

                    using (UnitOfWork verificationUnitOfWork = XpoHelper.GetNewUnitOfWork())
                    {
                        CompanyNew existingOwner = verificationUnitOfWork.GetObjectByKey<CompanyNew>(owner.Oid);
                        if (existingOwner != null)
                        {
                            throw new Exception("Owner with Oid = " + existingOwner.Oid.ToString() + " already exists");
                        }
                    }

                    owner.Save();
                    uow.CommitChanges();
                }

                json = service.GetStoreControllerSettings(storeControllerID, username, password);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreControllerSettings storeControllerSettings = new StoreControllerSettings(uow);
                    string error;
                    storeControllerSettings.FromJson(json, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    storeControllerSettings.UpdatedOnTicks--;

                    using (UnitOfWork verificationUnitOfWork = XpoHelper.GetNewUnitOfWork())
                    {
                        StoreControllerSettings existingStoreControllerSettings = verificationUnitOfWork.GetObjectByKey<StoreControllerSettings>(storeControllerSettings.Oid);
                        if (existingStoreControllerSettings != null)
                        {
                            throw new Exception("StoreControllerSettings with Oid = " + existingStoreControllerSettings.Oid.ToString() + " already exists");
                        }
                    }

                    storeControllerSettings.Save();
                    uow.CommitChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return false;
            }
        }

        private Guid GetStoreControllerGuidFromService(string url, string id, string username, string password)
        {
            RetailService.RetailService service = new RetailService.RetailService();
            service.Url = url;
            return service.GetStoreControllerGuid(id, username, password);
        }

        private void SetConnectionProperties()
        {
            XpoHelper.sqlserver = txtServer.Text;
            XpoHelper.username = txtDBUsername.Text;
            XpoHelper.pass = txtDBPassword.Text;
            XpoHelper.database = txtDatabase.Text;
            XpoHelper.databasetype = (DBType)Enum.Parse(typeof(DBType), cmbDBType.SelectedItem.ToString());
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFields();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFields();
        }

        private void dualModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFields();
        }

        private void UpdateSettingsFields()
        {
            grbSmtpSettings.Enabled = retailRadioButton.Checked || dualModeRadioButton.Checked;
            grbStoreControllerSettings.Enabled = storeControllerRadioButton.Checked;
            grbDualModeSettings.Enabled = dualModeRadioButton.Checked;
        }

        private void btnTestOlapConnection_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void btnDeployAnalysis_Click(object sender, EventArgs e)
        {
            string tempPath = Path.GetTempPath() + "ITS.Migration-" + Guid.NewGuid().ToString();

            Directory.CreateDirectory(tempPath);

            string[] variables = new string[] { txtDatabase.Text, txtOlapServer.Text, txtOlapUsername.Text, txtOlapPassword.Text };

            var assembly = Assembly.GetExecutingAssembly();
            string[] resources = new string[]
            {
                "WRM_Analysis.asdatabase",
                "WRM_Analysis.configsettings",
                "WRM_Analysis.deploymentoptions",
                "WRM_Analysis.deploymenttargets"
            };

            foreach (string resourceName in resources)
            {
                using (Stream stream = assembly.GetManifestResourceStream("ITS.Retail.MigrationTool.AnalysisServer." + resourceName))
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(tempPath + "\\" + resourceName))
                {
                    string fileContent = reader.ReadToEnd();
                    writer.Write(string.Format(fileContent, variables));
                }
            }
            ProcessStartInfo processInfo = new ProcessStartInfo("Microsoft.AnalysisServices.Deployment.exe", " \"" + tempPath + "\\WRM_Analysis.asdatabase\" /s")
            { RedirectStandardOutput = true, CreateNoWindow = true, UseShellExecute = false };

            using (Process deploy = Process.Start(processInfo))
            {
                deploy.WaitForExit();
                if (deploy.ExitCode == 0)
                {
                    logger.Success(deploy.StandardOutput.ReadToEnd());
                }
                else
                {
                    logger.Warning(deploy.StandardOutput.ReadToEnd());
                }
            }
            Directory.Delete(tempPath, true);
        }
    }
}
