/*
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "ImportButtonOnClick";
            */

function ImportButtonOnClick(s, e) {
    DialogCallbackPanel.PerformCallback();
}

function ExportButtonOnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        window.location.href = $("#HOME").val() + "CustomEnumeration/Export?HeadersGuids=" + selectedItemsArray.toString();
    }
}
function onFileUploadComplete_UploadControl(s, e) {
    if (e.errorText !== "" && e.errorText !== null) {
        setJSError(e.errorText);
    }
    else {
        Dialog.Hide();
        grdCustomEnumeration.PerformCallback();
    }
}