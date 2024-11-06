//Common Functions in both DocumentForms
var SaveFocus = false;
initializer_object = null;
initializer_command = null;
DocType = null;
DocSeries = null;
DocTypeValueChangeNeedsConfirmation = true;
DocSeriesValueChangeNeedsConfirmation = true;
focused_input_id = null;
document_detail_input_order = [];
itemPanelCallbackCommand = "";

$(document).ready(function () {

    $(document).on('keypress', function (e) {
        MessageTextOnKeyEnter(e);
    });
    UpdateTemporaryFilterForm();
});



function qty_spin_editLostFocus(s, e) {
    if (e.htmlEvent !== null && e.htmlEvent !== undefined) {
        var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
        if (unicode !== 13) {
            if ($('#qty_sv_btn').length > 0) {
                $('#qty_sv_btn').trigger('click');
            } else {
                UpdateItemSingleForm();
            }
        }
    }
}


function qty_spin_editLostFocusKeyPress(s, e) {

    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (unicode === 13) {
        if ($('#qty_sv_btn').length > 0) {
            $('#qty_sv_btn').trigger('click');
        }
        else {
            UpdateItemSingleForm();
        }
    }
    else if (unicode > 31) {
        ValidateChar(s, e);
    }
}

function OnGotFocus(s, e) {
    focused_input_id = $('#' + s.uniqueID + ' input').attr('id');
    if (document_detail_input_order === 0) {
        $.each($('.wrapper .container .Document #documentEditPageControl .dxtc-content .floated').find('table').not('.textOnly').find('input'), function (index, value) {
            document_detail_input_order.push(value.id);
        });
        document_detail_input_order.push($('.wrapper .container .Document #documentEditPageControl .dxtc-content .floated textarea').attr('id'));
    }
}

function MessageTextOnKeyEnter(e) {
    if (e.keyCode === 10 || (e.keyCode === 13 && e.ctrlKey === true)) {
        if ($('#btnSaveItem').length || $('#btnSaveCloseItem').length) {
            UpdateItem('btnSaveItem', e);
        }
        return false;
    }
    return true;
}

function checkPopUpButtonsVisibility(visible) {

    if (typeof (btnUpdate) !== typeof (undefined)) {
        btnUpdate.SetEnabled(visible);
    }

    if (typeof (btnUpdateandPrint) !== typeof (undefined)) {
        btnUpdateandPrint.SetEnabled(visible);
    }

    if (typeof (btnUpdateAndRecalculateCosts) !== typeof (undefined) && typeof $("#btnUpdateAndRecalculateCosts").html() !== typeof (undefined)) {
        btnUpdateAndRecalculateCosts.SetEnabled(visible);
    }

    if (typeof (btnRecalculate) !== typeof (undefined)) {
        btnRecalculate.SetEnabled(visible);
    }

}

function itemPanelBeginCallback(s, e) {

    if (Component.GetName('spinlineqty') !== null) {
        e.customArgs.spinlineqty = Math.round(Component.GetName('spinlineqty').GetValue() * QUANTITY_MULTIPLIER);
    }

    if ($("#isNewDetail") !== null && typeof $("#isNewDetail") !== "undefined") {
        e.customArgs.isNewDetail = $("#isNewDetail").val();
    }

    if (Component.GetName('item_info_name') !== null) {
        e.customArgs.item_info_name = Component.GetName('item_info_name').GetValue();
    }
    if (Component.GetName('custom_price') !== null) {
        e.customArgs.custom_price = Math.round(Component.GetName('custom_price').GetValue() * QUANTITY_MULTIPLIER);
        if (isNaN(e.customArgs.custom_price)) {
            e.customArgs.custom_price = Math.round(Component.GetName('custom_price').GetValue().replace(',', '.') * QUANTITY_MULTIPLIER);
            if (isNaN(e.customArgs.custom_price)) {
                e.customArgs.custom_price = Math.round(Component.GetName('custom_price').GetValue().replace('.', ',') * QUANTITY_MULTIPLIER);
            }
        }
    }

    if (Component.GetName('CustomMeasurementUnit') !== null && CustomMeasurementUnit.IsVisible()) {
        e.customArgs.CustomMeasurementUnit = Component.GetName('CustomMeasurementUnit').GetValue();
    }

    if (Component.GetName('Remarks') !== null && Remarks.IsVisible()) {
        e.customArgs.Remarks = Component.GetName('Remarks').GetValue();
    }

    if ($('#DocumentDetailFormMode') !== null && typeof $('#DocumentDetailFormMode') !== "undefined") {
        e.customArgs.DocumentDetailFormMode = $('#DocumentDetailFormMode').val();
    }

    if (Component.GetName('userDiscount') !== null && Component.GetName('userDiscount').val() !== null) {
        e.customArgs.userDiscount = Math.round($("#userDiscount").val().replace(',', '.') * QUANTITY_MULTIPLIER);
    }
    if ($("#isPercentage") !== null && $("#isPercentage").val() !== null) {
        e.customArgs.isPercentage = $("#isPercentage").val();
    }


    if (typeof (current_index) !== "undefined") {
        if (current_index < grdEditGrid.pageRowCount - 1) {
            e.customArgs.nextOid = grdEditGrid.keys[current_index + 1];
        }
        if (current_index > 0) {
            e.customArgs.previousOid = grdEditGrid.keys[current_index - 1];
        }
    }
    var checkbox = Component.GetName('checkNewItem');
    if (checkbox !== null) {
        e.customArgs.check_box_state = checkbox.GetChecked();
    }
}

function ShowDocumentHeader() {
    if ($("#LoadEditPopup_PW-1 .dxpc-header").first().is(":hidden") === true && $("#LoadEditPopup_PW-1 .button_container").is(":hidden") === true) {
        $("#LoadEditPopup_PW-1 .dxpc-header").first().show();
        $("#LoadEditPopup_PW-1 .button_container").show();
        LoadEditPopup.SetHeight(window.innerHeight);
        LoadEditPopup.SetWidth(window.innerWidth);
    }

}

function HideDocumentHeader() {
    if ($("#LoadEditPopup_PW-1 .dxpc-header").first().is(":visible") === true && $("#LoadEditPopup_PW-1 .button_container").is(":visible") === true) {
        $("#LoadEditPopup_PW-1 .dxpc-header").first().hide();
        $("#LoadEditPopup_PW-1 .button_container").hide();
        LoadEditPopup.SetHeight(window.innerHeight);
        LoadEditPopup.SetWidth(window.innerWidth);
    }
}

function itemPanelEndCallback(s, e) {

    if (Component.GetName('spinlineqty') !== null && $('#hidden_quantity').val() !== null && typeof $('#hidden_quantity').val() !== "undefined") {
        Component.GetName('spinlineqty').SetValue($('#hidden_quantity').val());
    }

    if (Component.GetName('SaveHyperLink') !== null) {
        if (Component.GetName('item_info_code').GetValue() === null || Component.GetName('item_info_code').GetValue() === "") {
            if (Component.GetName('barcode_search') !== null && barcode_search.GetMainElement() !== null) {
                itemImagePanel.PerformCallback();
                barcode_search.Focus();
            }
        }
        else {
            if (!SaveFocus) {
                $('close_btn').trigger('click');
                if (Component.GetName('item_info_name').GetEnabled()) {
                    if (itemPanelCallbackCommand === 'CUSTOM_PRICE') {
                        itemImagePanel.PerformCallback();
                        spinlineqty.Focus();
                    }
                    else {
                        item_info_name.Focus();
                    }
                }
                else {
                    itemImagePanel.PerformCallback();
                    spinlineqty.Focus();
                }
            }
        }
        if (addedItem === true) {
            documentInfoPanel.PerformCallback();
            addedItem = false;
        }
        if (SaveFocus) {
            $("#SaveHyperLink").focus();
            SaveFocus = false;
        }
        createTagClouds();
    }
    else {
        if (!(item_info_code.GetValue() === null || item_info_code.GetValue() === "")) {
            spinlineqty.Focus();
        }
        else {
            barcode_search.Focus();
        }
        if (typeof (DocumentPaymentMethodPanel) !== typeof (undefined)) {
            DocumentPaymentMethodPanel.PerformCallback();
        }

        if (typeof (vatAnalysisInfoPanel) !== typeof (undefined)) {
            vatAnalysisInfoPanel.PerformCallback();
        }
        else if (typeof (documentSummaryPanel) !== typeof (undefined)) {
            documentSummaryPanel.PerformCallback();
        }
        else if (typeof (documentSummaryPanelPartial) !== typeof (undefined)) {
            documentSummaryPanelPartial.PerformCallback();
        }
    }
    itemPanelCallbackCommand = '';
}



function OnTransformationRuleChanged(s, e) {
    DocumentSeriesCbPanel.PerformCallback(s.uniqueID);
}

function OnDocumentTypeChanged(s, e) {

    //Check if document contains details
    if (typeof (grdCompositionDecompositionMainLinesGrid) === typeof (undefined)
        && (typeof (grdEditGrid) === typeof (undefined)
            || (typeof (grdEditGrid) !== typeof (undefined) && grdEditGrid.keys.length === 0)
        )
    ) {
        DocTypeValueChangeNeedsConfirmation = false;

    }
    //Check if document contains details for composition decomposition
    if (typeof (grdEditGrid) === typeof (undefined)
        && (typeof (grdCompositionDecompositionMainLinesGrid) === typeof (undefined)
            || (typeof (grdCompositionDecompositionMainLinesGrid) !== typeof (undefined) && grdCompositionDecompositionMainLinesGrid.keys.length === 0)
        )
    ) {
        DocTypeValueChangeNeedsConfirmation = false;
    }

    //If user confirms or no confirmation is needed
    if (DocTypeValueChangeNeedsConfirmation) {
        if (window.confirm(valuesWillBeChangedDoYouWantToContinue)) {
            DocType = s.GetValue();
            UpdateDocHeader(s, e);
        }
        else {
            DocTypeValueChangeNeedsConfirmation = false;
            s.SetValue(DocType);
            DocTypeValueChangeNeedsConfirmation = true;
        }
    }
    else {
        DocType = s.GetValue();
        UpdateDocHeader(s, e);
    }

}

function OnDefaultCustomerDiscountChanged(s, e) {
    var path = $('#HOME').val() + 'Document/JsonUpdateCustomerDocumentDiscount';
    $.ajax({
        type: 'POST',
        url: path,
        data:
        {
            'documentDiscount': Component.GetName('DefaultDocumentDiscount').GetValue(),
            'customerDiscount': Component.GetName('CustomerDiscount').GetValue()
        },
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {
            grdEditGrid.Refresh();
        },
        error: commonError
    });
}

function OnDefaultDocumentDiscountChanged(s, e) {
    var path = $('#HOME').val() + 'Document/JsonUpdateDefaultDocumentDiscount';
    $.ajax({
        type: 'POST',
        url: path,
        data:
        {
            'documentDiscount': Component.GetName('DefaultDocumentDiscount').GetValue(),
            'customerDiscount': Component.GetName('CustomerDiscount').GetValue()
        },
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {
            grdEditGrid.Refresh();

        },
        error: commonError
    });
}


function ClearDefaultDocumentDiscount() {
    var path = $('#HOME').val() + 'Document/JsonClearDefaultDocumentDiscount';
    $.ajax({
        type: 'POST',
        url: path,
        data: {},
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {

        },
        error: commonError
    });
}

function ClearDefaultCustomerDiscount() {
    var path = $('#HOME').val() + 'Document/JsonClearDefaultCustomerDiscount';
    $.ajax({
        type: 'POST',
        url: path,
        data: {},
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {

        },
        error: commonError
    });
}


function TabletGotFocus(s, e) {
    return false;
}

function OnDocumentTypeGotFocus(s, e) {
    DocType = s.GetValue();
    DocTypeValueChangeNeedsConfirmation = true;
}

function OnDocumentSeriesGotFocus(s, e) {
    DocSeries = s.GetValue();
    DocSeriesValueChangeNeedsConfirmation = true;
}

function OnDocumentSeriesChanged(s, e) {
    //Check if document contains details
    if (grdEditGrid.keys.length === 0) {
        DocSeriesValueChangeNeedsConfirmation = false;
    }

    //If user confirms or no confirmation is needed
    if (DocSeriesValueChangeNeedsConfirmation) {
        if (window.confirm(valuesWillBeChangedDoYouWantToContinue)) {
            DocSeries = s.GetValue();
            UpdateDocHeader(s, e);
        }
        else {
            DocSeriesValueChangeNeedsConfirmation = false;
            s.SetValue(DocSeries);
            DocSeriesValueChangeNeedsConfirmation = true;
        }
    }
    else {
        DocSeries = s.GetValue();
        UpdateDocHeader(s, e);
    }
}

function TransformationDocumentSeriesCbPanelOnBeginCallback(s, e) {
    var docType = TransformationRuleDerrivedType.GetValue();
    e.customArgs.DocumentType = docType;

}

function InvoicingDateValueChanged(s, e) {
    var path = $('#HOME').val() + 'Document/jsonInvoicingDateValueChanged';

    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'InvoicingDate': typeof (InvoicingDate) === typeof (undefined) || InvoicingDate === null ? null : InvoicingDate.GetValue()
        },
        cache: false,
        dataType: 'json',
        success: function (data) {

        },
        error: commonError
    });
}

function CheckIfEnterKeyPressed(s, e) {
    if (e.htmlEvent.keyCode === 10 || e.htmlEvent.keyCode === 13) {
        e.htmlEvent.preventDefault();
    }
}

function SearchByDescriptionOnLostFocus(s, e) {
    if (s.GetValue() !== "" && s.GetValue() !== null) {
        return;
    }

    var input = s.GetInputElement();
    input.style.color = "gray";
    input.value = searchByDescriptionNullText;
}

function SearchByDescriptionOnInit(s, e) {
    SearchByDescriptionOnLostFocus(s, e);
}

function SearchByDescriptionOnGotFocus(s, e) {
    var input = s.GetInputElement();
    if (input.value === searchByDescriptionNullText) {
        input.value = "";
        input.style.color = "black";
        s.HideDropDown();
    }
}

function ErrorInItemInfo(data) {
    $('#item_info_code').text('');
    $('#item_info_barcode').text('');
    $('#item_info_name').text('');

    commonError();
}

function ClearDocumentLine() {
    barcode_search.SetText('');
    description_search.SetText('');
    Component.GetName('spinlineqty').SetValue();
}

function GetDocumentHeaderParameters() {
    var parameters_list = {};
    parameters_list.docExecutionDate = Component.GetName('ExecutionDate') !== null ? Component.GetName('ExecutionDate').GetValue() : null;
    parameters_list.doctype = Component.GetName('DocumentType') !== null ? Component.GetName('DocumentType').GetValue() : null;
    parameters_list.doc_series = Component.GetName('DocumentSeries') !== null ? Component.GetName('DocumentSeries').GetValue() : null;
    parameters_list.deliveryAddress = typeof DeliveryAddress !== "undefined" ? Component.GetName('DeliveryAddress').GetValue() : "";
    parameters_list.HasBeenChecked = typeof HasBeenChecked !== "undefined" && HasBeenChecked !== null ? Component.GetName('HasBeenChecked').GetValue() : false;
    parameters_list.HasBeenExecuted = Component.GetName('HasBeenExecuted') !== null ? Component.GetName('HasBeenExecuted').GetValue() : false;
    parameters_list.docNumber = Component.GetName('DocNumber') !== null ? Component.GetName('DocNumber').GetValue() : null;
    parameters_list.docTransferMethod = typeof TransferMethod !== "undefined" ? Component.GetName('TransferMethod').GetValue() : null;
    parameters_list.docPlaceOfLoading = Component.GetName('PlaceOfLoading') !== null ? Component.GetName('PlaceOfLoading').GetValue() : null;
    parameters_list.docVehicleNumber = Component.GetName('VehicleNumber') !== null ? Component.GetName('VehicleNumber').GetValue() : null;
    parameters_list.docTransferPurpose = Component.GetName('TransferPurpose') !== null ? Component.GetName('TransferPurpose').GetValue() : null;
    parameters_list.RefferenceDate = Component.GetName('RefferenceDate') !== null ? Component.GetName('RefferenceDate').GetValue() : "";
    parameters_list.finalizedDate = Component.GetName('FinalizedDate') !== null ? Component.GetName('FinalizedDate').GetValue() : "";

    parameters_list.billingAddress = Component.GetName('BillingAddress') !== null ? Component.GetName('BillingAddress').GetValue() : (Component.GetName('BillingAddress') !== null ? Component.GetName('BillingAddress').GetValue() : "");

    parameters_list.remarks = Component.GetName('documentcomments') !== null ? Component.GetName('documentcomments').GetValue() : "";
    parameters_list.documentStatus = Component.GetName('Status') !== null ? Component.GetName('Status').GetValue() : "";
    parameters_list.invoicingDate = Component.GetName('InvoicingDate') !== null ? Component.GetName('InvoicingDate').GetValue() : "";

    parameters_list.triangularCustomer = Component.GetName('TriangularCustomer') !== null ? Component.GetName('TriangularCustomer').GetValue() : "";
    parameters_list.triangularSupplier = Component.GetName('TriangularSupplier') !== null ? Component.GetName('TriangularSupplier').GetValue() : "";
    parameters_list.triangularStore = Component.GetName('TriangularStore') !== null ? Component.GetName('TriangularStore').GetValue() : "";
    parameters_list.ChargedToUser = Component.GetName('ChargedToUser') !== null ? Component.GetName('ChargedToUser').GetValue() : "";
    parameters_list.Tablet = Component.GetName('Tablet') !== null ? Component.GetName('Tablet').GetValue() : "";
    return parameters_list;
}


function UpdateDocumentHeaderAndPrint(s, e) {
    selectedItemsArray = [];
    if (ASPxClientEdit.ValidateEditorsInContainerById('docheader-form') === false) {
        return;
    }

    checkPopUpButtonsVisibility(false);
    var parameters = GetDocumentHeaderParameters();
    var path = $('#HOME').val() + 'Document/jsonCheckDocumentUserActions';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'IsNotDocumentOrder': true,
            'FinalizedDate': parameters.finalizedDate,
            'RefferenceDate': parameters.RefferenceDate,
            'BillingAddress': parameters.billingAddress,
            'Status': parameters.documentStatus,
            'Remarks': parameters.remarks,
            'DocumentType': parameters.doctype,
            'DocumentSeries': parameters.doc_series,
            'DeliveryAddress': parameters.deliveryAddress,
            'HasBeenChecked': parameters.HasBeenChecked,
            'HasBeenExecuted': parameters.HasBeenExecuted,
            'DocumentNumber': parameters.docNumber,
            'InvoicingDate': parameters.invoicingDate,
            'TransferMethod': parameters.docTransferMethod,
            'PlaceOfLoading': parameters.docPlaceOfLoading,
            'VehicleNumber': parameters.docVehicleNumber,
            'TransferPurpose': parameters.docTransferPurpose,
            'ExecutionDate': parameters.docExecutionDate,
            'TriangularCustomer': parameters.triangularCustomer,
            'TriangularSupplier': parameters.triangularSupplier,
            'TriangularStore': parameters.triangularStore,
            'ChargedToUser': parameters.ChargedToUser,
            'Tablet': parameters.Tablet
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.document_ok) {
                if (data.numberOfReports > 1) {
                    selectedItemsArray[0] = data.doid;//fix case 5591
                    DialogCallbackPanel.PerformCallback(data.doid);
                }
                else {  //one or no custom reports for this type/user compination
                    link_to = $('#HOME').val() + 'Document/Print?DOid=' + data.doid + '&directPrint=true';
                    LoadEditPopup.Hide();
                    window.open(link_to);
                }
            }
            else if (data.displayTab >= 0 && typeof (documentEditPageControl) !== typeof (undefined) && documentEditPageControl.GetActiveTabIndex() !== data.displayTab) {
                checkPopUpButtonsVisibility(true);
                documentEditPageControl.ChangeActiveTab(data.displayTab);
            }
            else {
                checkPopUpButtonsVisibility(true);
            }
            UpdateTemporaryFilterForm();
        },
        error: function (data) {
            checkPopUpButtonsVisibility(true);
            commonError(data);
            UpdateTemporaryFilterForm();
        }
    });

}

function DialogOKButton_OnClick(s, e) {
    if (DialogCustomReport.GetValue() !== null && DialogCustomReport.GetValue() !== "") {
        PrintFromDialog();
    }
}

function PrintFromDialog() {
    url = $("#HOME").val() + "Reports/DocumentCustomReport?Oid=" + DialogCustomReport.GetValue() + "&ObjectOid=" + selectedItemsArray[0];
    var win = window.open(url, '_blank');
    win.focus();
    if (window.location.href.indexOf("Edit") !== -1) {
        self.close();
    }
    Dialog.Hide();
}

function DialogCloseEvent(s, e) {
    Dialog.Hide();
    if (window.location.href.indexOf("Edit") !== -1) {
        self.close();
    }

}

function CloseDialog(s, e) {
    selectedItemsArray = [];
    Dialog.Hide();
}

function CustomItemDescriptionOnKeyPressed(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (unicode === 13) {
        if (typeof $("#custom_price").val() !== "undefined") {
            custom_price.Focus();
        }
        else {
            spinlineqty.Focus();
        }
    }
}

function Recalculate(s, e) {
    grdEditGrid.PerformCallback("RECALCULATE");
}

function GetItemInfo(param) {
    if (item_info_code.GetValue() === null && param !== null && param === false) {
        return;
    }
    else {
        if (param === true) {
            if (typeof itemPanel !== typeof undefined) {
                itemPanel.PerformCallback("SEARCH");
            }
            else if (typeof itemPanelPurchase !== typeof undefined) {
                if (typeof (item_info_code) !== typeof (undefined)) {
                    itemPanelPurchase.PerformCallback("SEARCH");
                }
            }
            else if (typeof itemPanelStore !== typeof undefined) {
                if (typeof (item_info_code) !== typeof (undefined)) {
                    itemPanelStore.PerformCallback("SEARCH");
                }
            }
        }
        else {
            if (typeof itemPanel !== typeof undefined) {
                itemPanel.PerformCallback();
            }
            else if (typeof itemPanelPurchase !== typeof undefined) {
                if (typeof (item_info_code) !== typeof (undefined)) {
                    itemPanelPurchase.PerformCallback();
                }
            }
            else if (typeof itemPanelStore !== typeof undefined) {
                if (typeof (item_info_code) !== typeof (undefined)) {
                    itemPanelStore.PerformCallback();
                }
            }
        }
    }
}



function SearchByBarcode(s, e) {
    if (e.htmlEvent.keyCode === 13 && barcode_search.GetValue() !== null && typeof barcode_search.GetValue() !== "undefined" && barcode_search.GetValue() !== "") {
        SearchByBarcodeProcedure();
    }
}

function SearchByBarcodeProcedure() {
    var path = $('#HOME').val() + 'Document/jsonSearchByBarcode';
    $.ajax({
        type: 'POST',
        url: path,
        async: false,
        data: {
            'user_search': barcode_search.GetValue(),
            'userDiscount': $('#userDiscount') !== null ? $('#userDiscount').val() : 0,
            'isPercentage': $('#isPercentage') !== null ? $('#isPercentage').val() : 1
        },
        cache: false,
        dataType: 'json',
        success: UpdateItemInfo,
        error: ErrorInItemInfo
    });
}

function SelectByDescription(s, e) {
    var IsInCallback = false;
    if (typeof itemPanel !== typeof undefined) {
        IsInCallback = itemPanel.InCallback();
    }
    else if (typeof itemPanelPurchase !== typeof undefined) {
        IsInCallback = itemPanelPurchase.InCallback();
    }
    else if (typeof itemPanelStore !== typeof undefined) {
        IsInCallback = itemPanelStore.InCallback();
    }
    var unicode;
    if (typeof e.htmlEvent !== "undefined") {
        unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
        if (unicode === 27) {
            e.htmlEvent.preventDefault();
            return;
        }
    }
    if ((typeof e.htmlEvent === "undefined") ||
        (
            (e.htmlEvent.keyCode === 10 || e.htmlEvent.keyCode === 13) &&
            description_search !== null && IsInCallback === false
        )
    ) {
        if (typeof (order_tabs) !== typeof (undefined)) {
            order_tabs.SetActiveTabIndexInternal(0);
        }
        var path = $('#HOME').val() + 'Document/jsonSelectByDescription';
        $.ajax({
            type: 'POST',
            url: path,
            async: false,
            data: {
                'user_search': description_search.GetValue(),
                'userDiscount': $('#userDiscount') !== null ? $('#userDiscount').val() : 0,
                'isPercentage': $('#isPercentage') !== null ? $('#isPercentage').val() : 1
            },
            cache: false,
            dataType: 'json',
            success: UpdateItemInfo,
            error: ErrorInItemInfo
        });
    }
}

function UpdateItemInfo(data) {
    if (data.item === "") {
        ClearDocumentLine();
        if (Component.GetName('itemPanel') !== null) {
            barcode_search.Focus();

        }
        if (Component.GetName('itemPanelPurchase') !== null) {
            barcode_search.Focus();
        }

        barcode_search.Focus();

    } else {
        if (data.existing_item_qty > 0) {
            $("#existing_item_qty").text(data.existing_item_qty);
        }
        if (typeof (order_tabs) !== typeof (undefined)) {
            order_tabs.SetActiveTabIndexInternal(0);
        }
        spinlineqty.SetValue(data.qty);
        barcode_search.SetText('');
        description_search.SetText('');
        GetItemInfo(true);
        spinlineqty.Focus();
    }
}

function OrderTabsCallbackPanelEndCallback(s, e) {
    if (typeof (grdEditGrid) === typeof (undefined) && typeof (documentEditPageControl) !== typeof (undefined) && DocumentEditTabControl.DocumentTabCanChange()
    ) {
        documentEditPageControl.SetActiveTab(documentEditPageControl.GetTabByName('PaymentMethods'));
    }
}
