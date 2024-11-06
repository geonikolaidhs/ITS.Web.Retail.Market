$(document).ready(function () {


});
function SearchSFA(s, e) {
    grdPOSs.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}


function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
    console.log("initPopup");
}

function IDTextBox_OnKeyPress(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    var actualkey = String.fromCharCode(unicode);
    var valid_chars = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
    if ((actualkey in valid_chars)) {
        e.htmlEvent.returnValue = true;
    }
    else if (unicode != 9 && //tab
        unicode != 8 && //backspace
        unicode != 37 &&  //left arrow
        unicode != 39)  //right arrow) 
    {
        e.htmlEvent.preventDefault();
    }
    else {
        e.htmlEvent.returnValue = true;
    }
}

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "SFA/CancelEdit",
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


function update() {

}

function OnBeginCallback(s, e) {
    console.log("BeginCallback");
    if (!s.IsEditing()) {
        e.customArgs.fid = Component.GetName('fid').GetValue();
        e.customArgs.fname = Component.GetName('fname').GetValue();
    }
}


var SFA = (function () {
    return {
        CreateSFADatabase: function (s, e) {
            if (confirm(processWillTakeAFewMinutesAreYouSure) === true) {
                var path = $('#HOME').val() + 'SFA/CreateSFADatabase';
                $.ajax({
                    type: 'POST',
                    url: path,
                    data: {
                    },
                    cache: false,
                    dataType: 'json',
                    success: function (data) {
                    },
                    error: function (data) {
                        setJSError(anErrorOccured);
                    }
                });
                DialogCallbackPanel.PerformCallback('DATABASE_PROCESS_DIALOG');
            }
        },
        DialogOnShown: function () {
            var interval = setInterval(function () {
                var path = $('#HOME').val() + 'SFA/jsonCheckSFADatabaseRunningStatus';
                $.ajax({
                    type: 'POST',
                    url: path,
                    data: {
                    },
                    cache: false,
                    dataType: 'json',
                    async: false,
                    success: function (data) {
                        if (data.done === true) {
                            btnDialogOK.SetVisible(true);
                            lblProgress.SetText(successMessage);
                            $('#processingImage').hide();
                            clearInterval(interval);
                        }
                    },
                    error: function (data) {
                    }
                });
            }, 3000);
        }
    }
})();


function DialogOkButton_OnClick(s, e) {
    Dialog.Hide();
}

function DialogCancelButton_OnClick(s, e) {
    Dialog.Hide();
}
