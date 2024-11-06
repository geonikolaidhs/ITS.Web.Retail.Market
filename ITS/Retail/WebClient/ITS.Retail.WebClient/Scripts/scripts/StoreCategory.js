var categoryID;

$(document).ready(function () {
    treeList.SetFocusedNodeKey("");
});

function OnViewNodeClick(clickedCategoryID) {
    categoryID = clickedCategoryID;
    grdStoreAnalyticTrees.PerformCallback();
}


function OnFocusedNodeChangedTreeList(s, e) {
    categoryID = s.GetFocusedNodeKey();
    grdStoreAnalyticTrees.PerformCallback();
}

function OnBeginCallbackStoreAnalyticTreeGrid(s, e) {
    e.customArgs.categoryID = categoryID;
    if (s.IsEditing()) {
        e.customArgs.storeID = StoreComboBox.GetValue();
    }
}

function AddNewStoreAnalyticTree() {
    if (treeList.GetFocusedNodeKey() === null || treeList.GetFocusedNodeKey() === "") {
        return;
    }
    grdStoreAnalyticTrees.AddNewRow();
}

function addNewRow() {
    if (treeList.GetFocusedNodeKey() === null || treeList.GetFocusedNodeKey() === "") {
        return;
    }
    var path = $("#HOME").val() + "StoreCategory/AddStoresToCategory?CategoryID=" + treeList.GetFocusedNodeKey();
    showModalPopupWindow(path);
}