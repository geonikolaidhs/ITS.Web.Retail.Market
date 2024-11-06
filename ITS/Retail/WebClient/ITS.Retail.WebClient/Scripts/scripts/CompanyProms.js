function SearchPromotions(s, e) {
    grdPromotions.PerformCallback("SEARCH");
    toolbarHideFiltersOnly();
}

function OnBeginCallback(s, e) {

    if (!s.IsEditing()) {
        //TODO ADD SEARCH FILTERS
    }
}




function PromotionTreeListEndCallback(s,e){

    var objecttoedit = document.getElementById('objectid');
    if (typeof (objecttoedit) !== "undefined" && objecttoedit.value !== "") {
        s.StartEdit(objecttoedit.value);
    }  
}

function PromotionExecutionsEndCallback(s, e) {

    var objecttoedit = document.getElementById('objectid');
    if (typeof (objecttoedit) !== "undefined" && objecttoedit.value !== "") {
        s.StartEditRowByKey(objecttoedit.value);
    }
}

function TypeSelectorSelectedIndex(s,e)
{
    var val = cmbTypeSelector.GetValue(); 
    if (val===0)
    {
        Quantity.SetEnabled(true);
        Value.SetValue(0);
        Value.SetEnabled(false);
    } 
    else
    {
        Quantity.SetValue(0);
        Quantity.SetEnabled(false); 
        Value.SetEnabled(true);
    } 
}

function PercTypeSelectorSelectedIndex(s, e)
{
    var val = cmbTypeSelector.GetValue(); 
    if (val===0)
    {
        Percentage.SetEnabled(true);
        Value.SetValue(0);
        Value.SetEnabled(false);
    } 
    else
    {
        Percentage.SetValue(0);
        Percentage.SetEnabled(false);
        Value.SetEnabled(true);
    } 
}
function AddNewPromotion()
{
    WizardCommon.ShowWizard();
}

function EditPromotion(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToEdit);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToEdit);
    }
    else if (selectedItemsArray.length === 1) {
        WizardCommon.ShowWizard(selectedItemsArray[0]);
        grdPromotions.UnselectAllRowsOnPage();
    }
}

function PriceCatalogTreeList_Previous(s, e) {
    PriceCatalogTreeList.GetSelectedNodeValues("Oid",
        function (values) {
            $('#PriceCatalogsString_selected').val(values.toString());
            WizardCommon.GoBack();
        });
}

function PriceCatalogTreeList_Next(s, e) {
    PriceCatalogTreeList.GetSelectedNodeValues("Oid",
        function (values)
        {
            $('#PriceCatalogsString_selected').val(values.toString());
            WizardCommon.MoveNext();
        });
}

function PriceCatalogPolicyList_Previous(s, e) {
    $('#PriceCatalogPoliciesString_selected').val(PriceCatalogPoliciesList.GetSelectedValues().toString());
    WizardCommon.GoBack();
}

function PriceCatalogPolicyList_Next(s, e) {
    $('#PriceCatalogPoliciesString_selected').val(PriceCatalogPoliciesList.GetSelectedValues().toString());
    WizardCommon.MoveNext();
}

function PriceCatalogTreeList_SelectionChanged(s, e) {
    if ($("#PriceCatalogTreeList_SelAll").val() == "U" || $("#PriceCatalogTreeList_SelAll").val() == "C") {
        PriceCatalogTreeList.ExpandAll();
    }
    PriceCatalogTreeList.GetSelectedNodeValues("Oid",
        function (values) {
            $('#PriceCatalogsString_selected').val(values.toString());
            //var text = '';
            //PriceCatalogsList.ClearItems();
            //if (values.length > 0) {
                //PriceCatalogsList.ClearItems();
                //$.each(values, function (index, resultValue) {
                //    text = $('#PriceCatalogTreeList_R-' + resultValue.replace(/-/g, '')).find('.dxtl').html();
                //    //PriceCatalogsList.AddItem(text, text);
                //});
            //}
        }
    );
}

function FillInPriceCatalogPoliciesList(s, e)
{
    var keys = $('#PriceCatalogPoliciesString_selected').val();
    var values = keys.split(',');
    PriceCatalogPoliciesList.SelectValues(values);
}

function PriceCatalogPoliciesListSelectionIndexChanged(s, e) {
    PriceCatalogPolicyPromotionList.ClearItems();
    for (var key in s.GetSelectedValues()) {
        PriceCatalogPolicyPromotionList.AddItem(key);
    }
}

function ItemExecutionModeChanged(s, e) {
    if (s.GetValue() == "DISCOUNT")
    {
        FinalUnitPrice.SetEnabled(false);
        FinalUnitPrice.SetValue(0);
        OncePerItem.SetEnabled(true);
        ValueOrPercentage.SetEnabled(true);
    }
    else if (s.GetValue() == "SET_PRICE")
    {
        FinalUnitPrice.SetEnabled(true);
        OncePerItem.SetEnabled(false);
        ValueOrPercentage.SetEnabled(false);
        ValueOrPercentage.SetValue(0);
    }
}

function ItemCategoryExecutionModeChanged(s, e) {
    if (s.GetValue() == "DISCOUNT") {
        FinalUnitPrice.SetEnabled(false);
        FinalUnitPrice.SetValue(0);
        OncePerItem.SetEnabled(true);
        ValueOrPercentage.SetEnabled(true);
    }
    else if (s.GetValue() == "SET_PRICE") {
        FinalUnitPrice.SetEnabled(true);
        OncePerItem.SetEnabled(false);
        ValueOrPercentage.SetEnabled(false);
        ValueOrPercentage.SetValue(0);
    }
}

function KeepOnlyPointsCheckedChanged(s, e) {
    if (s.GetValue() === false) {
        Points.SetEnabled(false);
        Points.SetValue(0);
        DiscountType.SetEnabled(true);
        ValueOrPercentage.SetEnabled(true);
    }
    else {
        Points.SetEnabled(true);
        DiscountType.SetEnabled(false);
        DiscountType.SetValue(null);
        ValueOrPercentage.SetEnabled(false);
        ValueOrPercentage.SetValue(0);
    }
}

function InitErrorPanel(s, e) {
    s.Show();
    s.SetHeaderText(errorMessage);
}

function PromotionTreeListBeginCallback(s,e)
{
    if(e.command == "CancelEdit")
    {
        e.customArgs.CANCELEDIT = true;
    }
}

var CompanyProms;

CompanyProms = (function () {
    return {
        PriceCatalogTreeListInit: function (s, e) {
            PriceCatalogTreeList.ExpandAll();
        }
    };
})();
