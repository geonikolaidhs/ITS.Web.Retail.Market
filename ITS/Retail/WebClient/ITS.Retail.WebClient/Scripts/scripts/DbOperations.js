$(document).ready(function () {
    if (OperationCheckbox.GetSelectedIndex() === 0) {
        $("#select-area").hide();
        $("#update-set-area").show();
        TableComboPartialCallbackPanel.SetVisible(true);
    }
    else {
        $("#select-area").show();
        $("#update-set-area").hide();
        TableComboPartialCallbackPanel.SetVisible(false);
    }

    TableComboPartialCallbackPanel.PerformCallback();
});

function OperationCheckboxValueChanged(s, e) {

    if (s.GetSelectedIndex() === 0) {
        TableComboPartialCallbackPanel.SetVisible(true);
        //TableComboPartialCallbackPanel.PerformCallback();
        $("#select-area").hide();
        $("#update-set-area").show();
        
    }
    else {                
        TableComboPartialCallbackPanel.SetVisible(false);
        $("#select-area").show();
        $("#update-set-area").hide();
    }
}



function clearComboBoxDbOperations(s, e) {
    s.SetText("");
    s.SetValue(null);
}