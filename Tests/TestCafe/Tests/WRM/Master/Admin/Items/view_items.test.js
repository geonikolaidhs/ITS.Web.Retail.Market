"@fixture View Item";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./../../../wrm_mixins/select_company_and_store.js';
'@require ./mixins/search_items.js';

"@test"["View Item"] = {
    'Log In': '@mixin AdminAuthentication',
    'Select Company and Store': '@mixin SelectCompanyAndStore',
    'Search Items': "@mixin SearchItems",
    "1.Assert Search returns Results": function() {
        eq($("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv").length > 0, true);
    },
    "2.Hover over First Result": function() {
        var actionTarget = function() {
            return $("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Click First Result": function() {
        var actionTarget = function() {
            return $("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.click(actionTarget);
    },
    "4.Hover over View Button": function() {
        act.hover(".btn.view");
    },
    "5.Click View Button": function() {
        act.click(".btn.view");
    },
    "6.Wait for Code input field to Show": function() {
        act.waitFor("#Code");
    }
};