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

function clearCategory() {
    SelectedNodeOid = -1;
    SelectedNodeText = '';
    $('#categoryfilterid').text(SelectedNodeOid);
    $('#categoryfiltertext').text(SelectedNodeText);
    $("#btnClearCategories").hide();
}

function OnBeginCallbackPriceCatalogDetailGrid(s, e) {
    CustomizationWindow();
    if (s.IsEditing()) {
        var barcodeComboBox = Component.GetName('BarcodesComboBox');
        e.customArgs.ItemBarcodeID = barcodeComboBox.GetValue();
        e.customArgs.VatIncluded = VatIncludedCheckbox.GetValue();
    }
    else {
        e.customArgs.Fcode = Component.GetName('Fcode').GetValue();
        e.customArgs.Fname = Component.GetName('Fname').GetValue();
        e.customArgs.Fbarcode = Component.GetName('Fbarcode').GetValue();
        e.customArgs.Factive = Component.GetName('Factive').GetValue();
        e.customArgs.Fcategory = Component.GetName('categoryfilterid').GetValue();
        e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
        e.customArgs.FupdatedOn = Component.GetName('FupdatedOn').GetText();
        e.customArgs.FitemSupplier = Component.GetName('FitemSupplier').GetValue();
        e.customArgs.Fbuyer = Component.GetName('Fbuyer').GetValue();
        e.customArgs.Fseasonality = Component.GetName('Fseasonality').GetValue();
        e.customArgs.Fmothercode = Component.GetName('Fmothercode').GetValue();
    }
}

function OnBeginCallbackStorePriceListGrid(s, e) {

    if (s.IsEditing()) {
        var storeComboBox = Component.GetName('StoresComboBox');
        e.customArgs.StoreID = storeComboBox.GetValue();

    }
}

function OnBeginCallbackPriceCatalogChildsGrid(s, e) {

    if (s.IsEditing()) {
        var comboBox = Component.GetName('ChildPriceCatalogsComboBox');
        if (comboBox !== null)
            e.customArgs.ComboBoxChildPriceCatalogID = comboBox.GetValue();

    }
}

//function OnBeginCallbackCustomerCategoryDiscountGrid(s, e) {
//    if (s.IsEditing()) {
//        var comboBox;
//        comboBox = Component.GetName('ItemCategoriesComboBox');
//        if (comboBox !== null)
//            e.customArgs.ItemCategoryID = comboBox.GetValue();
//        comboBox = Component.GetName('CustomerCategoriesComboBox');
//        if (comboBox !== null)
//            e.customArgs.CustomerCategoryID = comboBox.GetValue();
//    }
//}

function SearchItem(s, e) {
    grdPriceCatalogDetails.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function TreeViewGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
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
    SelectedNodeOid = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
}

//function checkForEmptyStores() {
//    var hasNoStores = true;
//    var path = $("#HOME").val() + "PriceCatalog/jsonCheckForEmptyStores";
//    $.ajax({
//        type: 'POST',
//        url: path,
//        data: {},
//        cache: false,
//        async: false,
//        success: function (data) {
//            hasNoStores = data.hasNoStores;
//        },
//        error: function (data) {
//            hasNoStores = true;
//        }
//    });

//    return hasNoStores;
//}

function CheckForExistingPriceCatalog(e) {
    var path = $("#HOME").val() + "PriceCatalog/jsonCheckForExistingPriceCatalog";
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'Code': Code.GetValue() },
        cache: false,
        success: function (result) {
        if (typeof result.Error == typeof undefined ) {
            PriceCatalogSuccess(result);
        }
    },
    });
}

function PriceCatalogSuccess(data) {
    if (data === false)
        return;
    if (document.URL.indexOf(data.PriceCatalogID) > 0)
        return;
    var message = priceCatalogAlreadyExists.replace("@1", data.PriceCatalogCode).replace("@2", data.PriceCatalogDescription);
    var priceCatalog = $("#PriceCatalogID").val();
    var answer = confirm(message);
    if (answer === false) {
        var previous = Code.FindInputElement();
        Code.SetValue(previous.defaultValue);
        return;
    }

    Component.EmptyCallbackPanels();
    PopupEditCallbackPanel.PerformCallback({ ID: priceCatalog });
}


function btnCancelClickV2(s, e) {
    var path = $("#HOME").val() + "PriceCatalog/CancelEdit";
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
                window.location.reload();
            }
        },
        error: function (data) {
            setJSError(data);
            LoadEditPopup.Hide();
            window.location.reload();
        }
    });
}


function PriceCatalogDetailTimeValueStartEdit(s, e) {
    if (e.rowValues[2].value == null) {
        e.rowValues[2].value = new Date();
        e.rowValues[2].value.setHours(23, 59, 59, 0);
    }
}


var EditPriceCatalog = (function () {
    return {
        Save: function (s,e) {
            Component.BtnUpdateClick(s, e);
            priceCatalogTreeViewCbPanel.PerformCallback();
        }
    };
})();
