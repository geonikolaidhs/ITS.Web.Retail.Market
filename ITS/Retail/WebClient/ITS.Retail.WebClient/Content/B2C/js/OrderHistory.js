

var OrderHistory = {
    OnGridFocusedRowChanged: function () {
        OrderDetailsCallbackPanel.PerformCallback();
    },
    //OnGetRowValues: function (values) {
    //    console.log(values[1]);


    //    //$.ajax(
    //    //{
    //    //    type: 'POST',
    //    //    url: path + "Base/jsonGetItemCategories",
    //    //    async: false,
    //    //    data: { CategoryOid: itemHref.replace('#', '') },
    //    //    cache: false,
    //    //    dataType: 'json',
    //    //    success: function (data) {
    //    //        $('#menu').multilevelpushmenu('additems', JSON.parse(data.result).items, $(element).find('div').first(), 0);
    //    //    }
    //    //});
    //},
    OrderHistoryEndCallBack: function () {
        setTimeout(function () {
            
            Layout.RedrawCategoryMenu();
            Layout.RedrawShoppingCartMenu();
        }, 500);

    },
    OrderHistoryBeginCallBack: function (s,e) {
        e.customArgs['documentGuid'] = grdB2CDocument.GetRowKey(grdB2CDocument.GetFocusedRowIndex());

    }


}