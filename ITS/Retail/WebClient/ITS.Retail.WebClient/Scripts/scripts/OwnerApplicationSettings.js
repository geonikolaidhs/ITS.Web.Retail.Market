function OnInitSupportLoyalty(s, e) {
    InitLoyaltyItemsVisibility(s);    
}
function OnChangeSupportLoyalty(s, e) {
    UpdateLoyaltyItemsVisibility(s);
}

function InitLoyaltyItemsVisibility(checkboxButton) {
    var supportsLoyalty = checkboxButton.GetChecked();
    ownerappsettings.GetItemByName("RefundPoints").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("DiscountAmount").SetVisible(false);
    ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(false);
    ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(false);
    ownerappsettings.GetItemByName("LoyaltyRefundType").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyOnDocumentSum").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("OnlyRefundStore").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("PointCost").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(false);
    ownerappsettings.AdjustControl();
}

function UpdateLoyaltyItemsVisibility(checkboxButton) {
    var supportsLoyalty = checkboxButton.GetChecked();
    ownerappsettings.GetItemByName("RefundPoints").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("DiscountAmount").SetVisible(false);
    ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(false);
    ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(false);
    ownerappsettings.GetItemByName("LoyaltyRefundType").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("LoyaltyOnDocumentSum").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("OnlyRefundStore").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("PointCost").SetVisible(supportsLoyalty);
    ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(false);

    var documentSumLoyalty = LoyaltyOnDocumentSum.GetChecked();
    if (!supportsLoyalty) {
        ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(false);
        ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(false);
        ownerappsettings.GetItemByName("DiscountAmount").SetVisible(false);
        ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(false);        
        ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(false);             
    }
    else {        
        ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(documentSumLoyalty);
        ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(documentSumLoyalty);
    }

    if (supportsLoyalty)
    {
        var loyaltyRefundType = LoyaltyRefundType.GetValue();
        if (loyaltyRefundType == "DISCOUNT") {
            loyaltyRefundTypeDiscount = true;
            loyaltyRefundTypePayment = false;

            ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(true);
            ownerappsettings.GetItemByName("DiscountAmount").SetVisible(false);
        }
        else {
            loyaltyRefundTypePayment = true;
            loyaltyRefundTypeDiscount = false;
            ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(false);
            ownerappsettings.GetItemByName("DiscountAmount").SetVisible(true);
        }
        ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(!loyaltyRefundTypeDiscount);
        ownerappsettings.GetItemByName("DiscountAmount").SetVisible(loyaltyRefundTypePayment);
        ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(!loyaltyRefundTypePayment);
    }
    ownerappsettings.AdjustControl();
}

function OnInitLoyaltyOnDocumentSum(s, e) {
    InitLoyaltyOnDocumentSumVisibility(s);
}

function OnChangeLoyaltyOnDocumentSum(s, e) {
    UpdateLoyaltyOnDocumentSumVisibility(s);
}

function InitLoyaltyOnDocumentSumVisibility(s, e) {
    if ( $("#SupportLoyalty_S").val()!= 'U') {
        
        var documentSumLoyalty = s.GetChecked();
        
        ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(documentSumLoyalty);
        ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(documentSumLoyalty);
    }
    else {
        ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(false);
        ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(false);
    }
    ownerappsettings.AdjustControl();
}

function UpdateLoyaltyOnDocumentSumVisibility(checkboxButton) {
    
    var documentSumLoyalty = checkboxButton.GetChecked();
    ownerappsettings.GetItemByName("DocumentSumForLoyalty").SetVisible(documentSumLoyalty);
    ownerappsettings.GetItemByName("LoyaltyPointsPerDocumentSum").SetVisible(documentSumLoyalty);
    ownerappsettings.AdjustControl();
}

function OnInitLoyaltyRefundType(s, e){
    InitLoyaltyRefundTypeVisibility(s);
}

function OnValueChangedLoyaltyRefundType(s, e){
    UpdateLoyaltyRefundTypeVisibility(s);
}

function InitLoyaltyRefundTypeVisibility(comboboxItemIndex) {

    if ($("#SupportLoyalty_S").val() != 'U') {
        var loyaltyRefundType = LoyaltyRefundType.GetValue();
        if (loyaltyRefundType == "DISCOUNT") {
            loyaltyRefundTypeDiscount = true;
            loyaltyRefundTypePayment = false;

            ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(true);
            ownerappsettings.GetItemByName("DiscountAmount").SetVisible(false);
        }
        else {
            loyaltyRefundTypePayment = true;
            loyaltyRefundTypeDiscount = false;
            ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(false);
            ownerappsettings.GetItemByName("DiscountAmount").SetVisible(true);

        }

        ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(!loyaltyRefundTypeDiscount);
        ownerappsettings.GetItemByName("DiscountAmount").SetVisible(loyaltyRefundTypePayment);
        ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(!loyaltyRefundTypePayment);

        ownerappsettings.AdjustControl();
    }
    

}


function UpdateLoyaltyRefundTypeVisibility(comboboxItemIndex){   
        
    if (comboboxItemIndex.GetValue() == "DISCOUNT" || comboboxItemIndex.GetValue() === null) {
        loyaltyRefundTypeDiscount = true;
        loyaltyRefundTypePayment = false;
    }
    else
    {
        loyaltyRefundTypePayment = true;
        loyaltyRefundTypeDiscount = false;
    }

    ownerappsettings.GetItemByName("LoyaltyPaymentMethod").SetVisible(!loyaltyRefundTypeDiscount);
    ownerappsettings.GetItemByName("DiscountAmount").SetVisible(loyaltyRefundTypePayment);
    ownerappsettings.GetItemByName("DiscountRadioList").SetVisible(!loyaltyRefundTypePayment);   
    
    ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(!loyaltyRefundTypePayment);
    ownerappsettings.AdjustControl();
    
}

function OnInitDiscountType(s, e) {
    InitDiscountTypeVisibility(s);
}

function OnChangeDiscountType(s, e) {
    UpdateDiscountTypeVisibility(s);
}

function InitDiscountTypeVisibility(radioButton) {

    if ($("#SupportLoyalty_S").val() != 'U') {
        if (radioButton.GetValue() == "DiscountAmountRadio" || radioButton.GetValue() === null) {
            discountAmount = true;            
        }
        else {
            discountAmount = false;            
        }

        ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(!discountAmount);
        ownerappsettings.GetItemByName("DiscountAmount").SetVisible(discountAmount);

        if (discountAmount) {
            DiscountPercentage.SetValue(0);
        }
        else {
            DiscountAmount.SetValue(0);
        }

        ownerappsettings.AdjustControl();
    }

}

function UpdateDiscountTypeVisibility(radioButton){  

    if (radioButton.GetValue() == "DiscountAmountRadio" || radioButton.GetValue() === null) {
        discountAmount = true;
    }
    else {
        discountAmount = false;
    }    

    ownerappsettings.GetItemByName("DiscountPercentage").SetVisible(!discountAmount);
    ownerappsettings.GetItemByName("DiscountAmount").SetVisible(discountAmount);

    if (discountAmount) {
        DiscountPercentage.SetValue(0);
    }
    else {
        DiscountAmount.SetValue(0);
    }

    ownerappsettings.AdjustControl();

}



function OnFileUploadComplete(s, e) {
    if (e.callbackData !== '') {
        $('#previewImage').attr('src', $('#HOME').val() + 'OwnerApplicationSettings/ShowImageId' + '?time=' + new Date().getTime());
    }
}



function B2CDocumentTypeValueChanged(s, e)
{
    B2CDocumentSeries.PerformCallback();
    B2CDocumentSeries.SetText("");
    B2CDocumentSeries.SetValue(null);
}

function B2CStoreValueChanged(s, e) {
    B2CDocumentType.PerformCallback();    
    B2CDocumentType.SetText("");
    B2CDocumentType.SetValue(null);
    B2CDocumentTypeValueChanged();
}

function B2CDocumentTypeBeginCallback(s, e) {
    e.customArgs.storeID = B2CStore.GetValue();
}

function B2CDocumentSeriesBeginCallback(s, e) {
    e.customArgs.documentTypeID = B2CDocumentType.GetValue();
    e.customArgs.storeID = B2CStore.GetValue();
}

