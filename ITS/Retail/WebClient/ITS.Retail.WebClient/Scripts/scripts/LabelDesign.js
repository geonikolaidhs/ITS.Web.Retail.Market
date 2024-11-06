var LabelDesign = {
    ExportEJF: function () {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObject);
        }
        else if (selectedItemsArray.length > 1) {
            setJSNotification(pleaseSelectOnlyOneObject);
        }
        else if (selectedItemsArray.length === 1) {
            window.location.href = $("#HOME").val() + "LabelDesign/DownloadEJF?LabelOid=" + selectedItemsArray[0];
        }
    }
};