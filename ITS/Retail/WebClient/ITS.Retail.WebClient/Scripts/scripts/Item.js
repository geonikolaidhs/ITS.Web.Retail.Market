$(document).ready(function () {

    $("#btnClearCategories").hide();

    $("#btnClearCategories").click(function () {
        SelectedNodeOid = -1;
        SelectedNodeText = "";
        $("#categoryfilterid").text(SelectedNodeOid);
        $("#categoryfiltertext").text(SelectedNodeText);
        $("#btnClearCategories").hide();
    });
});

function enableSpeechRecognition(s, e) {
    var input = s.GetInputElement();
    input.setAttribute("x-webkit-speech", "x-webkit-speech");
    input.onwebkitspeechchange = function (evt) {
        if (s.filterStrategy && s.filterStrategy.FilterStartTimer)
            s.filterStrategy.FilterStartTimer();
        else
            aspxETextChanged(s.name);
    };

}

function OnClickBtnChooseCategory(s, e) {
    $("#categoryfilterid").text(SelectedNodeOid);
    $("#categoryfiltertext").text(SelectedNodeText);
    $("#btnClearCategories").show();
    pcCategoriesPopup.Hide();
}

function SearchItem(s, e) {
    selectedItemsArray = [];
    grdItems.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function OnBeginCallback(s, e) {
    CustomizationWindow();
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
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
        e.customArgs.DoesNotAllowDiscount = Component.GetName('DoesNotAllowDiscount').GetValue();

    }
}

function TreeViewGetData(s, e) {

    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }

    if (SelectedNodeOid == s.GetSelectedNode().name) {
        if (SelectedNodeOid == s.GetSelectedNode().name) {
            if (document.getElementById("categoryfilterid") !== null) {
                $("#categoryfilterid").text(SelectedNodeOid);
            }
            if (document.getElementById('categoryfiltertext') !== null) {
                $("#categoryfiltertext").text(SelectedNodeText);
            }
            if (document.getElementById('btnClearCategories') !== null) {
                $("#btnClearCategories").show();
            }
            if (Component.GetName('pcCategoriesPopup') !== null) {
                pcCategoriesPopup.Hide();
            }
        }
    }
    SelectedNodeOid = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

function btnExportPricer_OnClick(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnItem);
    }
    else {
        window.location.href = $("#HOME").val() + "Item/ExportLabels?ItemGuids=" + selectedItemsArray.toString() + "&Type=Pricer";
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
    var result = LabelsPrintingCommon.printLabels(selectedItemsArray, 'item', '', label);
}

//function PrintLabelEndCallback(s, e) {
//    if (printPlugin() !== null) {
//        if (typeof (printPlugin().version) === "undefined") {
//            $('#pluginNotFoundPanel').show();
//        }
//        else if (LabelsPrintingCommon.getInstalledPluginVersion() != LabelsPrintingCommon.getPluginCurrentVersion()) {
//            $('#pluginNotCurrentVersion').show();
//            $('#lblInstalledVersion').html($('#lblInstalledVersion').html() + printPlugin().version);
//        }
//        $('#page_content').hide();
//        //grdLabels.SetVisible(false);
//    } 
// }

//function OnValueChangedPrintingTypeRadioList(s, e) {
//    if (s.GetValue() == "com") {
//        $("#comSettings").show(); 
//    }
//    else if (s.GetValue() == "windowsdriver") {
//        $("#comSettings").hide();
//    }
//}





