function UpdateNewSupplierDetails(e) {
    //var path = $("#HOME").val() + "Supplier/UpdateTraderData";
    //$.ajax({
    //    type: 'POST',
    //    url: path,
    //    data: { 'TaxCode': TaxCode.GetValue() },
    //    cache: false,
    //    success: function (result) {
    //        if (typeof result.Error == typeof undefined) {
    //            TraderExists(result);
    //        }
    //    }
    //});
}

function TraderExists(data) {
    if (data === false)
        return;
    var res = confirm(data.confirm_message);
    if (!res) {
        TaxCode.SetValue("");
        return;
    }

    var trd = document.getElementById('TraderID');
    if (trd !== null) {
        trd.value = data.TraderID;
    }

    //Component.EmptyCallbackPanels();
    //PopupAssosiatedEditCallbackPanel.PerformCallback({ TraderID: trd.value });
}
