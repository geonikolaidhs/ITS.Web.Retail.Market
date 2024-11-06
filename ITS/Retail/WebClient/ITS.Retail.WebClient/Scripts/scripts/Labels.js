var isLabelDocumentDetailGridChecked = null,
    selectedLabelsDetailGrid = new Dictionary(),
    selectedLabelDocumentDetailGrid = null;



function SearchDocument(s, e) {
    grdLabelDocumentsGrid.PerformCallback("SEARCH");
}

function Search(s, e) {

    if (!ValidateModalFormSingle()) {
        return;
    }
    grdLabels.PerformCallback("SEARCH"+s.name);
    toolbarHideFiltersOnly();
}

function LabelsGridOnBeginCallback(s, e) {

    if (!s.IsEditing()) {
        e.customArgs['FromDate'] = Component.GetName('fnewPricesFrom').GetText();
        e.customArgs['FromDateTime'] = Component.GetName('fnewPricesFromTime').GetText();
        e.customArgs['ToDate'] = Component.GetName('fnewPricesTo').GetText();
        e.customArgs['ToDateTime'] = Component.GetName('fnewPricesToTime').GetText();
        e.customArgs['FromCode'] = Component.GetName('fFromCode').GetValue();
        e.customArgs['ToCode'] = Component.GetName('fToCode').GetValue();
        e.customArgs['Barcode'] = Component.GetName('Barcode').GetValue();
        e.customArgs['Description'] = Component.GetName('fDescription').GetValue();
        e.customArgs['WithValueChangeOnly'] = Component.GetName('fValueChanged').GetValue();

        e.customArgs['WithTimeValueFilter'] = Component.GetName('WithTimeValueFilter').GetValue();
        e.customArgs['TimeValueFromDate'] = Component.GetName('TimeValueFromDate').GetText();
        e.customArgs['TimeValueFromTime'] = Component.GetName('TimeValueFromTime').GetText();
        e.customArgs['TimeValueToDate'] = Component.GetName('TimeValueToDate').GetText();
        e.customArgs['TimeValueToTime'] = Component.GetName('TimeValueToTime').GetText();
       
        if (e.command.search("APPLYCOLUMNFILTER") != -1) {
            selectedItemsArray = [];
            filterSelected = true;
        }
    }
    selectedGridName = s.name;
    if (selectedGridName == "grdLabels"){
        selectedLabelsTitleChanged(selectedGridName);
    }
}

function grdLabelDocumentsGridBeginCallback(s, e) {
    e.customArgs.FromDate = Component.GetName('Fromdate').GetText();
    e.customArgs.ToDate = Component.GetName('todate').GetText();
    e.customArgs['FromDateTime'] = Component.GetName('FromDateTime').GetText();
    e.customArgs['ToDateTime'] = Component.GetName('ToDateTime').GetText();
   
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        selectedItemsArray = [];
        filterSelected = true;
    }
}

function btnExportPricer_OnClick() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else {
        var mode = (labelsFilterPanel.GetActiveTab().index === 0) ? 'pcd' : 'document';
        window.location.href = $("#HOME").val() + "Labels/ExportLabels?PriceCatalogDetailGuids=" + selectedItemsArray.toString() + "&Type=Pricer&Mode=" + mode;
        grdLabels.UnselectRows();
        grdLabelDocumentsGrid.UnselectRows();
    }
}

$(document).ready(function () {

    if ($('.wrapper .container #FilterPanelSecond').length > 0) {

        $(".wrapper .container #FilterPanelSecond span#FilterPanelSecond_RPHT").click(function () {
            toolbarHideSecondFilters();
        });

        $('.wrapper .container .dxgvControl_ITSTheme1').addClass('styleGrid');
        $('.wrapper .container .filter_search_container').css('left', $('.wrapper .container #FilterPanel_RPHT').width() + 55);

        $(".wrapper .container #FilterPanelSecond #btnSearchLabelsFromDocumentTags").click(function () {
            toolbarHideSecondFilters();
        });
    }
});

function toolbarHideSecondFilters() {
    toolTipText = FillCriteriaInfo();
    if ($('.FilterPanel #FilterPanelSecond_RPC').is(':visible')) {
        $('.FilterPanel #FilterPanelSecond_RPC').slideToggle("200", function () {
            $('.wrapper .container #FilterPanelSecond td.dxrpHeader_ITSTheme1 span').addClass("up");
        });
    } else {
        $('.FilterPanel #FilterPanelSecond_RPC').slideToggle("slow", function () {
            $('.wrapper .container  #FilterPanelSecond td.dxrpHeader_ITSTheme1 span').removeClass("up");
        });
}
}

function clearAllSecondFilters() {
    $('.FilterPanel #FilterPanelSecond_RPC .search_filter').each(function (index) {
        Component.GetName($(this).attr('id')).SetValue(null);        
    });
}

function LabelDocumentsGridOnEndCallback(s, e) {

    grdOnEndCalback(s, e);
    selectedItemsArray = [];
    selectedLabelsDetailGrid = new Dictionary();  
}

function LabelsGridEndCallback(s, e) {
    if (filterSelected === true) {
        s.UnselectRows();
        filterSelected = false;
    }
    selectedGridName = s.name;
    if(selectedGridName == "grdLabels"){
        selectedLabelsTitleChanged(selectedGridName);
    }
}

function LabelsActiveTabChanged(s, e) {
    selectedItemsArray = [];
    if (e.tab.index === 0) {
        grdLabelDocumentsGrid.UnselectRows();
    }
    else if (e.tab.index === 1) {
        grdLabels.UnselectRows();
    }

}

function selectedLabelsTitleChanged() {
    
    var returnedMessage = null;
    var selectedGridNameIndex = null;

    if (labelsFilterPanel.GetActiveTabIndex() == 0) {
        returnedMessage = selectedLabelsNum.replace("@1", selectedItemsArray.length);
        selectedGridNameIndex = "#grdLabels .dxgvTitlePanel_ITSTheme1";
        $(selectedGridNameIndex).text(labelsText + returnedMessage);
    }
    else if (labelsFilterPanel.GetActiveTabIndex() == 1) {
        selectedGridNameIndex = "#grdLabelDocumentsGrid_DXTitle .dxgvTitlePanel_ITSTheme1";
        returnedMessage = selectedLabelsNum.replace("@1", selectedItemsArray.length);
        $(selectedGridNameIndex).text(labelsText + returnedMessage);
    }  
}


function LabelsSelectionChanged(s, e) {
    if (s.name != "grdLabels") {
        var cbSelectAll = Component.GetName("cbSelectAll" + s.cpDocumentOid);
        cbSelectAll.SetChecked(s.GetSelectedRowCount() == s.cpVisibleRowCount);
    }

    if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
    }
    else {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
    }
    if (!e.isSelected && e.visibleIndex == "-1") {
        if (s.name == "grdLabels") {
            grdLabels.ShowLoadingDivAndPanel();
            s.GetSelectedFieldValues("Oid", OnLabelsGetValues);
        }
        else {
            isLabelDocumentDetailGridChecked = s.GetSelectedRowCount() == s.cpVisibleRowCount;
            selectedLabelDocumentDetailGrid = s.name;
            grdLabelDocumentsGrid.ShowLoadingDivAndPanel();
            s.GetSelectedFieldValues("Oid", LabelsDocumentsDetailOnGetValues);
        }
    }
    if (!e.isAllRecordsOnPage && !e.isChangedOnServer) {
        if (e.isSelected) {
            selectedItemsArray.push(s.GetRowKey(e.visibleIndex));
        }
        else {
            selectedItemsArray.splice(selectedItemsArray.indexOf(s.GetRowKey(e.visibleIndex)), 1);
        }
    }
    else if (!e.isChangedOnServer) {
        var i;
        if (e.isSelected) {
            for (i = 0; i < s.keys.length ; i++) {
                if (selectedItemsArray.indexOf(s.keys[i]) === -1) {
                    selectedItemsArray.push(s.keys[i]);
                }
            }
        }
        else {
            for (i = 0; i < s.keys.length; i++) {
                selectedItemsArray.splice(selectedItemsArray.indexOf((s.keys[i])), 1);
            }
        }
    }

    if (selectedItemsArray.length > 0) {
        ShowToolbar();
    }
    else {
        HideToolbar();
    }
    selectedGridName = s.name;
    
    selectedLabelsTitleChanged(selectedGridName);    
}

function OnLabelsGetValues(values) {
    selectedItemsArray = [];
    for (var i = 0; i < values.length ; i++) {
        if (selectedItemsArray.indexOf(values[i]) === -1) {
            selectedItemsArray.push(values[i]);
        }
    }

    selectedLabelsTitleChanged(selectedGridName);    
    grdLabels.HideLoadingPanel();
    grdLabels.HideLoadingDiv();
    
}

function LabelsDocumentsDetailOnGetValues(values) {
    var i;
    if (isLabelDocumentDetailGridChecked) {
        for (i = 0; i < values.length ; i++) {
            if (selectedItemsArray.indexOf(values[i]) === -1) {
                selectedItemsArray.push(values[i]);    
            }
        }
        if (!selectedLabelsDetailGrid.ContainsKey(selectedLabelDocumentDetailGrid)) {
            selectedLabelsDetailGrid.Add(selectedLabelDocumentDetailGrid, values);
        }
    }
    else {
        if (selectedLabelsDetailGrid.ContainsKey(selectedLabelDocumentDetailGrid)) {
            var selectedDocumentDetailArray = selectedLabelsDetailGrid[selectedLabelDocumentDetailGrid];
            for (i = 0; i < selectedDocumentDetailArray.length; i++) {
                selectedItemsArray.splice(selectedItemsArray.indexOf(selectedDocumentDetailArray[i]), 1);
                
            }
            selectedLabelsDetailGrid.Remove(selectedLabelDocumentDetailGrid);
        }
    }
    grdLabelDocumentsGrid.HideLoadingPanel();
    grdLabelDocumentsGrid.HideLoadingDiv();
    selectedLabelsTitleChanged(selectedGridName);
}

function grdLabelDocumentsGridMasterRowCollapsed(s, e) {
    selectedLabelsTitleChanged(s.name);
}


var Label = (function () {

    function OnKeyPressFiltersPrivate(s, e, btnName) {
        if (e.htmlEvent.keyCode === 13) {
            Component.GetName(btnName).DoClick();
            s.Focus();
        }
    }

    // Return an object exposed to the public
    return {
        WithValueOnKeyPressFilters: function (s, e) {
            OnKeyPressFiltersPrivate(s, e, "btnSearchLabelsWithValueChangesFilters");
        },

        FromDocumentTagsOnKeyPressFilters: function (s, e) {
            OnKeyPressFiltersPrivate(s, e, "btnSearchLabelsFromDocumentTags");
        },
        TimeValueFilterValueChanged: function (s, e) {
            TimeValueFromDate.SetEnabled(s.GetChecked());
            TimeValueFromTime.SetEnabled(s.GetChecked());
            TimeValueToDate.SetEnabled(s.GetChecked());
            TimeValueToTime.SetEnabled(s.GetChecked());
        },
        buttonPrintClick: function () {
            if (Component.GetName('labels') !== null) {
                label = Component.GetName('labels').GetValue();
            }
            Component.GetName('labels').GetText();

            var output;
            var mode = (labelsFilterPanel.GetActiveTab().index === 0) ? '' : 'document';
            var result = LabelsPrintingCommon.printLabels(selectedItemsArray, 'pcd', mode, label);

            grdLabels.PerformCallback();
            grdLabels.UnselectRows();
            grdLabelDocumentsGrid.UnselectRows();
        }
    };
})();