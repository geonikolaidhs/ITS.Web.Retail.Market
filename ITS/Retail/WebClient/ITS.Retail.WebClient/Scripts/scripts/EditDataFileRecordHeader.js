
function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "DataFileRecordHeader/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {
                LoadEditPopup.Hide();
            }
        },
        error: function (data) {
            setJSError(data);
            LoadEditPopup.Hide();
        }
    });
}

function OnBeginCallbackDataFileRecordDetailGrid(s, e) {

    if (s.IsEditing()) {
        e.customArgs.Padding = PaddingCheckBox.GetChecked();
        e.customArgs.Trim = TrimCheckBox.GetChecked();
        e.customArgs.AllowNull = AllowNullCheckBox.GetChecked();
        e.customArgs.UseThirdPartNum = UseThirdPartNumCheckBox.GetChecked();
    }
}

function OnEndCallbackDataFileRecordDetailGrid(s, e) {
    KeyPropertyCbPanel.PerformCallback();
    ReferencePropertyCallbackPanel.PerformCallback();
}

function OnValueChangedEntityComboBox(s, e) {
    var path = $("#HOME").val() + "DataFileRecordHeader/jsonEntityChanged";
    //todo ask confirmation and clear existing details
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'EntityName': s.GetValue() },
        cache: false,
        dataType: 'json',
        success: function (data) {
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}