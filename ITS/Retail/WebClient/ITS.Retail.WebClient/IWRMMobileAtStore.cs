using ITS.MobileAtStore.ObjectModel;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.WebClient.RetailAtStoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ITS.Retail.WebClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWRMMobileAtStore" in both code and config file together.
    [ServiceContract]
    [ServiceKnownType(typeof(ITS.Retail.Mobile.AuxilliaryClasses.eDocumentType))]
    [ServiceKnownType(typeof(Header))]
    [ServiceKnownType(typeof(Line))]
    public interface IWRMMobileAtStore
    {
        [OperationContract]
        Product GetProduct(int id, string ip, string code, string suppcode, string compCode, string priceCatalogPolicy, eDocumentType eDocumentType);

        [OperationContract]
        Product GetReceiptProduct(int id, string ip, string code, string suppcode, string compCode, string priceCatalogPolicy, eDocumentType eDocumentType);

        [OperationContract]
        Offer[] GetOffers(int id, string ip, string code, string compCode);

        [OperationContract]
        Customer GetSupplier(int id, string ip, String searchString);

        [OperationContract]
        Customer GetCustomer(int id, string ip, String searchString);

        [OperationContract]
        List<Warehouse> GetWarehouses(int id, string ip);

        [OperationContract]
        bool ExportDocument(int id, string ip, Header header, out string errorMessage);

        //[OperationContract]
        //List<PriceList> GetPriceLists(int id, string ip);

        [OperationContract]
        DateTime GetNow(int id, string ip);

        //[OperationContract]
        //public List<InvLine> GetInvLines(string taxCode);
        
        [OperationContract]
        string GetWebServiceVersion(int id, string ip);

        [OperationContract]
        bool ProductCheckAdd(int id, string ip, string code, string compcode);

        [OperationContract]
        bool ProductCheckRemove(int id, string ip, string code, string compcode);

        [OperationContract]
        String[][] GetMobileFilelist(int id, string ip);

        [OperationContract]
        bool PerformInventoryExport(int id, string ip,out string errorMessage);

        [OperationContract]
        bool GetMobileConfig(int id, string ip, out string fileContent);

        [OperationContract]
        List<InvLine> GetInvLines(int id, string ip, string taxCode);


        [OperationContract]
        int CountInvLines(int id, string ip);

        [OperationContract]
        InvLine UploadInvLine(int id, string ip, InvLine line, decimal quantity, bool add, string outputPath);


        [OperationContract]
        List<ESLInvLine> GetESLInvLines(int id, string ip, string taxCode, string invNumber);

        [OperationContract]
        ESLInvLine UploadESLInvLine(int id, string ip, ESLInvLine line, decimal quantity, bool add);

        [OperationContract]
        PriceCatalogPolicy[] GetPriceCatalogPolicies(int id, string ip);

        [OperationContract]
        bool UpdateInvLine(int id, string ip);

        [OperationContract]
        MobileAtStoreSettings GetSettings();

        [OperationContract]
        List<MobileAtStore.ObjectModel.Warehouse> GetStores(int id, string ip);
    }
}
