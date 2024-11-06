    var GenericScanner = new ActiveXObject("SymbolBrowser.Generic");
    var enableScannerTime = 3000;



function search(searchCode) {
    var home = document.getElementById('HOME').value;
    var path = home + 'PriceChecker/CheckPrice?searchcode=' + searchCode;
    window.location.href = path;
}

function checkPrice(e) {
    var unicode = e.keyCode;
    var searchCode = document.getElementById('searchcode').value.trim();
    if ( unicode == 13 && searchCode != '' )
    {
        search(searchCode);
    }
}


function doSoftScan() {
    GenericScanner.InvokeMetaFunction('scanner', 'start');
}

function priceCheckerOnLoad() {
    document.getElementById('searchcode').focus();

    GenericScanner.InvokeMetaFunction('scanner', 'AIM_TYPE_CONTINUOUS_TRIGGER');
    GenericScanner.InvokeMetaFunction('scanner', 'enabled');
    GenericScanner.InvokeMetaFunction('scannernavigate', "javascript:doScan('%s');");
    doSoftScan();
}

function setTimeInterval() {
    setInterval(redirectToSearchPage, 5000);
}

function doScan(data) {
    setInterval(doSoftScan, enableScannerTime);
    search(data);
}

function redirectToSearchPage() {
    var home = document.getElementById('HOME').value;
    var path = home + 'PriceChecker/Index';
    window.location.href = path;
}
