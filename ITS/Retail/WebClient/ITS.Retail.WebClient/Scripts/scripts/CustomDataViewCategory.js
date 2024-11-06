var CustomDataViewCategory = {
    ExportButtonOnClick: function (s, e) {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObject);
        }
        else {
            window.location.href = $("#HOME").val() + "CustomDataViewCategory/ExportCustomDataViewCategories?CustomDataViewCategoriesOids=" + selectedItemsArray.toString();
        }
    },
    onFileUploadComplete_UploadControl: function (s, e) {
        if (e.errorText !== "" && e.errorText !== null) {
            setJSError(e.errorText);
        }
        else {
            Dialog.Hide();
            grdCustomDataViewCategories.PerformCallback();
        }
    },
    ImportButtonOnClick: function (s, e) {
        DialogCallbackPanel.PerformCallback();
    }
};