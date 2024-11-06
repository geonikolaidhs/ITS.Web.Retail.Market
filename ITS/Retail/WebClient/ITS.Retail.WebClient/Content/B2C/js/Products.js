var img_path = home + 'Content/B2C/img/icons';
var Products = {
    //index: 0,
    //topReturned:10,    
    Init: function () {
        $('.products .clearfix .right_side span.details').hover(function () {
            $(this).parent().parent().find('.info').show();
        });

        $('.products .clearfix .item').mouseleave(function () {
            $(this).parent().parent().find('.info').hide();
        });

        $('.products .clearfix .item .botton_side .color').click(function () {
        });


        var stop = false;

        if ($('.products .clearfix').length) {
            $(window).scrollAppend({
                url: path + 'Products/ItemsContent',
                params: {},
                appendTo: ".products .clearfix",
                footerClass: "#footer",
                contentClass: "#wrap",
                moreClass: "scroll_append_more", //todo create a class for "show more" button
                pagesToPause: 20,
                loadingImage: img_path + "/loader.gif", //tod add proper image
                disableCache: true,
                callback:
                    function () {
                        $('.products .clearfix .right_side span.details').hover(function () {
                            $(this).parent().parent().find('.info').show();
                        });

                        $('.products .clearfix .item').mouseleave(function () {
                            $(this).parent().parent().find('.info').hide();
                        });

                    }
            });
        }

    },
    ProductListing: function () {
        if ($("#products-list").length != 0) {

            $('#products-list').jplist({
                itemsBox: '.list'
                , itemPath: '.list-item'
                , panelPath: '.jplist-panel'
                , storage: 'cookies' //'', 'cookies', 'localstorage'			
                , storageName: 'jplist'
                , cookiesExpiration: 525600 //cookies expiration in minutes (-1 = cookies expire when browser is closed)                           
                , effect: 'fade'
                , duration: 500	//animation duration
                , fps: 24 //frames per second value
                , redrawCallback: function (collection, $dataview, statuses) {
                    Layout.RedrawCategoryMenu();
                    Layout.RedrawShoppingCartMenu();
                    //this code occurs on every jplist action
                    if ($(".popup").length != 0) {
                        $('.popup').magnificPopup({
                            type: 'inline',
                            fixedContentPos: true,
                            midClick: true // Allow opening popup on middle mouse click. Always set it to true if you don't provide alternative source in href.
                        });
                    }
                }
            });
        }

        animateScrollTop(0, 1000);

    },


    ItemSpinEdit_OnKeyPress: function (s, e) {
        if (s.uniqueID.indexOf('_Quantity2') != -1 || e.htmlEvent.keyCode == 13) { // enter or add button

            var priceCatalogDetailOid = s.name.split('_')[0];
            var qty;
            if (s.uniqueID.indexOf('_Quantity2') != -1) {
                qty = $('#' + priceCatalogDetailOid + '_Quantity').find('input').val();
            } else {
                qty = s.GetNumber();
            }
            $('#rs-action').val('ADD');
            $('#rs-PriceCatalogDetailOid').val(priceCatalogDetailOid);
            $('#rs-Qty').val(qty);
            $("#shopping-cart-form").submit();
            ASPxClientControl.GetControlCollection().GetByName(String(priceCatalogDetailOid + '_Quantity')).SetValue(1);            
        }
       
    },
    ItemSpinEditShoppingCart_OnKeyPress: function (s, e) {

        if ((s.uniqueID.indexOf('_Quantity_Cart2') != -1) || s.uniqueID.indexOf('_Quantity_Cart_List2') != -1 || (typeof (e.htmlEvent) != typeof (undefined) && e.htmlEvent.keyCode == 13)) { // enter or add button

            var documentDetailOid = s.name.split('_')[0];
            var qty;
            if (s.uniqueID.indexOf('_Quantity_Cart2') != -1) {
                qty = $('#' + documentDetailOid + '_Quantity_Cart').find('input').val();
            }
            else if (s.uniqueID.indexOf('_Quantity_Cart_List2') != -1) {
                qty = $('#' + documentDetailOid + '_Quantity_Cart_List').find('input').val();
            }
            else {
                qty = s.GetNumber();
            }
            $('#rs-action').val('UPDATE');
            $('#rs-DocumentDetailOid').val(documentDetailOid);
            $('#rs-Qty').val(qty);
            $("#shopping-cart-form").submit();

        }
    },
    CartItemDelete_Click: function (s, e) {

        var documentDetailOid = s.name.split('_')[1];
        $('#rs-action').val('DELETE');
        $('#rs-DocumentDetailOid').val(documentDetailOid);
        $("#shopping-cart-form").submit();       
    },
    WishListButtonPressed: function (s, e) {        
        var priceCatalogDetailOid = s.name.split('_')[0];

        var link = path + "Products/JsonAddToWishList";
        $.ajax({
            type: 'POST',
            url: link,
            async: false,
            cache: false,
            data: { 'PriceCatalogDetailOid': priceCatalogDetailOid },
            dataType: 'json',
            success: function (data) {
                if (data.success) {
                    $("#" + s.name).addClass('wishlist-red-button');
                    s.SetEnabled(false);
                    s.GetMainElement().title = GetMessage("addedToWishList");

                }
                Layout.notifyUser();
            },
            error: function (data) {
                Layout.notifyUser();
            }
        });
    },
    SwitchToPage: function(page)
    {
        $('#Page').val(page);
        $('#pagination-form').submit();
    },
};


