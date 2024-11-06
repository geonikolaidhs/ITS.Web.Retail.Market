var selectedDefaultPhone = null;


function grdAddressBeginCalback(s, e) {

    if (s.IsEditing()) {
        var comboBox;
        comboBox = ASPxClientControl.GetControlCollection().GetByName('AddressTypeCb');
        if (comboBox !== null)
            e.customArgs.AddressTypeID = comboBox.GetValue();

        comboBox = ASPxClientControl.GetControlCollection().GetByName('PostCode');
        if (comboBox !== null)
            e.customArgs.PostCodeID = comboBox.GetValue();

        comboBox = ASPxClientControl.GetControlCollection().GetByName('DefaultPhoneCb');
        if (comboBox !== null)
            e.customArgs.DefaultPhoneID = comboBox.GetValue();
        comboBox = ASPxClientControl.GetControlCollection().GetByName('Central');
        if (comboBox !== null)
            e.customArgs.CentralID = comboBox.GetValue();
    }
}

function grdAddressEndCallback(s, e) {

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

function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#previewImage').attr('src', $('#HOME').val() + 'OwnerApplicationSettings/ShowOwnerImage' + '?time=' + new Date().getTime());
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Company/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {
                
                if (typeof LoadEditPopup !== typeof undefined) {
                    LoadEditPopup.Hide();
                }
                if (typeof LoadAssosiatedCompanyEditPopup !== typeof undefined) {
                    LoadAssosiatedCompanyEditPopup.Hide();
                }
            }
        },
        error: function (data) {
            setJSError(data);
            if (typeof LoadEditPopup !== typeof undefined) {
                LoadEditPopup.Hide();
            }
            if (typeof LoadAssosiatedCompanyEditPopup !== typeof undefined) {
                LoadAssosiatedCompanyEditPopup.Hide();
            }
        }
    });
}