$(document).ready(function () {

    $('#panel-button').click(function () {
        SearchPanel.beforeSearchPanelOpen();
        $('#panel-button').hide();
        $(this).each(function () {
            $('#searchPanel').removeClass();
            $('.split-home-right').css("z-index", "7");
            $('#searchPanel').addClass("animated bounceInRight");

        });
    });

    $("#closeSearchPanel").click(function () {
        $('#panel-button').each(function () {
            $('#searchPanel').removeClass();
            $('.split-home-right').css("z-index", "");
            $('#searchPanel').addClass("animated bounceOutRight");
        });
        
        $('#panel-button').show();
        SearchPanel.afterSearchPanelOpen();
    });

});




var selectedValue = null;

var SearchPanel = {
    ShowResultsLoadingPanel: function () {
        ResultsLoadingPanel.Show();

    },
    HideResultsLoadingPanel: function () {
        ResultsLoadingPanel.Hide();
        $("#resultsSearchPanel .category .child .fa-eye").click(function () {
            selectedValue = { ID: $(this).data("id"), Type: $(this).data("type") };
            if ($(this).data("type") === "user") {
                GenericViewCallbackPanel.PerformCallback($(this).data("id") + ":" + $(this).data("type").charAt(0).toUpperCase() + $(this).data("type").substring(1));
            }
            else {
                PopupViewCallbackPanel.PerformCallback();
            }

        });
    },

    CheckMinLength: function (s, e) {


        if (searchPanelFormInput.GetValue() === null || searchPanelFormInput.GetValue().length < 3) {
            e.processOnServer = false;
            setJSError(pleaseWriteAtLeast3CharsOnSearchField);
            event.preventDefault();
            return false;
        }
        else {
            e.processOnServer = true;
            $("#search-form-mobile").submit();
        }

    },
    OnKeyDownSearchPanelFormInput: function (s, e) {
        if (e.htmlEvent.keyCode == 13) {
            SearchPanel.CheckMinLength(s, e);
        }
    },
    OnClickSearchPanelFormSubmit: function (s, e) {
        SearchPanel.CheckMinLength(s, e);
    },
    beforeSearchPanelOpen: function () {
        $('.menu_hide').show().removeClass('menu_hide');
        $('.hide_all').css('z-index', '7');
        $('.hide_all').css('background', 'rgba(255, 255, 255, 0.6)');
        $('.hide_all').show();
    },
    afterSearchPanelOpen: function () {
        $('.hide_all').hide();
        $('.hide_all.hidden').addClass('menu_hide');
        $('.hide_all').css('background', '');
        $('.hide_all').css('z-index', '');
    }
};

