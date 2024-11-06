"@fixture Search Documents";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./../../../wrm_mixins/select_company_and_store.js';
'@require ./mixins/search_documents.js'

"@test"["Search Sales Documents"] = {
    'Log In': '@mixin AdminAuthentication',
    'Select Company and Store': '@mixin SelectCompanyAndStore',
    'Search Documents' : '@mixin SearchDocumentsSales',
    "1.Check if Search returned results": function() {
        ok($("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv").length > 0, "Check if first Document Grid Row Exists");
    },
    "2.Hover over First Row Checkbox": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    }
};

"@test"["Search Purchase Documents"] = {
    'Log In': '@mixin AdminAuthentication',
    'Select Company and Store': '@mixin SelectCompanyAndStore',
    'Search Documents' : '@mixin SearchDocumentsPurchase',
    "1.Check if Search returned results": function() {
        ok($("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv").length > 0, "Check if first Document Grid Row Exists");
    },
    "2.Hover over First Row Checkbox": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    }
};

"@test"["Search Store Documents"] = {
    'Log In': '@mixin AdminAuthentication',
    'Select Company and Store': '@mixin SelectCompanyAndStore',
    'Search Documents' : '@mixin SearchDocumentsStore',
    "1.Check if Search returned results": function() {
        ok($("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv").length > 0, "Check if first Document Grid Row Exists");
    },
    "2.Hover over First Row Checkbox": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    }
};
