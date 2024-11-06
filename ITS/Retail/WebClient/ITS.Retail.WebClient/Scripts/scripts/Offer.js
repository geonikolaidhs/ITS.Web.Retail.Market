function SearchOffer(s, e) {
    selectedItemsArray = [];
    grdOffers.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function OnBeginCallback(s, e) {
    CustomizationWindow();
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        //clear selected items
        selectedItemsArray = [];
    }
    if (!s.IsEditing()) {
        e.customArgs.fcode = Component.GetName('fcode').GetValue();
        e.customArgs.fdescription = Component.GetName('fdescription').GetValue();
        e.customArgs.fpriceCatalog = Component.GetName('fpriceCatalog') === null ? null : Component.GetName('fpriceCatalog').GetValue();
        e.customArgs.fstartDate = Component.GetName('fstartDate').GetText();
        e.customArgs.fendDate = Component.GetName('fendDate').GetText();
    }
}

function ExportSelectedOffers() {
    window.open($('#HOME').val() + 'Reports/OffersReport');

}

function OnBeginCallbackOfferDetailGrid(s, e) {
    if (s.IsEditing()) {
        var itemComboBox = Component.GetName('ItemsComboBox');
        e.customArgs.ItemComboBoxID = itemComboBox.GetValue();
    }
}

function btnCancelClick(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Offer/CancelEdit",
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

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

