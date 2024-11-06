var selectedStoresArray = [],
    clearSupplier = false,
    clearCustomer = false,
    prevSupplier = -1,
    prevCustomer = -1,
    temp_pos_lvl;

function NumericTextBoxOnKeyDown(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (!(e.htmlEvent.keyCode >= 48 && e.htmlEvent.keyCode <= 57 && !e.htmlEvent.shiftKey) && //0-9
        !(e.htmlEvent.keyCode >= 96 && e.htmlEvent.keyCode <= 105) && //numpad 0-9
            unicode != 9 && //tab
            unicode != 8 && //backspace
            unicode != 46 &&  //delete
            unicode != 37 &&  //left arrow
            unicode != 39)  //right arrow
        ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
}

function OnValueChangedIsCentralStore(s, e) {
    StoresComboBox.SetEnabled(IsCentralStore.GetValue());
}

function clearCustomerSupplierComboBoxes() {
    CustomerComboBox.PerformCallback();
}

function SelectionChangedUserStores(s, e) {
    if (e.isChangedOnServer) {
        document.getElementById('selectedStores').value = selectedStoresArray;
        return;
    }
    if (e.isSelected) {
        selectedStoresArray.push(grdUserStores.GetRowKey(e.visibleIndex));
    }
    else {
        selectedStoresArray.splice(selectedStoresArray.indexOf(grdUserStores.GetRowKey(e.visibleIndex)), 1);
    }
    document.getElementById('selectedStores').value = selectedStoresArray;
    return;
}

function OnRoleChanged() {
    if (typeof (RoleCbPanel) !== "undefined" && Component.GetName('RoleCbPanel') !== null) {
        RoleCbPanel.PerformCallback();
    }
}

function RoleCbBeginCallback(s, e) {
    if (Component.GetName("Role") !== null) {
        e.customArgs.role = Component.GetName("Role").GetValue();
    }
    
}

function RoleCbEndCallback(){
    var ispos = RoleCbPanel.cp_IsForPOS;
    var pos_username = Component.GetName('POSUserName');
    var pos_pwd = Component.GetName('POSPassword');
    var pos_level = Component.GetName('POSUserLevel');
    if (ispos === true) {
        if (temp_pos_lvl !== null) {
            pos_level.SetValue(temp_pos_lvl);
        }
        CustomerComboBox.SetEnabled(false);
        pos_username.SetEnabled(true);
        pos_pwd.SetEnabled(true);
        pos_level.SetEnabled(true);
        temp_pos_lvl = null;
    }
    else {       
        CustomerComboBox.SetEnabled(true);
        temp_pos_lvl = pos_level.GetValue();
        pos_username.SetEnabled(false);
        addWhiteTextBox(pos_username.name);
        pos_pwd.SetEnabled(false);
        addWhiteTextBox(pos_pwd.name);
        pos_level.SetSelectedIndex(0);
        pos_level.SetEnabled(false);
        addWhiteTextBox(pos_level.name);
    }   
}

function OnInit(s, e) {
    s.SetValue(s.cp_Password);
}

function InitUserStores(s, e, storeOids) {
    if (typeof (storeOids) !== "undefined") {
        selectedStoresArray = [];
        selectedStoresArray = storeOids.split(",");
        grdUserStores.SelectRowsByKey(selectedStoresArray);
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "User/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== "undefined") {
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
