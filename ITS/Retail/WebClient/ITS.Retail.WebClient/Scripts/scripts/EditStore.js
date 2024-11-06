function StoreCentralChange(s, e) {
    if ($('#centralStoreBlock').val() == "True" && IsCentralStore.GetValue() === false) {
        setJSError(centralStoreProtection);
        IsCentralStore.SetValue(true);
    }
    Central.SetEnabled(!IsCentralStore.GetValue());
}

function AddressCbPanelBegin(s, e) {
    e.customArgs.current_address = document.getElementById("AddressID").value;
    selectedDefaultPhone = window.DefaultPhoneCb.GetValue();
}

function AddressCbPanelEnd(s, e) {
    window.DefaultPhoneCb.SetValue(selectedDefaultPhone);
}

function PriceCatalogPolicyEndCallback(s, e) {
    PriceCatalogPolicyCbPanel.PerformCallback();
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Store/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {
                LoadEditPopup.Hide();
            }
            //cancelSupplier();

        },
        error: function (data) {
            setJSError(data);
        }
    });
}

function btnDeleteItemImageClick(s, e) {
    var path = $('#HOME').val() + 'Store/jsonDeleteOwnerImage';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            //$('#previewImage').attr('src', $('#HOME').val() + 'Store/ShowImageId' + '?time=' + new Date().getTime());
            document.getElementById("previewImage").style.display = "none";
        },
        error: function (data) {
        }
    });
}


function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        document.getElementById("previewImage").style.display = "table";
        $('#previewImage').attr('src', $('#HOME').val() + 'Store/ShowImageId' + '?time=' + new Date().getTime());
    }
}