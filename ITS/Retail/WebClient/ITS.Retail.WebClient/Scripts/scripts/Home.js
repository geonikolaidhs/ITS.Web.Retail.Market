var DockPanelWidgets = [];
var isStatisticsChecked = null;

$(document).ready(function () {
    //fills the year picker select
    for (i = new Date().getFullYear() ; i > 2000; i--) {
        $('#CustomersYearFilter').append($('<option />').val(i).html(i));
    }
    if ($('.dockZone').attr("img") !=="") {
        var OwnerImage = $('.dockZone').attr("img");
        $('.dockZone').css('background', 'url(' + $('#HOME').val() + 'Base/ShowOwnerImage)');
    }
    $('.wrapper .container').addClass('home');

    sliderControl();

    $( ".Home_Index .presenter" ).on( "click", ".bar_c", function() {
        var that = $(this);
        var item;
        if (that.parent().find('#ItemList').length == 1) {
            item = $('.wrapper .Home_Index .presenter .lists div.container #ItemsChart');
        }
        else {
            item = $('.wrapper .Home_Index .presenter .lists div.container #ItemCategoriesChart');
        }
        if (item.css('z-index') == -1) {
            item.css('z-index', '1');
            that.addClass("info");
        }
        else {
            item.css('z-index', '-1');
            that.removeClass("info");

        }
    });

    reloadLists();
    $("#ListsValueOrQtyRadio").hide();
    $("#ItemsStatisticsCallbackPanel").hide();
    $("#DateFilterTrackBar").fadeOut(50);
    $("#TopItemsTrackBar").fadeOut(50);

    $("#statisticsCheckbox").labelauty();

    $("#statisticsCheckbox").on("click", function () {

        if ($("#statisticsCheckbox:checked").length === 0) {

            $("#ListsValueOrQtyRadio").fadeOut( 300 );
            $("#ItemsStatisticsCallbackPanel").fadeOut(300);

            $("#DateFilterTrackBar").fadeOut(300);
            $("#TopItemsTrackBar").fadeOut(300);
        }
        else {
            if (isStatisticsChecked===null) {
                $("#statisticsContentForm").submit();
                isStatisticsChecked = true;
            }
            
            $("#ListsValueOrQtyRadio").fadeIn(300);
            $("#ItemsStatisticsCallbackPanel").fadeIn(300);

            $("#TopItemsTrackBar").fadeIn(300);
            $("#DateFilterTrackBar").fadeIn(300);
            reloadLists();
        }
    });

});

function StatisticsVisibility(s, e) {
    $("#ListsValueOrQtyRadio").fadeIn(300);
    $("#ItemsStatisticsCallbackPanel").fadeIn(300);

    $("#TopItemsTrackBar").fadeIn(300);
    $("#DateFilterTrackBar").fadeIn(300);
    reloadLists();
}



function MenuCheckboxValueChanged(s, e) {
    var id = s.name;
    var exists = false;
    var index = "";
    for (var k in DockPanelWidgets) {
        if (DockPanelWidgets[k].DockPanel == id) {
            exists = true;
            index = k;
            break;
        }
    }
    if (exists) {
        DockPanelWidgets[index].IsVisible = s.GetValue();
    } else {
        DockPanelWidgets.push({
            DockPanel: id,
            DockZone: 1,
            IsVisible: s.GetValue()
        });
    }
    MenuDockPanels.PerformCallback();
}

function DockPanelDockUpdate(panel, zone) {
    var id = panel;
    var exists = false;
    var index = "";

    for (var k in DockPanelWidgets) {
        if (DockPanelWidgets[k].DockPanel == id) {
            exists = true;
            index = k;
            break;
        }
    }

    if (exists) {
        DockPanelWidgets[index].IsVisible = true;
        DockPanelWidgets[index].DockZone = zone;
    } else {
        DockPanelWidgets.push({
            DockPanel: id,
            DockZone: zone,
            IsVisible: true
        });
    }
}

function InitializeOrderShortcut(){
    $('.order-shortcut a').click(OrderShortcutClicked);
}

function CloseDockPanelDockChange(s, e) {
    if (DockPanelWidgets.length > 0) {
        var id = s.name.replace("dockPanel", "chk");

        Component.GetName(id).SetValue(false);
        for (var k in DockPanelWidgets) {            
            if (DockPanelWidgets[k].DockPanel == id) {
                DockPanelWidgets[k].IsVisible = false;
                DockPanelWidgets[k].DockZone = s.GetZoneUID().replace("Zone", "");
            }
        }
    }
    $("#SaveMenuDockPanels").show();
}

function DockPanelDockChange(s, e) {
    if (DockPanelWidgets.length > 0) {
        var id = s.name.replace("dockPanel", "chk");
        for (var k in DockPanelWidgets) {           
            if (DockPanelWidgets[k].DockPanel == id) {
                DockPanelWidgets[k].IsVisible = s.IsVisible();
                DockPanelWidgets[k].DockZone = s.GetZoneUID().replace("Zone", "");
            }
        }
    }
    $("#SaveMenuDockPanels").show();
}

function MenuDockPanelsBeginCallback(s, e) {
    if (DockPanelWidgets.length > 0) {
        e.customArgs.SelectedDockPanels = JSON.stringify(DockPanelWidgets);
    }
}


function onValueChangedDateFilterTrackBack(s, e)
{
    reloadLists();
}

function onValueChangedTopItemsTrackBack(s, e)
{
    reloadLists();
}

function onValueChangedListsValueOrQtyRadio(s, e)
{
    reloadLists();
}

function controlsSetEnabled(enabled)
{
    TopItemsTrackBar.SetEnabled(enabled);
    TopCustomerTrackBar.SetEnabled(enabled);
    DateFilterTrackBar.SetEnabled(enabled);
    CustomersBestOrWorseFilter.SetEnabled(enabled);
    ListsValueOrQtyRadio.SetEnabled(enabled);
    $('#CustomersYearFilter')[0].disabled = !enabled;
}

//function onBeginCallbackItemsList(s, e)
//{
//    controlsSetEnabled(false);
//}

//function onEndCallbackItemsList(s, e)
//{
//    controlsSetEnabled(true);
//}

function onBeginCallbackCustomersChart(s, e)
{
    CustomersChart.ShowLoadingElements();
    controlsSetEnabled(false);
}

function onEndCallbackCustomersChart(s, e)
{
    CustomersChart.HideLoadingElements();
    controlsSetEnabled(true);
}

function onCallbackError(s, e)
{
    setJSError(anErrorOccured);
}

function reloadLists()
{
    try{
        var selectedDateFilter = DateFilterTrackBar.GetValue();
        var selectedTopItemsFilter = TopItemsTrackBar.GetValue();
        var selectedDisplayBy = ListsValueOrQtyRadio.GetValue();
        ItemsStatisticsCallbackPanel.PerformCallback(",selectedDateFilter:" + selectedDateFilter + ",selectedTopItemsFilter:" + selectedTopItemsFilter + ",displayBy:" + selectedDisplayBy);
    }
    catch(err){
    }    
}

function onCustomersFilterChange()
{
    var selectedTopCustomersFilter = TopCustomerTrackBar.GetValue();
    var yearFilter = document.getElementById("CustomersYearFilter").value;
    var bestOrWorstFilter = CustomersBestOrWorseFilter.GetValue();
    CustomersChart.PerformCallback(",year:" + yearFilter + ",selectedTopCustomersFilter:" + selectedTopCustomersFilter+",bestOrWorstFilter:"+bestOrWorstFilter);
}

function OnInitPopupViewCallback(s, e) {
    
    e.customArgs = selectedValue;
    selectedValue = null;

}