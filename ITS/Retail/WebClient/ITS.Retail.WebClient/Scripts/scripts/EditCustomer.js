var SelectedNodeID = -1;
var selectedDefaultPhone = null;
var selectedDefaultAddress = null;

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Customer/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {

                if (typeof LoadEditPopup !== typeof undefined) {
                    LoadEditPopup.Hide();
                }
                if (typeof LoadAssosiatedCustomerEditPopup !== typeof undefined) {
                    LoadAssosiatedCustomerEditPopup.Hide();
                }
            }
        },
        error: function (data) {
            setJSError(data);
            if (typeof LoadEditPopup !== typeof undefined) {
                LoadEditPopup.Hide();
            }
            if (typeof LoadAssosiatedCustomerEditPopup !== typeof undefined) {
                LoadAssosiatedCustomerEditPopup.Hide();
            }
        }
    });
}

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

function OnSelectedStoreChange(s, e) {
    StorePriceListCbPanel.PerformCallback();
}

function grdCustomerStorePriceListCallback(s, e) {
    e.customArgs.StoreGUID = Store.GetValue();
}

function grdCustomerSupplierStoresCallback(s, e) {
    e.customArgs.SupplierGUID = DefaultSupplier.GetValue();
}

//function UpdateStoreComboBoxBeginCallback(s, e) {
//    //e.customArgs.SupplierID = SupplierKey.GetValue();
//}

//function SupplierComboboxValueChanged(s,e){
//    CustomerUpdateStoreCbPanel.PerformCallback();
//}

function StoreComboBoxValueChanged(s, e) {
    PriceCatalogCbPanel.PerformCallback();
}

function UpdatePriceCatalogComboBoxBeginCallback(s, e) {
    //e.customArgs.SupplierID = SupplierKey.GetValue();
    e.customArgs.StoreID = StoreKey.GetValue();
}

function OnBeginCallbackCustomerAnalyticTreeGrid(s, e) {
    if (s.IsEditing()) {
        e.customArgs.SelectedNodeID = SelectedNodeID;
    }
}

function OnSaveBtnClickCustomerAnalyticTree(s, e) {
    jsonCheckForDuplicateCategory();
}

function jsonCheckForDuplicateCategory() {
    var path = $('#HOME').val() + 'Customer/jsonCheckForDuplicateCategory';

    $.ajax({
        type: 'POST',
        url: path,
        data: { 'SelectedNodeID': SelectedNodeID },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.hasDuplicate) {
                setJSError(cannotInsertSameCategoryMultipleTimes);
            }
            else {
                grdCustomerAnalyticTree.UpdateEdit();
            }
        },
        error: function (data) {
        }
    });
}

function TreeViewGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
    SelectedNodeID = s.GetSelectedNode().name;
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

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function UpdateNewCustomerDetails(s, e) {
    try { onAjaxRequest = true; } catch (error) { console.log(error); }
    var path = $("#HOME").val() + "Customer/UpdateTraderData";
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'sender': s.name, 'Code': Code.GetValue(), 'TaxCode': TaxCode.GetValue() },
        cache: false,
        success: function (result) {
            try {
                onAjaxRequest = false;
                //checkVatBtnClick();

            } catch (error) { console.log(error); }

            if (typeof result.Error === "undefined" && (typeof result.NoDuplicateFound === "undefined" || result.NoDuplicateFound == false)) {
                Component.TraderExists(result);

            }
        }
    });
}

