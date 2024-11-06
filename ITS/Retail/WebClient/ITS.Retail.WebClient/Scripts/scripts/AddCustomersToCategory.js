/**
 * Add Customers To Category
 * @class AddCustomersToCategory
 */
var AddCustomersToCategory = {
    /**
     * 
     * @method SubmitCustomers
     * @param {Object} s
     * @param {Object} e
     * @return void
     */
    SubmitCustomers: function (s, e) {
        var path = $("#HOME").val() + "CustomerCategory/InsertCustomers";
        Component.SubmitItemsToCategory(path);
    },
    /**
     * @method OnBeginCallbackAllowedCustomersGrid
     * @param {Object} s
     * @param {Object} e
     * @return void
     */
    OnBeginCallbackAllowedCustomersGrid: function (s, e) {
        e.customArgs = {
            "customer_code": Component.GetName("customer_code").GetValue(),
            "card_id": Component.GetName("card_id").GetValue(),
            "customer_name": Component.GetName("customer_name").GetValue(),
            "customer_tax_number": Component.GetName("customer_tax_number").GetValue(),
            "is_active": Component.GetName("is_active").GetValue(),
            "FcreatedOn": Component.GetName("FcreatedOn").GetText(),
            "FupdatedOn": Component.GetName("FupdatedOn").GetText()
        };

        if (e.command.search("APPLYCOLUMNFILTER") !== -1) {
            Component.ClearFilters();
        }
    },
    /**
     * @method BtnClickCancelCategory
     * @param {Object} s
     * @param {Object} e
     */
    BtnClickCancelCategory: function (s, e) {
        LoadAddPopup.Hide();
    }
};
/**
 * @method GetSelectedFieldValuesCallback
 * @param {Object} values
 */
function GetSelectedFieldValuesCallback(values) {
    for (var i = 0; i < values.length; i++) {
        selectedItemsArray.push(values[i]);
    }
    btnOK.SetEnabled(true);
    allowedCustomersSelection.HideLoadingPanel();
}
