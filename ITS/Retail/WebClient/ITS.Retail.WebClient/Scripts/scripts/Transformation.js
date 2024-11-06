

function OnSelectionChangedTransformationDetailsGrid(s, e) {
    if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
    }
    else {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
    }
    if (!e.isSelected && e.visibleIndex === "-1") {

    }
    if (!e.isAllRecordsOnPage && !e.isChangedOnServer) {
        if (e.isSelected) {
            grdTransformationDetailGrid.cp_selectedTransformationItemsArray.push(s.GetRowKey(e.visibleIndex));
        }
        else {
            grdTransformationDetailGrid.cp_selectedTransformationItemsArray.splice(grdTransformationDetailGrid.cp_selectedTransformationItemsArray.indexOf(s.GetRowKey(e.visibleIndex)), 1);
        }
    }
    else {
        var i;
        if (e.isSelected || e.isChangedOnServer) {
            for (i = 0; i < s.keys.length ; i++) {
                if (grdTransformationDetailGrid.cp_selectedTransformationItemsArray.indexOf(s.keys[i]) === -1) {
                    grdTransformationDetailGrid.cp_selectedTransformationItemsArray.push(s.keys[i]);
                }
            }
        }
        else {
            for (i = 0; i < s.keys.length; i++) {
                grdTransformationDetailGrid.cp_selectedTransformationItemsArray.splice(grdTransformationDetailGrid.cp_selectedTransformationItemsArray.indexOf((s.keys[i])), 1);
            }
        }
    }        

    if (e.visibleIndex >= 0) {
        UpdateTransformationSelectedDocumentDetails(e.isSelected,s.GetRowKey(e.visibleIndex));
    }
    
}



function UpdateTransformationSelectedDocumentDetails(isSelected,key) {
    var path = $('#HOME').val() + 'Document/jsonUpdateTransformationSelectedDocumentDetails';
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: {
            'isSelected': isSelected,
            'key' : key
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            grdTransformationDetailGrid.PerformCallback();
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

