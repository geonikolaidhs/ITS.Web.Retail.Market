var Address = {
    Delete: function (s,e) {
        var link = path + "Membership/DeleteAddress";
        var result;
        $.ajax({
            type: 'POST',
            url: link,
            async: false,
            cache: false,
            dataType: 'json',
            data: { 'AddressOid': s.name.replace('DeleteAddress_','') },
            success: function (data) {
                AddressesCallbackPanel.PerformCallback();
                Layout.notifyUser();
            },
            error: function (data) {
                //Layout.notifyUser();
            }
        });
    },
    AddAddressOnComplete: function (s, e) {
        AddressesCallbackPanel.PerformCallback();
        Layout.notifyUser();
    },
    UpdateAddressOnComplete: function (s, e) {
        AddressesCallbackPanel.PerformCallback();
        Layout.notifyUser();
    },
}