function FieldSelectedIndexChanged(s, e) {
    if (s.GetValue().indexOf("CustomEnumeration") >= 0) {
        CustomEnumerationComboBox.SetEnabled(true);
        CustomEnumerationComboBox.SetVisible(true);
        CustomEnumerationLabel.SetVisible(true);
    }
    else {
        CustomEnumerationComboBox.SetEnabled(false);
        CustomEnumerationComboBox.SetVisible(false);
        CustomEnumerationComboBox.SetValue(null);
        CustomEnumerationComboBox.SetText(null);
        CustomEnumerationLabel.SetVisible(false);
    }
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "DiscountType/CancelEdit",
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

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}