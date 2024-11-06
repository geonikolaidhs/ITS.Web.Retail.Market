var isFromTransformation = null;
var hasReturnedFromOrderItemsForm = null;
var isTemporaryObject = null;

function UpdateDocumentGridForm(s, e) {
    toolbarHideFiltersOnly();
    grdDocument.ShowLoadingDivAndPanel();

}


function grdDocumentOnBeginCalback(s, e) {
    CustomizationWindow();
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        //clear selected items
        selectedItemsArray = [];
    }
    if (Component.GetName('Fromdate') !== null && Fromdate.GetDate() !== null) {
        e.customArgs.Fromdate = Fromdate.GetDate().toString();
    }

    if (Component.GetName('todate') !== null && todate.GetDate() !== null) {
        e.customArgs.todate = todate.GetDate().toString();
    }
    if (Component.GetName('FromExecdate') !== null && FromExecdate.GetDate() !== null) {
        e.customArgs.FromExecdate = FromExecdate.GetDate().toString();
    }
    if (Component.GetName('toExecdate') !== null && toExecdate.GetDate() !== null) {
        e.customArgs.toExecdate = toExecdate.GetDate().toString();
    }

    if (Component.GetName('statuscombo') !== null) {
        e.customArgs.statuscombo = statuscombo.GetValue();
    }

    if (Component.GetName('typecombo') !== null) {
        e.customArgs.typecombo = typecombo.GetValue();
    }

    if (Component.GetName('seriescombo') !== null) {
        e.customArgs.seriescombo = seriescombo.GetValue();
    }

    if (Component.GetName('txtnumberFrom') !== null) {
        e.customArgs.txtnumberFrom = txtnumberFrom.GetValue();
    }

    if (Component.GetName('txtnumberTo') !== null) {
        e.customArgs.txtnumberTo = txtnumberTo.GetValue();
    }

    if (Component.GetName("sell_from") !== null && Component.GetName("sell_from").GetValue() !== null && typeof Component.GetName("sell_from").GetValue() !== "undefined") {
        e.customArgs.store = sell_from.GetValue();
    }

    if (Component.GetName("Users") !== null) {
        e.customArgs.user = Users.GetValue();
    }

    if (Component.GetName("createdByDevice") !== null) {
        e.customArgs.createdByDevice = createdByDevice.GetSelectedValues();
    }

    if (Component.GetName("TransformationStatus") !== null &&
        Component.GetName("TransformationStatus").GetValue() !== null && typeof Component.GetName("TransformationStatus").GetValue() !== "undefined") {
        e.customArgs.TransformationStatus = Component.GetName("TransformationStatus").GetValue();
    }

    e.customArgs.DocumentsDivision = DocumentsDivision;

    if (typeof Mode !== "undefined") {
        e.customArgs.Mode = Mode.value;
    }

    if (action == 'Proforma') {
        e.customArgs.Proforma = "Proforma";
    }

}

function AddNewItem() {
    selectedItemsArray = [];

    Component.EmptyCallbackPanels();
    PopupEditCallbackPanel.PerformCallback();

    if (typeof grdDocument !== "undefined") {
        grdDocument.UnselectAllRowsOnPage();
    }

    return false;
}

function CancelDocument(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToEdit);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToEdit);
    }
    else if (selectedItemsArray.length == 1 && confirm(documentWillBeCanceledContinueAnyway)) {
        var path = $('#HOME').val() + 'Document/jsonCancelDocument';
        $.ajax({
            type: 'POST',
            async: false,
            url: path,
            data: { 'DocumentGuid': selectedItemsArray[0] },
            cache: false,
            dataType: 'json',
            success: function (data) {
                if (typeof data.success !== typeof undefined) {
                    setJSNotification(data.success);
                    PrintDocument(data.CancelingDocument, true);
                }
                else if (typeof data.error !== typeof undefined) {
                    setJSError(data.error);
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });

        return false;
    }
}

function OnColumnResizing(s, e) {
    if (e.column.name == "hidden") {
        e.cancel = true;
    }
}

function PrintSelectedDocuments() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToEdit);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToEdit);
    }
    else {
        PrintDocument(selectedItemsArray[0], false);
    }
}

function PrintDocument(DocumentOid, DirectPrint) {
    var path = $('#HOME').val() + 'Document/jsonGetNumberOfCustomReports';
    var numberOfReports;
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: { DOid: DocumentOid },
        cache: false,
        dataType: 'json',
        success: function (data) {
            numberOfReports = data.numberOfReports;
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
    if (numberOfReports > 1) {
        DialogCallbackPanelSecondary.PerformCallback(DocumentOid);
    }
    else if (numberOfReports === 0 || numberOfReports === 1) {
        url = $("#HOME").val() + "Document/Print?DOid=" + DocumentOid + '&directPrint=' + DirectPrint;
        var win = window.open(url, '_blank');
        win.focus();
    }
}

function TransformSelectedDocumentFast(s, e) {
    TransformSelectedDocument(s, e, true);
}

function TransformSelectedDocument(s, e, isfast) {
    var fastTransform = false;
    if (typeof (isfast) === typeof (false)) {
        fastTransform = isfast;
    }

    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
    }
    else if (fastTransform) {
        if ((selectedItemsArray.length > 1 && SelectedDocumentsCanBeTransformed(selectedItemsArray.toString(), true)) ||
            (selectedItemsArray.length == 1 && SelectedDocumentsCanBeTransformed(selectedItemsArray[0], true))) {
            TransformationPopupBtnContinue(undefined, undefined, true);
        }
    }
    else if (selectedItemsArray.length > 1 && SelectedDocumentsCanBeTransformed(selectedItemsArray.toString(), false) ||
        (selectedItemsArray.length == 1 && SelectedDocumentsCanBeTransformed(selectedItemsArray[0], false))
    ) {
        TransformationPopup.Show();
    }

}

function OnShownTransformationPopup(s, e) {
    s.PerformCallback({ documents: selectedItemsArray.toString() });
}

function TransformationPopupBtnContinue(s, e, isProforma) {
    if (typeof (isProforma) == typeof (undefined) && grdTransformationDetailGrid.cp_selectedTransformationItemsArray.length <= 0) {
        setJSError(pleaseSelectAtLeastOneObject);
    }
    else if (typeof (isProforma) == typeof (undefined) &&
        (Component.GetName('TransformationRuleDerrivedType').GetValue() === null ||
            Component.GetName('DocumentSeries').GetValue() === null
        )) {
        setJSError(pleaseSelectDocumentTypeAndDocumentSeries);
    }
    else {
        var childWindow;
        var postDocumentsParameterName;
        var documents;
        var transformationRuleDerrivedType;
        var documentSeries;

        if (typeof (isProforma) == typeof (undefined)) {
            transformationRuleDerrivedType = document.getElementById('TransformationRuleDerrivedType_VI').value;
            documentSeries = document.getElementById('DocumentSeries_VI').value;

            if (document.getElementById('DC') !== null && typeof document.getElementById('DC') !== "undefined") {
                postDocumentsParameterName = 'DC';
                documents = document.getElementById('DC').value;
            }
            else if (document.getElementById('DCs') !== null && typeof document.getElementById('DCs') !== "undefined") {
                postDocumentsParameterName = 'DCs';
                documents = document.getElementById('DCs').value;
            }

        }
        else if (isProforma) {
            if (selectedItemsArray.length == 1) {
                postDocumentsParameterName = 'DC';
            }
            else {
                postDocumentsParameterName = 'DCs';
            }

            documents = selectedItemsArray.toString();
        }
        else {
            setJSError(anErrorOccured);
        }

        var path = $('#HOME').val() + 'Document/TransformDocument?' + postDocumentsParameterName + '=' + documents + '&TransformationRuleDerrivedType=' + transformationRuleDerrivedType + '&DocumentSeries=' + documentSeries;

        $.ajax({
            type: 'POST',
            async: false,
            url: path,
            data: {},
            cache: false,
            dataType: 'json',
            success: function (data) {
                console.log(data);
                if (typeof data === "object") {
                    if (typeof (data.errormsg) !== typeof (undefined)) {
                        setJSError(data.result);
                    }
                    else if (typeof data.LoadFromSession !== typeof undefined) {
                        TransformationPopup.Hide();
                        selectedItemsArray = [];
                        isFromTransformation = true;
                        Component.EmptyCallbackPanels();
                        PopupEditCallbackPanel.PerformCallback();

                        grdDocument.UnselectAllRowsOnPage();

                        return false;
                    }
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}

function OnTransformationPopupCloseButtonClick(s, e) {
    var path = $('#HOME').val() + 'Document/jsonClearTransformationData';
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: {},
        cache: false,
        dataType: 'json',
        success: function (data) {
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
    grdTransformationDetailGrid.PerformCallback();
}

function TransformationPopupBtnClose(s, e) {
    OnTransformationPopupCloseButtonClick();
    TransformationPopup.Hide();
}

function SelectedDocumentsCanBeTransformed(documents, isFast) {
    var path = $('#HOME').val() + 'Document/jsonSelectedDocumentsCanBeTransformed';
    var returnValue = false;
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: { 'documents': documents, 'isFast': isFast },
        cache: false,
        dataType: 'json',
        success: function (data) {
            returnValue = data.returnValue;
        },
        error: function (data) {
            returnValue = false;
            setJSError(anErrorOccured);
        }
    });
    return returnValue === "";
}

function EditSelectedRows() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToEdit);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToEdit);
    }
    else if (selectedItemsArray.length == 1) {
        var result = CheckSelectedDocumentForEditing(selectedItemsArray[0]);
        if (result.returnValue) {

            Component.EmptyCallbackPanels();
            PopupEditCallbackPanel.PerformCallback();

            grdDocument.UnselectAllRowsOnPage();

            return false;
        }
    }
}



function OnTemporaryClick() {
    new DropDown($('#dd'));
    $(".temp-object-link").click(function () {
        var selectedOid = $(this).data('id');
        var path = $('#HOME').val() + 'TemporaryObject/StartEdit';
        $.ajax({
            type: 'POST',
            async: false,
            url: path,
            data: { 'Oid': selectedOid },
            cache: false,
            dataType: 'json',
            success: function (data) {
                if (typeof data.Success !== typeof undefined) {
                    isTemporaryObject = true;
                    grdDocument.UnselectAllRowsOnPage();
                    selectedItemsArray = [];
                    selectedItemsArray.push(selectedOid);

                    Component.EmptyCallbackPanels();
                    PopupEditCallbackPanel.PerformCallback();
                }
                else {
                    setJSError(data.Error);
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
        $('#temporaryDocumentInfoContent').toggleClass('active');
    });
}


function OnBeginCallbackOrderItemsPopUp(s, e) {

    if ($("#docType").length === 1) {
        e.customArgs.docType = $("#docType").val();
    }

    if ($("#docSeries").length === 1) {
        e.customArgs.docSeries = $("#docSeries").val();
    }

    if ($("#docNumber").length === 1) {
        e.customArgs.docNumber = $("#docNumber").val();
    }

    if ($("#docFinDate").length === 1) {
        e.customArgs.docTydocFinDatepe = $("#docFinDate").val();
    }

    if ($("#docStatus").length === 1) {
        e.customArgs.docStatus = $("#docStatus").val();
    }

    if ($("#docChecked").length === 1) {
        e.customArgs.docChecked = $("#docChecked").val();
    }

    if ($("#docExecuted").length === 1) {
        e.customArgs.docExecuted = $("#docExecuted").val();
    }

    if ($("#docInvoiceDate").length === 1) {
        e.customArgs.docInvoiceDate = $("#docInvoiceDate").val();
    }

    if ($("#docDelAddr").length === 1) {
        e.customArgs.docDelAddr = $("#docDelAddr").val();
    }

    if ($("#docRemarks").length === 1) {
        e.customArgs.docRemarks = $("#docRemarks").val();
    }

    if ($("#docTransferMethod").length === 1) {
        e.customArgs.docTransferMethod = $("#docTransferMethod").val();
    }

    if ($("#docPlaceOfLoading").length === 1) {
        e.customArgs.docPlaceOfLoading = $("#docPlaceOfLoading").val();
    }

    if ($("#docVehicleNumber").length === 1) {
        e.customArgs.docVehicleNumber = $("#docVehicleNumber").val();
    }

    if ($("#docTransferPurpose").length === 1) {
        e.customArgs.docTransferPurpose = $("#docTransferPurpose").val();
    }

    if ($("#docExecutionDate").length === 1) {
        e.customArgs.docExecutionDate = $("#docExecutionDate").val();
    }

    if ($("#docTriangularCustomer").length === 1) {
        e.customArgs.docTriangularCustomer = $("#docTriangularCustomer").val();
    }

    if ($("#docTriangularSupplier").length === 1) {
        e.customArgs.docTriangularSupplier = $("#docTriangularSupplier").val();
    }

    if ($("#docTriangularStore").length === 1) {
        e.customArgs.docTriangularStore = $("#docTriangularStore").val();
    }

    if ($("#docChargedToUser").length === 1) {
        e.customArgs.docChargedToUser = $("#docChargedToUser").val();
    }
}

function OrderTabsCustomerClick() {

    var index;
    index = parseInt($(this).data('index'));
    order_tabs.SetActiveTabIndex(index);

}

function OnEndCallbackOrderItemsPopUp(s, e) {
    $('#documentMenuPanel .basket').click(OrderTabsCustomerClick);


    if (order_tabs.GetActiveTabIndex() === 0) {
        sliderControl();
        $('.wrapper #document_wrapper .OrderItemPhoto').click(function () {
            var barcode = $(this).find('.OrderItemPhotoDiv').html();
            PerformItemTagCloudClick(barcode);
            $('.wrapper #document_wrapper').css('z-index', '0');
        });

        $('.wrapper #document_wrapper').hover(function () {
            clearInterval(sliders);
            $('.wrapper #document_wrapper .arrows').fadeToggle().stop(true, true);
        });

        $('.wrapper #document_wrapper').mouseleave(function () {
            sliderControl();
        });

    }


    $('.dxtc-content').attr("style", "");
    $('.dxtc-content').attr("style", "border: 0px solid #fff !important");
}


function CopySelectedDocument() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToCopy);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToCopy);
    }
    else if (selectedItemsArray.length == 1 && CheckSelectedDocumentForCopying(selectedItemsArray[0])) {
        var path = $('#HOME').val() + 'Document/Copy?FromDC=' + selectedItemsArray[0];
        $.ajax({
            type: 'POST',
            async: false,
            url: path,
            data: {},
            cache: false,
            dataType: 'json',
            success: function (data) {
                console.log(data);
                if (typeof data === "object") {
                    if (typeof data.LoadFromSession !== typeof undefined) {
                        isFromTransformation = true;
                        Component.EmptyCallbackPanels();
                        PopupEditCallbackPanel.PerformCallback();
                        grdDocument.UnselectAllRowsOnPage();
                        return false;
                    }
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}


function MergeSelectedDocuments() {

    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToCopy);
    }
    else {
        var oids = "mergeDocuments:" + (window.innerWidth - 200) + ":" + (window.innerHeight - 150) + ":";
        for (var i = 0; i < selectedItemsArray.length; i++) {
            oids = oids + selectedItemsArray[i];
            if ((i + 1) !== selectedItemsArray.length) {
                oids = oids + ":";
            }
        }
        DialogCallbackPanelSecondary.PerformCallback(oids);
    }
}

function grdMergedDocumentDetailOnBeginCalback(s, e) {

    CustomizationWindow();
    if (e.command.search("APPLYCOLUMNFILTER") != -1) {
        //clear selected items
        selectedItemsArray = [];
    }

    if (!s.IsEditing()) {
        try {
            var fbarcode = Component.GetName('grdMergedDocumentDetail_DXFREditorcol1');
            if (fbarcode != null) {
                e.customArgs.Barcode = fbarcode.GetValue();
            }
            var fcode = Component.GetName('grdMergedDocumentDetail_DXFREditorcol0');
            if (fcode != null) {
                e.customArgs.Code = fcode.GetValue();
            }
            var fdescription = Component.GetName('grdMergedDocumentDetail_DXFREditorcol2');
            if (fdescription != null) {
                e.customArgs.Description = fdescription.GetValue();
            }
            var fQty = Component.GetName('grdMergedDocumentDetail_DXFREditorcol3');
            if (fQty != null) {
                e.customArgs.Qty = fQty.GetValue();
            }
            var fvat = Component.GetName('grdMergedDocumentDetail_DXFREditorcol4');
            if (fvat != null) {
                e.customArgs.VatFactor = fvat.GetValue();
            }
            var fIsLinkedLine = Component.GetName('grdMergedDocumentDetail_DXFREditorcol5');
            if (fIsLinkedLine != null) {
                e.customArgs.IsLinkedLine = fIsLinkedLine.GetValue();
            }
            var fremarks = Component.GetName('grdMergedDocumentDetail_DXFREditorcol6');
            if (fremarks != null) {
                e.customArgs.Remarks = fremarks.GetValue();
            }
        } catch (err) {
            console.log(err);
        }
    }
}


function grdMergedDocumentDetailOnBeginCalbackOnEndCalback(s, e) {

}

function PrintMergedDocument() {
    url = $('#HOME').val() + "Document" + '/ExportMergeDetails';
    window.location.href = url;
}

function CheckSelectedDocumentForCopying(documentOid) {
    var path = $('#HOME').val() + 'Document/jsonIsDocumentCopyable';
    var returnValue = false;
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: { 'documentOid': documentOid },
        cache: false,
        dataType: 'json',
        success: function (data) {
            returnValue = data.returnValue;
        },
        error: function (data) {
            returnValue = false;
            setJSError(anErrorOccured);
        }
    });
    return returnValue;
}

function CheckSelectedDocumentForEditing(documentOid) {
    var path = $('#HOME').val() + 'Document/jsonIsDocumentEditable';
    var result = { returnValue: false, warnForCrashed: false };
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: { 'documentOid': documentOid },
        cache: false,
        dataType: 'json',
        success: function (data) {
            result.returnValue = data.returnValue;
            result.warnForCrashed = data.warnForCrashed;
        },
        error: function (data) {
            result.returnValue = false;
            setJSError(anErrorOccured);
        }
    });
    return result;
}

function ShowRelativeDocumentPopup(docGuid) {

    for (var i = 0; i < selectedItemsArray.length; i++) {
        console.log(docGuid);
        Component.GetName("document_" + docGuid.replaceAll("-", "_")).Show();
    }
}

function OnEndCallbackBackTransformationDetailsGrid(s, e) {
    if (order_action == "STARTEDIT" && typeof (qty_spin_edit) != "undefined" && qty_spin_edit !== null) {
        qty_spin_edit.Focus();
        qty_spin_edit.SelectAll();
    }
    if (order_action == "STARTEDIT" && grdTransformationDetailGrid.IsEditing() === false) {
        grdTransformationDetailGrid.SetFocusedRowIndex(e.visibleIndex);
        grdTransformationDetailGrid.StartEditRow(grdTransformationDetailGrid.GetFocusedRowIndex());
    }
    if (order_action == "UPDATEEDIT") {
        if (typeof (qty_spin_edit) != "undefined" && qty_spin_edit.GetMainElement() !== null) {

            qty_spin_edit.SetVisible(true);
            qty_spin_edit.SetValue(0);
            qty_spin_edit.Focus();
        }
    }
    click_fun = "TransformationDetailQtyChanged();";
    $(".inserted_btn").append("<div id=\"qty_sv_btn\" class=\"inside_btn\" onclick=\"" + click_fun + "\">OK</div>");
}

function TransformationDetailQtyChanged() {
    if (grdTransformationDetailGrid.IsEditing()) {
        grdTransformationDetailGrid.UpdateEdit();
    }
}

function OnBeginCallBackTransformationDetailsGrid(s, e) {


    order_action = e.command;
    if (e.command == "UPDATEEDIT") {
        e.customArgs.document_detail_association_guid = grdTransformationDetailGrid.GetRowKey(grdTransformationDetailGrid.GetFocusedRowIndex());
        qty_spin_edit.SetVisible(false);
        qty_spin_edit.SetValue(Math.round(qty_spin_edit.GetValue() * QUANTITY_MULTIPLIER));
    }
}

function OnRowClickedTransformationDetailsGrid(s, e) {
    if (grdTransformationDetailGrid.IsEditing()) {
        grdTransformationDetailGrid.UpdateEdit();
    }
    else {
        grdTransformationDetailGrid.SetFocusedRowIndex(e.visibleIndex);
        grdTransformationDetailGrid.StartEditRow(grdTransformationDetailGrid.GetFocusedRowIndex());
    }
}

// ListBox Func
var textSeparator = ";";
function DocumentOnListBoxSelectionChanged(listBox, args) {
    if (args.index === 0)
        if (args.isSelected) {
            listBox.SelectAll();
        }
        else {
            listBox.UnselectAll();
        }

    DocumentUpdateSelectAllItemState();
    DocumentUpdateText();
}
function DocumentUpdateSelectAllItemState() {
    if (DocumentIsAllSelected()) {
        createdByDevice.SelectIndices([0]);
    }
    else {
        createdByDevice.UnselectIndices([0]);
    }
}
function DocumentIsAllSelected() {
    for (var i = 1; i < createdByDevice.GetItemCount(); i++)
        if (!createdByDevice.GetItem(i).selected)
            return false;
    return true;
}
function DocumentUpdateText() {
    var selectedItems = createdByDevice.GetSelectedItems();
    checkComboBox.SetText(DocumentGetSelectedItemsText(selectedItems));
}
function SynchronizeListBoxValues(dropDown, args) {
    createdByDevice.UnselectAll();
    var texts = dropDown.GetText().split(textSeparator);
    var values = DocumentGetValuesByTexts(texts);
    createdByDevice.SelectValues(values);
    DocumentUpdateSelectAllItemState();
    DocumentUpdateText(); // for remove non-existing texts
}
function DocumentGetSelectedItemsText(items) {
    var texts = [];
    for (var i = 0; i < items.length; i++)
        if (items[i].index !== 0)
            texts.push(items[i].text);
    return texts.join(textSeparator);
}
function DocumentGetValuesByTexts(texts) {
    var actualValues = [];
    var item;
    for (var i = 0; i < texts.length; i++) {
        item = createdByDevice.FindItemByText(texts[i]);
        if (item !== null)
            actualValues.push(item.value);
    }
    return actualValues;
}
///End of ListBox Functions



function NumberOfOrderDocumentTypesDefined() {
    $.ajax({
        type: 'POST',
        url: $('#HOME').val() + 'Document/NumberOfOrderDocumentTypesDefined',
        cache: false,
        dataType: 'json',
        async: true,
        success: function (data) {
            if (typeof data.error !== "undefined") {
                setJSError(data.error);
            }

            if (data.numberOfOrderDocumentTypesDefined == 1) {
                PopupGenericEditCallbackPanel.PerformCallback({ ID: '00000000-0000-0000-0000-000000000000', Recover: false, Division: 'Sales', DocType: data.storeDocumentSeriesType, DisplayGeneric: true });
            }
            else if (data.numberOfOrderDocumentTypesDefined > 1) {
                DialogCallbackPanelSecondary.PerformCallback({ action: 'SELECT_ORDER' });
            }
        },
        error: function (data) {
            console.log(data);
        }
    });
}

function ContinueToOrder(s, e) {
    Dialog.Hide();
    PopupGenericEditCallbackPanel.PerformCallback({ ID: '00000000-0000-0000-0000-000000000000', Recover: false, Division: 'Sales', DocType: OrderDocumentType.GetValue(), DisplayGeneric: true });
}

function UpdateDocumentGridForm(s, e) {
    toolbarHideFiltersOnly();
    grdDocument.ShowLoadingDivAndPanel();

}

function OnSearchComplete() {
    grdDocument.UnselectRows();
}

function PreviewSelectedDocuments() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToEdit);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToEdit);
    }
    else {
        PrintDocument(selectedItemsArray[0], true);
    }
}
