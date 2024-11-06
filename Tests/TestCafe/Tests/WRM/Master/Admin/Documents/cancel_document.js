"@fixture Cancel Document";
"@page ./Home";
'@require ./../../../wrm_mixins/authentication/admin_authentication.js';
'@require ./../../../wrm_mixins/select_company_and_store.js';
'@require ./mixins/search_documents.js'

"@test"["Cancel Sales Document"] = {
	'Log In': '@mixin AdminAuthentication',
	'Select Company and Store': '@mixin SelectCompanyAndStore',
	'Search Documents' : '@mixin SearchDocumentsSales',
    "1.Assert Options Button": function() {
        eq($(".btn.options").length > 0, true, "Options Button Exists");
    },
    "2.Hover over First Result": function() {
        var actionTarget = function() {
            return $("#grdDocument_DXDataRow0").find(".dxgvCommandColumn_ITSTheme1.firstCollumn.dxgv");
        };
        act.hover(actionTarget);
    },
    "3.Click First Result": function() {
        act.click("#grdDocument_DXSelBtn0_D");
    },
    "4.Hover over Options Button": function() {
        act.hover(".btn.options");
    },
    "5.Click Options Button": function() {
        act.click(".btn.options");
    },
    "6.Wait 200 milliseconds": function() {
        act.wait(200);
    },
    "7.Assert Cancel Button Exists": function() {
        eq($(".btn.undo").length > 0, true, "Cancel Button Exists");
    },
    "8.Hover over Cancel Button": function() {
        act.hover(".btn.undo");
    },
    "9.Click Cancel Document Button": function() {
    	var cancelButton = $(".btn.undo");
        handleConfirm('OK');
    	//handleConfirm(true);//TODO see case 6099
        act.click(cancelButton);
    },
    // "10.Make test fail explicitly": function() {
    //     eq($(".btn.undooo").length > 0, true, "Confirmation needs to be removed.Afterwards fix test");
    // },
    "10.Wait 200 milliseconds": function() {
        act.wait(200);
    }
};

