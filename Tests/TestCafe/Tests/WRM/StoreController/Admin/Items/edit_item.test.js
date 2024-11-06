"@fixture Edit Item";
"@page ./Item/Index";
"@auth admin:admin";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js'
'@require ./mixins/search_items.js';

"@test"["Edit Item"] = {
    'Log In': '@mixin AdminAuthentication',
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
    "3.Assert Edit Button Does Not Exists": function() {
        eq($(".btn.edit").length > 0, false, "Edit Button Does Not Exists");
    }
};
