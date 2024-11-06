"@fixture View Document";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./mixins/search_documents.js'


"@test"["View Document Sales"] = {
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
    "4.Hover over View Button": function() {
        act.hover(".btn.view");
    },
    "5.Click View Button": function() {
        act.click(".btn.view");
    },
    "6.Wait For View Popup to Show": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert View Elements are Visible": function() {
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
        eq($("#PriceCatalog").length > 0, true, "PriceCatalog Exists");
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Customer Tab Exists");
        eq($("#documentEditPageControl_T3T").find(".dx-vam").length > 0, true, "Total Tab Exists");
    }
};

"@test"["View Document Purchase"] = {
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
    "4.Hover over View Button": function() {
        act.hover(".btn.view");
    },
    "5.Click View Button": function() {
        act.click(".btn.view");
    },
    "6.Wait For View Popup to Show": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert View Elements are Visible": function() {
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
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Supplier Tab Exists");
        eq($("#documentEditPageControl_T3T").find(".dx-vam").length > 0, true, "Total Tab Exists");
    }
};

"@test"["View Document Store"] = {
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
    "4.Hover over View Button": function() {
        act.hover(".btn.view");
    },
    "5.Click View Button": function() {
        act.click(".btn.view");
    },
    "6.Wait For View Popup to Show": function() {
        act.waitFor("#DocumentType");
    },
    "7.Assert View Elements are Visible": function() {
        eq($("#DocumentType").length > 0, true, "Document Type Exists");
        eq($("#DocumentSeries").length > 0, true, "Document Series Exists");
        eq($("#DocNumber").length > 0, true, "Document Number exists");
        eq($("#SecondaryStore").length > 0, true, "Secondary Store Exists");
        eq($("#FinalizedDate").length > 0, true, "Finilized Date Exists");
        eq($("#Status").length > 0, true, "Status Exists");
        eq($("#documentEditPageControl_AT0T").find(".dx-vam").length > 0, true, "Document Details Grid Exists");
        eq($("#documentEditPageControl_T1T").find(".dx-vam").length > 0, true, "Total Grid Exists");
    }
};