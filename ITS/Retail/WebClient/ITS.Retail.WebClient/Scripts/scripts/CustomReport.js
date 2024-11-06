function SearchCustomReport(s, e) {
    selectedItemsArray = [];
    grdCustomReport.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}



function ExportReport(s, e) {
    if (selectedItemsArray.length > 0) {        
        var arrayclone = selectedItemsArray;
        document.location = $("#HOME").val() + "CustomReport/Download?Oids="+arrayclone;
    }
    else {
        setJSNotification(pleaseSelectAnObject);
    }
    
}

function OnBeginCallback(s, e) {

    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        //clear selected items
        selectedItemsArray = [];
    }
    if (!s.IsEditing()) {
        e.customArgs.Fcode = Component.GetName('Fcode').GetValue();
        e.customArgs.Ftitle = Component.GetName('Ftitle').GetValue();
        e.customArgs.Fdescription = Component.GetName('Fdescription').GetValue();
        e.customArgs.FcultureInfo = Component.GetName('FcultureInfo').GetValue();
        e.customArgs.Fsupplier = Component.GetName('OwnersComboBox') === null ? null : Component.GetName('OwnersComboBox').GetValue();
    }
}


function PrintSelectedReports() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectForPrinting);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectForPrinting);
    }
    else {
        url = $('#HOME').val() + 'Reports/CustomReport?Oid=' + selectedItemsArray[0];
        window.open(url);
    }
}



