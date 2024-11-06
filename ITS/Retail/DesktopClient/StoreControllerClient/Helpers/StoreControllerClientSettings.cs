using System;
using System.Xml.Serialization;
using ITS.Retail.Common;
#if !_OFFICE_LOADER
using DevExpress.Xpo;
using ITS.Retail.Model;
#endif

using System.Globalization;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public class StoreControllerClientSettings : SettingsBase
    {
        // Fields...
#if !_OFFICE_LOADER
        private User _CurrentUser;
        private UnitOfWork _uow;
        private StoreControllerSettings _StoreControllerSettings;
        private DBType _UnderlyingeDatabaseType;
#endif

        private string _StoreControllerURL;
        private string _HQURL;
        private string _Culture;
        private string _LabelDocumentGridViewSettings;
        private string _PosGridViewSettings;
        private string _ReportsGridViewSettings;
        private string _CashierGridViewSettings;
        private string _CashierGridViewAddItemSettings;
        private string _LabelPCDGridViewSettings;
        private string _DefaultLabelPrinter;
        private bool _uowDisposed;
        private int _WebServiceTimeOut;
        private string _MasterURLService;
#if _OFFICE_LOADER
        private string _MasterAppInstance;
#else
        private eApplicationInstance _MasterAppInstance;
#endif
        private string _CustomerGridViewSettings;
        private string _DocumentLabelGridViewSettings;
        private string _SupplierGridViewSettings;
        private string _ItemGridViewSettings;
        private string _LayoutPurchaseTotal;
        private string _PurchaseDocumentDetail;
        private string _PurchaseDocumentHeader;
        private string _LayoutSalesTotal;
        private string _SalesDocumentDetail;
        private string _SalesDocumentHeader;
        private string _QuickAccessToolbarLayout;
        public const string WEB_SERVICE = "StoreControllerClientService.svc";
        public const string ITS_WEB_SERVICE = "ITSStoreControllerDesktopService.svc";
        private string _DocumentSalesGridViewSettings;
        private string _DocumentProformaGridViewSettings;
        private string _DocumentPurchaseGridViewSettings;
        private string _DocumentStoreGridViewSettings;
        private string _DocumentSpecialProformaGridViewSettings;
        private string _CashierMultithredCardViewSettings;

        
        public StoreControllerClientSettings()
            : base(null, false)
        {

        }

        public StoreControllerClientSettings(string fullfilename, bool autoSave)
            : base(fullfilename, autoSave)
        {

        }

        public override void AfterConstrunction()
        {
            base.AfterConstrunction();
            this.StoreControllerURL = null;
            this.WebServiceTimeOut = 0;
#if !_OFFICE_LOADER
            this.Culture = Platform.PlatformConstants.DefaultCulture.ToString();
#endif
        }
        public string CashierMultithredCardViewSettings
        {
            get
            {
                return _CashierMultithredCardViewSettings;
            }
            set
            {
                SetPropertyValue("CashierMultithredCardViewSettings", ref _CashierMultithredCardViewSettings, value);
            }
        }

        public string DefaultLabelPrinter
        {
            get
            {
                return _DefaultLabelPrinter;
            }
            set
            {
                SetPropertyValue("DefaultLabelPrinter", ref _DefaultLabelPrinter, value);
            }
        }

        public string LabelPCDGridViewSettings
        {
            get
            {
                return _LabelPCDGridViewSettings;
            }
            set
            {
                SetPropertyValue("LabelPCDGridViewSettings", ref _LabelPCDGridViewSettings, value);
            }
        }

        public int WebServiceTimeOut
        {
            get
            {
                return _WebServiceTimeOut;
            }
            set
            {
                SetPropertyValue("WebServiceTimeOut", ref _WebServiceTimeOut, value);
            }
        }


        public string LabelDocumentGridViewSettings
        {
            get
            {
                return _LabelDocumentGridViewSettings;
            }
            set
            {
                SetPropertyValue("LabelDocumentGridViewSettings", ref _LabelDocumentGridViewSettings, value);
            }
        }


        public string DocumentProformaGridViewSettings
        {
            get
            {
                return _DocumentProformaGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentProformaGridViewSettings", ref _DocumentProformaGridViewSettings, value);
            }
        }

        public string DocumentSpecialProformaGridViewSettings
        {
            get
            {
                return _DocumentSpecialProformaGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentSpecialProformaGridViewSettings", ref _DocumentSpecialProformaGridViewSettings, value);
            }
        }

        public string DocumentSalesGridViewSettings
        {
            get
            {
                return _DocumentSalesGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentSalesGridViewSettings", ref _DocumentSalesGridViewSettings, value);
            }
        }

        public string DocumentPurchaseGridViewSettings
        {
            get
            {
                return _DocumentPurchaseGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentPurchaseGridViewSettings", ref _DocumentPurchaseGridViewSettings, value);
            }
        }

        public string DocumentStoreGridViewSettings
        {
            get
            {
                return _DocumentStoreGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentStoreGridViewSettings", ref _DocumentStoreGridViewSettings, value);
            }
        }
        public string ReportsGridViewSettings
        {
            get
            {
                return _ReportsGridViewSettings;
            }
            set
            {
                SetPropertyValue("ReportsGridViewSettings", ref _ReportsGridViewSettings, value);
            }
        }

        public string PosGridViewSettings
        {
            get
            {
                return _PosGridViewSettings;
            }
            set
            {
                SetPropertyValue("PosGridViewSettings", ref _PosGridViewSettings, value);
            }
        }

        public string CashierGridViewSettings
        {
            get
            {
                return _CashierGridViewSettings;
            }
            set
            {
                SetPropertyValue("CashierGridViewSettings", ref _CashierGridViewSettings, value);
            }
        }
        public string CashierGridViewAddItemSettings
        {
            get
            {
                return _CashierGridViewAddItemSettings;
            }
            set
            {
                SetPropertyValue("CashierGridViewAddItemSettings", ref _CashierGridViewAddItemSettings, value);
            }
        }

        public string Culture
        {
            get
            {
                return _Culture;
            }
            set
            {
                SetPropertyValue("Culture", ref _Culture, value);
            }
        }

        public string StoreControllerURL
        {
            get
            {
                return _StoreControllerURL;
            }
            set
            {
                SetPropertyValue("StoreControllerURL", ref _StoreControllerURL, value);
            }
        }
        public string HQURL
        {
            get
            {
                return _HQURL;
            }
            set
            {
                SetPropertyValue("HQURL", ref _HQURL, value);
            }
        }

        public string MasterURLService
        {
            get
            {
                return _MasterURLService;
            }
            set
            {
                SetPropertyValue("MasterURLService", ref _MasterURLService, value);
            }
        }

#if _OFFICE_LOADER
        private string MasterAppInstance
        {
            get
            {
                return _MasterAppInstance;
            }
            set
            {
                SetPropertyValue("MasterAppInstance", ref _MasterAppInstance, value);
            }
        }
#else
        public eApplicationInstance MasterAppInstance
        {
            get
            {
                return _MasterAppInstance;
            }
            set
            {
                SetPropertyValue("MasterAppInstance", ref _MasterAppInstance, value);
            }
        }
#endif

        public string QuickAccessToolbarLayout
        {
            get
            {
                return _QuickAccessToolbarLayout;
            }
            set
            {
                SetPropertyValue("QuickAccessToolbarLayout", ref _QuickAccessToolbarLayout, value);
            }
        }

        public string SalesDocumentHeader
        {
            get
            {
                return _SalesDocumentHeader;
            }
            set
            {
                SetPropertyValue("SalesDocumentHeader", ref _SalesDocumentHeader, value);
            }
        }

        public string SalesDocumentDetail
        {
            get
            {
                return _SalesDocumentDetail;
            }
            set
            {
                SetPropertyValue("SalesDocumentDetail", ref _SalesDocumentDetail, value);
            }
        }

        public string LayoutSalesTotal
        {
            get
            {
                return _LayoutSalesTotal;
            }
            set
            {
                SetPropertyValue("LayoutSalesTotal", ref _LayoutSalesTotal, value);
            }
        }

        public string PurchaseDocumentDetail
        {
            get
            {
                return _PurchaseDocumentDetail;
            }
            set
            {
                SetPropertyValue("PurchaseDocumentDetail", ref _PurchaseDocumentDetail, value);
            }
        }

        public string PurchaseDocumentHeader
        {
            get
            {
                return _PurchaseDocumentHeader;
            }
            set
            {
                SetPropertyValue("PurchaseDocumentHeader", ref _PurchaseDocumentHeader, value);
            }
        }

        public string LayoutPurchaseTotal
        {
            get
            {
                return _LayoutPurchaseTotal;
            }
            set
            {
                SetPropertyValue("LayoutPurchaseTotal", ref _LayoutPurchaseTotal, value);
            }
        }

        [XmlIgnore]
        public string WebServiceURI
        {
            get
            {
                return string.Format("{0}/{1}", StoreControllerURL.TrimEnd('/'), WEB_SERVICE);
            }
        }

        [XmlIgnore]
        public string ITSWebServiceURI
        {
            get
            {
                return string.Format("{0}/{1}", StoreControllerURL.TrimEnd('/'), ITS_WEB_SERVICE);
            }
        }
        [XmlIgnore]
        public CultureInfo CultureInfo
        {
            get
            {
                return new CultureInfo(Culture);
            }
        }
#if !_OFFICE_LOADER
        [XmlIgnore]
        public User CurrentUser
        {
            get
            {
                return _CurrentUser;
            }
            set
            {
                SetPropertyValue("CurrentUser", ref _CurrentUser, value);
            }
        }


        [XmlIgnore]
        public UnitOfWork ReadOnlyUnitOfWork
        {
            get
            {
                if (_uow == null || _uowDisposed)
                {
                    _uow = XpoHelper.GetNewUnitOfWork();
                    _uowDisposed = false;
                    _uow.Disposed += _uow_Disposed;
                }
                return _uow;
            }
        }

        [XmlIgnore]
        public StoreControllerSettings StoreControllerSettings
        {
            get
            {
                if (_StoreControllerSettings == null)
                {
                    try
                    {
                        _StoreControllerSettings = ReadOnlyUnitOfWork.FindObject<StoreControllerSettings>(null);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.GetFullMessage();
                        _StoreControllerSettings = null;
                    }
                }
                return _StoreControllerSettings;
            }
        }


        [XmlIgnore]
        public ITSStoreControllerDesktopService.ITSStoreControllerDesktopServiceClient ITSStoreControllerDesktopService
        {
            get
            {
                ITSStoreControllerDesktopService.ITSStoreControllerDesktopServiceClient itsService = new ITSStoreControllerDesktopService.ITSStoreControllerDesktopServiceClient();

                itsService.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(this.ITSWebServiceURI), itsService.Endpoint.Address.Identity, itsService.Endpoint.Address.Headers);
                itsService.Endpoint.Binding.OpenTimeout = TimeSpan.FromMilliseconds(this.WebServiceTimeOut);

                return itsService;
            }
        }

        private void _uow_Disposed(object sender, EventArgs e)
        {
            _uowDisposed = true;
        }
#endif
        public string CustomerGridViewSettings
        {
            get
            {
                return _CustomerGridViewSettings;
            }
            set
            {
                SetPropertyValue("CustomerGridViewSettings", ref _CustomerGridViewSettings, value);
            }
        }

        public string ItemGridViewSettings
        {
            get
            {
                return _ItemGridViewSettings;
            }
            set
            {
                SetPropertyValue("ItemGridViewSettings", ref _ItemGridViewSettings, value);
            }
        }

        public string SupplierGridViewSettings
        {
            get
            {
                return _SupplierGridViewSettings;
            }
            set
            {
                SetPropertyValue("SupplierGridViewSettings", ref _SupplierGridViewSettings, value);
            }
        }

        public string DocumentLabelGridViewSettings
        {
            get
            {
                return _DocumentLabelGridViewSettings;
            }
            set
            {
                SetPropertyValue("DocumentLabelGridViewSettings", ref _DocumentLabelGridViewSettings, value);
            }
        }

#if !_OFFICE_LOADER
        public DBType UnderlyingeDatabaseType
        {
            get
            {
                return _UnderlyingeDatabaseType;
            }
            set
            {
                SetPropertyValue("UnderlyingeDatabaseType", ref _UnderlyingeDatabaseType, value);
            }
        }
#endif
    }
}
