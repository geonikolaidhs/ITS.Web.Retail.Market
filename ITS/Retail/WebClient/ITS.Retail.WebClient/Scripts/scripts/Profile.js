var selectedDefaultPhone = null;


function grdAddressBeginCalback(s, e) {

    if (s.IsEditing()) {

        var comboBox = Component.GetName('AddressTypeCb');
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

    if (typeof (CustomerCbPanel) !== "undefined" && CustomerCbPanel.GetMainElement() !== null) {
        CustomerCbPanel.PerformCallback();
    }
    phoneIDCallback = s.GetRowKey(s.GetFocusedRowIndex());
    if (typeof (AddressCbPanel) !== "undefined" && AddressCbPanel.GetMainElement() !== null) {
        AddressCbPanel.PerformCallback();
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

function StoreCheckChange(s, e) {
    StoreCode.SetEnabled(IsStore.GetValue());
    StoreName.SetEnabled(IsStore.GetValue());
    IsCentralStore.SetEnabled(IsStore.GetValue());
    Central.SetEnabled(IsStore.GetValue());

}

function StoreCentralChange(s, e) {
    Central.SetEnabled(!IsCentralStore.GetValue());
}