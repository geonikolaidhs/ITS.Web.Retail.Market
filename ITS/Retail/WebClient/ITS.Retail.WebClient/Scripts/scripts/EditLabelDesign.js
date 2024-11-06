function OnFileUploadComplete(s,e)
{
    if (e.callbackData !== '') {        
        LabelFileName.SetText(e.callbackData);
    }
}

function btnCancelClickV2(s, e) {
    LoadEditPopup.Hide();
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}


var EditLabelDesign = (function () {
    return {
        TestRemotePrinterServerConnection: function () {
            PrinterNickName.PerformCallback({
                "printerServer": RemotePrinterServices.GetValue(),
                "selectedPrinter": PrinterNickName.GetValue()
            });
        },
        PrinterNickNameBeginCallback: function (s, e) {
            TestConnectionButton.SetEnabled(false);

            e.customArgs = {
                "printerServer": RemotePrinterServices.GetValue(),
                "selectedPrinter": PrinterNickName.GetValue()
            };
        },
        PrinterNickNameEndCallback: function () {
            TestConnectionButton.SetEnabled(true);
        }
    }
})();