var dateString = "NoDate";

//$(document).ready(function () {
//    StoreDailyReport.bindWindow();
//});
function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}

var StoreDailyReport = {
    //unbindWindow: function(){
    //    //jQuery(window).unbind('beforeunload');
    //    jQuery(window).unbind('unload');
    //},
    //bindWindow : function(){
    //    //jQuery(window).bind("beforeunload", function (e) {
    //    //    e.returnMessage = closePageConfirmMessage;
    //    //    return e.returnMessage;
    //    //});

    //    jQuery(window).bind("unload", function (e) {
    //        var path = $("#HOME").val() + "StoreDailyReport/CloseReport";
    //        $.ajax({
    //            type: 'POST',
    //            url: path,
    //            async: false
    //        });
    //    });
    //},
    CloseReport: function (s, e) {
        if (
           !grdDailyTotals.InCallback()
        && !grdCredits.InCallback()
        && !grdAutodeliveries.InCallback()
        && !grdPayments.InCallback()
        && !grdStatistics.InCallback()
        && $.active <= 0
         ) {
            $.ajax({
                type: 'POST',
                url: $("#HOME").val() + "StoreDailyReport/CloseReport",
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
    },
    initializeDay: function (s, e) {
        var date = Component.GetName('ReportDate');
        if (date === null || date.GetValue() === null) {
            setJSError(noDateSelected);
        }
        else {
            dateString = date.GetText();
            LoadingPanel.Show();
            var path = $('#HOME').val() + 'StoreDailyReport/jsonInitializeDay';
            $.ajax({
                type: 'POST',
                url: path,
                data: {
                    'date': dateString
                },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    if (typeof(data.message) === typeof(undefined)) {
                        grdDailyTotals.PerformCallback();
                        grdCredits.PerformCallback();
                        grdAutodeliveries.PerformCallback();
                        grdPayments.PerformCallback();
                        grdStatistics.PerformCallback();
                        LoadingPanel.Hide();
                        StoreDailyReport.SetFormFields(data);
                    }
                    else {
                        setJSError(data.message);
                    }
                },
                error: function (data) {
                    setJSError(data.error);
                }
            });
        }
    },
    saveStoreDailyReport: function (s, e) {
        if(
            !grdDailyTotals.InCallback()
         && !grdCredits.InCallback()
         && !grdAutodeliveries.InCallback()
         && !grdPayments.InCallback()
         && !grdStatistics.InCallback()
         && $.active <= 0
          )
        {
            Component.BtnUpdateClick(s, e);
        }
    },
    updateFormValues: function () {
        var fields = StoreDailyReport.GetFormFields();
        var path = $('#HOME').val() + 'StoreDailyReport/jsonUpdateForm';
        $.ajax({
            type: 'POST',
            url: path,
            data: {
                'MainPOSWithDraws': fields.MainPOSWithDraws * QUANTITY_MULTIPLIER,
                'OtherExpanses': fields.OtherExpanses * QUANTITY_MULTIPLIER,
                'PaperMoney': fields.PaperMoney * QUANTITY_MULTIPLIER,
                'Coins': fields.Coins * QUANTITY_MULTIPLIER,
                'CollectionComplement': fields.CollectionComplement * QUANTITY_MULTIPLIER,
                'CollectionComplementText': fields.CollectionComplementText,
                'OtherExpansesText': fields.OtherExpansesText
            },
            cache: false,
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();
                StoreDailyReport.SetFormFields(data);
                grdStatistics.PerformCallback();
            },
            error: function (data) {
                setJSError(data.error);
            }
        });
    },
    dailyTotalsGridEndCallback: function (s, e) {
        StoreDailyReport.updateFormValues();
    },
    creditsGridEndCallback: function (s, e) {
        StoreDailyReport.updateFormValues();
    },
    autoDeliverieEndCallback: function (s, e) {
        StoreDailyReport.updateFormValues();
    },
    paymentsGridEndCallback: function (s, e) {
        StoreDailyReport.updateFormValues();
    },
    GetFormFields: function () {
        var fields_list = {};
        fields_list.MainPOSWithDraws = Component.GetName('MainPOSWithdraws') !== null ? Component.GetName('MainPOSWithdraws').GetValue() : null;
        fields_list.OtherExpanses = Component.GetName('OtherExpanses') !== null ? Component.GetName('OtherExpanses').GetValue() : null;
        fields_list.PaperMoney = Component.GetName('PaperMoney') !== null ? Component.GetName('PaperMoney').GetValue() : null;
        fields_list.Coins = Component.GetName('Coins') !== null ? Component.GetName('Coins').GetValue() : "";
        fields_list.CollectionComplement = Component.GetName('CollectionComplement') !== null ? Component.GetName('CollectionComplement').GetValue() : "";
        fields_list.CollectionComplementText = Component.GetName('CollectionComplementText') !== null ? Component.GetName('CollectionComplementText').GetValue() : "";
        fields_list.OtherExpansesText = Component.GetName('OtherExpansesText') !== null ? Component.GetName('OtherExpansesText').GetValue() : "";
        return fields_list;
    },
    SetFormFields: function (data) {
        $.each(data, function (field, val) {
            var item = Component.GetName(field);
            if (item !== null) {
                item.SetValue(val);
            }
        });
    },
    OnFieldValueChanged: function (s, e) {
        StoreDailyReport.updateFormValues();
    },
    OnUserAgreesCheck: function (s, value) {
        grdDailyTotals.PerformCallback(value);
    },
    ShowPopup: function () {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObjectToView);
        }
        else if (selectedItemsArray.length > 1) {
            setJSNotification(pleaseSelectOnlyOneObjectToView);
        }
        else if (selectedItemsArray.length === 1) {
            StoreDailyReport.EmptyCallbackPanels();
            PopupViewCallbackPanel.PerformCallback(selectedItemsArray[0]);
            StoreDailyReport.GetName(gridName).UnselectAllRowsOnPage();
        }
    },
    EmptyCallbackPanels: function () {
        if (typeof PopupViewCallbackPanel !== "undefined") {
            PopupViewCallbackPanel.SetContentHTML("");
        }

        if (typeof PopupEditCallbackPanel !== "undefined") {
            PopupEditCallbackPanel.SetContentHTML("");
        }

        if (typeof documentInfoPanel !== "undefined") {
            documentInfoPanel.SetContentHTML("");
        }
    },
    GetName: function (name) {
        return Component.GetName(name);
    },
    SearchDailyReport: function (s, e) {
        selectedItemsArray = [];
        grdStoreDailyReport.PerformCallback({ callbackparameter: "SEARCH" });
    },
    grdStoreDailyReportBeginCallback: function (s, e) {
        CustomizationWindow();
        e.customArgs.ReportCode = Component.GetName("ReportCode").GetValue();
        e.customArgs.StoreCode = Component.GetName("StoreCode").GetValue();
        e.customArgs.StartDate = Component.GetName("StartDate").GetText();
        e.customArgs.EndDate = Component.GetName("EndDate").GetText();
    },
};