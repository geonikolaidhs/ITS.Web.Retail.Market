var GenericScanner = new ActiveXObject("SymbolBrowser.Generic");
var enableScannerTime = 3000;

function doSoftScan() {
    GenericScanner.InvokeMetaFunction('scanner', 'start');
}

function doScan(data) {
    $.timer(enableScannerTime, doSoftScan);
    search(data);
}

$(document).ready(function () {
    GenericScanner.InvokeMetaFunction('scanner', 'AIM_TYPE_CONTINUOUS_TRIGGER');
    GenericScanner.InvokeMetaFunction('scanner', 'enabled');
    GenericScanner.InvokeMetaFunction('scannernavigate', "javascript:doScan('%s');");
    doSoftScan();
});