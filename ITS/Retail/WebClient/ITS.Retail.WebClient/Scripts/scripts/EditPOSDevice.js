
function ChangedConnectionTypeComboBox() {
    onValueChangedConnectionTypeComboBox(ConnectionType);
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "POSDevice/CancelEdit",
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

function SetTabVisibility(tabName, visibility) {

    pcConnectionSettings.GetTabByName(tabName).SetVisible(visibility);

}

function onValueChangedConnectionTypeComboBox(s, e) {
    
    var isCashRegister = $("#IsCashRegister").text();
    if (isCashRegister == "")
    {
        isCashRegister = "0";
    }
    if (s.GetValue() === null || s.GetValue() == "NONE" || s.GetValue() == "EMULATED")
    {
        SetTabVisibility("COMSettingsTab",false);
        SetTabVisibility("LPTSettingsTab",false);
        SetTabVisibility("OPOSSettingsTab",false);
        SetTabVisibility("ETHERNETSettingsTab",false);
        SetTabVisibility("INDIRECTSettingsTab", false);
        SetTabVisibility("CashierRegisterSettingsTab", false);
        SetTabVisibility("CashierVatFactorTabSettings", false);
        SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
	}
    else if (s.GetValue() == "COM") {

        SetTabVisibility("COMSettingsTab", true);
        SetTabVisibility("LPTSettingsTab", false);
        SetTabVisibility("OPOSSettingsTab", false);
        SetTabVisibility("ETHERNETSettingsTab", false);
        SetTabVisibility("INDIRECTSettingsTab", false);
        if(isCashRegister=="1")
        {
            SetTabVisibility("CashierRegisterSettingsTab", true);
            SetTabVisibility("CashierVatFactorTabSettings", true);
            SetTabVisibility("CashierRegisterDocumentSettingsTab", true);
        }
        else
        {
            SetTabVisibility("CashierRegisterSettingsTab", false);
            SetTabVisibility("CashierVatFactorTabSettings", false);
            SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
        }
	}
    else if (s.GetValue() == "LPT") {

        SetTabVisibility("LPTSettingsTab", true);
        SetTabVisibility("COMSettingsTab", false);
        SetTabVisibility("OPOSSettingsTab", false);
        SetTabVisibility("ETHERNETSettingsTab", false);
        SetTabVisibility("INDIRECTSettingsTab", false);
        SetTabVisibility("CashierRegisterSettingsTab", false);
        SetTabVisibility("CashierVatFactorTabSettings", false);
        SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
	}
    else if (s.GetValue() == "ETHERNET") {

        SetTabVisibility("ETHERNETSettingsTab", true);
        SetTabVisibility("LPTSettingsTab", false);
        SetTabVisibility("COMSettingsTab", false);
        SetTabVisibility("OPOSSettingsTab", false);
        SetTabVisibility("INDIRECTSettingsTab", false);
        if (isCashRegister == "1") {
            SetTabVisibility("CashierRegisterSettingsTab", true);
            SetTabVisibility("CashierVatFactorTabSettings", true);
            SetTabVisibility("CashierRegisterDocumentSettingsTab", true);
        }
        else {
            SetTabVisibility("CashierRegisterSettingsTab", false);
            SetTabVisibility("CashierVatFactorTabSettings", false);
            SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
        }
	}
    else if (s.GetValue() == "OPOS") {

        SetTabVisibility("OPOSSettingsTab", true);
        SetTabVisibility("ETHERNETSettingsTab", false);
        SetTabVisibility("LPTSettingsTab", false);
        SetTabVisibility("COMSettingsTab", false);
        SetTabVisibility("INDIRECTSettingsTab", false);
        SetTabVisibility("CashierRegisterSettingsTab", false);
        SetTabVisibility("CashierVatFactorTabSettings", false);
        SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
		if (DeviceType.GetValue() == "Printer") {
			SetPrinterSettingsVisible(true);
		}
		else {
			SetPrinterSettingsVisible(false);
		}
	}
    else if (s.GetValue() == "INDIRECT") {

        SetTabVisibility("INDIRECTSettingsTab", true);
        SetTabVisibility("OPOSSettingsTab", false);
        SetTabVisibility("ETHERNETSettingsTab", false);
        SetTabVisibility("LPTSettingsTab", false);
        SetTabVisibility("COMSettingsTab", false);
        SetTabVisibility("CashierRegisterSettingsTab", false);
        SetTabVisibility("CashierVatFactorTabSettings", false);
        SetTabVisibility("CashierRegisterDocumentSettingsTab", false);
	}
}

function onValueChangedDeviceTypeComboBox(s, e) {
    if (DeviceType.GetValue() == "Printer" && ConnectionType.GetValue() == "OPOS") 
    {
        SetPrinterSettingsVisible(true);
        SetScaleSettingsVisible(false);
	}
    else if (DeviceType.GetValue() == "Scale" && ConnectionType.GetValue() == "COM")
	{
        SetScaleSettingsVisible(true);
        SetPrinterSettingsVisible(false);
	}
    else 
    {
        SetPrinterSettingsVisible(false);
        SetScaleSettingsVisible(false);
	}

	SetTabVisibility("COMSettingsTab", false);
	SetTabVisibility("LPTSettingsTab", false);
	SetTabVisibility("OPOSSettingsTab", false);
	SetTabVisibility("ETHERNETSettingsTab", false);
	SetTabVisibility("INDIRECTSettingsTab", false);

	var selectedDeviceType = s.GetValue();
	if (selectedDeviceType !== null && typeof selectedDeviceType !== "undefined") {
	    DeviceSpecificTypeCBP.PerformCallback(selectedDeviceType);
	    ConnectionTypeCBP.PerformCallback(selectedDeviceType);
	}
}

function SetPrinterSettingsVisible(visible) {
	if (visible === true) {
		$('.printer_settings').show();
	}
	else {
		$('.printer_settings').hide();
	}
}


function SetScaleSettingsVisible(visible) {
    if (visible === true) {
        $('.scale_settings').show();
    }
    else {
        $('.scale_settings').hide();
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}