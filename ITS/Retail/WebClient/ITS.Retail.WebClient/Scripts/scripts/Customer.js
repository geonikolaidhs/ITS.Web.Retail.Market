/**
 * Customer
 * @class Customer
 */

/**
 * @method SearchCustomer
 * @param {Object} s
 * @param {Object} e
 */
function SearchCustomer(s, e) {
    selectedItemsArray = [];
    grdCustomer.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

/**
 * @method passValues
 * @param {Object} s
 * @param {Object} e
 */
function passValues(s, e) {
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        selectedItemsArray = [];
    }    
    e.customArgs.CustomerID = e.command.search('NEW') >= 0 ? -1 : grdCustomer.GetRowKey(grdCustomer.GetFocusedRowIndex());
    e.customArgs.customer_code = Component.GetName("customer_code").GetValue();
    e.customArgs.card_id = Component.GetName("card_id").GetValue();
    e.customArgs.customer_name = Component.GetName("customer_name").GetValue();
    e.customArgs.customer_tax_number = Component.GetName("customer_tax_number").GetValue();
    e.customArgs.is_active = Component.GetName("is_active").GetValue();
    e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
    e.customArgs.FupdatedOn = Component.GetName('FupdatedOn').GetText();
}
/**
 * @method onClickCheckAll
 * @param {Object} s
 * @param {Object} e
 */
function onClickCheckAll(s, e) {
    grdCustomer.SelectAllRowsOnPage(true);
}
/**
 * @method onClickUncheckAll
 * @param {Object} s
 * @param {Object} e
 */
function onClickUncheckAll(s, e) {
    grdCustomer.SelectAllRowsOnPage(false);
}
/**
 * @method grdAddress_Row_Changed
 * @param {Object} s
 * @param {Object} e
 */
function grdAddress_Row_Changed(s, e) {
    if (s.GetFocusedRowIndex() >= 0) {
        var fromObj = '#divPhone' + s.GetRowKey(s.GetFocusedRowIndex()).replace('-', '_').replace('-', '_').replace('-', '_').replace('-', '_');
        var toObj = '#' + s.name.replace('grdAddress', 'divGridPhone_');
        var ob1 = $(fromObj);
        var ob2 = $(toObj);
        ob2.html(ob1.html());
    }
}

/**
 * @method CorrectPoints
 * @param {Object} s
 * @param {Object} e
 */
function CorrectPoints(s, e) {
    grdPreviewCustomerPoints.PerformCallback("CorrectPoints");
}
function grdPreviewCustomerPoints_BeginCallback(s, e) {
    CustomizationWindow();
    if (e.command == "CUSTOMCALLBACK") {
        e.customArgs.guids = selectedItemsArray.toString();
    }
}

