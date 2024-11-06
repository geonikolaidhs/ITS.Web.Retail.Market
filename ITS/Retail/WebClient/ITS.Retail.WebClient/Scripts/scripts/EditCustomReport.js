function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "CustomReport/CancelEdit",
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

function btnDeleteReportFileClick(s, e) {
    var path = $('#HOME').val() + 'CustomReport/jsonDeleteReportFile';
	$.ajax({
		type: 'POST',
		url: path,
		cache: false,
		dataType: 'json',
		async: false,
		success: function (data) {
		    FileName.SetText("");
		    ReportType.SetText("");
		    ObjectType.SetText("");
		},
		error: function (data) {
		}
	});
}

function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        var fileName = e.callbackData.split('|')[0];
        var reportType = e.callbackData.split('|')[1];
        var objectType = e.callbackData.split('|')[2];
        FileName.SetText(fileName);
        ReportType.SetText(reportType);
        ObjectType.SetText(objectType);
	}
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}