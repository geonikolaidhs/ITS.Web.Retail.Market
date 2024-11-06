
function ReportToolbarOnClick(s, e) {
    if (e.item.name == 'CustomButton') {

        var documentOid = window.location.search.split('=')[1];  //or document.URL.split('?')[1].split('=')[1];
        downloadURL($('#HOME').val() + 'Reports/GetReport?DOid=' + documentOid);
    }
}

function downloadURL(url) {
    var iframe;
    var hiddenIFrameID = 'hiddenDownloader';
    iframe = document.getElementById(hiddenIFrameID);
    if (iframe === null) {
        iframe = document.createElement('iframe');
        iframe.id = hiddenIFrameID;
        iframe.style.display = 'none';
        document.body.appendChild(iframe);
    }
    iframe.src = url;
}
