using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;
using DevExpress.Xpo;
using System.Data;
using DevExpress.Xpo.DB;
using System.Data.SqlClient;
using ITS.Retail.MigrationTool.Classes;
using ITS.Retail.WebClient.Helpers;

namespace MigrationTool.MigrationScripts
{

    

    //[Migration(20130404)]
    [RetailMigration(author: "akm", year: 2014, month: 1, day: 1, order: 1, version: "2.0.0.67")]
    public class Retail_Version_1_to_Version2: Migration
    {

        private readonly String[][] RetailV1ForeignKeys = new String[][]{
             new String[] {"Company", "FK_Company_DefaultAddress"},
             new String[] {"Customer", "FK_Customer_DefaultAddress"},
             new String[] {"Phone", "FK_Phone_Address"},
             new String[] {"Store", "FK_Store_Address"},
             new String[] {"Supplier", "FK_Supplier_DefaultAddress"},
             new String[] {"Address", "FK_Address_AddressType"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_Barcode"},
             new String[] {"Item", "FK_Item_DefaultBarcode"},
             new String[] {"ItemStock", "FK_ItemStock_Barcode"},
             new String[] {"PriceCatalogDetail", "FK_PriceCatalogDetail_Barcode"},
             new String[] {"Barcode", "FK_Barcode_Type"},
             new String[] {"Item", "FK_Item_Buyer"},
             new String[] {"CategoryNode", "FK_CategoryNode_Parent"},
             new String[] {"CustomerAnalyticTree", "FK_CustomerAnalyticTree_Node"},
             new String[] {"CustomerAnalyticTree", "FK_CustomerAnalyticTree_Root"},
             new String[] {"CustomerCategory", "FK_CustomerCategory_Oid"},
             new String[] {"ItemAnalyticTree", "FK_ItemAnalyticTree_Node"},
             new String[] {"ItemAnalyticTree", "FK_ItemAnalyticTree_Root"},
             new String[] {"ItemCategory", "FK_ItemCategory_Oid"},
             new String[] {"StoreAnalyticTree", "FK_StoreAnalyticTree_Node"},
             new String[] {"StoreAnalyticTree", "FK_StoreAnalyticTree_Root"},
             new String[] {"StoreCategory", "FK_StoreCategory_Oid"},
             new String[] {"CustomerAnalyticTree", "FK_CustomerAnalyticTree_Object"},
             new String[] {"CustomerStorePriceList", "FK_CustomerStorePriceList_Customer"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_Customer"},
             new String[] {"Terminal", "FK_Terminal_Customer"},
             new String[] {"CustomerCategoryDiscount", "FK_CustomerCategoryDiscount_CustomerCategory"},
             new String[] {"DailyTotalsDetail", "FK_DailyTotalsDetail_DailyTotals"},
             new String[] {"DataFileRecordHeader", "FK_DataFileRecordHeader_KeyProperty"},
             new String[] {"DataFileRecordDetail", "FK_DataFileRecordDetail_Header"},
             new String[] {"DecodedRawData", "FK_DecodedRawData_Head"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_DeliveryType"},
             new String[] {"DocumentType", "FK_DocumentType_Division"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_DocumentHeader"},
             new String[] {"DocumentPayment", "FK_DocumentPayment_DocumentHeader"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_DocumentSeries"},
             new String[] {"DocumentSequence", "FK_DocumentSequence_DocumentSeries"},
             new String[] {"StoreDocumentSeries", "FK_StoreDocumentSeries_Series"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_Status"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_DocumentType"},
             new String[] {"StoreDocumentSeriesType", "FK_StoreDocumentSeriesType_DocumentType"},
             new String[] {"RoleEntityAccessPermision", "FK_RoleEntityAccessPermision_EnityAccessPermision"},
             new String[] {"ControllerMessage", "FK_ControllerMessage_FormMessage"},
             new String[] {"FormMessageDetail", "FK_FormMessageDetail_FormMessage"},
             new String[] {"Barcode", "FK_Barcode_Item"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_Item"},
             new String[] {"Item", "FK_Item_MotherCode"},
             new String[] {"ItemAnalyticTree", "FK_ItemAnalyticTree_Object"},
             new String[] {"ItemStock", "FK_ItemStock_Item"},
             new String[] {"ItemStore", "FK_ItemStore_Item"},
             new String[] {"LinkedItem", "FK_LinkedItem_Item"},
             new String[] {"LinkedItem", "FK_LinkedItem_SubItem"},
             new String[] {"OfferDetail", "FK_OfferDetail_Item"},
             new String[] {"PriceCatalogDetail", "FK_PriceCatalogDetail_Item"},
             new String[] {"CustomerCategoryDiscount", "FK_CustomerCategoryDiscount_ItemCategory"},
             new String[] {"MemberType", "FK_MemberType_Traders"},
             new String[] {"Barcode", "FK_Barcode_MesurmentUnit"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_MesurmentUnit"},
             new String[] {"OfferDetail", "FK_OfferDetail_Offer"},
             new String[] {"Customer", "FK_Customer_PaymentMethod"},
             new String[] {"DailyTotalsDetail", "FK_DailyTotalsDetail_Payment"},
             new String[] {"DocumentPayment", "FK_DocumentPayment_PaymentMethod"},
             new String[] {"UserDailyTotalsDetail", "FK_UserDailyTotalsDetail_Payment"},
             new String[] {"Address", "FK_Address_DefaultPhone"},
             new String[] {"Phone", "FK_Phone_PhoneType"},
             new String[] {"CustomerCategoryDiscount", "FK_CustomerCategoryDiscount_PriceCatalog"},
             new String[] {"Offer", "FK_Offer_PriceCatalog"},
             new String[] {"PriceCatalog", "FK_PriceCatalog_ParentCatalog"},
             new String[] {"PriceCatalogDetail", "FK_PriceCatalogDetail_PriceCatalog"},
             new String[] {"StorePriceList", "FK_StorePriceList_PriceList"},
             new String[] {"RoleEntityAccessPermision", "FK_RoleEntityAccessPermision_Role"},
             new String[] {"User", "FK_User_Role"},
             new String[] {"Item", "FK_Item_Seasonality"},
             new String[] {"Address", "FK_Address_Store"},
             new String[] {"Customer", "FK_Customer_Store"},
             new String[] {"DailyTotals", "FK_DailyTotals_Store"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_CentralStore"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_Store"},
             new String[] {"ItemStock", "FK_ItemStock_Store"},
             new String[] {"User", "FK_User_CentralStore"},
             new String[] {"UserDailyTotals", "FK_UserDailyTotals_Store"},
             new String[] {"ItemStore", "FK_ItemStore_Store"},
             new String[] {"Store", "FK_Store_Central"},
             new String[] {"StoreAnalyticTree", "FK_StoreAnalyticTree_Object"},
             new String[] {"StoreDocumentSeries", "FK_StoreDocumentSeries_Store"},
             new String[] {"StorePriceList", "FK_StorePriceList_Store"},
             new String[] {"Terminal", "FK_Terminal_Store"},
             new String[] {"StoreDocumentSeriesType", "FK_StoreDocumentSeriesType_StoreSeries"},
             new String[] {"CustomerStorePriceList", "FK_CustomerStorePriceList_StorePriceList"},
             new String[] {"Customer", "FK_Customer_DefaultSupplier"},
             new String[] {"Item", "FK_Item_DefaultSupplier"},
             new String[] {"Store", "FK_Store_Supplier"},
             new String[] {"FieldPermission", "FK_FieldPermission_TableName"},
             new String[] {"DailyTotals", "FK_DailyTotals_Terminal"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_Terminal"},
             new String[] {"UserDailyTotals", "FK_UserDailyTotals_Terminal"},
             new String[] {"Terminal", "FK_Terminal_TerminalType"},
             new String[] {"Address", "FK_Address_Trader"},
             new String[] {"Company", "FK_Company_Trader"},
             new String[] {"Customer", "FK_Customer_Trader"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_DeliveryTo"},
             new String[] {"Supplier", "FK_Supplier_Trader"},
             new String[] {"Address", "FK_Address_CreatedBy"},
             new String[] {"Address", "FK_Address_UpdatedBy"},
             new String[] {"AddressType", "FK_AddressType_CreatedBy"},
             new String[] {"AddressType", "FK_AddressType_UpdatedBy"},
             new String[] {"ApplicationLog", "FK_ApplicationLog_CreatedBy"},
             new String[] {"ApplicationLog", "FK_ApplicationLog_UpdatedBy"},
             new String[] {"VatCategory", "FK_VatCategory_UpdatedBy"},
             new String[] {"VatFactor", "FK_VatFactor_CreatedBy"},
             new String[] {"VatFactor", "FK_VatFactor_UpdatedBy"},
             new String[] {"VatLevel", "FK_VatLevel_CreatedBy"},
             new String[] {"VatLevel", "FK_VatLevel_UpdatedBy"},
             new String[] {"UserDailyTotalsDetail", "FK_UserDailyTotalsDetail_CreatedBy"},
             new String[] {"UserDailyTotalsDetail", "FK_UserDailyTotalsDetail_UpdatedBy"},
             new String[] {"UserTypeAccess", "FK_UserTypeAccess_CreatedBy"},
             new String[] {"UserTypeAccess", "FK_UserTypeAccess_UpdatedBy"},
             new String[] {"UserTypeAccess", "FK_UserTypeAccess_User"},
             new String[] {"VatCategory", "FK_VatCategory_CreatedBy"},
             new String[] {"Trader", "FK_Trader_UpdatedBy"},
             new String[] {"User", "FK_User_CreatedBy"},
             new String[] {"User", "FK_User_UpdatedBy"},
             new String[] {"UserDailyTotals", "FK_UserDailyTotals_CreatedBy"},
             new String[] {"UserDailyTotals", "FK_UserDailyTotals_UpdatedBy"},
             new String[] {"UserDailyTotals", "FK_UserDailyTotals_User"},
             new String[] {"TablePermission", "FK_TablePermission_User"},
             new String[] {"Terminal", "FK_Terminal_CreatedBy"},
             new String[] {"Terminal", "FK_Terminal_UpdatedBy"},
             new String[] {"TerminalType", "FK_TerminalType_CreatedBy"},
             new String[] {"TerminalType", "FK_TerminalType_UpdatedBy"},
             new String[] {"Trader", "FK_Trader_CreatedBy"},
             new String[] {"StorePriceList", "FK_StorePriceList_CreatedBy"},
             new String[] {"StorePriceList", "FK_StorePriceList_UpdatedBy"},
             new String[] {"Supplier", "FK_Supplier_CreatedBy"},
             new String[] {"Supplier", "FK_Supplier_UpdatedBy"},
             new String[] {"TablePermission", "FK_TablePermission_CreatedBy"},
             new String[] {"TablePermission", "FK_TablePermission_UpdatedBy"},
             new String[] {"StoreAnalyticTree", "FK_StoreAnalyticTree_CreatedBy"},
             new String[] {"StoreAnalyticTree", "FK_StoreAnalyticTree_UpdatedBy"},
             new String[] {"StoreDocumentSeries", "FK_StoreDocumentSeries_CreatedBy"},
             new String[] {"StoreDocumentSeries", "FK_StoreDocumentSeries_UpdatedBy"},
             new String[] {"StoreDocumentSeriesType", "FK_StoreDocumentSeriesType_CreatedBy"},
             new String[] {"StoreDocumentSeriesType", "FK_StoreDocumentSeriesType_UpdatedBy"},
             new String[] {"RoleEntityAccessPermision", "FK_RoleEntityAccessPermision_CreatedBy"},
             new String[] {"RoleEntityAccessPermision", "FK_RoleEntityAccessPermision_UpdatedBy"},
             new String[] {"Seasonality", "FK_Seasonality_CreatedBy"},
             new String[] {"Seasonality", "FK_Seasonality_UpdatedBy"},
             new String[] {"Store", "FK_Store_CreatedBy"},
             new String[] {"Store", "FK_Store_UpdatedBy"},
             new String[] {"PriceCatalog", "FK_PriceCatalog_CreatedBy"},
             new String[] {"PriceCatalog", "FK_PriceCatalog_UpdatedBy"},
             new String[] {"PriceCatalogDetail", "FK_PriceCatalogDetail_CreatedBy"},
             new String[] {"PriceCatalogDetail", "FK_PriceCatalogDetail_UpdatedBy"},
             new String[] {"Role", "FK_Role_CreatedBy"},
             new String[] {"Role", "FK_Role_UpdatedBy"},
             new String[] {"PaymentMethod", "FK_PaymentMethod_CreatedBy"},
             new String[] {"PaymentMethod", "FK_PaymentMethod_UpdatedBy"},
             new String[] {"Phone", "FK_Phone_CreatedBy"},
             new String[] {"Phone", "FK_Phone_UpdatedBy"},
             new String[] {"PhoneType", "FK_PhoneType_CreatedBy"},
             new String[] {"PhoneType", "FK_PhoneType_UpdatedBy"},
             new String[] {"MesurmentUnits", "FK_MesurmentUnits_CreatedBy"},
             new String[] {"MesurmentUnits", "FK_MesurmentUnits_UpdatedBy"},
             new String[] {"Offer", "FK_Offer_CreatedBy"},
             new String[] {"Offer", "FK_Offer_UpdatedBy"},
             new String[] {"OfferDetail", "FK_OfferDetail_CreatedBy"},
             new String[] {"OfferDetail", "FK_OfferDetail_UpdatedBy"},
             new String[] {"ItemStore", "FK_ItemStore_CreatedBy"},
             new String[] {"ItemStore", "FK_ItemStore_UpdatedBy"},
             new String[] {"LinkedItem", "FK_LinkedItem_CreatedBy"},
             new String[] {"LinkedItem", "FK_LinkedItem_UpdatedBy"},
             new String[] {"MemberType", "FK_MemberType_CreatedBy"},
             new String[] {"MemberType", "FK_MemberType_UpdatedBy"},
             new String[] {"ItemAnalyticTree", "FK_ItemAnalyticTree_CreatedBy"},
             new String[] {"ItemAnalyticTree", "FK_ItemAnalyticTree_UpdatedBy"},
             new String[] {"ItemImage", "FK_ItemImage_CreatedBy"},
             new String[] {"ItemImage", "FK_ItemImage_UpdatedBy"},
             new String[] {"ItemStock", "FK_ItemStock_CreatedBy"},
             new String[] {"ItemStock", "FK_ItemStock_UpdatedBy"},
             new String[] {"FormMessage", "FK_FormMessage_CreatedBy"},
             new String[] {"FormMessage", "FK_FormMessage_UpdatedBy"},
             new String[] {"FormMessageDetail", "FK_FormMessageDetail_CreatedBy"},
             new String[] {"FormMessageDetail", "FK_FormMessageDetail_UpdatedBy"},
             new String[] {"Item", "FK_Item_CreatedBy"},
             new String[] {"Item", "FK_Item_UpdatedBy"},
             new String[] {"DocumentType", "FK_DocumentType_CreatedBy"},
             new String[] {"DocumentType", "FK_DocumentType_UpdatedBy"},
             new String[] {"EntityAccessPermision", "FK_EntityAccessPermision_CreatedBy"},
             new String[] {"EntityAccessPermision", "FK_EntityAccessPermision_UpdatedBy"},
             new String[] {"FieldPermission", "FK_FieldPermission_CreatedBy"},
             new String[] {"FieldPermission", "FK_FieldPermission_UpdatedBy"},
             new String[] {"DocumentSequence", "FK_DocumentSequence_CreatedBy"},
             new String[] {"DocumentSequence", "FK_DocumentSequence_UpdatedBy"},
             new String[] {"DocumentSeries", "FK_DocumentSeries_CreatedBy"},
             new String[] {"DocumentSeries", "FK_DocumentSeries_UpdatedBy"},
             new String[] {"DocumentStatus", "FK_DocumentStatus_CreatedBy"},
             new String[] {"DocumentStatus", "FK_DocumentStatus_UpdatedBy"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_CreatedBy"},
             new String[] {"DocumentDetail", "FK_DocumentDetail_UpdatedBy"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_CreatedBy"},
             new String[] {"DocumentHeader", "FK_DocumentHeader_UpdatedBy"},
             new String[] {"DocumentPayment", "FK_DocumentPayment_CreatedBy"},
             new String[] {"DocumentPayment", "FK_DocumentPayment_UpdatedBy"},
             new String[] {"DecodedRawData", "FK_DecodedRawData_CreatedBy"},
             new String[] {"DecodedRawData", "FK_DecodedRawData_UpdatedBy"},
             new String[] {"DeliveryType", "FK_DeliveryType_CreatedBy"},
             new String[] {"DeliveryType", "FK_DeliveryType_UpdatedBy"},
             new String[] {"Division", "FK_Division_CreatedBy"},
             new String[] {"Division", "FK_Division_UpdatedBy"},
             new String[] {"DataFileReceived", "FK_DataFileReceived_CreatedBy"},
             new String[] {"DataFileReceived", "FK_DataFileReceived_UpdatedBy"},
             new String[] {"DataFileRecordDetail", "FK_DataFileRecordDetail_CreatedBy"},
             new String[] {"DataFileRecordDetail", "FK_DataFileRecordDetail_UpdatedBy"},
             new String[] {"DataFileRecordHeader", "FK_DataFileRecordHeader_CreatedBy"},
             new String[] {"DataFileRecordHeader", "FK_DataFileRecordHeader_UpdatedBy"},
             new String[] {"CustomerStorePriceList", "FK_CustomerStorePriceList_CreatedBy"},
             new String[] {"CustomerStorePriceList", "FK_CustomerStorePriceList_UpdatedBy"},
             new String[] {"DailyTotals", "FK_DailyTotals_CreatedBy"},
             new String[] {"DailyTotals", "FK_DailyTotals_UpdatedBy"},
             new String[] {"DailyTotalsDetail", "FK_DailyTotalsDetail_CreatedBy"},
             new String[] {"DailyTotalsDetail", "FK_DailyTotalsDetail_UpdatedBy"},
             new String[] {"Customer", "FK_Customer_CreatedBy"},
             new String[] {"Customer", "FK_Customer_UpdatedBy"},
             new String[] {"CustomerAnalyticTree", "FK_CustomerAnalyticTree_CreatedBy"},
             new String[] {"CustomerAnalyticTree", "FK_CustomerAnalyticTree_UpdatedBy"},
             new String[] {"CustomerCategoryDiscount", "FK_CustomerCategoryDiscount_CreatedBy"},
             new String[] {"CustomerCategoryDiscount", "FK_CustomerCategoryDiscount_UpdatedBy"},
             new String[] {"Company", "FK_Company_CreatedBy"},
             new String[] {"Company", "FK_Company_UpdatedBy"},
             new String[] {"ControllerMessage", "FK_ControllerMessage_CreatedBy"},
             new String[] {"ControllerMessage", "FK_ControllerMessage_UpdatedBy"},
             new String[] {"Country", "FK_Country_CreatedBy"},
             new String[] {"Country", "FK_Country_UpdatedBy"},
             new String[] {"Buyer", "FK_Buyer_CreatedBy"},
             new String[] {"Buyer", "FK_Buyer_UpdatedBy"},
             new String[] {"CategoryNode", "FK_CategoryNode_CreatedBy"},
             new String[] {"CategoryNode", "FK_CategoryNode_UpdatedBy"},
             new String[] {"CompaniesGroup", "FK_CompaniesGroup_CreatedBy"},
             new String[] {"CompaniesGroup", "FK_CompaniesGroup_UpdatedBy"},
             new String[] {"ApplicationSettings", "FK_ApplicationSettings_CreatedBy"},
             new String[] {"ApplicationSettings", "FK_ApplicationSettings_UpdatedBy"},
             new String[] {"Barcode", "FK_Barcode_CreatedBy"},
             new String[] {"Barcode", "FK_Barcode_UpdatedBy"},
             new String[] {"BarcodeType", "FK_BarcodeType_CreatedBy"},
             new String[] {"BarcodeType", "FK_BarcodeType_UpdatedBy"},
             new String[] {"UserDailyTotalsDetail", "FK_UserDailyTotalsDetail_UserDailyTotals"},
             new String[] {"DailyTotalsDetail", "FK_DailyTotalsDetail_VatCategory"},
             new String[] {"Item", "FK_Item_VatCategory"},
             new String[] {"UserDailyTotalsDetail", "FK_UserDailyTotalsDetail_VatCategory"},
             new String[] {"VatFactor", "FK_VatFactor_VatCategory"},
             new String[] {"Customer", "FK_Customer_VatLevel"},
             new String[] {"VatFactor", "FK_VatFactor_VatLevel"},
             new String[] {"CategoryNode", "FK_CategoryNode_ObjectType"}
        };
        private readonly String[][] RetailV1Indexes = new String[][]{
            new String[]{ "Address", "iTrader_Address"},
            new String[]{ "Address", "iGCRecord_Address"},
            new String[]{ "Address", "iCreatedBy_Address"},
            new String[]{ "Address", "iUpdatedBy_Address"},
            new String[]{ "Address", "iAddressType_Address"},
            new String[]{ "Address", "iDefaultPhone_Address"},
            new String[]{ "Address", "iStore_Address"},
            new String[]{ "AddressType", "iDescription_AddressType"},
            new String[]{ "AddressType", "iCode_AddressType"},
            new String[]{ "AddressType", "iGCRecord_AddressType"},
            new String[]{ "AddressType", "iCreatedBy_AddressType"},
            new String[]{ "AddressType", "iUpdatedBy_AddressType"},
            new String[]{ "ApplicationLog", "iGCRecord_ApplicationLog"},
            new String[]{ "ApplicationLog", "iCreatedBy_ApplicationLog"},
            new String[]{ "ApplicationLog", "iUpdatedBy_ApplicationLog"},
            new String[]{ "ApplicationSettings", "iGCRecord_ApplicationSettings"},
            new String[]{ "ApplicationSettings", "iCreatedBy_ApplicationSettings"},
            new String[]{ "ApplicationSettings", "iUpdatedBy_ApplicationSettings"},
            new String[]{ "Barcode", "iCode_Barcode"},
            new String[]{ "Barcode", "iGCRecord_Barcode"},
            new String[]{ "Barcode", "iCreatedBy_Barcode"},
            new String[]{ "Barcode", "iUpdatedBy_Barcode"},
            new String[]{ "Barcode", "iType_Barcode"},
            new String[]{ "Barcode", "iItem_Barcode"},
            new String[]{ "Barcode", "iMesurmentUnit_Barcode"},
            new String[]{ "BarcodeType", "iDescription_BarcodeType"},
            new String[]{ "BarcodeType", "iCode_BarcodeType"},
            new String[]{ "BarcodeType", "iGCRecord_BarcodeType"},
            new String[]{ "BarcodeType", "iCreatedBy_BarcodeType"},
            new String[]{ "BarcodeType", "iUpdatedBy_BarcodeType"},
            new String[]{ "Buyer", "iDescription_Buyer"},
            new String[]{ "Buyer", "iCode_Buyer"},
            new String[]{ "Buyer", "iGCRecord_Buyer"},
            new String[]{ "Buyer", "iCreatedBy_Buyer"},
            new String[]{ "Buyer", "iUpdatedBy_Buyer"},
            new String[]{ "CategoryNode", "iDescription_CategoryNode"},
            new String[]{ "CategoryNode", "iCode_CategoryNode"},
            new String[]{ "CategoryNode", "iGCRecord_CategoryNode"},
            new String[]{ "CategoryNode", "iCreatedBy_CategoryNode"},
            new String[]{ "CategoryNode", "iUpdatedBy_CategoryNode"},
            new String[]{ "CategoryNode", "iParent_CategoryNode"},
            new String[]{ "CategoryNode", "iObjectType_CategoryNode"},
            new String[]{ "CompaniesGroup", "iDescription_CompaniesGroup"},
            new String[]{ "CompaniesGroup", "iCode_CompaniesGroup"},
            new String[]{ "CompaniesGroup", "iGCRecord_CompaniesGroup"},
            new String[]{ "CompaniesGroup", "iCreatedBy_CompaniesGroup"},
            new String[]{ "CompaniesGroup", "iUpdatedBy_CompaniesGroup"},
            new String[]{ "Company", "iGCRecord_Company"},
            new String[]{ "Company", "iCreatedBy_Company"},
            new String[]{ "Company", "iUpdatedBy_Company"},
            new String[]{ "Company", "iTrader_Company"},
            new String[]{ "Company", "iDefaultAddress_Company"},
            new String[]{ "ControllerMessage", "iDescription_ControllerMessage"},
            new String[]{ "ControllerMessage", "iGCRecord_ControllerMessage"},
            new String[]{ "ControllerMessage", "iCreatedBy_ControllerMessage"},
            new String[]{ "ControllerMessage", "iUpdatedBy_ControllerMessage"},
            new String[]{ "ControllerMessage", "iFormMessage_ControllerMessage"},
            new String[]{ "Country", "iDescription_Country"},
            new String[]{ "Country", "iGCRecord_Country"},
            new String[]{ "Country", "iCreatedBy_Country"},
            new String[]{ "Country", "iUpdatedBy_Country"},
            new String[]{ "Customer", "iTrader_Customer"},
            new String[]{ "Customer", "iLoyalty_Customer"},
            new String[]{ "Customer", "iGCRecord_Customer"},
            new String[]{ "Customer", "iCreatedBy_Customer"},
            new String[]{ "Customer", "iUpdatedBy_Customer"},
            new String[]{ "Customer", "iStore_Customer"},
            new String[]{ "Customer", "iDefaultAddress_Customer"},
            new String[]{ "Customer", "iDefaultSupplier_Customer"},
            new String[]{ "Customer", "iPaymentMethod_Customer"},
            new String[]{ "Customer", "iVatLevel_Customer"},
            new String[]{ "CustomerAnalyticTree", "iRootNodeObject_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iNode_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iGCRecord_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iCreatedBy_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iUpdatedBy_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iRoot_CustomerAnalyticTree"},
            new String[]{ "CustomerAnalyticTree", "iObject_CustomerAnalyticTree"},
            new String[]{ "CustomerCategoryDiscount", "iGCRecord_CustomerCategoryDiscount"},
            new String[]{ "CustomerCategoryDiscount", "iCreatedBy_CustomerCategoryDiscount"},
            new String[]{ "CustomerCategoryDiscount", "iUpdatedBy_CustomerCategoryDiscount"},
            new String[]{ "CustomerCategoryDiscount", "iPriceCatalog_CustomerCategoryDiscount"},
            new String[]{ "CustomerCategoryDiscount", "iCustomerCategory_CustomerCategoryDiscount"},
            new String[]{ "CustomerCategoryDiscount", "iItemCategory_CustomerCategoryDiscount"},
            new String[]{ "CustomerStorePriceList", "iGCRecord_CustomerStorePriceList"},
            new String[]{ "CustomerStorePriceList", "iCreatedBy_CustomerStorePriceList"},
            new String[]{ "CustomerStorePriceList", "iUpdatedBy_CustomerStorePriceList"},
            new String[]{ "CustomerStorePriceList", "iCustomer_CustomerStorePriceList"},
            new String[]{ "CustomerStorePriceList", "iStorePriceList_CustomerStorePriceList"},
            new String[]{ "DailyTotals", "iFiscalDateTerminal_DailyTotals"},
            new String[]{ "DailyTotals", "iGCRecord_DailyTotals"},
            new String[]{ "DailyTotals", "iCreatedBy_DailyTotals"},
            new String[]{ "DailyTotals", "iUpdatedBy_DailyTotals"},
            new String[]{ "DailyTotals", "iTerminal_DailyTotals"},
            new String[]{ "DailyTotals", "iStore_DailyTotals"},
            new String[]{ "DailyTotalsDetail", "iGCRecord_DailyTotalsDetail"},
            new String[]{ "DailyTotalsDetail", "iCreatedBy_DailyTotalsDetail"},
            new String[]{ "DailyTotalsDetail", "iUpdatedBy_DailyTotalsDetail"},
            new String[]{ "DailyTotalsDetail", "iDailyTotals_DailyTotalsDetail"},
            new String[]{ "DailyTotalsDetail", "iVatCategory_DailyTotalsDetail"},
            new String[]{ "DailyTotalsDetail", "iPayment_DailyTotalsDetail"},
            new String[]{ "DataFileReceived", "iGCRecord_DataFileReceived"},
            new String[]{ "DataFileReceived", "iCreatedBy_DataFileReceived"},
            new String[]{ "DataFileReceived", "iUpdatedBy_DataFileReceived"},
            new String[]{ "DataFileRecordDetail", "iGCRecord_DataFileRecordDetail"},
            new String[]{ "DataFileRecordDetail", "iCreatedBy_DataFileRecordDetail"},
            new String[]{ "DataFileRecordDetail", "iUpdatedBy_DataFileRecordDetail"},
            new String[]{ "DataFileRecordDetail", "iHeader_DataFileRecordDetail"},
            new String[]{ "DataFileRecordHeader", "iGCRecord_DataFileRecordHeader"},
            new String[]{ "DataFileRecordHeader", "iCreatedBy_DataFileRecordHeader"},
            new String[]{ "DataFileRecordHeader", "iUpdatedBy_DataFileRecordHeader"},
            new String[]{ "DataFileRecordHeader", "iKeyProperty_DataFileRecordHeader"},
            new String[]{ "DecodedRawData", "iGCRecord_DecodedRawData"},
            new String[]{ "DecodedRawData", "iCreatedBy_DecodedRawData"},
            new String[]{ "DecodedRawData", "iUpdatedBy_DecodedRawData"},
            new String[]{ "DecodedRawData", "iHead_DecodedRawData"},
            new String[]{ "DeliveryType", "iDescription_DeliveryType"},
            new String[]{ "DeliveryType", "iGCRecord_DeliveryType"},
            new String[]{ "DeliveryType", "iCreatedBy_DeliveryType"},
            new String[]{ "DeliveryType", "iUpdatedBy_DeliveryType"},
            new String[]{ "Division", "iDescription_Division"},
            new String[]{ "Division", "iGCRecord_Division"},
            new String[]{ "Division", "iCreatedBy_Division"},
            new String[]{ "Division", "iUpdatedBy_Division"},
            new String[]{ "DocumentDetail", "iGCRecord_DocumentDetail"},
            new String[]{ "DocumentDetail", "iCreatedBy_DocumentDetail"},
            new String[]{ "DocumentDetail", "iUpdatedBy_DocumentDetail"},
            new String[]{ "DocumentDetail", "iCentralStore_DocumentDetail"},
            new String[]{ "DocumentDetail", "iItem_DocumentDetail"},
            new String[]{ "DocumentDetail", "iBarcode_DocumentDetail"},
            new String[]{ "DocumentDetail", "iMesurmentUnit_DocumentDetail"},
            new String[]{ "DocumentDetail", "iDocumentHeader_DocumentDetail"},
            new String[]{ "DocumentHeader", "iGCRecord_DocumentHeader"},
            new String[]{ "DocumentHeader", "iCreatedBy_DocumentHeader"},
            new String[]{ "DocumentHeader", "iUpdatedBy_DocumentHeader"},
            new String[]{ "DocumentHeader", "iTerminal_DocumentHeader"},
            new String[]{ "DocumentHeader", "iDocumentType_DocumentHeader"},
            new String[]{ "DocumentHeader", "iDocumentSeries_DocumentHeader"},
            new String[]{ "DocumentHeader", "iStore_DocumentHeader"},
            new String[]{ "DocumentHeader", "iCustomer_DocumentHeader"},
            new String[]{ "DocumentHeader", "iDeliveryType_DocumentHeader"},
            new String[]{ "DocumentHeader", "iDeliveryTo_DocumentHeader"},
            new String[]{ "DocumentHeader", "iStatus_DocumentHeader"},
            new String[]{ "DocumentPayment", "iGCRecord_DocumentPayment"},
            new String[]{ "DocumentPayment", "iCreatedBy_DocumentPayment"},
            new String[]{ "DocumentPayment", "iUpdatedBy_DocumentPayment"},
            new String[]{ "DocumentPayment", "iDocumentHeader_DocumentPayment"},
            new String[]{ "DocumentPayment", "iPaymentMethod_DocumentPayment"},
            new String[]{ "DocumentSequence", "iDescription_DocumentSequence"},
            new String[]{ "DocumentSequence", "iGCRecord_DocumentSequence"},
            new String[]{ "DocumentSequence", "iCreatedBy_DocumentSequence"},
            new String[]{ "DocumentSequence", "iUpdatedBy_DocumentSequence"},
            new String[]{ "DocumentSequence", "iDocumentSeries_DocumentSequence"},
            new String[]{ "DocumentSeries", "iDescription_DocumentSeries"},
            new String[]{ "DocumentSeries", "iCode_DocumentSeries"},
            new String[]{ "DocumentSeries", "iGCRecord_DocumentSeries"},
            new String[]{ "DocumentSeries", "iCreatedBy_DocumentSeries"},
            new String[]{ "DocumentSeries", "iUpdatedBy_DocumentSeries"},
            new String[]{ "DocumentStatus", "iDescription_DocumentStatus"},
            new String[]{ "DocumentStatus", "iCode_DocumentStatus"},
            new String[]{ "DocumentStatus", "iGCRecord_DocumentStatus"},
            new String[]{ "DocumentStatus", "iCreatedBy_DocumentStatus"},
            new String[]{ "DocumentStatus", "iUpdatedBy_DocumentStatus"},
            new String[]{ "DocumentType", "iDescription_DocumentType"},
            new String[]{ "DocumentType", "iCode_DocumentType"},
            new String[]{ "DocumentType", "iGCRecord_DocumentType"},
            new String[]{ "DocumentType", "iCreatedBy_DocumentType"},
            new String[]{ "DocumentType", "iUpdatedBy_DocumentType"},
            new String[]{ "DocumentType", "iDivision_DocumentType"},
            new String[]{ "EntityAccessPermision", "iGCRecord_EntityAccessPermision"},
            new String[]{ "EntityAccessPermision", "iCreatedBy_EntityAccessPermision"},
            new String[]{ "EntityAccessPermision", "iUpdatedBy_EntityAccessPermision"},
            new String[]{ "FieldPermission", "iGCRecord_FieldPermission"},
            new String[]{ "FieldPermission", "iCreatedBy_FieldPermission"},
            new String[]{ "FieldPermission", "iUpdatedBy_FieldPermission"},
            new String[]{ "FieldPermission", "iTableName_FieldPermission"},
            new String[]{ "FormMessage", "iGCRecord_FormMessage"},
            new String[]{ "FormMessage", "iCreatedBy_FormMessage"},
            new String[]{ "FormMessage", "iUpdatedBy_FormMessage"},
            new String[]{ "FormMessageDetail", "iLocale_FormMessageDetail"},
            new String[]{ "FormMessageDetail", "iDescription_FormMessageDetail"},
            new String[]{ "FormMessageDetail", "iGCRecord_FormMessageDetail"},
            new String[]{ "FormMessageDetail", "iCreatedBy_FormMessageDetail"},
            new String[]{ "FormMessageDetail", "iUpdatedBy_FormMessageDetail"},
            new String[]{ "FormMessageDetail", "iFormMessage_FormMessageDetail"},
            new String[]{ "Item", "iCode_Item"},
            new String[]{ "Item", "iName_Item"},
            new String[]{ "Item", "iGCRecord_Item"},
            new String[]{ "Item", "iCreatedBy_Item"},
            new String[]{ "Item", "iUpdatedBy_Item"},
            new String[]{ "Item", "iDefaultBarcode_Item"},
            new String[]{ "Item", "iVatCategory_Item"},
            new String[]{ "Item", "iMotherCode_Item"},
            new String[]{ "Item", "iSeasonality_Item"},
            new String[]{ "Item", "iBuyer_Item"},
            new String[]{ "Item", "iDefaultSupplier_Item"},
            new String[]{ "ItemAnalyticTree", "iRootNodeObject_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iNode_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iGCRecord_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iCreatedBy_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iUpdatedBy_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iRoot_ItemAnalyticTree"},
            new String[]{ "ItemAnalyticTree", "iObject_ItemAnalyticTree"},
            new String[]{ "ItemImage", "iDescription_ItemImage"},
            new String[]{ "ItemImage", "iGCRecord_ItemImage"},
            new String[]{ "ItemImage", "iCreatedBy_ItemImage"},
            new String[]{ "ItemImage", "iUpdatedBy_ItemImage"},
            new String[]{ "ItemStock", "iGCRecord_ItemStock"},
            new String[]{ "ItemStock", "iCreatedBy_ItemStock"},
            new String[]{ "ItemStock", "iUpdatedBy_ItemStock"},
            new String[]{ "ItemStock", "iStore_ItemStock"},
            new String[]{ "ItemStock", "iItem_ItemStock"},
            new String[]{ "ItemStock", "iBarcode_ItemStock"},
            new String[]{ "ItemStore", "iGCRecord_ItemStore"},
            new String[]{ "ItemStore", "iCreatedBy_ItemStore"},
            new String[]{ "ItemStore", "iUpdatedBy_ItemStore"},
            new String[]{ "ItemStore", "iItem_ItemStore"},
            new String[]{ "ItemStore", "iStore_ItemStore"},
            new String[]{ "LinkedItem", "iGCRecord_LinkedItem"},
            new String[]{ "LinkedItem", "iCreatedBy_LinkedItem"},
            new String[]{ "LinkedItem", "iUpdatedBy_LinkedItem"},
            new String[]{ "LinkedItem", "iItem_LinkedItem"},
            new String[]{ "LinkedItem", "iSubItem_LinkedItem"},
            new String[]{ "MemberType", "iDescription_MemberType"},
            new String[]{ "MemberType", "iCode_MemberType"},
            new String[]{ "MemberType", "iGCRecord_MemberType"},
            new String[]{ "MemberType", "iCreatedBy_MemberType"},
            new String[]{ "MemberType", "iUpdatedBy_MemberType"},
            new String[]{ "MemberType", "iTraders_MemberType"},
            new String[]{ "MesurmentUnits", "iDescription_MesurmentUnits"},
            new String[]{ "MesurmentUnits", "iCode_MesurmentUnits"},
            new String[]{ "MesurmentUnits", "iGCRecord_MesurmentUnits"},
            new String[]{ "MesurmentUnits", "iCreatedBy_MesurmentUnits"},
            new String[]{ "MesurmentUnits", "iUpdatedBy_MesurmentUnits"},
            new String[]{ "Offer", "iDescription_Offer"},
            new String[]{ "Offer", "iCode_Offer"},
            new String[]{ "Offer", "iStartDate_Offer"},
            new String[]{ "Offer", "iEndDate_Offer"},
            new String[]{ "Offer", "iGCRecord_Offer"},
            new String[]{ "Offer", "iCreatedBy_Offer"},
            new String[]{ "Offer", "iUpdatedBy_Offer"},
            new String[]{ "Offer", "iPriceCatalog_Offer"},
            new String[]{ "OfferDetail", "iGCRecord_OfferDetail"},
            new String[]{ "OfferDetail", "iCreatedBy_OfferDetail"},
            new String[]{ "OfferDetail", "iUpdatedBy_OfferDetail"},
            new String[]{ "OfferDetail", "iOffer_OfferDetail"},
            new String[]{ "OfferDetail", "iItem_OfferDetail"},
            new String[]{ "PaymentMethod", "iDescription_PaymentMethod"},
            new String[]{ "PaymentMethod", "iCode_PaymentMethod"},
            new String[]{ "PaymentMethod", "iGCRecord_PaymentMethod"},
            new String[]{ "PaymentMethod", "iCreatedBy_PaymentMethod"},
            new String[]{ "PaymentMethod", "iUpdatedBy_PaymentMethod"},
            new String[]{ "Phone", "iGCRecord_Phone"},
            new String[]{ "Phone", "iCreatedBy_Phone"},
            new String[]{ "Phone", "iUpdatedBy_Phone"},
            new String[]{ "Phone", "iPhoneType_Phone"},
            new String[]{ "Phone", "iAddress_Phone"},
            new String[]{ "PhoneType", "iDescription_PhoneType"},
            new String[]{ "PhoneType", "iCode_PhoneType"},
            new String[]{ "PhoneType", "iGCRecord_PhoneType"},
            new String[]{ "PhoneType", "iCreatedBy_PhoneType"},
            new String[]{ "PhoneType", "iUpdatedBy_PhoneType"},
            new String[]{ "PriceCatalog", "iDescription_PriceCatalog"},
            new String[]{ "PriceCatalog", "iCode_PriceCatalog"},
            new String[]{ "PriceCatalog", "iGCRecord_PriceCatalog"},
            new String[]{ "PriceCatalog", "iCreatedBy_PriceCatalog"},
            new String[]{ "PriceCatalog", "iUpdatedBy_PriceCatalog"},
            new String[]{ "PriceCatalog", "iParentCatalog_PriceCatalog"},
            new String[]{ "PriceCatalogDetail", "iPriceCatalogItemBarcode_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iItem_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iBarcode_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iGCRecord_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iCreatedBy_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iUpdatedBy_PriceCatalogDetail"},
            new String[]{ "PriceCatalogDetail", "iPriceCatalog_PriceCatalogDetail"},
            new String[]{ "Role", "iDescription_Role"},
            new String[]{ "Role", "iGCRecord_Role"},
            new String[]{ "Role", "iCreatedBy_Role"},
            new String[]{ "Role", "iUpdatedBy_Role"},
            new String[]{ "RoleEntityAccessPermision", "iGCRecord_RoleEntityAccessPermision"},
            new String[]{ "RoleEntityAccessPermision", "iCreatedBy_RoleEntityAccessPermision"},
            new String[]{ "RoleEntityAccessPermision", "iUpdatedBy_RoleEntityAccessPermision"},
            new String[]{ "RoleEntityAccessPermision", "iRole_RoleEntityAccessPermision"},
            new String[]{ "RoleEntityAccessPermision", "iEnityAccessPermision_RoleEntityAccessPermision"},
            new String[]{ "Seasonality", "iDescription_Seasonality"},
            new String[]{ "Seasonality", "iCode_Seasonality"},
            new String[]{ "Seasonality", "iGCRecord_Seasonality"},
            new String[]{ "Seasonality", "iCreatedBy_Seasonality"},
            new String[]{ "Seasonality", "iUpdatedBy_Seasonality"},
            new String[]{ "Store", "iCodeSupplier_Store"},
            new String[]{ "Store", "iGCRecord_Store"},
            new String[]{ "Store", "iCreatedBy_Store"},
            new String[]{ "Store", "iUpdatedBy_Store"},
            new String[]{ "Store", "iCentral_Store"},
            new String[]{ "Store", "iAddress_Store"},
            new String[]{ "Store", "iSupplier_Store"},
            new String[]{ "StoreAnalyticTree", "iRootNodeObject_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iNode_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iGCRecord_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iCreatedBy_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iUpdatedBy_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iRoot_StoreAnalyticTree"},
            new String[]{ "StoreAnalyticTree", "iObject_StoreAnalyticTree"},
            new String[]{ "StoreDocumentSeries", "iStoreSeries_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeries", "iGCRecord_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeries", "iCreatedBy_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeries", "iUpdatedBy_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeries", "iStore_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeries", "iSeries_StoreDocumentSeries"},
            new String[]{ "StoreDocumentSeriesType", "iGCRecord_StoreDocumentSeriesType"},
            new String[]{ "StoreDocumentSeriesType", "iCreatedBy_StoreDocumentSeriesType"},
            new String[]{ "StoreDocumentSeriesType", "iUpdatedBy_StoreDocumentSeriesType"},
            new String[]{ "StoreDocumentSeriesType", "iStoreSeries_StoreDocumentSeriesType"},
            new String[]{ "StoreDocumentSeriesType", "iDocumentType_StoreDocumentSeriesType"},
            new String[]{ "StorePriceList", "iGCRecord_StorePriceList"},
            new String[]{ "StorePriceList", "iCreatedBy_StorePriceList"},
            new String[]{ "StorePriceList", "iUpdatedBy_StorePriceList"},
            new String[]{ "StorePriceList", "iStore_StorePriceList"},
            new String[]{ "StorePriceList", "iPriceList_StorePriceList"},
            new String[]{ "Supplier", "iGCRecord_Supplier"},
            new String[]{ "Supplier", "iCreatedBy_Supplier"},
            new String[]{ "Supplier", "iUpdatedBy_Supplier"},
            new String[]{ "Supplier", "iTrader_Supplier"},
            new String[]{ "Supplier", "iDefaultAddress_Supplier"},
            new String[]{ "TablePermission", "iTableName_TablePermission"},
            new String[]{ "TablePermission", "iGCRecord_TablePermission"},
            new String[]{ "TablePermission", "iCreatedBy_TablePermission"},
            new String[]{ "TablePermission", "iUpdatedBy_TablePermission"},
            new String[]{ "TablePermission", "iUser_TablePermission"},
            new String[]{ "Terminal", "iDescription_Terminal"},
            new String[]{ "Terminal", "iCode_Terminal"},
            new String[]{ "Terminal", "iGCRecord_Terminal"},
            new String[]{ "Terminal", "iCreatedBy_Terminal"},
            new String[]{ "Terminal", "iUpdatedBy_Terminal"},
            new String[]{ "Terminal", "iTerminalType_Terminal"},
            new String[]{ "Terminal", "iStore_Terminal"},
            new String[]{ "Terminal", "iCustomer_Terminal"},
            new String[]{ "TerminalType", "iDescription_TerminalType"},
            new String[]{ "TerminalType", "iCode_TerminalType"},
            new String[]{ "TerminalType", "iGCRecord_TerminalType"},
            new String[]{ "TerminalType", "iCreatedBy_TerminalType"},
            new String[]{ "TerminalType", "iUpdatedBy_TerminalType"},
            new String[]{ "Trader", "iTaxCode_Trader"},
            new String[]{ "Trader", "iGCRecord_Trader"},
            new String[]{ "Trader", "iCreatedBy_Trader"},
            new String[]{ "Trader", "iUpdatedBy_Trader"},
            new String[]{ "User", "iUserName_User"},
            new String[]{ "User", "iGCRecord_User"},
            new String[]{ "User", "iCreatedBy_User"},
            new String[]{ "User", "iUpdatedBy_User"},
            new String[]{ "User", "iRole_User"},
            new String[]{ "User", "iCentralStore_User"},
            new String[]{ "UserDailyTotals", "iGCRecord_UserDailyTotals"},
            new String[]{ "UserDailyTotals", "iCreatedBy_UserDailyTotals"},
            new String[]{ "UserDailyTotals", "iUpdatedBy_UserDailyTotals"},
            new String[]{ "UserDailyTotals", "iUser_UserDailyTotals"},
            new String[]{ "UserDailyTotals", "iTerminal_UserDailyTotals"},
            new String[]{ "UserDailyTotals", "iStore_UserDailyTotals"},
            new String[]{ "UserDailyTotalsDetail", "iGCRecord_UserDailyTotalsDetail"},
            new String[]{ "UserDailyTotalsDetail", "iCreatedBy_UserDailyTotalsDetail"},
            new String[]{ "UserDailyTotalsDetail", "iUpdatedBy_UserDailyTotalsDetail"},
            new String[]{ "UserDailyTotalsDetail", "iUserDailyTotals_UserDailyTotalsDetail"},
            new String[]{ "UserDailyTotalsDetail", "iVatCategory_UserDailyTotalsDetail"},
            new String[]{ "UserDailyTotalsDetail", "iPayment_UserDailyTotalsDetail"},
            new String[]{ "UserTypeAccess", "iEntityOidUser_UserTypeAccess"},
            new String[]{ "UserTypeAccess", "iGCRecord_UserTypeAccess"},
            new String[]{ "UserTypeAccess", "iCreatedBy_UserTypeAccess"},
            new String[]{ "UserTypeAccess", "iUpdatedBy_UserTypeAccess"},
            new String[]{ "UserTypeAccess", "iUser_UserTypeAccess"},
            new String[]{ "VatCategory", "iDescription_VatCategory"},
            new String[]{ "VatCategory", "iCode_VatCategory"},
            new String[]{ "VatCategory", "iGCRecord_VatCategory"},
            new String[]{ "VatCategory", "iCreatedBy_VatCategory"},
            new String[]{ "VatCategory", "iUpdatedBy_VatCategory"},
            new String[]{ "VatFactor", "iDescription_VatFactor"},
            new String[]{ "VatFactor", "iCode_VatFactor"},
            new String[]{ "VatFactor", "iGCRecord_VatFactor"},
            new String[]{ "VatFactor", "iCreatedBy_VatFactor"},
            new String[]{ "VatFactor", "iUpdatedBy_VatFactor"},
            new String[]{ "VatFactor", "iVatLevel_VatFactor"},
            new String[]{ "VatFactor", "iVatCategory_VatFactor"},
            new String[]{ "VatLevel", "iDescription_VatLevel"},
            new String[]{ "VatLevel", "iCode_VatLevel"},
            new String[]{ "VatLevel", "iGCRecord_VatLevel"},
            new String[]{ "VatLevel", "iCreatedBy_VatLevel"},
            new String[]{ "VatLevel", "iUpdatedBy_VatLevel"}            
        };
        
        public override void Up()
        {
            //Create CompanyNew and fill data
            if (Schema.Table("Supplier").Exists())
            {
                Execute.Sql(@"select * into CompanyNew from Supplier where Oid in (select Supplier.Oid from StorePriceList 
                          inner join Store on StorePriceList.Store = Store.Oid
                         inner join Supplier on Store.Supplier = Supplier.Oid group by Supplier.oid)"
                           );


                Execute.Sql(@"if ((select count(*) from CompanyNew)<>1) Begin 	select * from NoSignleCompanySellerFound end");
            }
            
            //Delete all indexes
            foreach (String[] tableIndex in this.RetailV1Indexes)
            {
                if (Schema.Table(tableIndex[0]).Index(tableIndex[1]).Exists())
                {
                    Delete.Index(tableIndex[1]).OnTable(tableIndex[0]);
                }
                
            }
            // Delete all foreign keys
            foreach (String[] tableFk in this.RetailV1ForeignKeys)
            {
                if (Schema.Table(tableFk[0]).Constraint(tableFk[1]).Exists())
                {
                    Delete.ForeignKey(tableFk[1]).OnTable(tableFk[0]);
                }
            }

            if (Schema.Table("Supplier").Exists())
            {
                Execute.Sql("delete from Supplier where oid in (select oid from companynew)");


                //Rename Supplier to SupplierNew
                Rename.Table("Supplier").To("SupplierNew");
            }

            // Document Header

            Update.Table("DocumentHeader").Set(new { Division = 0 }).AllRows();
            if (Schema.Table("DocumentHeader").Column("PriceCatalog").Exists() == false)
            {
                Alter.Table("DocumentHeader").AddColumn("PriceCatalog").AsGuid().Nullable();
            }

            if (Schema.Table("Customer").Column("Store").Exists()
                && Schema.Table("StorePriceList").Column("Store").Exists())
            {
                Execute.Sql(@"select Customer.Code, Customer.companyname into ProblematicCustomers from 
                    CustomerStorePriceList 
                    inner join StorePriceList on CustomerStorePriceList.StorePriceList = StorePriceList.Oid
                    inner join Customer on CustomerStorePriceList.Customer = Customer.Oid 
                where 
                    customer.Store = StorePriceList.Store 
                group by 
                    Customer.Code, Customer.companyname 
                having 
                    Count(PriceList) > 1");


                Execute.Sql(@"
                Update documentheader set pricecatalog = (
                    select TOP 1 PriceList from
                    CustomerStorePriceList 
                    inner join StorePriceList on CustomerStorePriceList.StorePriceList = StorePriceList.Oid
                    inner join Customer on CustomerStorePriceList.Customer = Customer.Oid 
                    where 
                        customer.Store = StorePriceList.Store AND Customer = DocumentHeader.Customer
                    ORDER BY 
                        CustomerStorePriceList.UpdatedOnTicks DESC)
            ");
            }

            if (Schema.Table("Customer").Column("Store").Exists())
            {
                Execute.Sql(@"Insert into usertypeaccess(Oid, CreatedOnTicks, UpdatedOnTicks,RowDeleted,IsActive, IsSynchronized, [USER],EntityOid, EntityType,OptimisticLockField)
 select NEWID() as Oid, CreatedOnTicks, UpdatedOnTicks, 
0 as RowDeleted, 1 as IsActive, 1 as IsSynchronized, [USER],EntityOid, EntityType,0 as OptimisticLockField  from (
select distinct Customer.Store as EntityOid, 
UserTypeAccess.[User] as [User],
'ITS.Retail.Model.Store' as EntityType, 
(CAST(DATEDIFF(s,'1970-01-01 12:00:00',CURRENT_TIMESTAMP) As BIGINT)+ 62125920000)* 10000000 as CreatedOnTicks, 
(CAST(DATEDIFF(s,'1970-01-01 12:00:00',CURRENT_TIMESTAMP) As BIGINT)+ 62125920000)* 10000000 as UpdatedOnTicks  


from
                    CustomerStorePriceList 
                    inner join StorePriceList on CustomerStorePriceList.StorePriceList = StorePriceList.Oid
                    inner join Customer on CustomerStorePriceList.Customer = Customer.Oid 
                    inner join UserTypeAccess on EntityOid = customer.Oid
                    where 
                        Customer.Store = StorePriceList.Store) a
                        ");

            }

            //Rename Supplier columns to Owner Columns
            if (Schema.Table("Store").Column("Supplier").Exists())
            {
                Rename.Column("Supplier").OnTable("Store").To("Owner");
            }
            Execute.Sql(String.Format("update {0} set Owner = (select top 1 Oid from CompanyNew)", "Store"));
            if (Schema.Table("Customer").Column("DefaultSupplier").Exists())
            {
                Rename.Column("DefaultSupplier").OnTable("Customer").To("Owner");
            }
            if (Schema.Table("Customer").Column("Store").Exists())
            {
                Delete.Column("Store").FromTable("Customer");
            }

            //Mesurement --> Measurement
            if (Schema.Table("MesurmentUnits").Exists())
            {
                Rename.Table("MesurmentUnits").To("MeasurementUnit");
            }

            if (Schema.Table("MeasurementUnit").Column("Owner").Exists()==false)
            {

                Alter.Table("MeasurementUnit").AddColumn("Owner").AsGuid().Nullable();
            }
            Execute.Sql("update MeasurementUnit set [Owner] = (select top 1 Oid from CompanyNew) where [Owner] is null");

            if (Schema.Table("DocumentDetail").Column("MesurmentUnit").Exists())
            {
                Rename.Column("MesurmentUnit").OnTable("DocumentDetail").To("MeasurementUnit");
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
