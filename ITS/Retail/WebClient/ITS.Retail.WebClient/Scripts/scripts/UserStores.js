function jsonSelectStoreForOrder(s, e) {
    var path = $('#HOME').val() + 'Document/jsonSelectStoreForOrder';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'selected_store': Component.GetName('order_from_store') === null ? null : (Component.GetName('order_from_store').GetValue() === null ?
                null : Component.GetName('order_from_store').GetValue().toString())
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.url !== null && typeof data.url !== "undefined" && data.url !== "") {
                window.location = $('#HOME').val() + data.url;
            }
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}
