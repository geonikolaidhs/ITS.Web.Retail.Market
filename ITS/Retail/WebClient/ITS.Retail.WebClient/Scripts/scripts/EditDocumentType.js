function CancelDocumentType(s, e)
{   
    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "DocumentType/Cancel",
        async: false,
        cache: false,
        dataType: 'json',
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

function OnDivisionChanged(s, e) {

    $.ajax({
        type: 'POST',
        url: $("#HOME").val() + "DocumentType/GetPurchaseOid",
        data: { 'itemOid': s.GetValue() },
        async: false,
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (typeof data.result !== typeof undefined) {

                var division = Component.GetName('Division').GetValue();
                var chk_wholesale = Component.GetName('IsForWholesale');

                if (division == data.result) {
                    chk_wholesale.SetCheckState('Checked');
                    chk_wholesale.SetEnabled(false);
                }
                else {
                    chk_wholesale.SetEnabled(true);
                }
            }
            TraderTypeCombobox.PerformCallback();
        },
        error: function (data) {
            setJSError(data);
        }
    });
}



function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}
function OnBeginCallbackCustomerCategoryGrid(s, e) {
    if (s.IsEditing()) {
        e.customArgs.SelectedNodeOid = SelectedNodeOid;
    }
} function OnBeginCallbackCategoryGrid(s, e) {
    if (s.IsEditing()) {
        e.customArgs.SelectedNodeOid = SelectedNodeOid;
    }
}

function TreeViewGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
    SelectedNodeOid = s.GetSelectedNode().name;
}

function OnSaveBtnClickCustomerAnalyticTree(s, e) {
    gridDocTypeCustCategories.UpdateEdit();
}

function OnSaveBtnClickItemAnalyticTree(s, e, grid) {
    grid = ASPxClientGridView.Cast(grid);
    grid.UpdateEdit();
}

function SetDefaultCategory(s, e) {
    var path = $('#HOME').val() + 'DocumentType/SetDefaultCategory';
    $.ajax({
        type: 'POST',
        url: path,
        data: { 'itemOid': s.name },
        cache: false,
        dataType: 'json',
        success: function (data) {
            gridDocTypeCustCategories.PerformCallback();
        },
        error: commonError
    });
}
