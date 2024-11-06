"@fixture Document Sales Page";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/customer_authentication.js';
'@require ./mixins/search_documents.js'

"@test"["List Document Sales"] = {
    'Log In': '@mixin CustomerAuthentication',
    "1.Wait for Store Selection to Show": function() {
        act.waitFor("#LayoutStoreSelection");
    },
    "2.Assert Store Menu Exists": function() {
        eq($("#LayoutStoreSelection").length > 0, true, "Select Store Menu Exists");
    },
    "3.Hover over Select Store Menu": function() {
        act.hover("#LayoutStoreSelection");
    },
    "4.Click Select Store Menu": function() {
        act.click("#LayoutStoreSelection");
    },
    "5.Wait for Store Item Selection to Show": function() {
        act.waitFor("#LayoutStoreSelection ul li:first-child");
    },
    "6.Click First Store": function() {
        act.click("#LayoutStoreSelection ul li:first-child");
    },
    'Search Sales Documents': '@mixin SearchDocumentsSales',
    "7.Hover over first row checkbox": function() {
        act.hover("#grdDocument_DXSelBtn0_D");
    },
    "8.Click first row checkbox": function() {
        act.click("#grdDocument_DXSelBtn0_D");
    },
    "9.Hover over Edit Button": function() {
        act.hover(".btn.edit");
    },
    "10.Click Edit Button": function() {
        act.click(".btn.edit");
    }
};
