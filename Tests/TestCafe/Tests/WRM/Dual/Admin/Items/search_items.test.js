"@fixture Search Items";
"@page ./Home";
"@auth admin:admin";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./mixins/search_items.js';

"@test"["Search Items"] = {
    'Log In': '@mixin AdminAuthentication',
    'Search Items': "@mixin SearchItems",
    "1.Assert Search returns Results": function() {
        eq($("#grdItems_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv").length > 0, true);
    }
};
