/**
 * Add Items To Category
 * @class AddItemsToCategory
 */
var AddItemsToCategory = {
    /**
    * Submits Items to ItemCategory
     * @method SubmitItems
     * @param {Object} s
     * @param {Object} e
     * @return void
     */
    SubmitItems: function (s, e) {
        var path = $("#HOME").val() + "ItemCategory/AddItemsToCategory";
        Component.SubmitItemsToCategory(path);
    },
    /**
     * @method OnBeginCallbackAllowedItemsGrid
     * @param {Object} s
     * @param {Object} e
     */
    OnBeginCallbackAllowedItemsGrid: function (s, e) {
        /**
         * Custom arguments that are passed 
         * on begin callback of allowed items grid
         *
         * @attribute e.customArgs
         * @type object
         */
        e.customArgs = {
            "Fcode": Component.GetName("Fcode").GetValue(),
            "Fname": Component.GetName("Fname").GetValue(),
            "Fbarcode": Component.GetName("Fbarcode").GetValue(),
            "Factive": Component.GetName("Factive").GetValue(),
            "FitemSupplier": Component.GetName("FitemSupplier").GetValue(),
            "FcreatedOn": Component.GetName("FcreatedOn").GetText(),
            "FupdatedOn": Component.GetName("FupdatedOn").GetText(),
            "Fbuyer": Component.GetName("Fbuyer").GetValue(),
            "Fseasonality": Component.GetName("Fseasonality").GetValue(),
            "Fmothercode": Component.GetName("Fmothercode").GetValue(),
        };
        
        if (e.command.search("APPLYCOLUMNFILTER") !== -1) {
            Component.ClearFilters();
        }
    },
    /**
     * Click on Cancel Button of Add Items to Category
     * @method BtnClickCancelCategory
     * @param {Object} s
     * @param {Object} e
     */
    BtnClickCancelCategory: function (s, e) {
        LoadAddPopup.Hide();
    }
};