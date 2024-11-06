var SomeValueHasChanged = false;
var previous_values = [];
var doc_det_current_index = null;
var detail_form_mode = null;
var VatRecalcCallback = false;
var grdEditCallback = false;

$(document).ready(function () {

    if (typeof (barcode_search) != "undefined" && barcode_search.GetMainElement() !== null) {
        barcode_search.Focus();
    }
    else if (typeof (spinlineqty) != "undefined" && spinlineqty.GetMainElement() !== null) {
        spinlineqty.Focus();
        spinlineqty.SelectAll();
    }
    addedItem = false;

    if ($('.Document').length) {
        $('body').addClass('Document');
    }

    scrollbar();

    $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left .changeMode1').click(function () {
        var that = $(this);
        var other = $(this).parent().find('.changeMode2');
        var barcode = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left #document_details_item_description_search');
        var description = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left #document_details_barcode_search');
        changeMode(that, other, barcode, description);
    });

    $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left .changeMode2').click(function () {
        var that = $(this);
        var other = $(this).parent().find('.changeMode1');
        var barcode = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left #document_details_item_description_search');
        var description = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #left #document_details_barcode_search');
        changeMode(that, other, description, barcode);
    });

    $('.wrapper .container .Document #documentEditPageControl .dxtc-content .changeMode1').click(function () {
        var that = $(this);
        var other = $(this).parent().find('.changeMode2');
        var barcode = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #description_search');
        var description = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #barcode_search');
        changeMode(that, other, barcode, description);
    });

    $('.wrapper .container .Document #documentEditPageControl .dxtc-content .changeMode2').click(function () {
        var that = $(this);
        var other = $(this).parent().find('.changeMode1');
        var barcode = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #description_search');
        var description = $('.wrapper .container .Document #documentEditPageControl .dxtc-content #barcode_search');
        changeMode(that, other, description, barcode);
    });
});

function scrollbar() {
    $('#DocumentDetails').customScrollbar();
    ResetFocus();
}

function changeMode(that, other, down, up) {
    that.addClass('color-blue').css('color', 'white');
    other.removeClass('color-blue').css('color', '#666666');
    down.addClass('zIndex');
    up.removeClass('zIndex');
}

function DocumentStatusChanged(s, e) {
    var path = $('#HOME').val() + 'Document/jsonDocumentStatusChanged';
    if (Component.GetName('Status') !== null && Component.GetName('Status') !== undefined) {
        docStatus = Component.GetName('Status').GetValue().toString();
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'DocumentStatus': docStatus },
            cache: false,
            dataType: 'json',
            success: function (data) {
                if (data.AutoDocNumber) {
                    if (typeof (DocNumber) != "undefined") {
                        Component.GetName('DocNumber').SetText("0");
                        Component.GetName('DocNumber').SetValue("0");
                        Component.GetName('DocNumber').SetEnabled(false);
                    }
                }
                else {
                    if (typeof (DocNumber) != "undefined") {
                        Component.GetName('DocNumber').SetEnabled(true);
                    }
                }
            },
            error: commonError
        });
    }


}

function TabletValueChanged(s, e) {
    var path = $('#HOME').val() + 'Document/jsonTabletValueChanged';
    if (Component.GetName('Tablet') !== null && Component.GetName('Tablet') !== undefined) {
        tablet = Component.GetName('Tablet').GetValue().toString();
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'Tablet': tablet },
            cache: false,
            dataType: 'json',
            success: function (data) {
            },
            error: commonError
        });
    }

}



function itemPanelPurchaseBeginCallback(s, e) {
    if ($('#DocumentDetailFormMode') !== null && typeof $('#DocumentDetailFormMode') !== "undefined") {
        e.customArgs.DocumentDetailFormMode = $('#DocumentDetailFormMode').val();
    }

    if (typeof (doc_det_current_index) !== "undefined") {
        if (doc_det_current_index < grdEditGrid.pageRowCount - 1) {
            e.customArgs.nextOid = grdEditGrid.keys[doc_det_current_index + 1];
        }
        if (doc_det_current_index > 0) {
            e.customArgs.previousOid = grdEditGrid.keys[doc_det_current_index - 1];
        }
    }
    var checkbox = Component.GetName('checkNewItem');
    if (checkbox !== null) {
        e.customArgs.check_box_state = checkbox.GetChecked();
    }
}

function itemPanelPurchaseEndCallback(s, e) {
    if (Component.GetName('spinlineqty') !== null && $('#hidden_quantity').val() !== null && typeof $('#hidden_quantity').val() !== "undefined") {
        Component.GetName('spinlineqty').SetValue($('#hidden_quantity').val());
    }

    if (typeof (documentSummaryPanel) != "undefined") {
        documentSummaryPanel.PerformCallback();
    }

    if (typeof (vatAnalysisInfoPanel) != "undefined") {
        vatAnalysisInfoPanel.PerformCallback();
    }

    if (typeof (documentSummaryPanelPartial) != typeof (undefined)) {
        documentSummaryPanelPartial.PerformCallback();
    }

    spinlineqty.Focus();
}

function ResetFocus() {
    if (Component.GetName('item_info_code') !== null && Component.GetName('barcode_search') !== null) {
        if (typeof (item_info_code) !== "undefined" && item_info_code.IsVisible()) {
            val = Component.GetName('item_info_code').GetValue();
            if (val === "" || val === null || typeof val === "undefined") {
                barcode_search.Focus();
            }
            else {
                spinlineqty.Focus();
            }
        }
    }
    if (Component.GetName('grdEditGrid') !== null && grdEditGrid.keys.length > 0 && VatRecalcCallback) {
        VatRecalcCallback = false;
        grdEditGrid.PerformCallback();
    }

}

function CancelOrder(s, e) {
    selectedItemsArray = [];
    s.SetEnabled(false);
    var path = $('#HOME').val() + 'Document/jsonCancelOrder';
    $.ajax({
        type: 'POST',
        url: path,
        data: {},
        cache: false,
        dataType: 'json',
        success: function (data) {
            LoadEditPopup.Hide();
            UpdateTemporaryFilterForm();
        },
        error: function (data) {
            commonError(data, btnCancel);
            LoadEditPopup.Hide();
            UpdateTemporaryFilterForm();
        }
    });
}

function GetItemInfoOnEnterPressed(s, e) {
    var inputs_with_decimal_points = ['spinlineqty', 'custom_price', 'userDiscountPercentage', 'userDiscountMoneyValue'];
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (unicode == 13) {
        PostDocumentDetailForm(s.uniqueID);
    }
}

function GetDocumentFormData() {
    var form_data = {};
    var elementsToCheckForAlteredValues = ['fpa_factor', 'total_vat_amount', 'net_total'];
    $.each(ASPxClientControl.GetControlCollection().elements, function (index, value) {
        if (typeof value.GetValue === "undefined" || value.IsVisible() === false) {
        }
        else {
            form_data[value.name] = value.GetValue();
        }
    });
    return form_data;
}

function PostDocumentDetailForm(trigger, retain_focus) {

    form_data = GetDocumentFormData();
    form_data.triggered_by = trigger;
    form_data.retain_focus = retain_focus;
    if (Component.GetName('checkNewItem') !== null) {
        form_data.check_box_state = Component.GetName('checkNewItem').GetChecked();
    }

    if (trigger == 'userDiscountPercentage') {
        $("#userDiscount").val(userDiscountPercentage.GetValue());
        form_data.userDiscount = userDiscountPercentage.GetValue();
        $("#isPercentage").val(true);
    }
    else if (trigger == 'userDiscountMoneyValue') {
        $("#userDiscount").val(userDiscountMoneyValue.GetValue());
        form_data.userDiscount = userDiscountMoneyValue.GetValue();
        $("#isPercentage").val(false);
    }
    else {
        if (typeof ($("#isPercentage").val()) != typeof (undefined) && typeof ($("#userDiscount").val()) != typeof (undefined)) {
            if ($("#isPercentage").val() == "true") {
                $("#userDiscount").val(userDiscountPercentage.GetValue());
                form_data.userDiscount = userDiscountPercentage.GetValue();
            }
            else {
                $("#userDiscount").val(userDiscountMoneyValue.GetValue());
                form_data.userDiscount = userDiscountMoneyValue.GetValue();
            }
        }
    }
    if (typeof (isPriceCatalogPercentage) !== "undefined") {
        if (trigger == 'price_catalog_discount_percentage') {
            $("#userPriceCatalogDiscount").val(price_catalog_discount_percentage.GetValue());
            form_data.userPriceCatalogDiscount = price_catalog_discount_percentage.GetValue();
            $("#isPriceCatalogPercentage").val(true);
        }
        else if (trigger == 'price_catalog_discount_value') {
            $("#userPriceCatalogDiscount").val(price_catalog_discount_value.GetValue());
            form_data.userPriceCatalogDiscount = price_catalog_discount_value.GetValue();
            $("#isPriceCatalogPercentage").val(false);
        }
        else {
            if ($("#isPriceCatalogPercentage").val() == "true") {
                $("#userPriceCatalogDiscount").val(price_catalog_discount_percentage.GetValue());
                form_data.userPriceCatalogDiscount = price_catalog_discount_percentage.GetValue();
            } else {
                $("#userPriceCatalogDiscount").val(price_catalog_discount_value.GetValue());
                form_data.userPriceCatalogDiscount = price_catalog_discount_value.GetValue();
            }
        }
    }

    form_data.isPercentage = $("#isPercentage").val();
    form_data.isPriceCatalogPercentage = $("#isPriceCatalogPercentage").val();
    form_data.DocumentDetailFormMode = $("#DocumentDetailFormMode").val();


    var inputs_to_multiple = ['qty_spin_edit', 'spinlineqty', 'custom_price', 'userDiscount', 'net_total', 'total_vat_amount', 'userPriceCatalogDiscount'];
    for (var property in inputs_to_multiple) {
        var current_index = inputs_to_multiple[property];
        form_data[current_index] = Math.round(form_data[current_index] * QUANTITY_MULTIPLIER);
    }



    var path = $('#HOME').val() + 'Document/ProcessDocumentDetail';
    if (typeof (itemPanel) != typeof (undefined) && typeof (item_info_code) != typeof (undefined)) {
        path = $('#HOME').val() + 'Document/ProcessDocumentDetail';
    }
    else if (typeof (itemPanelPurchase) != typeof (undefined) && typeof (item_info_code) != typeof (undefined)) {
        path = $('#HOME').val() + 'Document/ProcessDocumentDetailPurchase';
    }
    else if (typeof (itemPanelStore) != typeof (undefined) && typeof (item_info_code) != typeof (undefined)) {
        path = $('#HOME').val() + 'Document/ProcessDocumentDetailStore';
    }

    elementsToBeUpdatedByForce = ["final_sum"];

    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'form_data': form_data
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.result !== null && typeof data.result !== "undefined") {
                $.each(data.result, function (index, value) {
                    passByForce = elementsToBeUpdatedByForce.indexOf(index) >= 0;
                    if (passByForce ||
                        (Component.GetName(index) !== null &&
                            typeof Component.GetName(index) !== "undefined" &&
                            Component.GetName(index).inputElement !== null &&
                            typeof Component.GetName(index).inputElement !== "undefined")
                    ) {
                        if (passByForce) {
                            if (Component.GetName(index) !== null) {
                                Component.GetName(index).SetValue(value);
                            }
                        }
                        else if (Component.GetName(index).inputElement.defaultValue == Component.GetName(index).inputElement.value) {
                            Component.GetName(index).SetValue(value);
                            Component.GetName(index).inputElement.defaultValue = Component.GetName(index).inputElement.value;
                        }
                        else {
                            Component.GetName(index).SetValue(value);
                        }
                    }
                    index = data.result.focus_on;
                    if (Component.GetName(index) !== null && typeof Component.GetName(index) !== "undefined") {
                        Component.GetName(index).Focus();
                    }
                });

                if (data.result.DetailIsForSave) {
                    var checkbox = Component.GetName('checkNewItem'),
                        path,
                        callbackparameter;

                    if ($('#DocumentDetailFormMode').val() == "Add") {
                        path = $('#HOME').val() + 'Document/AddItem';
                        callbackparameter = "SAVE";
                    }
                    else if ($('#DocumentDetailFormMode').val() == "Edit") {
                        path = $('#HOME').val() + 'Document/UpdateItem';
                        callbackparameter = "UPDATE";
                    }
                    $.ajax({
                        type: 'POST',
                        url: path,
                        async: false,
                        data: { 'check_box_state': checkbox.GetChecked() },
                        cache: false,
                        dataType: 'json',
                        success: function (data) {
                            if (typeof (data.error) !== "undefined") {
                                setJSError(data.error);
                            }
                            else {
                                if (item_info_code.GetValue() === null) {
                                    setJSNotification(pleaseSelectAnObject);
                                }
                                else {
                                    if (typeof (itemPanel) !== "undefined") {
                                        itemPanel.PerformCallback({ updateMode: callbackparameter });
                                    }
                                    else if (typeof (itemPanelPurchase) !== "undefined") {
                                        itemPanelPurchase.PerformCallback({ updateMode: callbackparameter });
                                    }
                                    else if (typeof (itemPanelStore) !== "undefined") {
                                        itemPanelStore.PerformCallback({ updateMode: callbackparameter });
                                    }
                                }
                                SomeValueHasChanged = false;
                                itemPanelInitialization();
                                if (name == "btnSaveCloseItem") {
                                    grdEditGrid.CancelEdit();
                                    ShowDocumentHeader();
                                }
                            }
                        },
                        error: commonError
                    });
                }
            }

        },
        error: commonError
    });
}

function RecalculateDocumentDetail(s, e) {
    PostDocumentDetailForm(s.uniqueID);
}

function UpdateDocumentHeader(s, e) {
    selectedItemsArray = [];
    if (ASPxClientEdit.ValidateEditorsInContainerById('docheader-form') === false) { return; }

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
                LoadEditPopup.Hide();
            }
            else if (typeof (documentEditPageControl) != typeof (undefined) && data.displayTab >= 0 && documentEditPageControl.GetActiveTabIndex() != data.displayTab) {
                checkPopUpButtonsVisibility(true);
                documentEditPageControl.ChangeActiveTab(data.displayTab);
            }
            else {
                checkPopUpButtonsVisibility(true);
                btnUpdate.SetEnabled(true);
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

function SaveOrderAndRecalculateCosts(s, e) {
    selectedItemsArray = [];
    checkPopUpButtonsVisibility(false);

    var path = $('#HOME').val() + 'Document/jsonCheckDocumentUserActions';
    var parameters = GetDocumentHeaderParameters();

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
            'ChargedToUser': parameters.ChargedToUser,
            "Tablet": Tablet
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.document_ok) {
                if (data.displayMarkUpForm) {
                    LoadEditPopup.Hide();
                    MarkUpPopUp.PerformCallback({ DOids: data.doid });
                    MarkUpPopUp.Show();
                }
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

function StartAddItem(s, e) {
    var existingPanelsInCallback = false;
    var storeTraders;
    if (typeof storeSuppliers !== typeof undefined) {
        storeTraders = storeSuppliers;
        if (storeSuppliers.InCallback()) {
            existingPanelsInCallback = true;
        }
    }
    else if (typeof storeCustomers !== typeof undefined) {
        storeTraders = storeCustomers;
        if (storeCustomers.InCallback()) {
            existingPanelsInCallback = true;
        }
    }

    if (
        grdEditGrid.InCallback() ||
        (typeof BillingAddressCombobox !== "undefined" && BillingAddressCombobox.InCallback()) ||
        (typeof PriceCatalogPolicyCb !== "undefined" && PriceCatalogPolicyCb.InCallback())
    ) {
        existingPanelsInCallback = true;
    }
    if ((typeof (secondaryStores) !== typeof (undefined) && Component.GetName('secondaryStores') !== null && Component.GetName('secondaryStores').GetValue() === null)
        || (typeof (storeTraders) !== typeof (undefined) && storeTraders.GetValue() === null)
        || Component.GetName('DocumentType').GetValue() === null || Component.GetName('DocumentSeries').GetValue() === null
        || (typeof (PriceCatalogPolicyCb) !== typeof (undefined) && Component.GetName('PriceCatalogPolicyCb').GetValue() === null)
    ) {
        setJSError(PleaseFillInDocumentHeaderInfo);
    }
    else {
        if (existingPanelsInCallback === false) {
            grdEditGrid.AddNewRow();
        }
    }
}

function AddMoreOrderItems() {
    PrepareDocumentHeaderValues("");

    $.ajax({
        type: 'POST',
        url: $('#HOME').val() + 'Document/jsonCheckDocumentLinesCount',
        success: function (data) {
            if (data.result == "") {
                OrderItemsPopUp.PerformCallback();
                OrderItemsPopUp.Show();
            }
            else {
                setJSError(data.result);
            }
        },
        error: function (data) {
            commonError(data);
        }
    });
}

function DocumentEditGridBeginCallback(s, e) {
    if (e.command == "STARTEDIT") {
        if (doc_det_current_index === null) {
            doc_det_current_index = s.lastMultiSelectIndex;
        }
        if (doc_det_current_index < grdEditGrid.pageRowCount - 1) {
            e.customArgs.nextOid = grdEditGrid.keys[doc_det_current_index + 1];
        }
        if (doc_det_current_index > 0) {
            e.customArgs.previousOid = grdEditGrid.keys[doc_det_current_index - 1];
        }
        var checkbox = Component.GetName('checkNewItem');
        if (checkbox !== null) {
            e.customArgs.check_box_state = checkbox.previousValue;
        }
    }
    else if (e.command == "CANCELEDIT") {
        doc_det_current_index = null;
    }
}

function DocumentEditGridEndCallback(s, e) {
    CustomizationWindow(s, e);
    click_fun = "UpdateItemSingleForm();";
    $(".wrapper .Document .inserted_btn").append("<div id=\"qty_sv_btn\" class=\"inside_btn\" onclick=\"" + click_fun + "\">OK</div>");

    if (typeof (customerInfoPanel) != typeof (undefined)) {
        customerInfoPanel.PerformCallback();
    }
    if (typeof (supplierInfoPanel) != typeof (undefined)) {
        supplierInfoPanel.PerformCallback();
    }
    if (typeof (vatAnalysisInfoPanel) != typeof (undefined)) {
        vatAnalysisInfoPanel.PerformCallback();
    }
    if (typeof (documentSummaryPanel) != typeof (undefined)) {
        documentSummaryPanel.PerformCallback();
    }
    if (typeof (documentSummaryPanelPartial) != typeof (undefined)) {
        documentSummaryPanelPartial.PerformCallback();
    }

}

function UpdateItemSingleForm() {
    PostDocumentDetailForm('btnRecalculateItem');
    grdEditGrid.CancelEdit();
}

function PrepareDocumentHeaderValues(form_id) {
    var formID;
    if (form_id === null || form_id === "") {
        formID = "";
    } else {
        formID = form_id;
    }
    console.log(formID);
    if (Component.GetName('document.ExecutionDate') !== null) {
        document.getElementById("docExecutionDate" + formID).value = Component.GetName('document.ExecutionDate').GetValue();
    }

    if (Component.GetName('DocumentType') !== null) {
        document.getElementById("docType" + formID).value = Component.GetName('DocumentType').GetValue();
    }
    if (Component.GetName('DocumentSeries') !== null) {
        document.getElementById("docSeries" + formID).value = Component.GetName('DocumentSeries').GetValue();
    }

    if (Component.GetName('DocNumber') !== null && typeof Component.GetName('DocNumber') !== "undefined") {
        document.getElementById("docNumber" + formID).value = Component.GetName('DocNumber').GetValue();
    }

    if (Component.GetName('FinalizedDate') !== null) {
        document.getElementById("docFinDate" + formID).value = Component.GetName('FinalizedDate').GetText();
    }
    if (Component.GetName('Status!Key') !== null) {
        document.getElementById("docStatus" + formID).value = Component.GetName('Status!Key').GetValue();
    }
    if (Component.GetName('Tablet!Key') !== null) {
        document.getElementById("Tablet" + formID).value = Component.GetName('Tablet!Key').GetValue();
    }
    console.log(Component.GetName('Tablet!Key').GetValue())

    if (Component.GetName('HasBeenChecked') !== null && typeof Component.GetName('HasBeenChecked') !== "undefined") {
        document.getElementById("docChecked" + formID).value = Component.GetName('HasBeenChecked').GetValue();
    }
    if (Component.GetName('HasBeenExecuted') !== null && typeof Component.GetName('HasBeenExecuted') !== "undefined") {
        document.getElementById("docExecuted" + formID).value = Component.GetName('HasBeenExecuted').GetValue();
    }

    if (Component.GetName('InvoicingDate') !== null && typeof Component.GetName('InvoicingDate') !== "undefined") {
        document.getElementById("docInvoiceDate" + formID).value = Component.GetName('InvoicingDate').GetText();
    }

    document.getElementById("docRemarks" + formID).value =
        Component.GetName('documentcomments') === null ||
            Component.GetName('documentcomments').GetValue() === null ? '' :
            Component.GetName('documentcomments').GetValue();

    if (Component.GetName('DeliveryAddress') !== null && Component.GetName('DeliveryAddress').GetVisible() === true) {
        document.getElementById("docDelAddr" + formID).value = Component.GetName('DeliveryAddress').GetValue();
    }


    document.getElementById("docTransferMethod" + formID).value =
        Component.GetName('TransferMethod') === null ||
            Component.GetName('TransferMethod').GetValue() === null ? '' :
            Component.GetName('TransferMethod').GetValue();

    document.getElementById("docPlaceOfLoading" + formID).value =
        Component.GetName('PlaceOfLoading') === null ||
            Component.GetName('PlaceOfLoading').GetValue() === null ? '' :
            Component.GetName('PlaceOfLoading').GetValue();

    document.getElementById("docVehicleNumber" + formID).value =
        Component.GetName('VehicleNumber') === null ||
            Component.GetName('VehicleNumber').GetValue() === null ? '' :
            Component.GetName('VehicleNumber').GetValue();

    document.getElementById("docTransferPurpose" + formID).value =
        Component.GetName('TransferPurpose') === null ||
            Component.GetName('TransferPurpose').GetValue() === null ? '' :
            Component.GetName('TransferPurpose').GetValue();


}

function UpdateItem(s, e) {
    if (e.type == "keypress") {
        name = s;
    }
    else {
        name = s.name;
    }
    PostDocumentDetailForm(name);
}

function ChangeItem(s, e, oid) {
    doc_det_current_index = grdEditGrid.keys.indexOf(oid);
    var inputs_to_check = ['spinlineqty', 'userDiscountMoneyValue', 'userDiscountPercentage'];
    var index = 0;
    while ((index < inputs_to_check.length) && SomeValueHasChanged === false) {
        var v = Component.GetName(inputs_to_check[index]);
        if (v.number != previous_values[v.name]) {
            SomeValueHasChanged = true;
        }
        index++;
    }
    if (SomeValueHasChanged === true) {
        if (confirm(closePageConfirmMessage)) {
            if (oid !== null) {
                grdEditGrid.StartEditRowByKey(oid);
            }
            else {
                alert(noRowFound);
            }

        }
    }
    else {
        if (oid !== null) {
            grdEditGrid.StartEditRowByKey(oid);
        }
        else {
            alert(noRowFound);
        }
    }
}

function CustomPriceOnLostFocus(s, e) {
    val = custom_price.GetValue();
    if (val !== "" && val !== null && typeof val !== "undefined") {
        val = val.replace('.', ',');
        itemPanelCallbackCommand = 'CUSTOM_PRICE';
        itemPanel.PerformCallback('CUSTOM_PRICE');
    } else {
        val = "-";
        custom_price.Focus();
        final_sum.SetText(val + '€');
    }
}

function OnPaymentMethodChanged(s, e) {
    var path = $('#HOME').val() + 'Document/jsonPaymentMethodChanged';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'paymentMethodOid': Component.GetName('PaymentMethod!Key').GetValue()
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            DocumentPaymentMethodPanel.PerformCallback();
        },
        error: commonError
    });
}

function DocumentTabChanging(s, e) {

    DocumentEditTabControl.DocumentTabCanChange(s, e);
    var requiredFields = ['DocumentType', 'DocumentSeries', 'storeSuppliers', 'storeCustomers', 'secondaryStores'];
    for (var field in requiredFields) {
        var fld = Component.GetName(requiredFields[field]);
        if (fld !== null) {
            if ($.trim(fld.GetValue()) === "") {
                setJSError(PleaseFillInDocumentHeaderInfo);
                e.cancel = true;
                return;
            }
        }

    }
}

function SelectTriangularAddress(s, e) {
    var objectOid;
    if (typeof (TriangularCustomer) !== "undefined" && TriangularCustomer.GetValue() !== null) {
        objectOid = TriangularCustomer.GetValue();
    }
    else if (typeof (TriangularSupplier) !== "undefined" && TriangularSupplier.GetValue() !== null) {
        objectOid = TriangularSupplier.GetValue();
    }
    else if (typeof (TriangularStore) !== "undefined" && TriangularStore.GetValue() !== null) {
        objectOid = TriangularStore.GetValue();

    }
    else {
        setJSError(PleaseSelectTriangular);
        return;
    }
    var args = ['DisplayTriangularAddresses', objectOid];
    DialogCallbackPanel.PerformCallback(args);
}

function SelectDeliveryAddress(s, e) {
    var objectOid;
    if (typeof (storeCustomers) !== "undefined" && storeCustomers.GetValue() !== null) {
        objectOid = storeCustomers.GetValue();
    }
    else if (typeof (storeSuppliers) !== "undefined" && storeSuppliers.GetValue() !== null) {
        objectOid = storeSuppliers.GetValue();
    }
    else {
        setJSError(PleaseSelectACustomer);
        return;
    }
    var args = ['DisplayCustomerAddresses', objectOid];
    DialogCallbackPanel.PerformCallback(args);
}

function SelectExistingAddress(addressOid) {
    var attrs = ['Street', 'POBox', 'PostCode', 'City', 'Profession'];
    for (var attrr in attrs) {
        var current_index = attrs[attrr];
        Component.GetName(current_index).SetValue($('#' + current_index + '_' + addressOid).html());
    }
}

function SetAddressDialog(s, e, dialog) {
    if (ValidateModalFormSingle()) {

        var attrs = ['Street', 'POBox', 'PostCode', 'City'];
        var final_address = '';
        var profession = Component.GetName('Profession').GetValue();

        for (var attrr in attrs) {
            var current_index = attrs[attrr];
            var current_val = Component.GetName(current_index).GetValue();
            if (current_val !== null) {
                final_address += ' ' + current_val;
            }
        }

        var path = $('#HOME').val() + 'Document/Set' + dialog + 'Address';

        $.ajax({
            type: 'POST',
            url: path,
            data: {
                'address': final_address,
                'profession': profession
            },
            cache: false,
            dataType: 'json',
            success: function (data) {

                Component.GetName(dialog + 'Address').SetValue(data.address);
                Component.GetName(dialog + 'Profession').SetValue(data.profession);
                attrs.push('Profession');
                for (var attrr in attrs) {
                    var current_index = attrs[attrr];
                    $("#" + dialog + current_index).val(Component.GetName(current_index).GetValue());
                }

                Dialog.Hide();
            },
            error: commonError
        });
    }
    else {
        setJSError(markedFieldsAreRequired);
        return false;
    }
}

function InitAddressDialog(s, e, dialog) {

    var attrs = ['Street', 'POBox', 'PostCode', 'City', 'Profession'];
    for (var attrr in attrs) {
        var current_index = attrs[attrr];
        Component.GetName(current_index).SetValue($("#" + dialog + current_index).val());
    }
}

function CancelItem(s, e) {
    if (typeof (itemPanel) != typeof undefined) {
        itemPanel.PerformCallback("CLEAN");
    }
    else if (typeof (itemPanelPurchase) != typeof undefined) {
        itemPanelPurchase.PerformCallback("CLEAN");
    }
    else if (typeof (itemPanelStore) != typeof undefined) {
        itemPanelStore.PerformCallback("CLEAN");
    }
    grdEditGrid.CancelEdit();
    ShowDocumentHeader();
}

function RefreshUserHiddenInputs(s, e) {
    if (s.uniqueID == 'userDiscountPercentage') {
        $("#userDiscount").val(userDiscountPercentage.GetValue());
        $("#isPercentage").val(true);
    }
    else if (s.uniqueID == 'userDiscountMoneyValue') {
        $("#userDiscount").val(userDiscountMoneyValue.GetValue());
        $("#isPercentage").val(false);
    }
    else if (s.uniqueID == 'price_catalog_discount_percentage') {
        $("#userPriceCatalogDiscount").val(price_catalog_discount_percentage.GetValue());
        $("#isPriceCatalogPercentage").val(true);
    }
    else if (s.uniqueID == 'price_catalog_discount_value') {
        $("#userPriceCatalogDiscount").val(price_catalog_discount_value.GetValue());
        $("#isPriceCatalogPercentage").val(false);
    }
}

function BillingAddressValueChanged(s, e) {
    var path = $('#HOME').val() + 'Document/BillingAddressValueChanged';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'address': Component.GetName('BillingAddress!Key').GetValue()
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (Component.GetName('grdEditGrid') !== null && grdEditGrid.keys.length > 0) {
                grdEditGrid.PerformCallback();
            }
        },
        error: commonError
    });
}

function ComboboxValueChanged(s, e) {
    PostDocumentDetailForm(s.uniqueID, true);
}

function SetCustomerOnDocumentHiddenValues(s, e) {
    SetDocumentHiddenValues(storeCustomers);
}

function SetSupplierOnDocumentHiddenValues(s, e) {
    SetDocumentHiddenValues(storeSuppliers);
}

function SetPriceCatalogOnDocumentHiddenValues(s, e) {
    SetDocumentHiddenValues(storePriceCatalogs);
}

function SetPriceCatalogPolicyOnDocumentHiddenValues(s, e) {
    SetDocumentHiddenValues(PriceCatalogPolicyCb);
}


function SetDocumentHiddenValues(param) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
    else if (selectedItemsArray.length == 1) {
        param.SetValue(selectedItemsArray[0]);
        UpdateDocHeader(param, null);
        selectedItemsArray = [];
        Dialog.Hide();
    }
    else {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
}

function SetSupplier(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
    else if (selectedItemsArray.length === 1) {
        DocumentSupplierCbPanel.PerformCallback(selectedItemsArray[0]);
        selectedItemsArray = [];
        Dialog.Hide();
    }
    else {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
}

function SetPriceCatalog(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
    else if (selectedItemsArray.length === 1) {
        DocumentPriceCatalogCbPanel.PerformCallback(selectedItemsArray[0]);
        selectedItemsArray = [];
        Dialog.Hide();
    }
    else {
        setJSNotification(pleaseSelectOnlyOneObject);
    }
}

function vatAnalysisInfoPanelBeginCallback(s, e) {

    var vatInputs = {};
    $.each($('.vatFactorInput'), function (index, value) {
        vatInputs[value.id.replace('vatFactor', '')] = Component.GetName(value.id).GetValue();
    });
    e.customArgs.vatInputs = JSON.stringify(vatInputs);

    var netTotalInputs = {};
    $.each($('.netTotalInput'), function (index, value) {
        netTotalInputs[value.id.replace('netTotal', '')] = Component.GetName(value.id).GetValue();
    });
    e.customArgs.netTotalInputs = JSON.stringify(netTotalInputs);
}

function RecalculateVatFactorDeviations(s, e) {
    VatRecalcCallback = true;
    vatAnalysisInfoPanel.PerformCallback("RECALCULATE");
    documentSummaryPanel.PerformCallback();

    documentSummaryPanelPartial.PerformCallback();

}

function grdEditGridColumnMoving(s, e) {
    if (e.destinationColumn === null) {
        var alwaysVisibleColumns = ["Item.Code", "CustomDescription", "Qty"];
        if (alwaysVisibleColumns.indexOf(e.sourceColumn.fieldName) >= 0) {
            e.allow = false;
            setJSNotification(columnCannotBeHidden);
        }
    }
}

function clearCustomerComboBox(s, e) {
    if (e.buttonIndex == 1) {
        DialogCallbackPanel.PerformCallback(s.name);
    }
    else {
        //in case you add another button
    }
}

function clearSupplierComboBox(s, e) {
    if (e.buttonIndex === 0) {
        clearComboBox(s, e);
        UpdateDocHeader(s, e);
    } else if (e.buttonIndex == 1) {
        DialogCallbackPanel.PerformCallback(s.name);
    }
}

function itemPanelInitialization(s, e) {
    SomeValueHasChanged = false;
    var inputs_to_get_value = ['spinlineqty', 'userDiscountMoneyValue', 'userDiscountPercentage'];
    for (var index in inputs_to_get_value) {
        var v = Component.GetName(inputs_to_get_value[index]);
        previous_values[inputs_to_get_value[index]] = v.GetValue();
    }
    checkNewItem.SetChecked(true);
    HideDocumentHeader();
}

function UpdateDocHeader(s, e) {
    grdEditCallback = !(s.uniqueID == "HasBeenExecuted" || s.uniqueID == "Status" || s.uniqueID == "secondaryStores");
    $("#field_name").val(s.uniqueID);
    $("#field_value").val(Component.GetName(s.uniqueID).GetValue());
    $("#docheader-form").submit();
}

function UserValidatePop() {

    UserValidatePopUp.Show();
}

function OnDocHeaderUpdated() {
    if (grdEditCallback) {
        OrderTabsCallbackPanel.PerformCallback();
    }
    Component.PositionDocumentPartialAtHeader();
}

function AddRemainingPaymentAmountClick(s, e) {
    if (Component.GetName("PaymentMethod!Key") === null) {
        return;
    }
    if (Component.GetName("PaymentMethod!Key").GetValue() === null) {
        Component.GetName("PaymentMethod!Key").Validate();
        return;
    }
    var path = $('#HOME').val() + 'Document/AddRemainingPaymentAmount';
    $.ajax({
        type: 'POST',
        url: path,
        data: {},
        cache: false,
        dataType: 'json',
        success: function (data) {
            Amount.SetValue(data.remainingPaymentAmount);
        },
        error: commonError
    });
}

function itemPanelStoreBeginCallback(s, e) {
    if ($('#DocumentDetailFormMode') !== null && typeof $('#DocumentDetailFormMode') !== "undefined") {
        e.customArgs.DocumentDetailFormMode = $('#DocumentDetailFormMode').val();
    }

    if (typeof (doc_det_current_index) !== "undefined") {
        if (doc_det_current_index < grdEditGrid.pageRowCount - 1) {
            e.customArgs.nextOid = grdEditGrid.keys[doc_det_current_index + 1];
        }
        if (doc_det_current_index > 0) {
            e.customArgs.previousOid = grdEditGrid.keys[doc_det_current_index - 1];
        }
    }
    var checkbox = Component.GetName('checkNewItem');
    if (checkbox !== null) {
        e.customArgs.check_box_state = checkbox.GetChecked();
    }
}




function itemPanelStoreEndCallback(s, e) {
    if (Component.GetName('spinlineqty') !== null && $('#hidden_quantity').val() !== null && typeof $('#hidden_quantity').val() !== "undefined") {
        Component.GetName('spinlineqty').SetValue($('#hidden_quantity').val());
    }

    if (typeof (documentSummaryPanel) !== "undefined") {
        documentSummaryPanel.PerformCallback();
    }

    if (typeof (documentSummaryPanelPartial) != typeof (undefined)) {
        documentSummaryPanelPartial.PerformCallback();
    }

    if (typeof (vatAnalysisInfoPanel) !== "undefined") {
        vatAnalysisInfoPanel.PerformCallback();
    }

    spinlineqty.Focus();
}

function NotAvailableItemUnchecked(s, e) {
    var path = $('#HOME').val() + 'Document/RemoveUnavailableItem';
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'itemOid': s.name },
        cache: false,
        dataType: 'json',
        success: function (data) {
            grdEditGrid.PerformCallback();
        },
        error: commonError
    });
}

function UpdateTriangularAddress(s, e) {
    var path = $('#HOME').val() + 'Document/jsonUpdateTriangularAddress';
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'senderName': s.name, 'senderOid': s.lastSuccessValue },
        cache: false,
        dataType: 'json',
        success: function (data) {
            Component.GetName('TriangularAddress').SetValue(data.triangularAddress);
        },
        error: commonError
    });
}

function clearTriangularComboBox(s, e) {
    clearComboBox(s, e);
    UpdateTriangularAddress(s, e);
}

var CompositionDecomposition;

CompositionDecomposition = (function () {

    var mainLineCommand = '';
    var linkedLineCommand = '';

    UpdateCompositionDecompositionItemInfo = function (data) {
        if (data.item === "") {
            ClearDocumentLine();
            description_search.Focus();
        }
        else {
            Qty.SetValue(1);
            barcode_search.SetText('');
            description_search.SetText('');

            if (typeof (data.item_info_name) != typeof (undefined)
                && typeof (data.item_info_code) != typeof (undefined)
                && typeof (data.item_info_barcode) != typeof (undefined)
            ) {
                var information = data.item_info_name + " (" + data.item_info_code + " , " + data.item_info_barcode + ")";
                $('#itemInfo').html(information);
            }

            Qty.Focus();
        }
    };

    return {
        UpdateMainLine: function (s, e) {
            grdCompositionDecompositionMainLinesGrid.UpdateEdit();
        },
        CancelMainLine: function (s, e) {
            grdCompositionDecompositionMainLinesGrid.CancelEdit();
        },
        SelectByDescription: function (s, e) {
            var unicode;
            if (typeof e.htmlEvent !== "undefined") {
                unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
                if (unicode == 27) {
                    e.htmlEvent.preventDefault();
                    return;
                }
            }
            if ((typeof e.htmlEvent === "undefined") ||
                (
                    (e.htmlEvent.keyCode == 10 || e.htmlEvent.keyCode == 13)
                    && description_search !== null
                )
            ) {
                var path = $('#HOME').val() + 'Document/jsonSelectByDescription';
                $.ajax({
                    type: 'POST',
                    url: path,
                    async: false,
                    data: {
                        'user_search': description_search.GetValue(),
                        'userDiscount': 0,
                        'isPercentage': 1
                    },
                    cache: false,
                    dataType: 'json',
                    success: UpdateCompositionDecompositionItemInfo,
                    error: ErrorInItemInfo
                });
            }
        },
        SearchByBarcodeCompositionDecomposition: function (s, e) {
            if (e.htmlEvent.keyCode == 13 && barcode_search.GetValue() !== null && typeof barcode_search.GetValue() !== "undefined" && barcode_search.GetValue() !== "") {
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
                    success: UpdateCompositionDecompositionItemInfo,
                    error: ErrorInItemInfo
                });
            }
        },
        ViewLinkedLines: function (s, e) {
            if (grdCompositionDecompositionMainLinesGrid.IsEditing() == false) {
                grdCompositionDecompositionLinkedLinesGrid.PerformCallback({ mainLine: grdCompositionDecompositionMainLinesGrid.GetRowKey(e.visibleIndex) });
            }
        },
        MainLinesGridEndCallback: function (s, e) {
            if (mainLineCommand != 'ADDNEWROW'
                && mainLineCommand != 'STARTEDIT'
            ) {
                grdCompositionDecompositionLinkedLinesGrid.SetEnabled(true);
            }
            mainLineCommand = '';
            CustomizationWindow(s, e);
            if (typeof (documentSummaryPanelPartial) != typeof (undefined)) {
                documentSummaryPanelPartial.PerformCallback();
            }
            grdCompositionDecompositionLinkedLinesGrid.PerformCallback({ mainLine: '' });
        },
        LinkedLinesGridEndCallback: function (s, e) {
            if (linkedLineCommand != 'ADDNEWROW'
                && linkedLineCommand != 'STARTEDIT'
            ) {
                grdCompositionDecompositionMainLinesGrid.SetEnabled(true);
            }
            linkedLineCommand = '';
            CustomizationWindow(s, e);
            if (typeof (documentSummaryPanelPartial) != typeof (undefined)) {
                documentSummaryPanelPartial.PerformCallback();
            }
        },
        MainLinesGridBeginCallback: function (s, e) {
            mainLineCommand = e.command;
            //if (e.command == "ADDNEWROW") {
            //    if ((typeof (storeTraders) !== typeof (undefined) && storeTraders.GetValue() == null)
            //    || (typeof (secondaryStores) !== typeof (undefined) && secondaryStores.GetValue() == null)
            //      ) {
            //        grdCompositionDecompositionLinkedLinesGrid.SetEnabled(false);
            //    }
            //}
        },
        LinkedLinesGridBeginCallback: function (s, e) {
            linkedLineCommand = e.command;
            grdCompositionDecompositionMainLinesGrid.SetEnabled(false);
            //if (grdCompositionDecompositionMainLinesGrid.InCallback()
            // || grdCompositionDecompositionMainLinesGrid.IsEditing()
            //   ) {
            //    e.Valid = false;
            //}            
        },
        UpdateLinkedLine: function (s, e) {
            grdCompositionDecompositionLinkedLinesGrid.UpdateEdit();
        },
        CancelLinkedLine: function (s, e) {
            grdCompositionDecompositionLinkedLinesGrid.CancelEdit();
        },
        LinkedLinesQuantityKeyPress: function (s, e) {
            var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
            if (unicode == 13) {
                CompositionDecomposition.UpdateLinkedLine();
            }
        },
        MainLinesQuantityKeyPress: function (s, e) {
            var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
            if (unicode == 13) {
                CompositionDecomposition.UpdateMainLine();
            }
        },
        MainLinesGridAddNewRow: function () {
            var enabled = DocumentType.GetValue() != null
                && DocumentSeries.GetValue() != null
                && (typeof (secondaryStores) != typeof (undefined) && secondaryStores.GetValue() != null);
            if (enabled) {
                grdCompositionDecompositionMainLinesGrid.AddNewRow();
            }
            else {
                setJSError(fillInAllTheFields);
            }
        },
        LinkedLinesGridAddNewRow: function () {
            var traderIsDefined = (typeof (secondaryStores) != typeof (undefined) && secondaryStores.GetValue() != null);
            var enabled = DocumentType.GetValue() != null
                && DocumentSeries.GetValue() != null
                && traderIsDefined
                && grdCompositionDecompositionMainLinesGrid.InCallback() == false
                && grdCompositionDecompositionMainLinesGrid.IsEditing() == false;
            if (enabled) {
                grdCompositionDecompositionLinkedLinesGrid.AddNewRow();
            }
            else if (traderIsDefined == false) {
                setJSError(fillInAllTheFields);
            }
        }
    };
})();

var DocumentEditTabControl;

DocumentEditTabControl = (function (s, e) {
    return {
        DocumentTabCanChange: function (s, e) {
            var requiredFields = ['DocumentType', 'DocumentSeries', 'storeSuppliers', 'storeCustomers', 'secondaryStores'];
            for (var field in requiredFields) {
                var fld = Component.GetName(requiredFields[field]);
                if (fld !== null) {
                    if ($.trim(fld.GetValue()) === "") {
                        return false;
                    }
                }
            }
            return true;
        }
    };
})();
