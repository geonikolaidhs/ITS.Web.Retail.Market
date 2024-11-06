var SynchronizationInfo = {
    RefreshButton_OnClick: function () {
        PivotGrid.PerformCallback();
    },
    PivotGrid_OnCellClick: function (grid, cell) {

        var cellRow = cell.HtmlEvent.target.parentElement;
        var entityHeaderRow = $("td").filter(function () {
            return $(this).text() == cell.RowValue;
        }).closest("tr");

        var highlighted = entityHeaderRow.hasClass('highlighted'); //see if the current row is already highlighted
        $('.highlighted').removeClass('highlighted');  //clear all existing highlighted
        
        $.each(cellRow.cells, function () {
            if (!highlighted) {
                this.classList.add('highlighted');
            }
        });

        if (!highlighted) {
            entityHeaderRow.addClass('highlighted');
        }
    },
    clickedBox: function(s) {

        $(":checkbox").change(function () {
            $.ajax({ url: $("#HOME").val() + 'SynchronizationInfo/ControlThreads', data: { "Row": this.id, "Checked": this.checked }, dataType: 'json', type: 'POST' });
            
        })
    }
   
   
};

$(document).ready(function () {
    setInterval(grdUpdateUpdaterThreads, 30000);
});

function grdUpdateUpdaterThreads() {
    $(function () {
        UpdaterThreads.PerformCallback();
    })
}

window.onunload = function () {
    $.ajax({ url: $("#HOME").val() + 'SynchronizationInfo/PauseThreads', data: {}, dataType: 'json', type: 'GET' });
}
