
function UsersGridOnBeginCallback(s, e) {
    e.customArgs.name = Component.GetName("name").GetValue();
    e.customArgs.supplier = Component.GetName("supplier").GetValue();
    e.customArgs.customer = Component.GetName("customer").GetValue();
    e.customArgs.taxCode = Component.GetName("taxCode").GetValue();
    e.customArgs.role = Component.GetName("role").GetValue();
    e.customArgs.isActive = Component.GetName("isActive").GetValue();  
}

function onEndCallback(s, e) {
    if (grdUsers.IsEditing()) {
        var supplierComboBox = Component.GetName('SupplierComboBox');
        var storeComboBox = Component.GetName('StoreComboBox');
        if (supplierComboBox.GetValue() === null) {
            $('#UserStoreAccessGridDiv').hide();           
            $('#Message').html(supplierSelectedHasNoStores);
        }
        else {
            $('#Message').html('');
            $('#UserStoreAccessGridDiv').show();
        }
        if (typeof (IsActive) !== "undefined" && IsActive.GetMainElement() !== null && IsActive.GetValue() === null) {
            IsActive.SetValue(false);
        }
        jsonGetSupplierStores();
    }
}

function SearchUser(s, e) {
    grdUsers.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}


