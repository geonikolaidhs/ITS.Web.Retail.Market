

function FormMessageDetailOnBeginCallback(sender, e) {
	if (sender.IsEditing()) {
	    var text = Description.GetValue();
		text = $('<div/>').text(text).html();
		Description.SetValue(text);
	}
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "FormMessage/CancelEdit",
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


function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}
