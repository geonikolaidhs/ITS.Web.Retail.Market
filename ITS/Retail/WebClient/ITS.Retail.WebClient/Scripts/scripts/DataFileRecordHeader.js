function refreshgrid() {
    grdDataFileRecordHeaders.Refresh();
}

function ExportButtonOnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        window.location.href = $("#HOME").val() + "DataFileRecordHeader/Export?HeadersGuids=" + selectedItemsArray.toString();
    }
}


function onFileUploadComplete_UploadControl(s, e) {
    if (e.errorText !== "" && e.errorText !== null) {
        setJSError(e.errorText);
    }
    else {
        Dialog.Hide();
        grdDataFileRecordHeaders.PerformCallback();
    }
}


function ImportButtonOnClick(s, e) {
    DialogCallbackPanel.PerformCallback();
}
