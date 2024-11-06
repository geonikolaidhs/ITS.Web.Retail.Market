function SearchLeaflet(s, e) {
    selectedItemsArray = [];
    grdLeaflets.PerformCallback("SEARCH");
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
        //e.customArgs.fpriceCatalog = Component.GetName('fpriceCatalog') === null ? null : Component.GetName('fpriceCatalog').GetValue();
        e.customArgs.fstartDate = Component.GetName('fstartDate').GetText();
        e.customArgs.fendDate = Component.GetName('fendDate').GetText();
    }
}

function ExportSelectedLeaflets() {
    window.open($('#HOME').val() + 'Reports/LeafletsReport');

}

function OnBeginCallbackLeafletDetailGrid(s, e) {
    if (s.IsEditing()) {
        var barcodesComboBoxPartial = Component.GetName('BarcodesComboBox');
        e.customArgs.barcodesComboBoxPartialID = barcodesComboBoxPartial.GetValue();
    }
}

function OnBeginCallbackLeafletStoreGrid(s, e) {
    if (s.IsEditing()) {
        var storeComboBox = Component.GetName('StoresComboBox_LeafletStore');
        e.customArgs.StoreID = storeComboBox.GetValue();

    }
}

function btnCancelClick(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "Leaflet/CancelEdit",
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

function CheckForExistingLeafletStore() {
    var path = $("#HOME").val() + "Leaflet/jsonCheckForExistingLeafletStore";
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'StoreID': StoresComboBox_LeafletStore.GetValue()
        },
        cache: false,
        success: function (data) {
            if (data.allow === true) {
                grdLeafletStore.UpdateEdit();
            }
            else {
                setJSError(cannotHaveMultipleStorage);
            }
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}


function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#imgLeafletImage').attr('src', $('#HOME').val() + 'Leaflet/ShowImage' + '?time=' + new Date().getTime());
        $('#previewImage').attr('src', $('#HOME').val() + 'Leaflet/ShowImage' + '?time=' + new Date().getTime());
    }
}

function btnDeleteLeafletImageClick(s, e) {
    var path = $('#HOME').val() + 'Leaflet/jsonDeleteLeafletImage';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            $('#imgLeafletImage').attr('src', $('#HOME').val() + 'Leaflet/ShowImage' + '?time=' + new Date().getTime());
            $('#previewImage').attr('src', $('#HOME').val() + 'Leaflet/ShowImage' + '?time=' + new Date().getTime());
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}