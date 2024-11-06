function SearchPOS(s, e) {
    grdPOSs.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function OnBeginCallback(s, e) {

	if (!s.IsEditing()) {
		e.customArgs.fid = Component.GetName('fid').GetValue();
		e.customArgs.fname = Component.GetName('fname').GetValue();
		e.customArgs.fcustomer = Component.GetName('fcustomer').GetValue();
		e.customArgs.fFiscalDevice = Component.GetName('fFiscalDevice').GetValue();
		e.customArgs.fIPAddress = Component.GetName('fIPAddress').GetValue();
	}
}

function MassivelyCreatePOS(s, e) {
    createMassivelyForm.Show();
}

function MassivelyCreatePOSCancelClick(s, e) {
    createMassivelyForm.Hide();
}

function MassivelyCreatePOSCreateClick() {
    if (ValidateModalFormSingle() === false) {
        setJSError(markedFieldsAreRequired);
    }
    else
    {
        var path = $('#HOME').val() + 'POS/CreateMassivelyPOS';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
                'IDs': $.trim(IDs.GetValue()),
                'ABCDirectory': $.trim(ABCDirectory.GetValue()),
                'FiscalDevice': $.trim(FiscalDevice.GetValue()),
                'ReceiptFormat': $.trim(ReceiptFormat.GetValue()),
                'XFormat': $.trim(XFormat.GetValue()),
                'ZFormat': $.trim(ZFormat.GetValue()),
                'POSKeysLayout': $.trim(POSKeysLayout.GetValue()),
                'POSLayout': $.trim(POSLayout.GetValue()),
                'DefaultPaymentMethod': $.trim(DefaultPaymentMethod.GetValue()),
                'UsesTouchScreen': $.trim(UsesTouchScreen.GetValue()),
                'UsesKeyLock': $.trim(UsesKeyLock.GetValue()),
                'POSActionLevelsSet': $.trim(POSActionLevelsSet.GetValue()),
                'POSDevices': selectedPOSDevices.toString(),
                "AutoFocus": $.trim(AutoFocus.GetValue()),
                "AsksForStartingAmount": $.trim(AsksForStartingAmount.GetValue()),
                'EnableLowEndMode': $.trim(EnableLowEndMode.GetValue()),
                'DemoMode': $.trim(DemoMode.GetValue()),
                "AsksForFinalAmount": $.trim(AsksForFinalAmount.GetValue()),
                "ForcedWithdrawMode": $.trim(ForcedWithdrawMode.GetValue()),
                "ForcedWithdrawCashAmountLimit": $.trim(ForcedWithdrawCashAmountLimit.GetValue())
            },
            cache: false,
            dataType: 'json',
            success: function (data) {
                createMassivelyForm.Hide();
                grdPOSs.PerformCallback();
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}

function CreatePOSDatabase(s, e) {    
    if (confirm(processWillTakeAFewMinutesAreYouSure) === true) {
        var path = $('#HOME').val() + 'POS/CreatePOSDatabase';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
            },
            cache: false,
            dataType: 'json',
            success: function (data) {
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
        DialogCallbackPanel.PerformCallback('DATABASE_PROCESS_DIALOG');
    }
}

function Dialog_OnShown(s, e) {

    var interval= setInterval(function () {
        var path = $('#HOME').val() + 'POS/jsonCheckPOSDatabaseRunning';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
            },
            cache: false,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (data.done === true) {
                    btnDialogOK.SetVisible(true);
                    lblProgress.SetText(successMessage);
                    $('#processingImage').hide();
                    clearInterval(interval);
                }
            },
            error: function (data) {
            }
        });
    }, 3000);    
}


var selectedPOSDevices = [];
function POSDeviceSelectionChanged(s,e) {
    if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
    }
    else {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
    }

    if (!e.isSelected && e.visibleIndex == "-1") {
        Component.GetName(s.name).ShowLoadingDivAndPanel();
        s.GetSelectedFieldValues("Oid", OnPOSDeviceGetValues);
    }

    if (!e.isAllRecordsOnPage) {
        if (e.isSelected) {
            selectedPOSDevices.push(s.GetRowKey(e.visibleIndex));
        }
        else {
            selectedPOSDevices.splice(selectedPOSDevices.indexOf(s.GetRowKey(e.visibleIndex)), 1);
        }
    }
    else {
        var i;
        if (e.isSelected) {
            for (i = 0; i < s.keys.length ; i++) {
                if (selectedPOSDevices.indexOf(s.keys[i]) === -1) {
                    selectedPOSDevices.push(s.keys[i]);
                }
            }
        }
        else {
            for (i = 0; i < s.keys.length; i++) {
                selectedPOSDevices.splice(selectedPOSDevices.indexOf((s.keys[i])), 1);
            }
        }
    }
} 

function OnPOSDeviceGetValues(values) {
    selectedPOSDevices = [];
    for (var i = 0; i < values.length ; i++) {
        selectedPOSDevices.push(values[i]);
    }

    if (selectedPOSDevices.length > 0) {
        ShowToolbar();
    }
    else {
        HideToolbar();
    }

    Component.GetName(gridName).HideLoadingPanel();
    Component.GetName(gridName).HideLoadingDiv();
}

function DialogOkButton_OnClick(s, e) {
    Dialog.Hide();
}

function DialogCancelButton_OnClick(s, e) {
    Dialog.Hide();
}