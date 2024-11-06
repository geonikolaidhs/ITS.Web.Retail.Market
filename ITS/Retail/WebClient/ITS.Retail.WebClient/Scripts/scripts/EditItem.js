var SelectedNodeID = -1;
var needsBarcodeFromCode = false;
var changeCodePreviousValue = false;
var newSeasonality = "00000000-0000-0000-0000-000000000000";//emptyGuid;
var newBuyer = "00000000-0000-0000-0000-000000000000";//emptyGuid;
var codeRequired = null;

function btnCancelClickV2(s, e) {
    var path = $("#HOME").val() + "Item/CancelEdit";
    $.ajax({
        type: 'POST',
        url: path,
        async: false,
        success: function (data) {
            if (typeof data.error !== typeof undefined) {
                setJSError(data.error);
            }
            else {
                LoadEditPopup.Hide();
            }
        },
        error: function (data) {
            setJSError(data);
            LoadEditPopup.Hide();
        }
    });
}


function BuyerDialogOkButton_OnClick(s, e) {
    if ($.trim(BuyerCode.GetText()) === "" || $.trim(BuyerDescription.GetText()) === "") {
        setJSError(markedFieldsAreRequired);
    }
    else {
        var path = $('#HOME').val() + 'Item/jsonNewBuyer';
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'Code': BuyerCode.GetValue(), 'Description': BuyerDescription.GetValue() },
            cache: false,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (data.success) {
                    Dialog.Hide();
                    newBuyer = data.buyerGuid;
                    Buyer.PerformCallback();
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}

function BuyerEndCallback(s, e) {
    if (newBuyer != emptyGuid) {
        Buyer.SetValue(newBuyer);
    }
    newBuyer = emptyGuid;
}

function SeasonalityDialogOkButton_OnClick(s, e) {
    if ($.trim(SeasonalityCode.GetText()) === "" || $.trim(SeasonalityDescription.GetText()) === "") {
        setJSError(markedFieldsAreRequired);
    }
    else {
        var path = $('#HOME').val() + 'Item/jsonNewSeasonality';
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'Code': SeasonalityCode.GetValue(), 'Description': SeasonalityDescription.GetValue() },
            cache: false,
            dataType: 'json',
            async: false,
            success: function (data) {
                if (data.success) {
                    Dialog.Hide();
                    newSeasonality = data.seasonalityGuid;
                    Seasonality.PerformCallback();
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}

function SeasonalityEndCallback(s, e) {
    if (newSeasonality != emptyGuid) {
        Seasonality.SetValue(newSeasonality);
    }
    newSeasonality = emptyGuid;
}

function OnBeginCallbackBarcodeGrid(s, e) {
    if (s.IsEditing()) {
        var comboBox;
        comboBox = Component.GetName('Type!Key');
        if (comboBox !== null)
            e.customArgs.TypeID = comboBox.GetValue();

        comboBox = Component.GetName('MeasurementUnit!Key');
        if (comboBox !== null)
            e.customArgs.MeasurementUnitID = comboBox.GetValue();

        e.customArgs.BarcodeCode = BarcodeCode.GetValue();
        if (changeCodePreviousValue) {
            //previous Item Code value
            document.getElementById("Code_I").defaultValue = BarcodeCode.GetValue();
        }
    }
}

function OnEndCallbackBarcodeGrid(s, e) {

    if (needsBarcodeFromCode) {
        if (codeRequired === null) {
            BarcodeCode.SetValue(Code.GetValue());
        }
        else {
            BarcodeCode.SetValue(codeRequired);
        }
        codeRequired = null;
        needsBarcodeFromCode = false;
        changeCodePreviousValue = true; //need to change previous Item Code value
    }
    if (!s.IsEditing()) {
        ItemCbPanel.PerformCallback();
        if (grdPriceCatalogDetails !== null) {
            grdPriceCatalogDetails.PerformCallback();
        }
    }
}

function OnBeginCallbackItemsOfMotherCodeGrid(s, e) {
    if (s.IsEditing()) {
        var comboBox = Component.GetName('childItemcomboBox');
        if (comboBox !== null)
            e.customArgs.ComboBoxChildItemID = comboBox.GetValue();
    }
}

function btnDeleteItemImageClick(s, e) {
    var path = $('#HOME').val() + 'Item/jsonDeleteItemImage';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            $('#imgItemImage').attr('src', $('#HOME').val() + 'Item/ShowImage' + '?time=' + new Date().getTime());
            $('#previewImage').attr('src', $('#HOME').val() + 'Item/ShowImage' + '?time=' + new Date().getTime());
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

function btnUpdateClick(s, e) {

    s.SetEnabled(false);

    if (ValidateModalFormSingle() === true) {
        var path = $('#HOME').val() + 'Item/jsonCheckForBarcodeFromItemCode';
        $.ajax({
            type: 'POST',
            url: path,
            cache: false,
            dataType: 'json',
            data: { 'ItemCode': Code.GetValue(), 'PreviousItemCode': document.getElementById("Code_I").defaultValue },
            async: false,
            success: function (data) {
                if (data.timeValuesResultValid === false) {
                    setJSError(data.timeValuesMessage);
                    btnUpdate.SetEnabled(true);
                }
                else if (data.needsBarcodeFromCode) {
                    btnUpdate.SetEnabled(true);
                    var answer = confirm(createBarcodeFromItemCode);
                    if (answer === true) {
                        needsBarcodeFromCode = data.needsBarcodeFromCode;
                        pcFeatures2.SetActiveTabIndex(0);
                        codeRequired = data.codeBarcode;
                        if (data.replaceExisting) {
                            grdBarcodes.StartEditRowByKey(data.existingBarcodeID);
                        }
                        else {
                            grdBarcodes.AddNewRow();
                        }
                    }
                }
                else {
                    btnUpdate.SetEnabled(false);
                    if (document.getElementById('ItemImageDescription') !== null) {
                        if (document.getElementById('ItemImageDescription').value !== null) {
                            if (typeof (txtItemImageDescription) !== "undefined") {
                                document.getElementById('ItemImageDescription').value = txtItemImageDescription.GetValue();
                            }

                        }
                    }
                    if (typeof (txtItemImageInfo) !== "undefined") {
                        document.getElementById('ItemImageInfo').value = txtItemImageInfo.GetValue();
                    }

                    var form_to_submit = Component.GetCorrectForm(document.forms[0]);

                    $.ajax({
                        url: form_to_submit.action,
                        type: form_to_submit.method,
                        data: $(form_to_submit).serialize(),
                        cache: false,
                        success: function (data) {
                            if (typeof data.error !== "undefined") {
                                setJSError(data.error);
                            }
                            else {
                                LoadEditPopup.Hide();
                            }
                        }
                    });
                }
            },
            error: function (data) {
                btnUpdate.SetEnabled(true);
            }
        });
    }
    else {
        setJSError(markedFieldsAreRequired);
        btnUpdate.SetEnabled(true);
    }
}

function OnBeginCallbackPriceCatalogDetailGrid(s, e) {
    if (s.IsEditing()) {
        var barcodeComboBox = Component.GetName('BarcodesComboBox_PriceCatalogDetail');
        e.customArgs.BarcodeComboBoxID = barcodeComboBox.GetValue();
        var priceCatalogComboBox = Component.GetName('PriceCatalogsComboBox');
        e.customArgs.PriceCatalogComboBoxID = priceCatalogComboBox.GetValue();
        e.customArgs.VatIncluded = VatIncludedCheckbox.GetValue();
    }
}

function TreeViewGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
    if (SelectedNodeID == s.GetSelectedNode().name) {
        if (document.getElementById("categoryfilterid") !== null) {
            $("#categoryfilterid").text(SelectedNodeID);
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
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
    SelectedNodeID = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
}

function OnBeginCallbackItemAnalyticTreeGrid(s, e) {
    if (s.IsEditing()) {
        e.customArgs.SelectedNodeID = SelectedNodeID;
    }
}

function OnBeginCallbackLinkedSubItemsGrid(s, e) {
    if (s.IsEditing()) {
        var comboBox = Component.GetName('LinkedSubItemsComboBox');
        if (comboBox !== null)
            e.customArgs.SubItemID = comboBox.GetValue();
    }
}

function OnBeginCallbackLinkedToItemsGrid(s, e) {
    if (s.IsEditing()) {
        var comboBox = Component.GetName('LinkedToItemsComboBox');
        if (comboBox !== null)
            e.customArgs.BaseItemID = comboBox.GetValue();
    }
}

function OnSaveBtnClickItemAnalyticTree(s, e) {
    jsonCheckForDuplicateCategory();
}

function jsonCheckForDuplicateCategory() {
    var path = $('#HOME').val() + 'Item/jsonCheckForDuplicateCategory';

    $.ajax({
        type: 'POST',
        url: path,
        data: { 'SelectedNodeID': SelectedNodeID },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.hasDuplicate) {
                setJSError(cannotInsertSameCategoryMultipleTimes);
            }
            else {
                grdItemAnalyticTree.UpdateEdit();
            }
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

function CheckForExistingItem(e) {
    if (Code.GetValue() != document.getElementById("Code_I").defaultValue) {
        var path = $("#HOME").val() + "Item/jsonCheckForExistingItem";
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'Code': Code.GetValue() },
            cache: false,
            success: function (result) {
                if (typeof result.Error == typeof undefined) {
                    ItemSuccess(result);
                }
            },
            error: function (data) {
                setJSError(anErrorOccured);
            }
        });
    }
}

function ItemSuccess(data) {
    if (data === false) {
        return;
    }

    var message = itemAlreadyExists.replace("@1", data.ItemCode).replace("@2", data.ItemName);
    var answer = confirm(message);
    if (answer === false) {
        var previous = Code.FindInputElement();
        Code.SetValue(previous.defaultValue);
        return;
    }
    selectedItemsArray = [];
    selectedItemsArray.push(data.ItemID);
    Component.EmptyCallbackPanels();
    PopupEditCallbackPanel.PerformCallback();

    if (typeof grid !== typeof undefined) {
        grid.UnselectAllRowsOnPage();
    }

}

function OnBeginCallbackItemStoreGrid(s, e) {
    if (s.IsEditing()) {
        var storeComboBox = Component.GetName('StoresComboBox_ItemStore');
        e.customArgs.StoreID = storeComboBox.GetValue();

    }
}

function OnSaveBtnClickItemStore(s, e) {
    if (ValidateModalFormSingle()) {
        CheckForExistingItemStore();
    } else {
        setJSError(markedFieldsAreRequired);
        //return validation;
    }
}

function CheckForExistingItemStore() {
    var path = $("#HOME").val() + "Item/jsonCheckForExistingItemStore";
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'StoreID': StoresComboBox_ItemStore.GetValue()
        },
        cache: false,
        success: function (data) {
            if (data.allow === true) {
                grdItemStore.UpdateEdit();
            }
            else {
                setJSError(cannotHaveMultipleStorage);
            }
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#imgItemImage').attr('src', $('#HOME').val() + 'Item/ShowImage' + '?time=' + new Date().getTime());
        $('#previewImage').attr('src', $('#HOME').val() + 'Item/ShowImage' + '?time=' + new Date().getTime());
    }
}


function OnExtraFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        ExtraFileName.SetText(e.callbackData);
    }
}

function DeleteExtraFileOnClick(s, e) {
    var path = $('#HOME').val() + 'Item/JsonDeleteExtraFile';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            ExtraFileName.SetText("");
        },
        error: function (data) {
        }
    });
}

function UploadExtraFileOnClick(s, e) {
    UploadExtraFileControl.Upload();
}

function PriceCatalogDetailTimeValueStartEdit(s, e) {
    if (e.rowValues[2].value == null) {
        e.rowValues[2].value = new Date();
        e.rowValues[2].value.setHours(23, 59, 59);
    }
    if (e.rowValues[1].value == null) {
        e.rowValues[1].value = new Date();
        e.rowValues[1].value.setHours(0, 0, 0);
    }
    if (e.rowValues[3].value == null) {
        e.rowValues[3].value = 1;
    }
    if (e.rowValues[4].value == null) {
        e.rowValues[4].value = true;
    }
}




function OnBeginCallbackItemExtraInfoStoreGrid(s, e) {
    if (s.IsEditing()) {
        var storeComboBox = Component.GetName('StoresComboBox_ItemExtraInfo');
        if (storeComboBox != null) {
            e.customArgs.StoreID = storeComboBox.GetValue();
        }

    }
}


//var ItemExtraInfo = (function () {
//    getValue = function (fieldName) {
//        var field = Component.GetName(fieldName);
//        return field == null || typeof (field) === typeof (undefined) ? '' : field.GetValue();
//    };
//    return {
        //OnBeginCallbackItemExtraInfoGrid: function (s,e) {            
            //e.customArgs = {
            //    "description": getValue("ItemExtraInfoDescription"),
            //    "lot": getValue("ItemExtraInfoLot"),
            //    "ingredientes": getValue("ItemExtraInfoIngredients"),
            //    "packetAt": getValue("ItemExtraInfoPackedAt"),
            //    "expiresAt": getValue("ItemExtraInfoExpiresAt"),
            //    "origin": getValue("ItemExtraInfoOrigin"),
            //    "store": getValue("StoresComboBox_ItemExtraInfo")
            //};
        //},
        //OnSaveBtnClickItemExtraInfo: function (s,e) {
        //    //grdItemExtraInfo.UpdateEdit();
        //}
//    };
//})();

