function SearchPOSDevices(s, e) {
    grdPOSDevices.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}



function onFileUploadComplete_UploadControl(s, e) {
    if (e.errorText !== "" && e.errorText !== null) {
        setJSError(e.errorText);
    }
    else {
        Dialog.Hide();
        grdPOSDevices.PerformCallback();
    }
}

function OnBeginCallback(s, e) {

    if (!s.IsEditing()) {
        e.customArgs.fconnectionType = Component.GetName('fconnectionType').GetValue();
        e.customArgs.fname = Component.GetName('fname').GetValue();
        e.customArgs.fdeviceType = Component.GetName('fdeviceType').GetValue();
    }
}



function ImportButtonOnClick(s, e) {
    DialogCallbackPanel.PerformCallback();
}



function ExportButtonOnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        window.location.href = $("#HOME").val() + "POSDevice/ExportDevices?POSDeviceGuids=" + selectedItemsArray.toString();

    }

}

function CreatePOSDeviceDatabase(s, e) {
    if (confirm(processWillTakeAFewMinutesAreYouSure) === true) {
        var path = $('#HOME').val() + 'POSDevice/CreatePOSDeviceDatabase';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
            },
            cache: false,
            dataType: 'json',
            success: function (data) {
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
        DialogCallbackPanel.PerformCallback('DEVICE_DATABASE_PROCESS_DIALOG');
    }
}

function Dialog_OnShown(s, e) {

    var interval = setInterval(function () {
        var path = $('#HOME').val() + 'POSDevice/jsonCheckPOSDeviceDatabaseRunning';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
            },
            cache: false,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (data.done === true) {
                    btnDialogOK.SetVisible(true);
                    lblProgress.SetText(successMessage);
                    $('#processingImage').hide();
                    clearInterval(interval);
                }
            },
            error: function (data) {
            }
        });
    }, 3000);
}
