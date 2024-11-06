var POSLayout = (function () {
    return {
        ExportLayout: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            }
            else if (selectedItemsArray.length > 1) {
                setJSNotification(pleaseSelectOnlyOneObject);
            }
            else if (selectedItemsArray.length === 1) {
                window.location.href = $("#HOME").val() + "POSLayout/DownloadPOSLayout?LayoutOid=" + selectedItemsArray[0];
            }
        }
    };
})();