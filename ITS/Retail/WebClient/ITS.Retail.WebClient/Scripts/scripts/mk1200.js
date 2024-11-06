
var myTimer = {};
var holdScannedCode;
var goBackTimerValue = 20000; // 20000000000000000; //20000;

function search(scannedCode) {
    stopTimer();
    holdScannedCode = scannedCode;
    var options = {
        type: "POST",
        url: "EnterCode.aspx/ScannedHTMLResult",
        contentType: "application/json; charset=utf-8",
        data: '{ "scannedCode" : "' + scannedCode + '" }',
        dataType: "json",
        success: function (msg) {
            $("#contentDiv").html(msg.toString());
            $("#contentDiv").show();
            $("#startingDiv").hide();
            getOffers();
        }
    };
    $.ajax(options);
    scannedCode = "";
    resetTimer();
}

function getOffers() {
    if ($("#offersDiv").length > 0) {
        var options = {
            type: "POST",
            url: "EnterCode.aspx/GetOffers",
            contentType: "application/json; charset=utf-8",
            data: '{}',
            dataType: "json",
            success: function (msg) {
                $("#offersDiv").html(msg.toString());
                if ($("#offerRowsTable").height() >= 70)
                    $("#offerRowsTable").width(310);
            }
        };
        $.ajax(options);
    }
}

function stopTimer() {
    $.clearTimer(myTimer);
}

function resetTimer() {
    myTimer = $.timer(goBackTimerValue, showStartingDiv);
}

function goToProcessCode() {
    if (holdScannedCode != null && $("#productTable").length > 0)
        window.location = "./ProcessCode.aspx?&scannedCode=" + holdScannedCode;
}

function showStartingDiv() {
    holdScannedCode = null;
    $("#contentDiv").hide();
    $("#startingDiv").show();
}
function showscan(event) {
    if (typeof event == "undefined") {
        event = window.event;
    }

    var val = event.keyCode;
    if (val == 13) {
        search($('#txtScannedCode').val());
    }
}

$(document).ready(function () {

    var scannedCode = $().getUrlParam("scannedCode");
    if (scannedCode) {
        search(scannedCode);
    }
});

function document_onkeyup() {
    //The first button was pressed
    if (window.event.keyCode == 49) {
        //FirstButton
    }
    if (window.event.keyCode == 50) {
        //SecondButton
    }
    if (window.event.keyCode == 51) {
        //ThirdButton
    }
    if (window.event.keyCode == 52) {
        //FourtButton
        //goToProcessCode();
    }
}