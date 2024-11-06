$(document).ready(function () {

    if ($('.wrapper .container #FilterPanelSecond').length > 0) {

        $(".wrapper .container #FilterPanelSecond span#FilterPanelSecond_RPHT").click(function () {
            Cashier.HideFilters();
        });

        $('.wrapper .container .dxgvControl_ITSTheme1').addClass('styleGrid');
        $('.wrapper .container .filter_search_container').css('left', $('.wrapper .container #FilterPanel_RPHT').width() + 55);
    }
});


var Cashier = (function () {
    OnKeyPressFiltersPrivate = function (s, e, btnName) {
        if (e.htmlEvent.keyCode === 13) {
            Component.GetName(btnName).DoClick();
            s.Focus();
        }
    };

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
        },        
        ClearAllFilters: function () {
            $('.FilterPanel #FilterPanelSecond_RPC .search_filter').each(function (index) {
                Component.GetName($(this).attr('id')).SetValue(null);
            });
        },
        HideFilters: function () {
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
        },
        CashierRegisterItemsBeginCallback: function (s, e) {
            if (filterSelected === true) {
                s.UnselectRows();
                filterSelected = false;
            }

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
                e.customArgs['Cashier'] = Component.GetName("CashierRegister").GetValue();

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
        },
        SendPayments: function () {
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            var path = $("#HOME").val() + 'CashierRegister/AddPaymentMethods';
            $.ajax({
                type: 'POST',
                url: path,
                data: { cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (data.success === 'success') {

                    }
                    else {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data.error);
                }
            });
        },
        SendItems: function () {
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }

            if (selectedItemsArray.length <= 0) {
                setJSNotification(pleaseSelectAtLeastOneObject);
                return;
            }
            
            var path = $("#HOME").val() + 'CashierRegister/SendItemsToDevice';
            $.ajax({
                type: 'POST',
                url: path,
                data: { SelectedItems: selectedItemsArray.toString(), cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (typeof (data.error) !== typeof (undefined) && data.error != '') {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data);
                }
            });
        },
        ClearAllItems: function()
        {
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            var path = $("#HOME").val() + 'CashierRegister/ClearAllItems';
            $.ajax({
                type: 'POST',
                url: path,
                data: { cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (typeof (data.error) !== typeof (undefined) && data.error != '') {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data);
                }
            });
        },
        ClearSelectedItems: function()
        {
            if (selectedItemsArray.length <= 0) {
                return;
            }

            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            var path = $("#HOME").val() +'CashierRegister/ClearSelectedItems';
            $.ajax({
                type: 'POST',
                url: path,
                data: { SelectedItems: selectedItemsArray.toString(), cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (data.success === 'success') {

                    }
                    else {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data);
                }
            });
        },
        IssueZ: function()
        {
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            var path = $("#HOME").val() +'CashierRegister/ZIssue';
            $.ajax({
                type: 'POST',
                url: path,
                data: {cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (data.success === 'success') {

                    }
                    else {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data);
                }
            });
        },
        IssueX: function() {
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            var path = $("#HOME").val() +'CashierRegister/XIssue';
            $.ajax({
                type: 'POST',
                url: path,
                data: { cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    grdCashierRegisterItems.PerformCallback("CLEAR");
                    if (data.success === 'success') {

                    }
                    else {
                        setJSError(data.error);
                    }
                },
                error: function (data) {
                    setJSError(data);
                }
            });
        },
        DailyTotal: function()
        {
            DialogCallbackPanel.PerformCallback('DAILY');
        },
        LoadGridDailySales: function()
        {
            var path = $("#HOME").val() +'CashierRegister/DailyTotalGrid';
            var oid = CashierRegister.GetValue();
            if (oid == null) {
                setJSError(pleaseSelectACashierRegister);
                return;
            }
            $.ajax({
                type: 'POST',
                url: path,
                data: { cashierOid: oid },
                cache: false,
                dataType: 'json',
                success: function (data) {

                },
                error: function (data) {
           
                }
            });
        },
        DailyItemsSales: function() {
            //Shows pop up with daily totals
            DialogCallbackPanel.PerformCallback('DAILY_ITEMS');
        },
        Search: function (s, e) {

            if (!ValidateModalFormSingle()) {
                return;
            }
            grdCashierRegisterItems.PerformCallback("SEARCH" + s.name);
            toolbarHideFiltersOnly();
        }
    };
})();
