$(document).ready(function () {
    if ($(".POS_EditView").length) {
        $(".wrapper .POS.EditView").css("min-width", "1075px");
    }
});

function btnCancelClickV2(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "POS/CancelEdit",
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

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

var ITSPOS = (function () {
    ManageReportComponent = function (component) {
        Component.GetName(component).SetVisible(!Component.GetName(component).GetVisible());
        if (Component.GetName(component).GetVisible() == false) {
            Component.GetName(component).SetValue(null);
        }
    };
    return {
        PrinterTypeSelectionChanged: function (s, e) {
            ManageReportComponent("ThermalReport");
            ManageReportComponent("WindowsReport");
        }
    };
})();