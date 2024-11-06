var popupGenericCallbackPanelIsInit;

$(document).ready(function () {
    $('#GenericViewCallbackPanel_LD ').addClass('showLoadingPanel');
    $('#GenericViewCallbackPanel_LP ').addClass('showLoadingPanelGif');
    
});

$(window).load(function () {
    $('#GenericViewCallbackPanel_LD ').removeClass('showLoadingPanel');
    $('#GenericViewCallbackPanel_LP ').removeClass('showLoadingPanelGif');

    if (typeof (barcode_search) !== "undefined" && barcode_search.GetMainElement() !== null) {
        barcode_search.Focus();
    }
    else if (typeof (spinlineqty) !== "undefined" && spinlineqty.GetMainElement() !== null) {
        spinlineqty.Focus();
        spinlineqty.SelectAll();
    }
    addedItem = false;

    $.fn.tagcloud.defaults = {
        size: { start: 8, end: 15, unit: 'px' },
        color: { start: '#06A7FF', end: '#D50000' }
    };

    createTagClouds();
});

function createTagClouds() {
    if ($('#div_ItemTagCloudLastTwoMonths a').length > 0) {
        $('#div_ItemTagCloudLastTwoMonths a').tagcloud();
    }
    if ($('#div_ItemTagCloudLastYearTwoMonths a').length > 0) {
        $('#div_ItemTagCloudLastYearTwoMonths a').tagcloud();
    }
}

function DocumentTreeViewGetData(s, e) {
    if (grdMembersOfNode.IsEditing()) {
        qty_spin_edit.Focus();
    }
    else{
        var originalElement = e.htmlEvent.srcElement || e.htmlEvent.originalTarget;
        if (originalElement.className.search("node_img") != -1) {
            //Image clicked
            ClearSelectedItemsOfNode(s, e);
        }
        else {
            //Node Selected
            $('.node_img').addClass('hidden');
            var node = s.GetSelectedNode();
            if (node !== null && typeof node !== "undefined") {
                $('#' + node.name + ' img').removeClass('hidden');
            }
        }
    }
}

function DocumentOnBeginCallBackMembersOfNodeGrid(s, e) {
	order_action = e.command;
	if (CategoryTreeView.GetSelectedNode() !== null) {
		e.customArgs.categoryid = CategoryTreeView.GetSelectedNode().name;
		if (order_tabs.GetActiveTabIndex() === 1) {
			e.customArgs.itemSupplierId = Component.GetName('itemSuppliers_categories').GetValue();
		}
		if (order_tabs.GetActiveTabIndex() === 2) {
			e.customArgs.itemSupplierId = Component.GetName('itemSuppliers_offers').GetValue();
		}
	}
	if (order_action == "UPDATEEDIT") {
		qty_spin_edit.SetVisible(false);
		qty_spin_edit.SetValue(Math.round(qty_spin_edit.GetValue() * QUANTITY_MULTIPLIER));
    }
}

function CreateGridButton(s,e) {
    if (order_action == "STARTEDIT") {

        click_fun = "";
        if (s.name == 'grdMembersOfNode') {
                click_fun = "ItemsOfNodeRowKeyChanged();";
            }
            else if (s.name == 'grdNewItems') {
                click_fun = "NewItemQtyChanged();";
            }
            else if (s.name == 'grdOfferDetails') {
              click_fun =  "OfferQtyChanged();";
            }

            $(".inserted_btn").append("<div id=\"qty_sv_btn\" class=\"inside_btn\" onclick=\"" + click_fun + "\">OK</div>");
    }
}

function OnEndCallbackItemsOfNodeGrid(s, e) {
	if (addedItem === true) {
		documentInfoPanel.PerformCallback();
		addedItem = false;
	}

	if (order_action == "STARTEDIT" && typeof (qty_spin_edit) !== "undefined" && qty_spin_edit !== null) {
		qty_spin_edit.Focus();
		qty_spin_edit.SelectAll();
	}
	if (order_action == "STARTEDIT" && grdMembersOfNode.IsEditing() === false) {
	    grdMembersOfNode.SetFocusedRowIndex(e.visibleIndex);
		grdMembersOfNode.StartEditRow(grdMembersOfNode.GetFocusedRowIndex());
	}
	if (order_action == "UPDATEEDIT" && typeof (qty_spin_edit) !== "undefined" && qty_spin_edit.GetMainElement() !== null) {
		qty_spin_edit.SetVisible(true);
		qty_spin_edit.SetValue(0);
		qty_spin_edit.Focus();
	}
    CreateGridButton(s,e);
}

function ItemsOfNodeRowKeyChanged(s, e) {
    if (grdMembersOfNode.IsEditing()) {
	    grdMembersOfNode.UpdateEdit();
	}
}

function ItemsOfNodeRowClicked(s, e) {
    if (grdMembersOfNode.IsEditing()) {
	    grdMembersOfNode.UpdateEdit();
	}
	else {
	    grdMembersOfNode.SetFocusedRowIndex(e.visibleIndex);
		grdMembersOfNode.StartEditRow(e.visibleIndex);
	}
}



function itemImageEndCallback(s, e) {
    if (Component.GetName('item_info_code').GetValue() === null || Component.GetName('item_info_code').GetValue() === "") {
	    if (typeof (barcode_search) !== "undefined" && barcode_search.GetMainElement() !== null) {
			barcode_search.Focus();
		}
	}
	else {
	    if (!SaveFocus) {
	        if (Component.GetName('item_info_name').GetEnabled()) {
	            if (itemPanelCallbackCommand == 'CUSTOM_PRICE') {	                
	                spinlineqty.Focus();
	            }
	            else {
	                item_info_name.Focus();
	            }
	        }
	        else {
	            spinlineqty.Focus();
	        }			
		}
	}
}

function GetItemInfoOnEnterPressedSimple(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;

    var IsInCallback = false;
    if (typeof (itemPanel) === "undefined") {
        IsInCallback = itemPanelPurchase.InCallback();
    }
    else {
        IsInCallback = itemPanel.InCallback();
    }

    if (IsInCallback) {
        e.htmlEvent.preventDefault();
    }
    else {

        if (unicode == 13) {
            order_tabs.SetActiveTabIndexInternal(0);
            $("#SaveHyperLink").focus();
            SaveFocus = true;
            GetItemInfo();
        }
        else if (unicode > 31) {
            ValidateChar(s, e);
        }
        else {
            e.htmlEvent.returnValue = true;
        }
    }
}

function OnBeginCallBackOfferDetails(s, e) {
	order_action = e.command;
	if (e.command == "UPDATEEDIT") {
		e.customArgs.offer_detail_item_guid = grdOfferDetails.GetRowKey(grdOfferDetails.GetFocusedRowIndex());
	}
	else if (e.command == "GETOFFERDETAILS") {
		e.customArgs.selected_offer = grdOrderByOffer.GetRowKey(grdOrderByOffer.GetFocusedRowIndex());
	}
	if (order_tabs.GetActiveTabIndex() == 1) {
		e.customArgs.itemSupplierId = Component.GetName('itemSuppliers_categories').GetValue();
	}
	if (order_tabs.GetActiveTabIndex() == 2) {
		e.customArgs.itemSupplierId = Component.GetName('itemSuppliers_offers').GetValue();
	}

	if (order_action == "UPDATEEDIT") {
		qty_spin_edit.SetVisible(false);
		qty_spin_edit.SetValue(Math.round(qty_spin_edit.GetValue() * QUANTITY_MULTIPLIER));
	}
}

function GetOfferDetails(s, e) {
	if (grdOfferDetails.IsEditing()) {
		qty_spin_edit.Focus();
	}
	else {
	    grdOfferDetails.PerformCallback("GETOFFERDETAILS:" + s.GetRowKey(e.visibleIndex));
	}
}

function OnEndCallbackBackOfferDetails(s, e) {
	if (addedItem === true) {
		documentInfoPanel.PerformCallback();
		addedItem = false;
	}
	if (order_action == "STARTEDIT" && typeof (qty_spin_edit) !== "undefined" && qty_spin_edit !== null) {
		qty_spin_edit.Focus();
		qty_spin_edit.SelectAll();
	}
	if (order_action == "STARTEDIT" && grdOfferDetails.IsEditing() === false) {
		grdOfferDetails.SetFocusedRowIndex(e.visibleIndex);
		grdOfferDetails.StartEditRow(grdOfferDetails.GetFocusedRowIndex());
	}
	if (order_action == "UPDATEEDIT" && typeof (qty_spin_edit) !== "undefined" && qty_spin_edit.GetMainElement() !== null) {
		qty_spin_edit.SetVisible(true);
		qty_spin_edit.SetValue(0);
		qty_spin_edit.Focus();
    }
    CreateGridButton(s,e);
}

function OfferDetailsOnRowClicked(s, e) {
	if (grdOfferDetails.IsEditing()) {
		grdOfferDetails.UpdateEdit();
	}
	else {
		grdOfferDetails.SetFocusedRowIndex(e.visibleIndex);
		grdOfferDetails.StartEditRow(e.visibleIndex);
	}
}

function SvDocumentLine(s, e) {
    if (itemPanel.InCallback()) {
        return;
    }
	if (item_info_code.GetValue() === null) {
	    setJSNotification(pleaseSelectAnObject);
	}
	else if (spinlineqty.GetValue() !== null && spinlineqty.GetValue() !== '') {
		if (Component.GetName('barcode_search') !== null) {
			barcode_search.Focus();
		}
		addedItem = true;
		itemPanel.PerformCallback("ADDITEM");
	}
	else {
	    setJSNotification(pleaseSelectQuantity);
	    spinlineqty.Focus();

	}
}

function OrderMultipleOffers() {
	addedItem = true;
	grdOfferDetails.PerformCallback("ORDEROFFERS");

}

function OrderMultipleItems(s,e) {
	addedItem = true;
	grdMembersOfNode.PerformCallback("ORDERITEMS");
	if (Component.GetName('grdOfferDetails') !== null)
		GetOfferDetails(grdOrderByOffer,e);
	if (Component.GetName('grdNewItems') !== null)
		SearchNewItems();
}

function OrderFreshItems() {
	addedItem = true;
	grdNewItems.PerformCallback("ORDERITEMS");


}

function OfferQtyChanged(s, e) {
	if (grdOfferDetails.IsEditing()) {
		grdOfferDetails.UpdateEdit();
	}
}

function ClearSelectedItemSupplier(s, e) {

	var edit_in_progress = false;
	if ((order_tabs.GetActiveTabIndex() === 1 && grdMembersOfNode.IsEditing()) || (order_tabs.GetActiveTabIndex() === 2 && grdOfferDetails.IsEditing())) {
		edit_in_progress = true;
	}

	if (edit_in_progress) {
		qty_spin_edit.Focus();
	} else {
		if (order_tabs.GetActiveTabIndex() === 1) {
			Component.GetName('itemSuppliers_categories').SetValue();
			itemSupplierLostFocus(itemSuppliers_categories, e);
			grdMembersOfNode.PerformCallback();
		}
		if (order_tabs.GetActiveTabIndex() === 2) {
			Component.GetName('itemSuppliers_offers').SetValue();
			itemSupplierLostFocus(itemSuppliers_offers, e);
			grdOfferDetails.PerformCallback("GETOFFERDETAILS:" + grdOrderByOffer.GetRowKey(grdOrderByOffer.GetFocusedRowIndex()));
		}
	}
}

function itemSupplierValueChanged(s, e) {
	var edit_in_progress = false;
	if ((order_tabs.GetActiveTabIndex() === 1 && grdMembersOfNode.IsEditing()) || (order_tabs.GetActiveTabIndex() === 2 && grdOfferDetails.IsEditing())
    ) {
		edit_in_progress = true;
	}

	if (edit_in_progress) {
		qty_spin_edit.Focus();
	} else {
		if (order_tabs.GetActiveTabIndex() === 1) {
		    grdMembersOfNode.PerformCallback();
		}
		if (order_tabs.GetActiveTabIndex() === 2) {
			grdOfferDetails.PerformCallback("GETOFFERDETAILS:" + grdOrderByOffer.GetRowKey(grdOrderByOffer.GetFocusedRowIndex()));
		}
	}
}

function itemSupplierLostFocus(s, e) {
	if (s.GetValue() !== "" && s.GetValue() !== null)
		return;

	var input = s.GetInputElement();
	input.style.color = "gray";
	input.value = searchItemSupplierNullText;
}

function itemSupplierGotFocus(s, e) {
	var input = s.GetInputElement();
	if (input.value == searchItemSupplierNullText) {
		input.value = "";
		input.style.color = "black";
	}
}

function itemSupplierInit(s, e) {
	itemSupplierLostFocus(s, e);
}

function OrderTabChanging(s, e) {
	if (order_tabs.InCallback()) {
		e.cancel = true;
		return;
	}

	e.cancel = CheckIfItemIsNotAddedToBasket(confirmTabChange);
}

function CheckIfItemIsNotAddedToBasket(message) {
    if (order_tabs.GetActiveTabIndex() === 0 && item_info_code.GetValue() !== null) {
        if (!confirm(message)) {
            
            return true;
        }
    }

    if ((order_tabs.GetActiveTabIndex() === 1 && typeof (grdMembersOfNode) !== "undefined" && grdMembersOfNode.IsEditing()) ||
        (order_tabs.GetActiveTabIndex() === 2 && typeof (grdOfferDetails) !== "undefined" && grdOfferDetails.IsEditing()) ||
        (order_tabs.GetActiveTabIndex() === 3 && typeof (grdNewItems) !== "undefined" && grdNewItems.IsEditing())) {
        qty_spin_edit.Focus();
        
        return true;
    }

    if ((order_tabs.GetActiveTabIndex() === 1 && typeof (grdMembersOfNode) !== "undefined" && (grdMembersOfNode.IsEditing() || grdMembersOfNode.cp_edited === true)) ||
        (order_tabs.GetActiveTabIndex() === 2 && typeof (grdOfferDetails) !== "undefined" && (grdOfferDetails.IsEditing() || grdOfferDetails.cp_edited === true)) ||
        (order_tabs.GetActiveTabIndex() === 3 && typeof (grdNewItems) !== "undefined" && (grdNewItems.IsEditing() || grdNewItems.cp_edited === true))
    ) {
        if (!confirm(message)) {
            
            return true;
        }
    }
    return false;
}

function ClearSelectedOffers(s, e) {
	grdOfferDetails.PerformCallback("CLEAROFFERS");
}

function ClearSelectedItemsOfNode(s, e) {
    grdMembersOfNode.PerformCallback("CLEARITEMSOFNODE");
}

function NewItemQtyChanged(s, e) {
	if (grdNewItems.IsEditing()) {
		grdNewItems.UpdateEdit();
	}
}

function SearchNewItems(s, e) {
	if (grdNewItems.IsEditing()) {
		qty_spin_edit.Focus();
	} else {
		grdNewItems.PerformCallback("SEARCH");
	}
}

function OnBeginCallBackNewItems(s, e) {
	e.customArgs.inserted_after = inserted_after.GetText();
	e.customArgs.rdBLNewItems = rdBLNewItems.GetValue();
	order_action = e.command;
	if (e.command == "UPDATEEDIT") {
		e.customArgs.fresh_item_guid = grdNewItems.GetRowKey(grdNewItems.GetFocusedRowIndex());
		qty_spin_edit.SetVisible(false);
		qty_spin_edit.SetValue(Math.round(qty_spin_edit.GetValue() * QUANTITY_MULTIPLIER));
	}
}

function OnRowClickedNewItems(s, e) {
	if (grdNewItems.IsEditing()) {
		grdNewItems.UpdateEdit();
	}
	else {
		grdNewItems.SetFocusedRowIndex(e.visibleIndex);
		grdNewItems.StartEditRow(e.visibleIndex);
	}
}

function OnEndCallbackBackNewItems(s, e) {
	if (addedItem === true) {
		documentInfoPanel.PerformCallback();
		addedItem = false;
	}
	if (order_action == "STARTEDIT" && typeof (qty_spin_edit) !== undefined && qty_spin_edit !== null) {
		qty_spin_edit.Focus();
		qty_spin_edit.SelectAll();
	}
	if (order_action == "STARTEDIT" && grdNewItems.IsEditing() === false) {
		grdNewItems.SetFocusedRowIndex(e.visibleIndex);
		grdNewItems.StartEditRow(grdNewItems.GetFocusedRowIndex());
	}
	if (order_action == "UPDATEEDIT" && typeof (qty_spin_edit) !== "undefined" && qty_spin_edit.GetMainElement() !== null) {

		qty_spin_edit.SetVisible(true);
		qty_spin_edit.SetValue(0);
		qty_spin_edit.Focus();
    }
    CreateGridButton(s,e);
}

function rdBLNewItems_SelectedIndexChanged(s, e) {

	if (grdNewItems.IsEditing()) {
		qty_spin_edit.Focus();
	} else {

		if (rdBLNewItems.GetValue() == "TimePeriod") {
			inserted_after.SetEnabled(true);
		}
		else {
			inserted_after.SetEnabled(false);
		}
	}
}

function showHideItemsOfNodeFilters(s, e) {
    if (!grdMembersOfNode.IsEditing() && !grdMembersOfNode.InCallback()) {
        grdMembersOfNode.PerformCallback("SHOWHIDEFILTERS");
	}
}

function showHideFreshItemFilters(s, e) {
	if (!grdNewItems.IsEditing() && !grdNewItems.InCallback()) {
		grdNewItems.PerformCallback("SHOWHIDEFILTERS");
	}
}

function showHideOffersFilters(s, e) {
	if (!grdOfferDetails.IsEditing() && !grdOfferDetails.InCallback()) {
		grdOfferDetails.PerformCallback("SHOWHIDEFILTERS");
	}
}

function increaseQty(s, e) {
	if (!itemPanel.InCallback()) {
		var qty = spinlineqty.GetValue();
		qty++;
		if (qty <= 0) {
			qty = 1;
		}
		spinlineqty.SetValue(qty);
		triggerItemInfo();
	}
}

function decreaseQty(s, e) {
	if (!itemPanel.InCallback()) {
		var qty = spinlineqty.GetValue();
		qty--;
		if (qty <= 0) {
			qty = 1;
		}
		spinlineqty.SetValue(qty);
		triggerItemInfo();
	}
}

function triggerItemInfo() {
	order_tabs.SetActiveTabIndexInternal(0);
	$("#SaveHyperLink").focus();
	SaveFocus = true;
	GetItemInfo();
}

function ContinueButtonClick(s, e) {
    var notAdded = CheckIfItemIsNotAddedToBasket(confirmCloseWindow);
    
    if (!notAdded) {
        OrderItemsPopUp.Hide();
        hasReturnedFromOrderItemsForm = true;
        if (typeof PopupEditCallbackPanel != typeof undefined && typeof popupGenericCallbackPanelIsInit ===  typeof undefined) {
            PopupEditCallbackPanel.PerformCallback();
        }
        else {
            popupGenericCallbackPanelIsInit = undefined;
            PopupGenericEditCallbackPanel.PerformCallback({ ID: $("#Oid").val(), Recover: false, Division: $("#Mode").val(), HasReturnedFromOrderItemsForm: hasReturnedFromOrderItemsForm, DisplayGeneric: true });             
        }      
    } 
}

function PopupGenericEditCallbackPanelBeginCallback(s, e) {
    popupGenericCallbackPanelIsInit = true;
}

function PerformItemTagCloudClick(code) {
    barcode_search.SetText(code);
    SearchByBarcodeProcedure();
}