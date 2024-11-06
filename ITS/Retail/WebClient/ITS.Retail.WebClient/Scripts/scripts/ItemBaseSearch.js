var ItemBaseID = -1,
    ItemBaseName;

function OnKeyPressBarcodeSearchBox(s, e)
{
    if (e.htmlEvent.keyCode === 13) {
        var path = $('#HOME').val() + 'Item/jsonCheckForExistingItemBase';

        $.ajax({
            type: 'POST',
            url: path,
            cache: false,
            dataType: 'json',
            data: { 'barcodeCode': BarcodeSearchBox.GetText() },
            async: false,
            success: function (data) {
                if (data.ItemBaseID != '-1') {
                    ItemBaseID = data.ItemBaseID;
                    BarcodesListBox.PerformCallback();
                    ItemBaseName = data.ItemBaseName;
                    ItemName.SetText(ItemBaseName);
                    NextButton.SetEnabled(true);
                }
                else {
                    ItemBaseID = null;
                    BarcodesListBox.PerformCallback();
                    ItemName.SetText(itemNotFound);
                    //updateNotifications();
                    NextButton.SetEnabled(false);
                }
            },
            error: function (data) {
            }
        });
    }
}

function OnBeginCallbackBarcodesListBox(s, e) {
    e.customArgs.ItemBaseID = ItemBaseID;

}

function OnClickNewItemButton(s, e) {
        var st = window.location.toString();

        st = st.replace("ItemBaseSearch", "NewItemFromNewItemBase");
        var frm = document.getElementById('ItemBaseSearchForm');

        frm.action = st;
        frm.submit();
}

function OnClickNewItemFromExistingBaseItem(s, e) {
    var st = window.location.toString();

    st = st.replace("ItemBaseSearch", "NewItemFromExistingItemBase?ItemBaseID=" + ItemBaseID);
    var frm = document.getElementById('ItemBaseSearchForm');

    frm.action = st;
    frm.submit();

}