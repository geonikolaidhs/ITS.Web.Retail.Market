var path = home + "B2C/";
var WishList = {
    WishListItemDelete_Click: function (s, e) {
        var documentDetailOid = s.name.split('_')[1];
        var link = path + "Products/JsonDeleteFromWishList";
        $.ajax({
            type: 'POST',
            url: link,
            async: false,
            cache: false,
            data: { 'DocumentDetailOid': documentDetailOid },
            dataType: 'json',
            success: function (data) {
                if (data.success) {
                    Layout.notifyUser();
                    callbackWishList.PerformCallback();
                }
            },
            error: function (data) {
                Layout.notifyUser();
            }
        });
    },
    WishListDeleteFromWishListAddToCart_Click: function (s, e) {
        var documentDetailOid = s.name.split('_')[1];
        $('#rs-action').val('ADDFROMWISHLIST');
        $('#rs-DocumentDetailOid').val(documentDetailOid);
        $("#shopping-cart-form").submit();
    }

}