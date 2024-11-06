function ExportButtonOnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        window.location.href = $("#HOME").val() + "POSPrintFormat/ExportDevices?POSDeviceGuids=" + selectedItemsArray.toString();

    }

}