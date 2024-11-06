function printPlugin() {
    return document.getElementById('printPlugin');
}

var LabelsPrintingCommon = {
    getInstalledPluginVersion: function () {
        return printPlugin().version;
    },
    getPluginCurrentVersion: function () {
        return pluginCurrentVersion;
    },
    printLabels: function (selectedItems, type, mode, label) {
        pcd_oids = selectedItems;
        item_type = type;
        print_mode = mode;
        var path = $('#HOME').val() + 'Labels/jsonPrint';
        var result;
        $.ajax({
            type: 'POST',
            url: path,
            cache: false,
            data: { 'SelectedItems': selectedItems, "Type": type, "Mode": mode, "label": label },
            dataType: 'json',
            async: false,
            success: function (data) {
                output = data.output;
                error = data.error;
                //if (data.printingType == "com") {
                //    result = printPlugin().printToCom(data.output, data.portName);
                //}
                //else if (data.printingType == "windowsdriver") {
                //    result = printPlugin().printToWindowsPrinter(data.output);
                //}
                if (typeof data.success !== typeof undefined) {

                    setJSNotification(successful_label_print);

                    LabelsPrintingCommon.mark_printed_pcds(pcd_oids, item_type, print_mode);
                }
                else if (result !== null && typeof result !== "undefined") {
                    if (result === "") {
                        setJSError(cancelled_label_print); //User cancelled the print process
                    }
                    else {
                        setJSError(result);    //ERROR
                    }
                }
            },
            error: function (data) {
                setJSError(data);
            }
        });
        return result;
    },
    mark_printed_pcds: function (selectedOids, type, mode) {
        $.ajax({
            type: 'POST',
            url: $('#HOME').val() + 'Labels/jsonMarkPrintedPriceCatalogDetails',
            cache: false,
            data: { 'SelectedOids': selectedOids, 'type': type, 'mode': mode },
            dataType: 'json',
            async: false,
        });
    }

};