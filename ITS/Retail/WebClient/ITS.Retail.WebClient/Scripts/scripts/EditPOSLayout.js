function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "POSLayout/CancelEdit",
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
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

function btnDeleteMainLayoutFileClick(s, e) {
    var path = $('#HOME').val() + 'POSLayout/jsonDeleteMainLayoutFile';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            $('#previewMainImage').attr('src', $('#HOME').val() + 'POSLayout/ShowImage' + '?type=0&time=' + new Date().getTime());
        },
        error: function (data) {
        }
    });
}

function OnMainLayoutFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#previewMainImage').attr('src', $('#HOME').val() + 'POSLayout/ShowImage' + '?type=0&time=' + new Date().getTime());
    }
}

function btnDeleteSecondaryLayoutFileClick(s, e) {
    var path = $('#HOME').val() + 'POSLayout/jsonDeleteSecondaryLayoutFile';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            $('#previewSecondaryImage').attr('src', $('#HOME').val() + 'POSLayout/ShowImage' + '?type=1&time=' + new Date().getTime());
        },
        error: function (data) {
        }
    });
}

function OnSecondaryLayoutFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#previewSecondaryImage').attr('src', $('#HOME').val() + 'POSLayout/ShowImage' + '?type=1&time=' + new Date().getTime());
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}