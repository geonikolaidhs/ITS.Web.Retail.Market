"@fixture Edit Item";
"@page ./Home";
"@auth admin:admin";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./../../../wrm_mixins/select_company_and_store.js';
'@require ./mixins/search_items.js';

"@test"["Edit Item"] = {
    'Log In': '@mixin AdminAuthentication',
    'Select Company and Store': '@mixin SelectCompanyAndStore',
    'Search Items': "@mixin SearchItems",
    "1.Hover First Result": function() {
        var actionTarget = function() {
            return $("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "2.Click and Select First Result": function() {
        var actionTarget = function() {
            return $("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.click(actionTarget);
    },
    "3.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    },
    "4.Click Edit Button": function() {
        act.click(".btn.edit");
    },
    "5.Wait for Code input field to Show": function() {
        act.waitFor("#Code");
    },
    "6.Assert Edit Form Opens": function() {
        eq($("#btnUpdate").length > 0, true, "Save Button Exists");
        eq($("#btnCancel").length > 0, true, "Cancel Button Exists");
    }
};
