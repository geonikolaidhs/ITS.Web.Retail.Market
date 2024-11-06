function SearchPOSKeysLayout(s, e) {
    grdPOSKeysLayouts.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}



function onFileUploadComplete_UploadControl(s, e) {
    if (e.errorText !== "" && e.errorText !== null) {
        setJSError(e.errorText);
    }
    else {
        Dialog.Hide();
        grdPOSKeysLayouts.PerformCallback();
    }
}


function ImportButtonOnClick(s, e) {
    DialogCallbackPanel.PerformCallback();
}


function OnBeginCallback(s, e) {

	if (!s.IsEditing()) {
		e.customArgs.fcode = Component.GetName('fcode').GetValue();
		e.customArgs.fdescription = Component.GetName('fdescription').GetValue();
	}
}

function ExportButtonOnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        window.location.href = $("#HOME").val() + "POSKeysLayout/ExportKeysLayout?POSKeysLayoutGuids=" + selectedItemsArray.toString();
    }
}


