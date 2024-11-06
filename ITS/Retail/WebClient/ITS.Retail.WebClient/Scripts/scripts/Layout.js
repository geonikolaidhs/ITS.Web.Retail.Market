var PerformGridOfNodesCallback,
    SelectedNodeOid = -1,
    SelectedNodeText = "",
    gridSellectAll = false,
    legacyModeWindowOpened = false,
    legacyChildWindow = null,
    sliders = null,
    msg = null,
    toolTipText = null,
    clopy = false,
    legacyModeWindowOpened = false,
    isChrome = !!window.chrome,
    filterSelected = false,
    selectedGridName = null,
    selectedItemsArray = [],
    Dictionary = (function () {
        function Dictionary() {
            if (!(this instanceof Dictionary))
                return new Dictionary();
        }

        Dictionary.prototype.Count = function () {
            var key,
                count = 0;

            for (key in this) {
                if (this.hasOwnProperty(key))
                    count += 1;
            }
            return count;
        };

        Dictionary.prototype.Keys = function () {
            var key,
                keys = [];

            for (key in this) {
                if (this.hasOwnProperty(key))
                    keys.push(key);
            }
            return keys;
        };

        Dictionary.prototype.Values = function () {
            var key,
                values = [];

            for (key in this) {
                if (this.hasOwnProperty(key))
                    values.push(this[key]);
            }
            return values;
        };

        Dictionary.prototype.KeyValuePairs = function () {
            var key,
                keyValuePairs = [];

            for (key in this) {
                if (this.hasOwnProperty(key))
                    keyValuePairs.push({
                        Key: key,
                        Value: this[key]
                    });
            }
            return keyValuePairs;
        };

        Dictionary.prototype.Add = function (key, value) {
            this[key] = value;
        };

        Dictionary.prototype.Clear = function () {
            var key,
                dummy;

            for (key in this) {
                if (this.hasOwnProperty(key))
                    dummy = delete this[key];
            }
        };

        Dictionary.prototype.ContainsKey = function (key) {
            return this.hasOwnProperty(key);
        };

        Dictionary.prototype.ContainsValue = function (value) {
            var key;

            for (key in this) {
                if (this.hasOwnProperty(key) && this[key] === value)
                    return true;
            }
            return false;
        };

        Dictionary.prototype.Remove = function (key) {
            var dummy;

            if (this.hasOwnProperty(key)) {
                dummy = delete this[key];
                return true;
            } else
                return false;
        };

        return Dictionary;
    }());


$.cookie.raw = true;

$(document).ready(function () {

    if ($(".wrapper .header .userInformation").width() > $(".wrapper .header .header_container").width() * 0.5) {
        $("body").css('min-width', '960px');
    }



    $("#DocumentsPanel").css("height", $("#lookupFieldsPanel").height());
    var hyperlink = $("#linkMailTo");
    var text = hyperlink.text();
    var address = text.replace(/\[to\]/, "@@");
    hyperlink.attr('href', 'mailto:' + address);

    if ($('.wrapper .fast_menu_options').length > 0) {
        $(".wrapper .fast_menu_options_btn").show();
        $('.wrapper .fast_menu_options').delay(500).animate({ top: "50px" }, 500, "linear").show();
        $(".wrapper .fast_menu_options_btn").click(function () {
            if ($(this).hasClass('arrow_down_color')) {
                ShowToolbar();
            }
            else {
                HideToolbar();
            }

        });
    }

    $(document).on('click', '#settings .title_settings', function () {
        var rightContainer = $("#settings").parent();
        if (rightContainer.has('.col-lg-4-5').length > 0) {
            $("#settings .second").hide(function () {
                rightContainer.children('.col-lg-4-5').removeClass('col-lg-4-5').addClass('col-1');
                $("#settings .shown").hide();
                $("#settings .hidden").show();
                RefreshGrid();
            });
        }
        else {
            $("#settings .hidden").hide();
            $("#settings .shown").show();
            $("#settings .second").show();

            rightContainer.children('.col-1').removeClass('col-1').addClass('col-lg-4-5');
            RefreshGrid();
        }
    });


    //if ($(" .wrapper .container #settings h4.title_settings span").length > 0) {
    //    $('.wrapper .fast_menu_options #gear').show();
    //}

    //$(document).on('click', '.wrapper .fast_menu_options #gear', function () {
    //    if ($(" .wrapper .container #settings").css('marginLeft') != '0px') {
    //        $(" .wrapper .container #settings").stop('true', 'true').animate({ marginLeft: "0px" }, 500, "linear");

    //        $(" .wrapper .container .settingsPage ").animate({ marginLeft: "315px" }, 500, "linear", function () {

    //            RefreshGrid();

    //        });

    //    }
    //    else {
    //        $(" .wrapper .container #settings").animate({ marginLeft: "-350px" }, 500, "linear");

    //        $(" .wrapper .container .settingsPage ").animate({ marginLeft: "0px" }, 500, "linear", function () {

    //            RefreshGrid();

    //        });
    //    }
    //});

    //if ($('#connectedUsers').html() !== null) {
    //    PollUsers();
    //    //setInterval(PollUsers, 30000);
    //}

    $('.close_btn').click(function (e) {
        $(this).parent().stop().hide();
        $('.hide_all').stop().hide();
        if ($(".wrapper .fast_menu_options").length > 0) {
            $(".wrapper .fast_menu_options").show();
        }
        $('.IndexView_button_container').show();
        e.stopPropagation();
    });


    $('.disabled').click(function () { return false; });

    $('#settings .subtitle').click(function () {

        $(this).parent().find('.options li').slideToggle();

        if ($(this).find('h4 span').hasClass('maximaze')) {
            $('#settings .subtitle').find("h4 span").removeClass('maximaze');
            $(this).find('h4 span').removeClass('maximaze');
        }
        else {
            $('#settings .subtitle').find("h4 span").removeClass('maximaze');
            $(this).parent().find(' h4 span').addClass('maximaze');
        }
        $(this).parent().siblings().find('.options li').slideUp();
    });

    $(document).click(function () {
        $('.header .header_container .menu').hide();
        $('.menu_hide').hide();
        if ($(".wrapper .fast_menu_options").length > 0) {
            $(".wrapper .fast_menu_options").show();
        }
        $('.wrapper .header_container .dropdown').find('ul').stop().slideUp();
    });

    var max = 0;
    var submenus = 0;
    $('.header .header_container .menu ul.megamenu ul').each(function (index) {
        submenus++;
        if ($(this).children().length > max) {
            max = $(this).children().length;
        }
    });
    if (submenus < max) {
        $('.header .header_container .menu ul.megamenu').height(max * 32);
    }

    $('.header .header_container .menu ul.active').parent().find('h3').addClass('color-blue');

    $('.header .header_container .menu li h3').click(function (e) {
        $('.header .header_container .menu li h3').removeClass('color-blue');
        $(this).addClass('color-blue');
        $('.header .header_container .menu li ul').removeClass('active');
        $(this).parent().find('ul').addClass('active');
        e.stopPropagation();
    });

    $(document).on('click', '.header .header_container div.navmenu', function (e) {
        if ($('.header .header_container .menu').is(':visible')) {
            $('.IndexView_button_container').delay(300).show();
            $('.header .header_container .menu').hide();
            $('.hide_all').hide();
        } else {
            $('.IndexView_button_container').hide();
            $('.header .header_container .menu').stop().show();
            $('.hide_all').show();
        }
        $('.wrapper .header_container .dropdown').find('ul').stop().slideUp();
        e.stopPropagation();
    });

    $(document).ready(function () {
        if ($('.header .header_container .menu .megamenu li ul li.active')) {
            var close = $('.header .header_container .menu .megamenu li ul li.active').closest('ul').addClass('active');
            close.parent('li').children('h3.title').addClass('color-blue');

        }

    });

    $('.wrapper .header_container .dropdown').click(function (e) {
        $('.wrapper .header_container .dropdown').find('ul').stop().slideUp();
        $(this).find('ul').stop().slideToggle();
        e.stopPropagation();
    });

    if ($('.wrapper .container table.FilterPanel').length > 0 && $('.wrapper .container table.FilterPanel.relative').length === 0) {
        Component.InitializeFilterNonRelative();
    }


    if ($('.wrapper .container table.FilterPanel').length > 0 && $('.wrapper .container table.FilterPanel.relative').length > 0) {
        $(".wrapper .container #FilterPanel #btnSearch").click(function () {
            toolTipText = FillCriteriaInfo();
            if (toolTipText !== null) {
                var left = parseInt($('.wrapper .container .filter_search_container').css('left'));
                $('.wrapper .container #filterinfo').css('left', left + 55).fadeIn();
            }
        });

        $('.wrapper .container .dxgvControl_ITSTheme1').addClass('styleGrid');
        $('.wrapper .container .filter_search_container').css('left', $('.wrapper .container #FilterPanel_RPHT').width() + 5);

        $(".wrapper .container #btnClear").click(function () {
            $('.wrapper .container #filterinfo').fadeOut();
        });
    }

    var flag = false;
    $('#headerMessagePanel').hide();
    $('#footerMessagePanel').hide();
    $('#footerMessagePanel div').each(function () {
        if ($(this).html() === "") {
            $(this).hide();
            flag = true;
        }
        else {
            flag = false;
        }
        if (!flag) {
            $('#footerMessagePanel').show();
        }
    });

    $('#headerMessagePanel').hide();
    $('#headerMessagePanel div').each(function () {
        if ($(this).html() === "") {
            $(this).hide();
            flag = true;
        }
        else {
            flag = false;
        }
        if (!flag) {
            $('#headerMessagePanel').show();
        }
    });

    $('#btnsearch').focus();

    if ($(".wrapper_init .info_banner #slider").length) {
        $(".wrapper_init .info_banner #slider").responsiveSlides({
            auto: true,                 // Boolean: Animate automatically, true or false
            speed: 500,                 // Integer: Speed of the transition, in milliseconds
            timeout: 3000,              // Integer: Time between slide transitions, in milliseconds
            pager: false,               // Boolean: Show pager, true or false
            nav: false,                 // Boolean: Show navigation, true or false
            random: false,              // Boolean: Randomize the order of the slides, true or false
            pause: false,               // Boolean: Pause on hover, true or false
            pauseControls: false,       // Boolean: Pause when hovering controls, true or false
            prevText: "",               // String: Text for the "previous" button
            nextText: "",               // String: Text for the "next" button
            maxwidth: "",               // Integer: Max-width of the slideshow, in pixels
            navContainer: "",           // Selector: Where controls should be appended to, default is after the 'ul'
            manualControls: "",         // Selector: Declare custom pager navigation
            namespace: "",              // String: Change the default namespace used
            before: function () { },    // Function: Before callback
            after: function () { }      // Function: After callback
        });
    }

    $('.wrapper #document_wrapper .arrows_left').bind('click', function () {
        $('.wrapper #document_wrapper .carousel-content:first:animated').stop(true, true);
        sliderMotionsLeft();
    });

    $('.wrapper #document_wrapper .arrows_right').bind('click', function () {
        $('.wrapper #document_wrapper .carousel-content:first:animated').stop(true, true);
        sliderMotionsRight();
    });

    $('.wrapper #document_wrapper .view_carousel').click(function () {
        $('.wrapper #document_wrapper ').css('z-index', '1');
    });

    $('.wrapper #document_wrapper #close_btn').click(function () {
        $('.wrapper #document_wrapper').css('z-index', '0');
    });


    if ($('.wrapper .fast_menu_options .options .moreMenu').children().length === 0) {
        $('.wrapper .fast_menu_options .options').hide();
    }
    $('.wrapper .fast_menu_options .options').click(function () {
        $(this).find('.moreMenu').stop().slideToggle();
    });

    $(document).tooltip({
        items: "img, [key], [title]",
        content: function () {
            var element = $(this);
            if (element.parent().attr('id') == 'ItemsChart' || element.parent().attr('id') == 'ItemCategoriesChart' || element.parent().attr('id') == 'CustomersChart') {
                return element.attr("");
            }
            else if (element.is("[key]")) {
                $('.ui-tooltip').remove();//remove the tool tip element
                return toolTipText;
            }
            else if (element.is("[title]")) {
                $('.ui-tooltip').remove();//remove the tool tip element
                return element.attr("title");
            }
            else if (element.is("img") && element.attr('alt') != 'v') {
                $('.ui-tooltip').remove();//remove the tool tip element
                return element.attr("alt");
            }
        }
    });


    $(document).ajaxComplete(function (ev, req, opt) {
        if (opt.url.toUpperCase().indexOf("JSONNOTIFYUSER") < 0 && opt.url.toUpperCase().indexOf("USERCONNECTED") < 0 &&
            opt.url.toUpperCase().indexOf("DOCUMENTINFOPANEL") < 0 && opt.url.toUpperCase().indexOf("JSONGETSUPPLIERSTORES") < 0) {
            updateNotifications(req.responseJSON);
        }
    });

    if ($('.settingsPage').length > 0) {
        UpdateSettingsBar($('.settingsPage'), "");
    }

    if ($('.Reports_CustomReport').length > 0) {
        var id = window.location.href.split("?")[1];
        UpdateSettingsBar($('.Reports_CustomReport'), "?" + id);
    }

});

/**
 * @description Function to load with animation the Settings Menu.
 */
$(window).bind('load', function () {
    if ($('#settings').length === 1) {
        $('#settings').show(function () {
            $('#settings').removeClass('hidden');
        });
    }
});

function UpdateTemporaryFilterForm() {
    if ($("#temporaryFilterContainer").length === 1) {
        $.ajax({
            url: $("#HOME").val() + "Document/TemporaryFilter",
            type: "POST",
            data: { ModeForTemporaryForm: $("#ModeForTemporaryForm").val() }
        })
            .done(function (partialViewResult) {
                $("#temporaryFilterContainer").html(partialViewResult);
                OnTemporaryClick();
            });

    }

}

function UpdateSettingsBar(container, id) {
    if (container.attr('Class').split(" ")[0] == ITScontroller && container.attr('Class').split(" ")[1] == action/*$('#controller').val() + "_" + $('#action').val()*/) {

        $('#settings ul li a').each(function () {
            var str = $(this).attr('href').split("/");
            if (str[str.length - 2] + "/" + str[str.length - 1] == ITScontroller + "/" + action + id) {//$('#controller').val() + "/" + $('#action').val() + id) {
                $(this).parent().show();
                $(this).parent().parent().find('span').addClass('maximaze');
                $(this).parent().siblings().show();
                $(this).css('background-color', '#06A7FF');
                $(this).css('color', '#fff');
            }
        });
    }
}

function RefreshGrid() {
    if (typeof (gridName) !== "undefined") {
        var grid = Component.GetName(gridName);
        if (grid !== null) {
            grid.Refresh();
            if (grid.name === "grdDocument") {
                $("#filter-form").submit();
            }
        }
    }
}

String.prototype.replaceAll = function (search, replace) {
    //if replace is null, return original string otherwise it will
    //replace search string with 'undefined'.
    if (!replace) return this;

    return this.replace(new RegExp('[' + search + ']', 'g'), replace);
};



function SelectionChanged(s, e) {

    if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
    }
    else {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
    }

    if (!e.isSelected && e.visibleIndex == "-1") {
        selectedGridName = s.name;
        Component.GetName(selectedGridName).ShowLoadingDivAndPanel();
        s.GetSelectedFieldValues("Oid", OnGetValues);
    }

    if (!e.isAllRecordsOnPage && !e.isChangedOnServer) {
        if (e.isSelected) {
            selectedItemsArray.push(s.GetRowKey(e.visibleIndex));
        }
        else {
            selectedItemsArray.splice(selectedItemsArray.indexOf(s.GetRowKey(e.visibleIndex)), 1);
        }
    }
    else if (!e.isChangedOnServer) {
        var i;
        if (e.isSelected) {
            for (i = 0; i < s.keys.length; i++) {
                if (selectedItemsArray.indexOf(s.keys[i]) == -1) {
                    selectedItemsArray.push(s.keys[i]);
                }
            }
        }
        else {
            for (i = 0; i < s.keys.length; i++) {
                selectedItemsArray.splice(selectedItemsArray.indexOf((s.keys[i])), 1);
            }
        }
    }

    if (selectedItemsArray.length > 0) {
        ShowToolbar();
    }
    else {
        HideToolbar();
    }
}

function OnGetValues(values) {
    selectedItemsArray = [];
    for (var i = 0; i < values.length; i++) {
        selectedItemsArray.push(values[i]);
    }

    if (selectedItemsArray.length > 0) {
        ShowToolbar();
    }
    else {
        HideToolbar();
    }
    Component.GetName(selectedGridName).HideLoadingPanel();
    Component.GetName(selectedGridName).HideLoadingDiv();
}

function grdOnEndCalback(s, e) {
    $(".wrapper .container .sellect_all_box span").bind("click", function () {
        if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
            if (typeof gridName !== typeof undefined) {
                Component.GetName(gridName).SelectAllRowsOnPage(true);
            }
            $(".wrapper .container span.sellect_all_box span").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
        }
        else {
            if (typeof gridName !== typeof undefined) {
                Component.GetName(gridName).SelectAllRowsOnPage(false);
            }
            $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
        }
    });

    if ($(".wrapper .container table .firstCollumn").find(".dxWeb_edtCheckBoxUnchecked_ITSTheme1").length > 0) {
        $(".wrapper .container span.sellect_all_box span").removeClass("dxWeb_edtCheckBoxChecked_ITSTheme1").addClass("dxWeb_edtCheckBoxUnchecked_ITSTheme1");
    }
    else if ($(".wrapper .container table .firstCollumn").children().length > 0) {
        $(".wrapper .container span.sellect_all_box span").addClass("dxWeb_edtCheckBoxChecked_ITSTheme1");
    }
    CustomizationWindow(s, e);
    if (filterSelected === true) {
        s.UnselectRows();
        filterSelected = false;
    }
}

function toolbarHideFilters() {
    toolTipText = FillCriteriaInfo();
    if ($('.FilterPanel #FilterPanel_RPC').is(':visible')) {
        $('.FilterPanel #FilterPanel_RPC').slideToggle("200", function () {
            $('.wrapper .container table.FilterPanel td.dxrpHeader_ITSTheme1 span').addClass("up");
        });
    } else {
        $('.FilterPanel #FilterPanel_RPC').slideToggle("slow", function () {
            $('.wrapper .container table.FilterPanel td.dxrpHeader_ITSTheme1 span').removeClass("up");
        });
    }
}

function toolbarHideFiltersOnly() {
    toolTipText = FillCriteriaInfo();
    if ($('.FilterPanel #FilterPanel_RPC').is(':visible')) {
        $('.FilterPanel #FilterPanel_RPC').slideUp("200", function () {
            $('.wrapper .container table.FilterPanel td.dxrpHeader_ITSTheme1 span').addClass("up");
        });
    }
}

function clearAllFilters() {
    $('.FilterPanel #FilterPanel_RPC .search_filter').each(function (index) {
        Component.GetName($(this).attr('id')).SetValue(null);
    });
}




//function PollUsers() {

//    $.ajax({
//        type: 'POST',
//        url: $("#HOME").val() + "Base/UserConnected",
//        data: {},
//        cache: false,
//        dataType: 'json',
//        success: function (data) {
//            $('#connectedUsers').html(data.count + ' user(s) connected. Application mode:' + data.applicationInstance + "." + ((data.userNames.length > 1) ? " Registered Users:" + data.userNames : ""));
//        }
//    });
//}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}


function chooseLanguage(newLocale) {

    if ($.cookie("_culture") === null && newLocale == Globalize.culture().name.toLowerCase()) return;
    if ($.cookie("_culture") == newLocale) return;
    $.cookie("_culture", newLocale, { expires: 365, path: '/' });
    window.location.reload();
}

function updateNotifications(response) {
    if (typeof (response) !== "undefined" && (typeof (response.ApplicationError) !== "undefined" || typeof (response.ApplicationNotice) !== "undefined")) {
        if (response.ApplicationError !== null && response.ApplicationError !== "" && response.ApplicationError.length > 0) {
            setJSError(response.ApplicationError);
        }
        if (response.ApplicationNotice !== null && response.ApplicationNotice !== "" && response.ApplicationNotice.length > 0) {
            setJSNotification(response.ApplicationNotice);
        }
        return;
    }
    var path = $("#HOME").val() + "Notification/jsonNotifyUser";
    $.ajax({
        type: 'POST',
        url: path,
        data: {},
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.error !== null) {
                setJSError(data.error);
            }
            if (data.notification !== null) {
                setJSNotification(data.notification);
            }
        }
    });
}

function setJSNotification(message) {
    if (msg != message) {
        msg = message;
        var n = noty({
            text: message,
            layout: 'topCenter',
            type: 'information',
            timeout: 6000,
            callback: {
                onShow: function () { },
                afterShow: function () { },
                onClose: function () { notyClosed(message); },
                afterClose: function () { }
            },
            maxVisible: 10,
            animation: {
                open: 'animated bounceInRight',
                close: 'animated bounceOutRight',
                easing: 'swing',
                speed: 500
            }
        });
    }
}

function setJSError(message) {
    if (msg != message) {
        msg = message;
        var n = noty({
            text: message,
            type: 'error',
            layout: 'topCenter',
            timeout: 6000,
            callback: {
                onShow: function () { },
                afterShow: function () { },
                onClose: function () { notyClosed(message); },
                afterClose: function () { }
            },
            maxVisible: 10,
            animation: {
                open: 'animated bounceInLeft',
                close: 'animated bounceOutLeft',
                easing: 'swing',
                speed: 500
            }
        });
    }
}

function notyClosed(message) {
    if (msg == message) {
        msg = null;
    }
}

//function sleepScript(millis) {
//    var date = new Date();
//    var curDate = null;
//    do { curDate = new Date(); }
//    while (curDate - date < millis);
//}

function beforeLoadPopup() {
    $('.menu_hide').show().removeClass('menu_hide');
    $('.hide_all').css('z-index', '99999');
    $('.hide_all').show();
}

function afterPopupIsClosed() {
    $('.hide_all').hide();
    $('.hide_all.hidden').addClass('menu_hide');
    $('.hide_all').css('z-index', '');
}

//function focusLegacyHandler() {
//    if (legacyChildWindow === null)
//        return;
//    if (legacyChildWindow.closed) {
//        afterPopupIsClosed();
//    }
//}

function showModalPopupWindowLegacy(url, urlWidth, urlHeight) {
    beforeLoadPopup();
    legacyModeWindowOpened = true;
    legacyChildWindow = window.open(url, "",
        "toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=no, width=" + urlWidth + ", height=" + urlHeight);

    $(legacyChildWindow).bind("beforeunload", function () { afterPopupIsClosed(); });
    return legacyChildWindow;
}

//function showModalJQuery(url) {
//    $.showModalDialog({
//        url: url,
//        height: window.innerHeight-10,
//        width: window.innerWidth - 10
//    });

//}

function showModalPopupWindow(url) {

    var ratio = 0.9;
    var dialogHeight;
    var dialogWidth;

    dialogHeight = screen.availHeight;
    dialogWidth = screen.availWidth;
    window.resizeTo(dialogWidth, dialogHeight);

    if (!window.showModalDialog) {
        return showModalPopupWindowLegacy(url, dialogWidth, dialogHeight);
    }

    beforeLoadPopup();
    if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
        neww = window.showModalDialog(url, 'popup', 'dialogheight:' + dialogHeight + 'px; dialogwidth:' + dialogWidth + 'px;resizable: yes; scroll: yes ;fullScreen=yes; dialogTop: 0px');
    }
    else {
        neww = window.showModalDialog(url, 'popup', 'dialogheight:' + dialogHeight + 'px; dialogwidth: ' + dialogWidth + 'px;resizable: yes; scroll: yes ; center: yes;fullScreen=yes');
    }

    afterPopupIsClosed();
    return false;

}

//function changeFont(element, step) {
//    document.getElementById(element).style.fontSize = (parseInt(document.getElementById(element).style.fontSize, 10) + parseInt(step, 10)) + 'px';
//}

//function chooseTheme(newLocale) {

//    if ($.cookie("_theme") == newLocale)
//        return;
//    $.cookie("_theme", newLocale, { expires: 365, path: '/' });
//    // reload
//    window.location.reload(); 
//}

function jsonSetCurrentStore(storeOid) {
    var path = $('#HOME').val() + 'Home/jsonSetCurrentStore';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'StoreOid': storeOid
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.CompanyName !== "") {
                $("#DefaultSupplierCompanyName").html(data.CompanyName);
            }

            if (data.StoreName !== "") {
                $("#StoreName").html(data.StoreName);
            }
            $(".wrapper .store_selection ul li a.selected").removeClass('selected');
            $(".wrapper .header_container .order_btn").show();
            $("#" + storeOid).addClass('selected');
            $(".wrapper .header_container .megamenu ul li.order-shortcut").removeClass('disabled');
            $(".wrapper .header_container .megamenu ul li").removeClass('disabled');


            $('#storeSelection').animate({ top: -($('#storeSelection').height() + 100) + 'px' }, 500, function () {
                $(".wrapper .store_selection").hide();
                $(".wrapper .Home ul li a").removeClass('selected');
                $('#' + storeOid).removeClass('selected').addClass('selected');
                $('#headerMessagePanel').show();
                $('#footerMessagePanel').show();
            });

            var pagesThatNeedRefresh = ['/Item/Index', '/PriceCatalog/Index'];
            for (var i = 0; i < pagesThatNeedRefresh.length; i++) {
                if (window.location.href.search(pagesThatNeedRefresh[i]) > 0) {
                    break;
                }
            }
            window.location.reload();
        },
        error: commonError
    });
}

function commonError(data, name) {
    setJSError(anErrorOccured);
}

function ShowOrder(order_url) {
    var path = $('#HOME').val() + 'Document/jsonGetCurrentStore';
    var defaultOrderHasBeenAsked = order_url.indexOf('DocType=ORDER') > 0;
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'defaultOrderHasBeenAsked': defaultOrderHasBeenAsked
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.IsLoggedIn && data.CanOrderFromSelectedStore) {
                var childWindow;
                childWindow = showModalPopupWindow(order_url);
                if (typeof (grdDocument) != typeof (undefined)) {
                    grdDocument.UnselectAllRowsOnPage();
                }
            }
        },
        error: commonError
    });
}

//function ImageSliderOnInit(s, e) {
//    $(window).resize(function () {
//        imageSlider.AdjustControl();
//    });
//}


//function ToggleToolbar() {
//    if ($(".wrapper .fast_menu_options_btn").hasClass("fast_menu_options_left")) {
//        HideToolbar();
//    } else {
//        ShowToolbar();
//    }
//}

function ShowToolbar() {
    $('.wrapper .fast_menu_options').stop('true', 'true').animate({ top: "50px" }, 500, "linear", function () {
        $(".wrapper .fast_menu_options_btn").removeClass("arrow_down_color");
    });
}

function HideToolbar() {
    $('.wrapper .fast_menu_options').stop('true', 'true').animate({ top: "10px" }, 500, "linear", function () {
        $('.wrapper .fast_menu_options .options .moreMenu').hide();
        $(".wrapper .fast_menu_options_btn").addClass("arrow_down_color");
    });
}

function UserHasChosenAtLeastOneFilter() {
    var userHasChosenAtLeastOneFilter = false;
    $(".search_filter input").each(function () {
        $this = $(this);
        $label = $('label[for="' + $this.attr('id') + '"]');
        if ($label.length > 0 && $this.val() !== null && $this.val() !== "" && userHasChosenAtLeastOneFilter === false) {
            userHasChosenAtLeastOneFilter = true;
        }
    });
    return userHasChosenAtLeastOneFilter;
}

function FillCriteriaInfo() {

    if (UserHasChosenAtLeastOneFilter()) {
        toolTipText = $('<table class="customTooltip"><tr><th>' + selectedFilter + '</th><th>' + valueMessage + '</th></tr></table>');

        //var fields_to_read_text_from = new Object();
        //fields_to_read_text_from['createdByDevice'] = 'checkComboBox';//i.e. when search input is key (createdByDevice) then get text from 'checkComboBox'
        ////mostly key and value are the same but for multiselect combobox this is not the case.

        $(".search_filter input").each(function () {
            $this = $(this);
            $label = $('label[for="' + $this.attr('id') + '"]');
            if ($label.length > 0 && $this.val() !== null && $this.val() !== "") {
                var displayValue = null;
                var nameAttr = $this.attr('name');

                displayValue = Component.GetName(nameAttr).GetValue();

                if (displayValue !== null) {
                    if (nameAttr === "createdByDevice") {
                        var createdByDeviceSelectedItems = Component.GetName(nameAttr).GetSelectedItems();
                        if (createdByDeviceSelectedItems.length > 0) {
                            displayValue = "";
                            $.each(createdByDeviceSelectedItems, function (index, value) {
                                displayValue += " " + value.text;
                            });
                        }

                    }
                    else {
                        displayValue = Component.GetName(nameAttr).GetText();
                    }

                    var row = $('<tr><td>' + $label.html() + '</td>').append('<td>' + displayValue + '</td></tr>');
                    toolTipText.append(row);
                }

            }
        });

        return toolTipText;

    }
    else {
        $('#filterinfo').hide();
        return null;
    }
}

function sliderControl() {
    sliders = setInterval(function () {
        sliderMotion();
    }, 3000);
}

function sliderMotion() {
    var left = $('.wrapper #document_wrapper .carousel-content').width();
    $('.wrapper #document_wrapper .carousel-content:first').animate({ marginLeft: "-" + left - 50 + "px" }, 2000, function () {
        $(this).stop().removeAttr('style').detach().appendTo('.wrapper #document_wrapper .carousel-slider');
    });
}

function sliderMotion_new() {
    var left = $('.wrapper #document_wrapper .carousel-content').width();
    $('.wrapper #document_wrapper .carousel-content:first').delay(2001).animate({ marginLeft: "-" + left - 50 + "px" }, 2000, function () {
        $(this).stop().removeAttr('style').detach().appendTo('.wrapper #document_wrapper .carousel-slider');
        sliderMotion_new2();
    });

}

function sliderMotion_new2() {
    var left = $('.wrapper #document_wrapper .carousel-content').width();
    $('.wrapper #document_wrapper .carousel-content:first').delay(2001).animate({ marginLeft: "-" + left - 50 + "px" }, 2000, function () {
        $(this).stop().removeAttr('style').detach().appendTo('.wrapper #document_wrapper .carousel-slider');
        sliderMotion_new();
    });

}

function sliderMotionsLeft() {
    clearInterval(sliders);
    sliders = null;
    var left = $('.wrapper #document_wrapper .carousel-content').width();
    $('.wrapper #document_wrapper .carousel-content:first').stop().animate({ marginLeft: "-" + left - 50 + "px" }, 1000, function () {
        $(this).stop().removeAttr('style').detach().appendTo('.wrapper #document_wrapper .carousel-slider');
    });
}

function sliderMotionsRight() {
    clearInterval(sliders);
    sliders = null;
    var left = $('.wrapper #document_wrapper .carousel-content').width();
    $('.wrapper #document_wrapper .carousel-content:last').stop().css('marginLeft', "-" + left - 50 + "px").prependTo('.wrapper #document_wrapper .carousel-slider');
    $('.wrapper #document_wrapper .carousel-content:first').stop().animate({ marginLeft: "50px" }, 1000, function () { });
}

function GetFormData() {
    var dat = {};
    $(':input').not(':button').each(function () {
        if ($(this).val() !== null && $(this).val() !== "" && typeof $(this).attr('id') !== "undefined" && $(this).attr('id') !== null && $(this).attr('id') !== "") {
            dat[$(this).attr('id').replace('_I', '').replace('_S', '').replace('_VI', '')] = $(this).val();
        }
    });
    return dat;
}

function UserHasOwnerPermition(oid) {
    var path = $("#HOME").val() + ITScontroller + "/JsonUserHasPermission";
    var result;
    $.ajax({
        type: 'POST',
        url: path,
        async: false,
        data: {
            'StringOid': oid
        },
        cache: false,
        success: function (data) {
            result = data.Permitted;
        },
        error: function (data) {
            result = false;
        }
    });
    return result;
}

function OwnerIsSelected() {
    var path = $("#HOME").val() + ITScontroller + "/JsonOwnerIsSelected";
    var result;
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        async: false,
        success: function (data) {
            result = data.Result;
        },
        error: function (data) {
            result = false;
        }
    });
    return result;
}


function DeleteSelectedRows(s, e) {
    var grid = Component.GetName(gridName);
    if (grid !== null) {
        if (selectedItemsArray.length > 0) {
            var answer = confirm(multiDeleteconfirmation.replace('@1', selectedItemsArray.length));
            var arrayclone = selectedItemsArray;
            if (answer === true) {
                grid.PerformCallback("DELETESELECTED|" + arrayclone);
                selectedItemsArray = [];
            }
        }
        else {
            setJSError(pleaseSelectAnObject);
        }
    }
}

function EditSelectedRowsCustomV2() {

    var grid = Component.GetName(gridName);
    if (grid !== null) {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObjectToEdit);
        }
        else if (selectedItemsArray.length > 1) {
            setJSNotification(pleaseSelectOnlyOneObjectToEdit);
        }
        else if (selectedItemsArray.length === 1) {
            if (UserHasOwnerPermition(selectedItemsArray[0])) {

                Component.EmptyCallbackPanels();
                PopupEditCallbackPanel.PerformCallback();

                grid.UnselectAllRowsOnPage();

                return false;

            }
            else {
                setJSNotification(youCannotEditThisElement);
            }
        }
    }
}


function AddItemsToCategory() {
    addNewRow(CategoryTreeView);
    return false;
}

function AddNewCustomV2() {
    var grid = Component.GetName(gridName);
    if (grid !== null) {

        if (OwnerIsSelected()) {

            selectedItemsArray = [];

            Component.EmptyCallbackPanels();
            PopupEditCallbackPanel.PerformCallback();

            if (typeof grid !== "undefined") {
                grid.UnselectAllRowsOnPage();
            }

            return true;
        }
        else {
            setJSNotification(youMustSelectCompany);
            return false;
        }
    }
}

function AddNewFromGrid() {
    var grid = Component.GetName(gridName);
    if (grid !== null) {
        grid.AddNewRow();
        grid.UnselectAllRowsOnPage();
    }
}

function EditSelectedRowsFromGrid() {
    var grid = Component.GetName(gridName);

    if (grid !== null) {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObjectToEdit);
        }
        else if (selectedItemsArray.length > 1) {
            setJSNotification(pleaseSelectOnlyOneObjectToEdit);
        }
        else if (selectedItemsArray.length === 1) {
            grid.StartEditRowByKey(selectedItemsArray[0]);
            grid.UnselectAllRowsOnPage();
        }
    }
}

function CopySelectedRow(s, e) {
    var grid = Component.GetName(gridName);

    if (grid !== null) {
        if (selectedItemsArray.length === 0) {
            setJSNotification(pleaseSelectAnObjectToCopy);
        }
        else if (selectedItemsArray.length > 1) {
            setJSNotification(pleaseSelectOnlyOneObjectToCopy);
        }
        else if (selectedItemsArray.length === 1) {
            clopy = true;
            objectToBeCopied = selectedItemsArray[0];
            switch (insertItemJSFunction) {
                case "AddNewCustom":
                    AddNewCustom();
                    break;
                case "AddNewCustomV2":
                    AddNewCustomV2();
                    break;
                case "AddNewFromGrid":
                    grid.AddNewRow();
                    break;
                default:
                    setJSError(anErrorOccured);
            }
            return false;
        }
    }
}

//function isJqueryLiveSupported() {
//    return !check_jquery_version_geq_than(1, 7, 0);
//}

//function check_jquery_version_geq_than(i, j, k) {
//    var vernums = $.fn.jquery.split('.');
//    if (parseInt(vernums[0]) > i)
//        return true;
//    if (parseInt(vernums[0]) >= i && parseInt(vernums[1]) > j)
//        return true;
//    if (parseInt(vernums[0]) >= i && parseInt(vernums[1]) >= j && parseInt(vernums[2]) >= k)
//        return true;

//    return false;
//}

function SetCurrentCompany(companyGuid) {
    var path = $("#HOME").val() + "Home/SetCurrentCompany";
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'companyOid': companyGuid },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success === true) {
                if (data.CompanyName !== "") {
                    $("#DefaultSupplierCompanyName").html(data.CompanyName);
                }

                if (data.StoreName !== "") {
                    $("#StoreName").html(data.StoreName);
                }
                $(".wrapper .header_container .order_btn").hide();
                $(".wrapper .header_container #store_btn").show();
                $(".wrapper .header_container .megamenu ul li a#newSalesDocument").addClass('disabled');
                $(".wrapper .header_container .megamenu ul li a#newPurchaseDocument").addClass('disabled');
                $(".wrapper .header_container .megamenu ul li a#newStoreDocument").addClass('disabled');
                $(".wrapper .header_container .megamenu ul li").addClass('disabled');

                var pagesThatNeedRefresh = ['/Item/Index', '/PriceCatalog/Index'];
                for (var i = 0; i < pagesThatNeedRefresh.length; i++) {
                    if (window.location.href.search(pagesThatNeedRefresh[i]) > 0) {
                        break;
                    }
                }
                LayoutStoreSelection.PerformCallback();
            }
            window.location.reload();
        },
        error: function (data) {
        }
    });
}


function ValidateDecimalValuesChar(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;

    if (unicode == 13) {
        spinlineqty.Focus();
    }


    var actualkey = String.fromCharCode(unicode);
    var valid_chars = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
    var valid_decimal_separator = (actualkey == "." || actualkey == ",");

    if ((actualkey in valid_chars) || valid_decimal_separator) {
        if (actualkey == ",") {
            e.htmlEvent.preventDefault();
            s.SetText(s.GetText() + ".");
            return;
        }
        e.htmlEvent.returnValue = true;
    }
    else if (unicode != 9 && //tab
        unicode != 8 && //backspace
        unicode != 46 &&  //delete
        unicode != 37 &&  //left arrow
        unicode != 39)  //right arrow) 
    {
        e.htmlEvent.preventDefault();
    }
    else {
        e.htmlEvent.returnValue = true;
    }
}


function ValidateChar(s, e) {
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    var actualkey = String.fromCharCode(unicode);
    var valid_chars = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
    var valid_decimal_separator = false;
    if ($('#qty_support_decimal').val() !== null && typeof $('#qty_support_decimal').val() !== "undefined" && ($('#qty_support_decimal').val() == "1" || $('#qty_support_decimal').val() == 1)) {
        valid_decimal_separator = (actualkey == "." || actualkey == ",");
    }
    if ((actualkey in valid_chars) || valid_decimal_separator) {
        if (actualkey == ",") {
            e.htmlEvent.preventDefault();
            s.SetText(s.GetText() + ".");
            return;
        }
        e.htmlEvent.returnValue = true;
    }
    else if (unicode != 9 && //tab
        unicode != 8 && //backspace
        //unicode != 46 &&  //delete no its dot
        unicode != 37 &&  //left arrow
        unicode != 39)  //right arrow) 
    {
        e.htmlEvent.preventDefault();
    }
    else {
        e.htmlEvent.returnValue = true;
    }
}

function ShowGenericView() {
    var grid = Component.GetName(gridName);
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToView);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToView);
    }
    else if (selectedItemsArray.length === 1) {
        var oid = selectedItemsArray[0];
        var entityName = customjs_entityName;

        if (oid === null || entityName === null) {
        }
        else {
            GenericViewCallbackPanel.PerformCallback(oid + ":" + entityName);
            grid.UnselectAllRowsOnPage();
        }
    }
}

function ValidateForm(s, e, gridName) {
    s.SetEnabled(false);
    if (gridName != undefined && gridName != "undefined") {
        if (gridName.name === "grdAddressEdit") {
            var profession = $(".AdressGrid #Profession_I").val();
            if (profession === "" || profession === undefined || profession === "undefined") {
                setJSError(markedFieldsAreRequired);
                s.SetEnabled(true);
                return false;
            }
        }
    }

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
            btnSaveClick(s, e);
        }
        return validation;
    }
    s.SetEnabled(true);
    setJSError(markedFieldsAreRequired);
    return validation;
}

//function treeViewCallbackOnUnload(treeViewName) {
//    treeViewName.PerformCallback();
//}

function ValidateModalFormSingle() {
    if ($('.validateForm').length > 0) {
        var validation = ASPxClientEdit.ValidateEditorsInContainerById($('.validateForm'));
        if (validation) {
            return validation;
        }
        setJSError(markedFieldsAreRequired);
        return validation;
    }
}

function btnSaveClick(s, e) {
    jQuery(window).unbind("beforeunload");
    jQuery(window).unbind("unload");
    var form_to_submit = Component.GetCorrectForm(document.forms[0]);
    form_to_submit.submit();
}

function btnCancelClick(s, e) {
    self.close();
}

function ExportSelectedItems(s, e) {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    if (typeof ITScontroller !== "undefined" && ITScontroller !== null) {
        var exportType = $(this).attr('id');
        url = $('#HOME').val() + ITScontroller + '/ExportTo?' + exportType + '=1&Oids=' + selectedItemsArray.toString();
        window.location.href = url;
    }
}

function RefreshCompaniesAndStoresMenu(s, e) {
    LayoutCompanySelection.PerformCallback();
    LayoutStoreSelection.PerformCallback();
}

function UpdateCustomizationWindowVisibility(grid) {
    if (grid.IsCustomizationWindowVisible())
        grid.HideCustomizationWindow();
    else
        grid.ShowCustomizationWindow();
}
function CustomizationWindow(s, e) {
    if (typeof (s) !== "undefined" && GetHiddenColumns(s, e) === 0) {
        $('.btCustomizationWindow').removeClass('color-red');
    } else {
        $('.btCustomizationWindow').addClass('color-red');
    }
}

function GetHiddenColumns(s, e) {
    var count = 0;
    for (var i = 0; i < s.GetColumnCount(); i++) {
        if (s.GetColumn(i).visible === false) {
            count++;
        }
    }
    return count;
}

function clearComboBox(s, e) {
    if (e.buttonIndex === 0) {
        s.SetText("");
        s.SetValue(null);
    } else if (e.buttonIndex === 1) {
        DialogCallbackPanel.PerformCallback(s.name);
    } else {
        //in case you add another button
    }
}

function addWhiteTextBox(name) {

    $("#" + name).addClass('white-input');
}

function removeWhiteTextBox(name) {

    $("#" + name).removeClass('white-input');
}


function InitializeValidationRulesForForm(formName) {
    var form = $('#' + formName);
    if (form.executed)
        return;
    form.removeData("validator");
    $.validator.unobtrusive.parse(document);
    form.executed = true;
}


function DropDown(el) {
    this.dd = el;
    this.initEvents();
}

DropDown.prototype = {
    initEvents: function () {
        var obj = this;

        obj.dd.on('click', function (event) {
            $('#temporaryDocumentInfoContent').toggleClass('active');
            event.stopPropagation();
        });
    }
};



//fixes case 5631
function btnDeleteItemImageClick(s, e) {
    var path = $('#HOME').val() + 'OwnerApplicationSettings/jsonDeleteOwnerImage';
    $.ajax({
        type: 'POST',
        url: path,
        cache: false,
        dataType: 'json',
        async: false,
        success: function (data) {
            $('#previewImage').attr('src', $('#HOME').val() + 'OwnerApplicationSettings/ShowImageId' + '?time=' + new Date().getTime());
        },
        error: function (data) {
        }
    });
}

function ExpandNodeRoute() {

    if (typeof nodeRouteToExpand !== typeof undefined && nodeRouteToExpand.length > 0) {
        CategoryTreeView.GetNodeByName(nodeRouteToExpand.pop().name).SetExpanded(true);
    }

}

function CategoryTreeViewExpanded(s, e) {
    ExpandNodeRoute();
    if (typeof CategoryTreeViewSelectedNodeRoute !== "undefined" && CategoryTreeViewSelectedNodeRoute !== null && CategoryTreeViewSelectedNodeRoute.length > 0) {
        if (deletedNodeParent !== null && nodeMode == "delete") {

            CategoryTreeViewSelectedNodeRoute.pop().SetExpanded(true);
            if (CategoryTreeViewSelectedNodeRoute.length === 1) {
                CategoryTreeViewSelectedNodeRoute = null;
            }
        }
        else {
            //TODO Check after 15.1 update doesnt work

            //CategoryTreeViewSelectedNodeRoute.pop().SetExpanded(true);
        }
    }
}

function DisplayOverflowOnBody() {
    $("body").css('overflow', 'hidden');
    $(".wrapper").css("overflow", 'hidden');
}

function HideOverflowOnBody() {
    $("body").css('overflow', 'auto');
    $(".wrapper").css("overflow", 'auto');
}

function GetExtraFile(s, e) {
    if (selectedItemsArray.length > 0) {
        var arrayclone = selectedItemsArray;
        document.location = $("#HOME").val() + "InformationSheet/DownloadExtraFile?Oids=" + arrayclone;
    }
    else {
        setJSError(pleaseSelectAnObject);
    }
}

function GenericViewPopWithoutDetailsShown() {

    GenericViewPopup.SetWidth(window.innerWidth - 20 > $('#genericViewTable').width() + 30 ? $('#genericViewTable').width() + 30 : window.innerWidth - 20);
    GenericViewPopup.SetHeight(window.innerHeight - 20 > $('#genericViewTable').height() + 100 ? $('#genericViewTable').height() + 100 : window.innerHeight - 20);
    GenericViewPopup.UpdatePosition();

}

function GenericViewPopWithDetails() {
    GenericViewPopup.SetWidth(window.innerWidth - 20 > $('#genericViewTable').width() + 150 ? $('#genericViewTable').width() + 150 : window.innerWidth - 20);
    GenericViewPopup.SetHeight(window.innerHeight - 20 > $('#genericViewTable').height() + 100 + genericViewDetails.GetHeight() ? $('#genericViewTable').height() + 100 + genericViewDetails.GetHeight() : window.innerHeight - 20);
    GenericViewPopup.UpdatePosition();

}


$(function () {
    $('.order.dropdown li a').click(OrderShortcutClicked);

    $('.order-shortcut a').click(OrderShortcutClicked);
});

function OrderShortcutClicked(e) {

    if (!$(this).parent().hasClass('disabled')) {
        Component.EmptyCallbackPanels();

        var docType = null;

        if (typeof $(this).data("doctype") != "undefined") {
            docType = $(this).data("doctype");
        }

        PopupGenericEditCallbackPanel.PerformCallback({ ID: $(this).data("oid"), Recover: false, Division: $(this).data("mode"), DocType: docType, DisplayGeneric: true });
    }
}
