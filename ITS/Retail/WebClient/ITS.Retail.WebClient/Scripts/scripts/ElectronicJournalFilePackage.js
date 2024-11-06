function DownloadPackage(s, e) {
    if (selectedItemsArray.length > 0) {
        $("#Oids").val(selectedItemsArray.toString());
        $("#downloadEJs").submit();
    }
}