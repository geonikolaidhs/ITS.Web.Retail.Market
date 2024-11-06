$(document).ready(function () {
    if (typeof (printPlugin) !== "undefined" && printPlugin() !== null) {
        if (typeof (printPlugin().version) === "undefined") {
            $('#pluginNotFoundPanel').show();
            $('#page_content').hide();
            $('.FilterPanel').hide();
            $('.fast_menu_options').hide();
            grdItemBarcodes.SetVisible(false);
        }
        else if (LabelsPrintingCommon.getInstalledPluginVersion() != LabelsPrintingCommon.getPluginCurrentVersion()) {
            $('#pluginNotCurrentVersion').show();
            $('#lblInstalledVersion').html($('#lblInstalledVersion').html() + printPlugin().version);
            $('#page_content').hide();
            $('.FilterPanel').hide();
            $('.fast_menu_options').hide();
            grdItemBarcodes.SetVisible(false);
        }
    }

    $("#btnClearCategories").hide();

    $("#btnClearCategories").click(function () {
        SelectedNodeOid = -1;
        SelectedNodeText = "";
        $("#categoryfilterid").text(SelectedNodeOid);
        $("#categoryfiltertext").text(SelectedNodeText);
        $("#btnClearCategories").hide();
    });

});

function OnClickBtnChooseCategory(s, e) {
    $("#categoryfilterid").text(SelectedNodeOid);
    $("#categoryfiltertext").text(SelectedNodeText);
    $("#btnClearCategories").show();
    pcCategoriesPopup.Hide();
}

function SearchItemBarcode(s, e) {
    selectedItemsArray = [];
    grdItemBarcodes.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}



function OnColumnResizing(s, e) {
    if (e.column.name == "hidden") {
        e.cancel = true;
    }
}

function OnBeginCallback(s, e) {
    CustomizationWindow();
    if (e.command.search("APPLYCOLUMNFILTER") != -1)
    {
        //clear selected items
        selectedItemsArray = [];
    }
    if (!s.IsEditing()) {
        e.customArgs.Fcode = Component.GetName('Fcode').GetValue();
        e.customArgs.Fname = Component.GetName('Fname').GetValue();
        e.customArgs.Fbarcode = Component.GetName('Fbarcode').GetValue();
        e.customArgs.Factive = Component.GetName('Factive').GetValue();
        e.customArgs.Fcategory = Component.GetName('categoryfilterid').GetValue();
        e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
        e.customArgs.FupdatedOn = Component.GetName('FupdatedOn') === null ? null : Component.GetName('FupdatedOn').GetText();
        e.customArgs.FitemSupplier = Component.GetName('FitemSupplier').GetValue();
        e.customArgs.Fbuyer = Component.GetName('Fbuyer') === null ? null : Component.GetName('Fbuyer').GetValue();
        e.customArgs.Fseasonality = Component.GetName('Fseasonality') === null ? null : Component.GetName('Fseasonality').GetValue();
        e.customArgs.Fmothercode = Component.GetName('Fmothercode').GetValue();
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function TreeViewGetData(s, e) {

    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }

    if (SelectedNodeOid == s.GetSelectedNode().name) {
        $("#categoryfilterid").text(SelectedNodeOid);
        $("#categoryfiltertext").text(SelectedNodeText);
        $("#btnClearCategories").show();
        pcCategoriesPopup.Hide();
    }
    SelectedNodeOid = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
}

function btnPrintClick() {
    if (selectedItemsArray.length != 1) {
        setJSNotification(pleaseSelectOnlyOneObject);
        return;
    }
    var output;
    var label;
    if (Component.GetName('labels') !== null) {
        label = Component.GetName('labels').GetValue();
    }
    var result = LabelsPrintingCommon.printLabels(selectedItemsArray, 'barcode', '', label);
}

function PrintLabelEndCallback(s, e) {
    if (printPlugin() !== null) {
        if (typeof (printPlugin().version) === "undefined") {
            $('#pluginNotFoundPanel').show();
        }
        else if (LabelsPrintingCommon.getInstalledPluginVersion() != LabelsPrintingCommon.getPluginCurrentVersion()) {
            $('#pluginNotCurrentVersion').show();
            $('#lblInstalledVersion').html($('#lblInstalledVersion').html() + printPlugin().version);
        }
        //$('#page_content').hide();
        //grdLabels.SetVisible(false);
    }
}

function btnExportPricer_OnClick(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnItem);
    }
    else {
        window.location.href = $("#HOME").val() + "ItemBarcode/ExportLabels?ItemGuids=" + selectedItemsArray.toString() + "&Type=Pricer";
    }    
}

function btnPrintClick() {
    if (selectedItemsArray.length !== 1) {
        setJSNotification(pleaseSelectOnlyOneObject);
        return;
    }
    var output;
    var label;
    if (Component.GetName('labels') !== null) {
        label = Component.GetName('labels').GetValue();
    }
    var result = LabelsPrintingCommon.printLabels(selectedItemsArray, 'barcode', '', label);
}

//function PrintLabelEndCallback(s, e) {
//    if (LabelsPrintingCommon.checkPlugin() === false) {
//        if (LabelsPrintingCommon.getInstalledPluginVersion() != LabelsPrintingCommon.getPluginCurrentVersion()) {
//            $('#pluginNotCurrentVersion').show();
//            $('#lblInstalledVersion').html($('#lblInstalledVersion').html() + printPlugin().version);
//        }
//        else {
//            $('#pluginNotFoundPanel').show();
//        }
//        $('#page_content').hide();
//        grdItemBarcodes.SetVisible(false);
//    }
//}

function btnExportPricer_OnClick(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnItem);
    }
    else {
        window.location.href = $("#HOME").val() + "ItemBarcode/ExportLabels?ItemGuids=" + selectedItemsArray.toString() + "&Type=Pricer";
    }
}