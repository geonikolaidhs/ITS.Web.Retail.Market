var selectedDefaultPhone = null;
var selectedDefaultAddress = null;

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Supplier/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {
                if (typeof LoadEditPopup !== typeof undefined) {
                    LoadEditPopup.Hide();
                }
                if (typeof LoadAssosiatedSupplierEditPopup !== typeof undefined) {
                    LoadAssosiatedSupplierEditPopup.Hide();
                }
            }
        },
        error: function (data) {
            setJSError(data);
            if (typeof LoadEditPopup !== typeof undefined) {
                LoadEditPopup.Hide();
            }
            if (typeof LoadAssosiatedSupplierEditPopup !== typeof undefined) {
                LoadAssosiatedSupplierEditPopup.Hide();
            }

        }
    });
}

//function grdSupplierAddressCallback(s, e) {
//    selectedDefaultAddress = DefaultAddress.GetValue();
//}

//function grdSupplierAddressEndCallback(s,e){
//    DefaultAddress.SetValue(selectedDefaultAddress);
//}

function grdAddressBeginCalback(s, e) {
    jQuery(window).unbind("beforeunload");
    jQuery(window).unbind("unload");

    if (s.IsEditing()) {
        var comboBox = Component.GetName('AddressType');
        if (comboBox !== null)
            e.customArgs.AddressTypeID = comboBox.GetValue();

        comboBox = Component.GetName('PostCode');
        if (comboBox !== null)
            e.customArgs.PostCodeID = comboBox.GetValue();

        comboBox = Component.GetName('DefaultPhoneCb');
        if (comboBox !== null)
            e.customArgs.DefaultPhoneID = comboBox.GetValue();
        comboBox = Component.GetName('Central');
        if (comboBox !== null)
            e.customArgs.CentralID = comboBox.GetValue();
    }
}

function grdAddressEndCallback(s, e) {
    bindEvents();
    if (typeof (SupplierCbPanel) !== "undefined" && SupplierCbPanel.GetMainElement() !== null) {
        SupplierCbPanel.PerformCallback();
    }
    phoneIDCallback = s.GetRowKey(s.GetFocusedRowIndex());
    if (typeof (AddressCbPanel) !== "undefined" && AddressCbPanel.GetMainElement() !== null) {
        AddressCbPanel.PerformCallback();
    }
    if (typeof (IsStore) !== "undefined" && IsStore.GetMainElement() !== null) {
        StoreCheckChange(null, null);
    }
}

function grdAddressPhoneCallback(s, e) {
    e.customArgs.current_address = document.getElementById("AddressID").value;
}

function phoneGridOnBeginCallback(s, e) {

    if (e.command == "ADDNEWROW") {
        $("#PhoneGrid .button_container").hide();
    } else {
        $("#PhoneGrid .button_container").show();
    }
}

function phoneGridOnEndCallback(s, e) {
    bindEvents();
    if (typeof (AddressCbPanel) !== "undefined" && AddressCbPanel.GetMainElement() !== null) {
        AddressCbPanel.PerformCallback();
    }
}


function AddressCbPanelBegin(s, e) {
    e.customArgs.current_address = document.getElementById("AddressID").value;
    selectedDefaultPhone = window.DefaultPhoneCb.GetValue();
}

function AddressCbPanelEnd(s, e) {
    window.DefaultPhoneCb.SetValue(selectedDefaultPhone);
}


//function StoreBeginCallback(s, e) {
//    if (s.IsEditing()) {
//        var comboBox = Component.GetName('DefaultPhoneCb');
//        if (comboBox !== null)
//            e.customArgs.DefaultPhoneID = comboBox.GetValue();
//    }
//}

//function StoreEndCallback(s, e) {
//    bindEvents();
//    if (typeof (SupplierCbPanel) !== "undefined" && SupplierCbPanel.GetMainElement() !== null) {
//        SupplierCbPanel.PerformCallback();
//    }
//    phoneIDCallback = s.GetRowKey(s.GetFocusedRowIndex());
//    if (typeof (AddressCbPanel) !== "undefined" && AddressCbPanel.GetMainElement() !== null) {
//        AddressCbPanel.PerformCallback();
//    }

//    if (typeof (IsDefault) !== "undefined" && IsDefault.GetMainElement() !== null && IsDefault.GetValue() === null) {
//        IsDefault.SetValue(false);
//    }
//}

function CreateStore(supoid, addroid) {
    var path = $("#HOME").val() + "Address/CreateStore";
    $.ajax({
        type: 'POST',
        url: path,
        data: 
        { 
            'SupplierID': supoid,
            'AddressID': addroid
        },
        cache: false,
        success: StoreSuccess,
        fail: StoreFail
    });
}

function StoreFail() {
    setJSError(anErrorOccured);
}

function StoreSuccess(data) {
    setJSNotification(successMessage);
    var addressGrid = Component.GetName('grdAddressEdit' + data.traderoid);
    var storeGrid = Component.GetName('grdStoreEdit' + data.supplieroid);
    addressGrid.PerformCallback();
    storeGrid.PerformCallback('NEWSTOREFROMADDRESS');
}


function StoreCheckChange(s, e) {

    StoreCode.SetEnabled(IsStore.GetValue());
    StoreName.SetEnabled(IsStore.GetValue());
    IsCentralStore.SetEnabled(IsStore.GetValue());
    Central.SetEnabled(IsStore.GetValue());
}

function StoreCentralChange(s, e) {
    if ($('#centralStoreBlock').val() == "True" && IsCentralStore.GetValue() === false) {
        setJSError(centralStoreProtection);
        IsCentralStore.SetValue(true);
    }
    Central.SetEnabled(!IsCentralStore.GetValue());
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function btnSaveOnClick(s, e) {

    var taxoffice = Component.GetName('TaxOfficeLookUpOid');
    if (taxoffice !== null) {
        if (taxoffice.GetValue() === null) {
            if (window.confirm(taxofficeIsNotSelected)) {
                Component.ValidateAndSubmitForm(s, e);
            }
        }
        else {
            Component.ValidateAndSubmitForm(s, e);
        }
    }

}

function UpdateNewSupplierDetails(s, e) {
    var path = $("#HOME").val() + "Supplier/UpdateTraderData";
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'sender': s.name, 'Code': Code.GetValue(), 'TaxCode': TaxCode.GetValue() },
        cache: false,
        success: function (result) {
            if (typeof result.Error === "undefined" && ( typeof result.NoDuplicateFound === "undefined" ||  result.NoDuplicateFound == false) ) {
                Component.TraderExists(result);
            }
        }
    });
}

//function TraderExists(data) {
//    if (data === false) {
//        return;
//    }
//    var res = confirm(data.confirm_message);
//    if (!res) {
//        if (data.triggered_by == "TaxCode") {
//            TaxCode.SetValue("");
//        }
//        if (data.triggered_by == "Code") {
//            Code.SetValue("");
//        }
//        return;
//    }

//    selectedItemsArray = [];
//    //selectedItemsArray.push(data.supplier_id);
//    Component.EmptyCallbackPanels();
//    PopupEditCallbackPanel.PerformCallback();
//}


