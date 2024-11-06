var PrintServiceSettings = (function () {
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