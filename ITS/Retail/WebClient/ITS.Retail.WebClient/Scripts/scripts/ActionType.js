var ActionType;

ActionType = (function () {
    return {
        RecalculateActionTypes: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            } else {
                ActionTypeRecalculateForm.Show();
                ActionTypeRecalculateForm.PerformCallback();
            }
        },
        ActionTypeRecalculateFormBeginCallback: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            } else {
                e.customArgs['ActionTypes'] = selectedItemsArray.toString();
            }
        },
        OnActionTypeCategoryChanged: function (s, e) {
            e.customArgs['category'] = Component.GetName('Category').GetValue();
        },
        ExportButtonOnClick: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            } else {
                window.location.href = $("#HOME").val() + "ActionType/ExportActionTypes?ActionTypesOids=" + selectedItemsArray.toString();
            }
        },
        onFileUploadComplete_UploadControl: function (s, e) {
            if (e.errorText !== "" && e.errorText !== null) {
                setJSError(e.errorText);
            } else {
                Dialog.Hide();
                grdActionType.PerformCallback();
            }
        },
        ImportButtonOnClick: function (s, e) {
            DialogCallbackPanel.PerformCallback();
        },
        UpdateModeValueChanged: function (s, e) {
            StoresComboBoxCallBackPanel.PerformCallback({
                updatemode: s.GetValue()
            });
        }
    };
})();