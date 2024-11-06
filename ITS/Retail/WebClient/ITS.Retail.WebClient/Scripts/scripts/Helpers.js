var Component = (function () {
    autoCenterLayoutColumn = function (container) {
        $(container).css("float", 'none');
        $(container).css("margin", 'auto');
        $(container).css("margin-bottom", '20px');
    };
    return {
        GetName: function (name) {
            return ASPxClientControl.GetControlCollection().GetByName(name);
        },
        GetByID: function (name) {
            return document.getElementById(name);
        },
        OnKeyPressFilters: function (s, e) {
            if (e.htmlEvent.keyCode === 13) {
                btnSearch.DoClick();
                s.Focus();
            }
        },
        SubmitItemsToCategory: function (path) {
            $.ajax({
                type: "POST",
                url: path,
                async: false,
                traditional: true,
                data: { "CategoryID": document.getElementById("CategoryID").value, "data": selectedItemsArray },
                success: LoadAddPopup.Hide()
            });
        },
        ClearFilters: function () {
            //clear selected items
            selectedItemsArray = [];
            filterSelected = true;
        },
        ClearAllowedItemsAndSearch: function (s, e) {
            //Clear Selected Items
            selectedItemsArray = [];

            if (typeof gridName === typeof undefined && typeof allowedCustomersSelection !== typeof undefined) {
                //Works on CustomerCategory
                allowedCustomersSelection.PerformCallback("SEARCH");
            }
            else if (typeof gridName === typeof undefined && typeof allowedItemsSelection !== typeof undefined) {
                //Works on ItemCategory
                allowedItemsSelection.PerformCallback("SEARCH");
            }
            else {
                //Works on Customer Index
                Component.GetName(gridName).PerformCallback("SEARCH");
            }
            toolbarHideFiltersOnly();
        },
        SearchSupplier: function (s, e) {
            selectedItemsArray = [];
            Component.GetName(gridName).PerformCallback("SEARCH");
            toolbarHideFiltersOnly();
        },
        PassValuesOnGrid: function (s, e) {
            CustomizationWindow();
            if (e.command.search("APPLYCOLUMNFILTER") !== -1) {
                //clear selected items
                selectedItemsArray = [];
                filterSelected = true;
            }
            if (gridName === "grdCustomer" || gridName === "grdGDPRCustomer" || gridName === "grdGDPRAnonymizeCustomer") {


                if (e.command.search("NEW") >= 0) {
                    e.customArgs.CustomerID = -1;
                }
                else {
                    e.customArgs.CustomerID = Component.GetName(gridName).GetRowKey(Component.GetName(gridName).GetFocusedRowIndex());
                }

                e.customArgs.customer_code = Component.GetName("customer_code").GetValue();
                e.customArgs.card_id = Component.GetName("card_id").GetValue();
                e.customArgs.customer_name = Component.GetName("customer_name").GetValue();
                e.customArgs.customer_tax_number = Component.GetName("customer_tax_number").GetValue();
            }
            else if (gridName === "grdSupplier" || gridName === "grdCompany" || gridName === "grdGDPRExportSupplier" || gridName === "grdGDPRAnonymizeSupplier") {


                if (e.command.search("NEW") >= 0) {
                    e.customArgs.SupplierID = -1;
                }
                else {
                    e.customArgs.SupplierID = Component.GetName(gridName).GetRowKey(Component.GetName(gridName).GetFocusedRowIndex());
                }

                e.customArgs.supplier_code = Component.GetName("supplier_code").GetValue();
                e.customArgs.supplier_name = Component.GetName("supplier_name").GetValue();
                e.customArgs.supplier_tax_number = Component.GetName("supplier_tax_number").GetValue();
            }
            e.customArgs.is_active = Component.GetName("is_active").GetValue();
            e.customArgs.FcreatedOn = Component.GetName("FcreatedOn").GetText();
            e.customArgs.FupdatedOn = Component.GetName("FupdatedOn").GetText();
        },
        PhoneGridOnBeginCallback: function (s, e) {
            jQuery(window).unbind("beforeunload");
            jQuery(window).unbind("unload");

            if (e.command === "ADDNEWROW") {
                $("#PhoneGrid .button_container").hide();
            } else {
                $("#PhoneGrid .button_container").show();
            }
        },
        PhoneGridOnEndCallback: function (s, e) {
            if (typeof bindEvents !== "undefined") {
                bindEvents();
            }

            if (typeof AddressCbPanel !== "undefined" && AddressCbPanel.GetMainElement() !== null) {
                AddressCbPanel.PerformCallback();
            }
        },
        OnAddressEndCalback: function (s, e) {
            if (typeof bindEvents !== "undefined") {
                bindEvents();
            }

            if (typeof SupplierCbPanel !== "undefined" && SupplierCbPanel.GetMainElement() !== null) {
                SupplierCbPanel.PerformCallback();
            }

            if (typeof AddressCbPanel !== typeof undefined && AddressCbPanel.GetMainElement() !== null) {
                AddressCbPanel.PerformCallback();
            }

            if (typeof IsStore !== "undefined" && IsStore.GetMainElement() !== null) {
                Component.StoreCheckChange();
            }

            if (typeof CustomerCbPanel !== "undefined" && CustomerCbPanel.GetMainElement() !== null) {
                CustomerCbPanel.PerformCallback();
            }
        },
        OnAddressBeginCalback: function (s, e) {
            e.customArgs["currentDefaultAddress"] = Component.GetName("DefaultAddress").GetValue();
        },
        StoreCheckChanged: function () {
            StoreCode.SetEnabled(IsStore.GetValue());
            StoreName.SetEnabled(IsStore.GetValue());
            IsCentralStore.SetEnabled(IsStore.GetValue());
            Central.SetEnabled(IsStore.GetValue());
        },
        OnAddressGridFocusedRowChanged: function (s, e) {

            if (s.GetFocusedRowIndex() >= 0) {
                $("#" + s.name.replace("grdAddress", "divGridPhone_")).html($("#divPhone" + s.GetRowKey(s.GetFocusedRowIndex()).replace("-", "_").replace("-", "_").replace("-", "_").replace("-", "_")).html());
            }

        },
        ShowPopup: function () {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObjectToView);
            }
            else if (selectedItemsArray.length > 1) {
                setJSNotification(pleaseSelectOnlyOneObjectToView);
            }
            else if (selectedItemsArray.length === 1) {
                Component.EmptyCallbackPanels();
                PopupViewCallbackPanel.PerformCallback(selectedItemsArray[0]);
                Component.GetName(gridName).UnselectAllRowsOnPage();
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

            if (typeof PopupAssosiatedEditCallbackPanel !== "undefined") {
                PopupAssosiatedEditCallbackPanel.SetContentHTML("");
            }

            if (typeof PopupGenericEditCallbackPanel !== "undefined") {
                PopupGenericEditCallbackPanel.SetContentHTML("");
            }

            if (Component.GetName('customerInfoPanel') !== null) {
                customerInfoPanel = undefined;
            }
            if (Component.GetName('BillingAddressCombobox') !== null) {
                BillingAddressCombobox = undefined;
            }
            if (Component.GetName('supplierInfoPanel') !== null) {
                supplierInfoPanel = undefined;
            }
            if (Component.GetName('PriceCatalogPolicyCb') !== null) {
                PriceCatalogPolicyCb = undefined;
            }

            if (Component.GetName('DocumentPriceCatalogCbPanel') !== null) {
                DocumentPriceCatalogCbPanel = undefined;
            }
            if (Component.GetName('storeSuppliers') !== null) {
                storeSuppliers = undefined;
            }
            if (Component.GetName('storeCustomers') !== null) {
                storeCustomers = undefined;
            }

            if (Component.GetName('secondaryStores') !== null) {
                secondaryStores = undefined;
            }

            if (typeof itemPanel !== "undefined") {
                itemPanel = undefined;
            }
            if (typeof itemPanelPurchase !== "undefined") {
                itemPanelPurchase = undefined;
            }
            if (typeof itemPanelStore !== "undefined") {
                itemPanelStore = undefined;
            }

            if (typeof Component.GetName('DeliveryAddress') !== "undefined") {
                DeliveryAddress = undefined;
            }

            if (typeof Component.GetName('HasBeenExecuted') !== "undefined") {
                HasBeenExecuted = undefined;
            }
            if (typeof Component.GetName('HasBeenChecked') !== "undefined") {
                HasBeenChecked = undefined;
            }

            if (typeof Component.GetName('TransferMethod') !== "undefined") {
                TransferMethod = undefined;
            }
            if (typeof Component.GetName('DocNumber') !== "undefined") {
                DocNumber = undefined;
            }

            if (typeof Component.GetName('PlaceOfLoading') !== "undefined") {
                PlaceOfLoading = undefined;
            }

            if (typeof Component.GetName('VehicleNumber') !== "undefined") {
                VehicleNumber = undefined;
            }

            if (typeof Component.GetName('TransferPurpose') !== "undefined") {
                TransferPurpose = undefined;
            }

            if (typeof Component.GetName('RefferenceDate') !== "undefined") {
                RefferenceDate = undefined;
            }

            if (typeof (customerInfoPanel) !== "undefined") {
                customerInfoPanel = undefined;
            }
            if (typeof (supplierInfoPanel) !== "undefined") {
                supplierInfoPanel = undefined;
            }
            if (typeof (vatAnalysisInfoPanel) !== "undefined") {
                vatAnalysisInfoPanel = undefined;
            }
            if (typeof (documentSummaryPanel) != "undefined") {
                documentSummaryPanel = undefined;
            }
            if (typeof (documentSummaryPanelPartial) != "undefined") {
                documentSummaryPanelPartial = undefined;
            }
        },
        EmptyAddCallbackPanel: function () {
            if (typeof PopupAddCallbackPanel !== "undefined") {
                PopupAddCallbackPanel.SetContentHTML("");
            }
        },
        OnColumnResizing: function (s, e) {
            if (e.column.name === "hidden") {
                e.cancel = true;
            }
        },
        InitializeFilterNonRelative: function () {
            $(".wrapper .container #FilterPanel #btnSearch").click(function () {
                toolTipText = FillCriteriaInfo();
                if (toolTipText !== null) {
                    var left = parseInt($('.wrapper .container .filter_search_container').css('left'));
                    $('.wrapper .container #filterinfo').css('left', left + 110).fadeIn();
                }
            });
            $(".wrapper .container #FilterPanel span#FilterPanel_RPHT").click(function () {
                toolbarHideFilters();
            });
            $('.wrapper .container .dxgvControl_ITSTheme1').addClass('styleGrid');
            $('.wrapper .container .filter_search_container').css('left', $('.wrapper .container #FilterPanel_RPHT').width() + 55);

            $(".wrapper .container #btnClear").click(function () {
                $('.wrapper .container #filterinfo').fadeOut();
            });
        },
        RefreshGridMemberOfNode: function (s, e) {
            var grid = Component.GetName("grdMembersOfNode");
            if (grid !== null) {
                grid.Refresh();
            }
        },
        BtnUpdateClick: function (s, e, page) {
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
                        if (page === "PriceCatalog") {
                            window.location.reload();
                        }
                    }
                }
            });
        },
        ValidateAndSubmitForm: function (s, e, gridName) {
            s.SetEnabled(false);

            var form = $('#' + s.name).parent().parent().parent();
            var table = form.find('table').first();
            table.attr('id', 'formId');
            var validation = ASPxClientEdit.ValidateEditorsInContainerById('formId');
            table.attr('id', '');
            if (validation) {
                if (typeof gridName !== "undefined") {
                    gridName.UpdateEdit();
                }
                else {
                    Component.BtnUpdateClick(s, e);
                }
                s.SetEnabled(true);
                return validation;
            }
            s.SetEnabled(true);
            setJSError(markedFieldsAreRequired);
            return validation;
        },
        OnInitPopupEditCallbackPanel: function (s, e) {
            if (selectedItemsArray.length === 0) {
                //AddNewItem Situation
                e.customArgs.ID = "00000000-0000-0000-0000-000000000000";
                if (typeof objectToBeCopied !== "undefined") {
                    e.customArgs.Copy = objectToBeCopied;
                }
            }
            else {

                e.customArgs.ID = selectedItemsArray[0];
                selectedItemsArray = [];

            }
        },
        ExternalEdit: function () {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObjectToEdit);
            }
            else if (selectedItemsArray.length > 1) {
                setJSNotification(pleaseSelectOnlyOneObjectToEdit);
            }
            else if (selectedItemsArray.length === 1) {
                if (UserHasOwnerPermition(selectedItemsArray[0])) {

                    window.open(window.location.href + '/Edit?Oid=' + selectedItemsArray[0], '_top');

                    return false;

                }
                else {
                    setJSNotification(youCannotEditThisElement);
                }
            }
        },
        ExternalAdd: function () {
            window.open(window.location.href + '/Edit?Oid=00000000-0000-0000-0000-000000000000', '_top');
        },
        SubmitForm: function () {
            var form_to_submit = Component.GetCorrectForm(document.forms[0]);
            form_to_submit.submit();
        },
        ValidateAndSubmitExternalForm: function () {
            var validation = ASPxClientEdit.ValidateEditorsInContainerById($("form").attr('id'));
            if (validation) {
                Component.SubmitForm();
            }
            else {
                setJSError(markedFieldsAreRequired);
            }
        },
        PositionDocumentPartialAtHeader: function () {
            /**
            * 1st case where we have width larger or equal to 1408
            **/
            if (window.innerWidth >= 1408) {

                ///**
                //* Set Right Columnm Height equal to Left Column and the opposite 
                //**/
                //if ($("#documentSummaryPanelPartial .style_table").height() < $("#FormDocHeaderAdvanced").height()) {
                //    $("#documentSummaryPanelPartial .style_table").height($("#FormDocHeaderAdvanced").height() + 16);
                //}
                //else {
                //    $("#FormDocHeaderAdvanced").height($("#documentSummaryPanelPartial .style_table").height() - 16);
                //}

                /**
                * Calculate left margin for both Columns to center them on screen
                **/
                var leftMargin = (window.innerWidth - $("#documentSummaryPanelPartial .style_table").width() - $("#FormDocHeaderAdvanced").width() - $("#updatedByInfoPanel .style_table").width() - 80) / 2;
                /**
                * Set Margin Left on left Column
                **/
                $("#FormDocHeaderAdvanced").css("margin-left", leftMargin);
            }
            /**
            *Second case where we have smaller screen sizes 
            **/
            else {
                /**
                * Center both Columns one on top of the other
                **/
                autoCenterLayoutColumn("#FormDocHeaderAdvanced");
                autoCenterLayoutColumn("#documentSummaryPanelPartial .style_table");
            }
        },
        TraderExists: function (data) {
            if (data === false) {
                return;
            }
            var res = confirm(data.confirm_message);
            if (!res) {
                if (data.triggered_by == "TaxCode") {
                    TaxCode.SetValue("");
                }
                if (data.triggered_by == "Code") {
                    Code.SetValue("");
                }
                return;
            }

            selectedItemsArray = [];
            if (data.supplier_id !== "" && data.controller == "supplier") {
                selectedItemsArray.push(data.supplier_id);
            }
            else if (data.customer_id !== "" && data.controller == "customer") {
                selectedItemsArray.push(data.customer_id);
            }
            else if (data.supplier_id !== "" && data.controller == "customer") {
                selectedItemsArray.push(data.supplier_id);
            }
            else if (data.customer_id !== "" && data.controller == "supplier") {
                selectedItemsArray.push(data.customer_id);
            }
            //else {
            //    selectedItemsArray.push(data.TraderID);
            //}
            Component.EmptyCallbackPanels();
            PopupEditCallbackPanel.PerformCallback();
        },
        ToggleSelectAll: function () {
            var wrappedContainer = $('.wrapper .container');
            wrappedContainer.find('.sellect_all_box span').bind('click', function () {
                if (wrappedContainer.find('table .firstCollumn').find('.dxWeb_edtCheckBoxUnchecked_ITSTheme1').length > 0) {
                    if (typeof gridName !== "undefined") {
                        Component.GetName(gridName).SelectAllRowsOnPage(true);
                    }
                    wrappedContainer.find('span.sellect_all_box span').addClass('dxWeb_edtCheckBoxChecked_ITSTheme1');
                } else {
                    if (typeof gridName !== "undefined") {
                        Component.GetName(gridName).SelectAllRowsOnPage(false);
                    }
                    wrappedContainer.find('span.sellect_all_box span').removeClass('dxWeb_edtCheckBoxChecked_ITSTheme1').addClass('dxWeb_edtCheckBoxUnchecked_ITSTheme1');
                }
            });
            if (wrappedContainer.find('table .firstCollumn').find('.dxWeb_edtCheckBoxUnchecked_ITSTheme1').length > 0) {
                return wrappedContainer.find('span.sellect_all_box span').removeClass('dxWeb_edtCheckBoxChecked_ITSTheme1').addClass('dxWeb_edtCheckBoxUnchecked_ITSTheme1');
            } else if (wrappedContainer.find('table .firstCollumn').children().length > 0) {
                return wrappedContainer.find('span.sellect_all_box span').addClass('dxWeb_edtCheckBoxChecked_ITSTheme1');
            }
        },
        GetCorrectForm: function (form) {
            return form.action === "" ? document.forms[1] : form;
        },
        EditSelectedObjectOnFullPage: function () {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObjectToEdit);
            }
            else if (selectedItemsArray.length > 1) {
                setJSNotification(pleaseSelectOnlyOneObjectToEdit);
            }
            else if (selectedItemsArray.length === 1) {
                if (UserHasOwnerPermition(selectedItemsArray[0])) {
                    var controllerSeperator = $("#HOME").val().endsWith('/') ? '' : '/';
                    window.location.href = $("#HOME").val() + controllerSeperator + ITScontroller + '/' + editAction + '?' + editIDParameter + '=' + selectedItemsArray[0];
                    return false;
                }
                else {
                    setJSNotification(youCannotEditThisElement);
                }
            }
        }
        //OnDataViewsCategoryChanged: function (s, e) {
        //    CustomDataView.PerformCallback({ categoryOid: Component.GetName("CustomDataViewsCategory").GetValue() });
        //},
        //DisplayVariableValues: function (s, e) {
        //    $("#entityOids").val(selectedItemsArray.toString() == "" ? ( Component.GetByID("ID") != null ? Component.GetByID("ID").value : "") : selectedItemsArray.toString());
        //    $("#customDataViewOid").val(Component.GetName("CustomDataView").GetValue());
        //    $("#varvalues-form").submit();
        //},
        //handleError: function (data) {
        //    setJSError(data.error);
        //},
        //ShowVariableValues: function() {
        //    if (selectedItemsArray.length === 0) {
        //        setJSNotification(pleaseSelectAnObjectToCopy);
        //    }
        //    else {
        //            VariableValuesPopUp.Show();
        //    }
        //},
        //VariableValuesPopUpClose: function() {
        //    VariableValuesPopUp.Hide();
        //},
    };
})();
