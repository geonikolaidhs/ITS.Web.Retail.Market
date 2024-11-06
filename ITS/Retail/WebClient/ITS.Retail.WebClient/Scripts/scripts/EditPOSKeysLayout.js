function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "POSKeysLayout/CancelEdit",
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

//function btnImportClick() {
//    ImportKeyMappingsPopup.Show();
//}

//function btnExportClick() {
//    window.location.href = $("#HOME").val() + "POSKeysLayout/ExportKeyMappings";
//}

function KeyDataTextBox_OnKeyPress(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    var actualkey = String.fromCharCode(unicode);
    var valid_chars = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
    if ((actualkey in valid_chars)) {
        e.htmlEvent.returnValue = true;
    }
    else if (unicode != 9 && //tab
             unicode != 8 && //backspace
             unicode != 37 &&  //left arrow
             unicode != 39)  //right arrow) 
    {
        e.htmlEvent.preventDefault();
    }
    else {
       e.htmlEvent.returnValue = true;
    }
}

function ManageVisibilityOfInputs() {
    if (EntryMode.GetSelectedItem().index === 0) {
        $("#keydataEntryMode").hide();
        $("#redirectKeydataEntryMode").hide();
        $("#keysEntryMode").show();
        $("#redirectKeysEntryMode").show();
        if (ActionCode.GetValue() == "NONE") {
            $("#redirectKeysEntryMode").css("visibility", "visible");
        }
        else {
            $("#redirectKeysEntryMode").css("visibility", "hidden");
            ClearRedirectKeyInputFields();
        }
    }
    else if (EntryMode.GetSelectedItem().index === 1) {
        $("#keysEntryMode").hide();
        $("#redirectKeysEntryMode").hide();
        $("#keydataEntryMode").show();
        $("#redirectKeydataEntryMode").show();
        if (ActionCode.GetValue() == "NONE") {
            $("#redirectKeydataEntryMode").css("visibility", "visible");
            $("#redirectKeydataEntryMode").show();
            $('#RedirectKeyData').parent().css('visibility', 'visible');
        }
        else {
            $("#redirectKeydataEntryMode").css("visibility", "hidden");
            $('#RedirectKeyData').parent().css('visibility', 'hidden');
            ClearRedirectKeyInputFields();
        }
    }
}

function OnValueChangedEntryModeRadioList(s, e) {
    ManageVisibilityOfInputs();
}

function ActionCodeComboBoxOnValueChanged(s, e) {
    ManageVisibilityOfInputs();
    ActionParametersCallbackPanel.PerformCallback(ActionCode.GetValue());
}

function ClearKeyInputFields() {
	ActionCode.SetSelectedIndex(0);
	KeyData.SetText("");
	KeyCode.SetSelectedIndex(0);
	CtrlCheckBox.SetChecked(false);
	ShiftCheckBox.SetChecked(false);
	AltCheckBox.SetChecked(false);
	ActionCodeComboBoxOnValueChanged();
}

function ClearRedirectKeyInputFields() {
	RedirectKeyData.SetText("");
	RedirectKeyCode.SetSelectedIndex(0);
	RedirectCtrlCheckBox.SetChecked(false);
	RedirectShiftCheckBox.SetChecked(false);
	RedirectAltCheckBox.SetChecked(false);
}

function AddKeyMapping(s, e) {
	var selectedAction = ActionCode.GetValue(),
		selectedMode = EntryMode.GetSelectedItem().text,
		keyData = KeyData.GetValue(),
		keyCode = KeyCode.GetValue(),
		isCtrlChecked = CtrlCheckBox.GetChecked(),
		isShiftChecked = ShiftCheckBox.GetChecked(),
		isAltChecked = AltCheckBox.GetChecked(),
		redirectKeyData = RedirectKeyData.GetValue(),
		redirectKeyCode = RedirectKeyCode.GetValue(),
		isRedirectCtrlChecked = RedirectCtrlCheckBox.GetChecked(),
		isRedirectShiftChecked = RedirectShiftCheckBox.GetChecked(),
		isRedirectAltChecked = RedirectAltCheckBox.GetChecked(),
		actionParameters = {},
		controls = ASPxClientControl.GetControlCollection();

	for (var name in controls.elements) {
		if (endsWith(name, "_actionParam")) {
		    var control = controls.GetByName(name);
		    if (control.GetValueInputToValidate() !== null) {
		        actionParameters[control.name.substring(0, control.name.indexOf("_actionParam"))] = control.GetValue();
		    }
		}
	}

	$.ajax({
		type: 'POST',
		url: $('#HOME').val() + 'POSKeysLayout/jsonAddKeyMapping',
		cache: false,
		dataType: 'json',
		data: {
		    'POSKeysLayoutGuid': document.getElementById('POSKeysLayoutGuid').value,
		    'ActionCode': selectedAction, 'EntryMode': selectedMode, 'KeyData': keyData,
		    'KeyCode': keyCode, 'isCtrlChecked': isCtrlChecked, 'isAltChecked': isAltChecked,
		    'isShiftChecked': isShiftChecked, 'RedirectKeyData': redirectKeyData, 'RedirectKeyCode': redirectKeyCode, 'isRedirectCtrlChecked': isRedirectCtrlChecked,
		    'isRedirectAltChecked': isRedirectAltChecked, 'isRedirectShiftChecked': isRedirectShiftChecked, 'actionParameters': JSON.stringify(actionParameters)
		},
		async: false,
		success: function (data) {
		    $('#message').empty();
		    $('#message').show();
		    if (data.success) {
		        ClearKeyInputFields();
		        ClearRedirectKeyInputFields();
		        grdPOSKeyMappings.PerformCallback();
		        $('#message').css('color', 'green');
		        $('#message').html('Success');
		    }
		    else {
		        $('#message').css('color', 'red');
		        $('#message').html(data.error).fadeOut(2000);
		    }
		},
		error: function (data) {
		}
	});
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        ImportKeyMappingsPopup.Hide();
        grdPOSKeyMappings.PerformCallback();
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}