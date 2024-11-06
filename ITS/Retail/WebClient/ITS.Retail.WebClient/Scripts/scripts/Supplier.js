function SearchSupplier(s, e) {
    selectedItemsArray = [];
    grdSupplier.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function passValues(s, e) {
    if (e.command.search("APPLYCOLUMNFILTER") !== -1) {
        //clear selected items
        selectedItemsArray = [];
        filterSelected = true;
    }
    
    e.customArgs.SupplierID = e.command.search('NEW') >= 0 ? - 1: grdSupplier.GetRowKey(grdSupplier.GetFocusedRowIndex());    

    e.customArgs.supplier_code = Component.GetName("supplier_code").GetValue();
    e.customArgs.supplier_name = Component.GetName("supplier_name").GetValue();
    e.customArgs.supplier_tax_number = Component.GetName("supplier_tax_number").GetValue();
    e.customArgs.is_active = Component.GetName("is_active").GetValue();
    e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
    e.customArgs.FupdatedOn = Component.GetName('FupdatedOn').GetText();
}

function grdAddress_Row_Changed(s, e) {
    if (s.GetFocusedRowIndex() >= 0) {
        var fromObj = '#divPhone' + s.GetRowKey(s.GetFocusedRowIndex()).replace('-', '_').replace('-', '_').replace('-', '_').replace('-', '_');
        var toObj = '#' + s.name.replace('grdAddress', 'divGridPhone_');
        var ob1 = $(fromObj);
        var ob2 = $(toObj);
        ob2.html(ob1.html());
    }
}

function grdStore_Row_Changed(s, e) {
    if (s.GetFocusedRowIndex() >= 0) {
        var fromObj = '#divPhone' + s.GetRowKey(s.GetFocusedRowIndex()).replace('-', '_').replace('-', '_').replace('-', '_').replace('-', '_');
        var toObj = '#' + s.name.replace('grdStore', 'divGridPhoneStore_');
        var ob1 = $(fromObj);
        var ob2 = $(toObj);
        ob2.html(ob1.html());
    }
}

//function onKeyPressFilters(s, e) {
//    if (e.htmlEvent.keyCode == 13) {
//        btnSearch.DoClick();
//        s.Focus()
//    }
//}











