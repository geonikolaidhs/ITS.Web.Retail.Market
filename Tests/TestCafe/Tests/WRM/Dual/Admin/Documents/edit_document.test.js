"@fixture Edit Document";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./mixins/search_documents.js'


"@test"["Edit Document Sales"] = {
  'Log In': '@mixin AdminAuthentication',
  'Search Documents' : '@mixin SearchDocumentsSales',
    "2.Hover over First Result": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Click First Result": function() {
        act.click("#grdDocument_DXSelBtn0_D");
    },
    "4.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    },
    "5.Click Edit Button": function() {
        act.click(".btn.edit");
    },
    "6.Wait for Edit From to Appear": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert Edit Elements are Visible": function() {
        eq($("#DocumentType").length > 0, true, "Document Type Exists");
        eq($("#DocumentSeries").length > 0, true, "Document Series Exists");
        eq($("#DocNumber").length > 0, true, "Document Number exists");
        eq($("#storeCustomers").length > 0, true, "Customer Exists");
        eq($("#RefferenceDate").length > 0, true, "Reference Date Exists");
        eq($("#InvoicingDate").length > 0, true, "Invoice Date Exists");
        eq($("#HasBeenExecuted").length > 0, true, "Invoice CheckBox Exists");
        eq($("#BillingAddress").length > 0, true, "Billing Address Exists");
        eq($("#FinalizedDate").length > 0, true, "Finilized Date Exists");
        eq($("#Status").length > 0, true, "Status Exists");
        eq($("#HasBeenChecked").length > 0, true, "Has Been Checked Exists");
        eq($("#ExecutionDate").length > 0, true, "Execution Date Exists");
        eq($("#storePriceCatalogs").length > 0, true, "PriceCatalog Exists");
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Customer Tab Exists");
        eq($("#documentEditPageControl_T3T").find(".dx-vam").length > 0, true, "Total Tab Exists");
    },
    "8.Assert Button Add New Item Exists": function() {
        eq($("#btnStartAddItem").length > 0, true, "Button Add New Item Exists");
    },
    "9.Hover over Button Add New Item": function() {
        act.hover("#btnStartAddItem");
    },
    "10.Click Button Add new Item": function() {
        act.click("#btnStartAddItem");
    },
    "11.Wait For Form to Open": function() {
        act.waitFor("#grdEditGrid_DXPEForm_tcefnew");
    },
    "12.Assert Edit Form Opens": function() {
        eq($("#grdEditGrid_DXPEForm_tcefnew").length > 0, true, "Edit Form Opens");
    }
};


"@test"["Edit Document Purchase"] = {
  'Log In': '@mixin AdminAuthentication',
  'Search Documents' : '@mixin SearchDocumentsPurchase',
    "2.Hover over First Result": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Click First Result": function() {
        act.click("#grdDocument_DXSelBtn0_D");
    },
    "4.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    },
    "5.Click Edit Button": function() {
        act.click(".btn.edit");
    },
    "6.Wait for Edit From to Appear": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert Edit Elements are Visible": function() {
        eq($("#DocumentType").length > 0, true, "Document Type Exists");
        eq($("#DocumentSeries").length > 0, true, "Document Series Exists");
        eq($("#DocNumber").length > 0, true, "Document Number exists");
        eq($("#storeSuppliers").length > 0, true, "Supplier Exists");
        eq($("#RefferenceDate").length > 0, true, "Reference Date Exists");
        eq($("#InvoicingDate").length > 0, true, "Invoice Date Exists");
        eq($("#HasBeenExecuted").length > 0, true, "Invoice CheckBox Exists");
        eq($("#BillingAddress").length > 0, true, "Billing Address Exists");
        eq($("#FinalizedDate").length > 0, true, "Finilized Date Exists");
        eq($("#Status").length > 0, true, "Status Exists");
        eq($("#HasBeenChecked").length > 0, true, "Has Been Checked Exists");
        eq($("#ExecutionDate").length > 0, true, "Execution Date Exists");
        eq($("#storePriceCatalogs").length > 0, false, "PriceCatalog Does Not Exists");
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Supplier Tab Exists");
        eq($("#documentEditPageControl_T3T").find(".dx-vam").length > 0, true, "Total Tab Exists");
    },
    "8.Assert Button Add New Item Exists": function() {
        eq($("#btnStartAddItem").length > 0, true, "Button Add New Item Exists");
    },
    "9.Hover over Button Add New Item": function() {
        act.hover("#btnStartAddItem");
    },
    "10.Click Button Add new Item": function() {
        act.click("#btnStartAddItem");
    },
    "11.Wait For Form to Open": function() {
        act.waitFor("#grdEditGrid_DXPEForm_tcefnew");
    },
    "12.Assert Edit Form Opens": function() {
        eq($("#grdEditGrid_DXPEForm_tcefnew").length > 0, true, "Edit Form Opens");
    }
};

"@test"["Edit Document Store"] = {
  'Log In': '@mixin AdminAuthentication',
  'Search Documents' : '@mixin SearchDocumentsStore',
    "2.Hover over First Result": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Click First Result": function() {
        act.click("#grdDocument_DXSelBtn0_D");
    },
    "4.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    },
    "5.Click Edit Button": function() {
        act.click(".btn.edit");
    },
    "6.Wait for Edit From to Appear": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert Edit Elements are Visible": function() {
        eq($("#DocumentType").length > 0, true, "Document Type Exists");
        eq($("#DocumentSeries").length > 0, true, "Document Series Exists");
        eq($("#DocNumber").length > 0, true, "Document Number exists");
        eq($("#secondaryStores").length > 0, true, "Secondary Store Exists");
        eq($("#RefferenceDate").length > 0, false, "Reference Date Does Not Exists");
        eq($("#InvoicingDate").length > 0, false, "Invoice Date Does Not Exists");
        eq($("#HasBeenExecuted").length > 0, false, "Invoice CheckBox Does Not Exists");
        eq($("#BillingAddress").length > 0, false, "Billing Address Does Not Exists");
        eq($("#FinalizedDate").length > 0, true, "Finilized Date Exists");
        eq($("#Status").length > 0, true, "Status Exists");
        eq($("#HasBeenChecked").length > 0, false, "Has Been Checked Does Not Exists");
        eq($("#ExecutionDate").length > 0, false, "Execution Date Not Exists Exists");
        eq($("#storePriceCatalogs").length > 0, false, "PriceCatalog Does Not Exists");
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Supplier Tab Exists");
        eq($("#documentEditPageControl_T2T").find(".dx-vam").length > 0, false, "Third Tab Does Not Exists");
    },
    "8.Assert Button Add New Item Exists": function() {
        eq($("#btnStartAddItem").length > 0, true, "Button Add New Item Exists");
    },
    "9.Hover over Button Add New Item": function() {
        act.hover("#btnStartAddItem");
    },
    "10.Click Button Add new Item": function() {
        act.click("#btnStartAddItem");
    },
    "12.Wait For Form to Open": function() {
        act.waitFor("#grdEditGrid_DXPEForm_tcefnew");
    },
    "13.Assert Edit Form Opens": function() {
        eq($("#grdEditGrid_DXPEForm_tcefnew").length > 0, true, "Edit Form Opens");
    }
};
