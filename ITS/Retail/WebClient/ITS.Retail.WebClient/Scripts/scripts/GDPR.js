/**
 * GDPR
 * @class Customer
 */

/**
 * @method SearchCustomer
 * @param {Object} s
 * @param {Object} e
 */
function SearchCustomer(s, e) {
    selectedItemsArray = [];
    grdGDPRCustomer.PerformCallback("SEARCH");
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
    e.customArgs.CustomerID = e.command.search('NEW') >= 0 ? -1 : grdGDPRCustomer.GetRowKey(grdGDPRCustomer.GetFocusedRowIndex());
    e.customArgs.customer_code = Component.GetName("customer_code").GetValue();
    e.customArgs.card_id = Component.GetName("card_id").GetValue();
    e.customArgs.customer_name = Component.GetName("customer_name").GetValue();
    e.customArgs.customer_tax_number = Component.GetName("customer_tax_number").GetValue();
    e.customArgs.is_active = Component.GetName("is_active").GetValue();
    e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
    e.customArgs.FupdatedOn = Component.GetName('FupdatedOn').GetText();
}

