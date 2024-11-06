var mode = "0",
    oid = "-1",
    $tree_dialog,
    nodeMode,
    deletedNodeParent = null,
    unexpandable = false,
    parentunexpandable = false,
    nodeRouteToExpand;

AnalyticTreeID = -1;
PerformGridOfNodesCallback = true;


function OpenCategoryTreeView(s, e) {
    showpopup(CategoryTreeView, 3);
    CategoryTreeView.SetSelectedNode(null);
    $('.node_img').addClass('hidden');
}

function test(s, e) {
    var gridInstance = ASPxClientLabel.Cast("CategoryTreeView");
    alert(gridInstanceGetSelectedNode().parent.name);
    alert(gridInstance.GetRootNode().name);
}

function GetRoot(node) {
    while (node.parent !== null) {
        GetRoot(node.parent);
    }
    return node.name;
}


function ItemAnalyticTreeStartEdit(s, e) {
    console.log(e);
    if (e.rowValues[2].value == null) {
        e.rowValues[2].value = new Date();
        e.rowValues[2].value.setHours(23, 59, 59);
    }
    if (e.rowValues[1].value == null) {
        e.rowValues[1].value = new Date();
        e.rowValues[1].value.setHours(0, 0, 0);
    }
    if (e.rowValues[3].value == null) {
        e.rowValues[3].value = 1;
    }
    if (e.rowValues[4].value == null) {
        e.rowValues[4].value = true;
    }
}

function showpopup(tree, mymode) {
    mode = mymode;
    var TreeView = ASPxClientTreeView.Cast(tree);
    if (mymode === 3) {
        oid = -1;
        EditTreeViewPopup.Show();
    }
    else {
        if (TreeView.GetSelectedNode() === null) {
            setJSNotification(chooseCategoryFirst);
        }
        else {
            if (mymode === 0) {
                oid = -1;
            }
            else {
                oid = TreeView.GetSelectedNode().name;
            }
            EditTreeViewPopup.Show();
        }
    }
}

function addNewRow(tree) {
    var TreeView = ASPxClientTreeView.Cast(tree);
    if (TreeView.GetSelectedNode() === null) {
        setJSNotification(chooseCategoryFirst);
        return false;
    }
    if (UserHasOwnerPermition(TreeView.GetSelectedNode().name)) {
        //var path = tree.callbackUrl.replace("TreeView", "AddItemsToCatecory?CategoryID=" + TreeView.GetSelectedNode().name);
        //var childWindow = showModalPopupWindow(path);

        //if (typeof (grdMembersOfNode) != typeof (undefined)) {
        //    if (isChrome) {
        //        $(childWindow).load(function () {
        //            $(childWindow).unload(function () {
        //                grdMembersOfNode.PerformCallback();
        //            });
        //        });
        //    }
        //    else {
        //        grdMembersOfNode.PerformCallback();
        //    }
        //}
    }
    else {
        setJSNotification(youCannotEditThisElement);
    }
}

function AddNewRowOnTree() {
    var TreeView = ASPxClientTreeView.Cast(CategoryTreeView);
    if (TreeView.GetSelectedNode() === null) {
        setJSNotification(chooseCategoryFirst);
        return false;
    }
    if (UserHasOwnerPermition(TreeView.GetSelectedNode().name)) {

        Component.EmptyAddCallbackPanel();
        PopupAddCallbackPanel.PerformCallback({ ID: TreeView.GetSelectedNode().name, Text: TreeView.GetSelectedNode().text });

        return true;


        //var path = tree.callbackUrl.replace("TreeView", "AddItemsToCatecory?CategoryID=" + TreeView.GetSelectedNode().name);
        //var childWindow = showModalPopupWindow(path);

        //if (typeof (grdMembersOfNode) != typeof (undefined)) {
        //    if (isChrome) {
        //        $(childWindow).load(function () {
        //            $(childWindow).unload(function () {
        //                grdMembersOfNode.PerformCallback();
        //            });
        //        });
        //    }
        //    else {
        //        grdMembersOfNode.PerformCallback();
        //    }
        //}
    }
    else {
        setJSNotification(youCannotEditThisElement);
    }
}

function TreeViewGetData(s, e) {

    nodeMode = "click";
    //DO NOT DELETE
    var target = e.htmlEvent.target || e.htmlEvent.srcElement;
    if (target.className.search("node_img") !== -1) {
        //Image clicked
        showMembersOfCategoryBtnClick(s, e);
    }
    else {
        //Node Selected
        $('.node_img').addClass('hidden');
        var node = s.GetSelectedNode();
        if (node !== null && typeof node !== "undefined") {
            $('#' + node.name + ' img').removeClass('hidden');

            var insertBtn = Component.GetName('btninsert'),
                editBtn = Component.GetName('btnedit'),
                deleteBtn = Component.GetName('btndelete'),
                hasPermition = UserHasOwnerPermition(node.name);

            if (insertBtn !== null && typeof insertBtn !== "undefined") {
                btninsert.SetEnabled(hasPermition);
            }
            if (editBtn !== null && typeof editBtn !== "undefined") {
                editBtn.SetEnabled(hasPermition);
            }
            if (deleteBtn !== null && typeof deleteBtn !== "undefined") {
                deleteBtn.SetEnabled(hasPermition);
            }
        }
    }
}

function showMembersOfCategoryBtnClick(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }

    SelectedNodeOid = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (PerformGridOfNodesCallback)
        grdMembersOfNode.PerformCallback();
}

function CategoryTreeViewPopupGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
    CategoryID = s.GetSelectedNode().name;
    if (SelectedNodeOid !== s.GetSelectedNode().name) {
        var path = s.callbackUrl.replace("TreeViewPopup", "jsonChangeCategory");
        $.ajax({
            type: 'POST',
            url: path,
            data: { 'AnalyticTreeID': AnalyticTreeID, 'CategoryID': CategoryID },
            async: false,
            cache: false,
            dataType: 'json',
            success: function (data) {
                if (data.success) {
                    grdMembersOfNode.PerformCallback();
                }
            }
        });
        pcCategoriesPopup.Hide();
    }
}

function deletenode(s, e, tree) {
    var TreeView = ASPxClientTreeView.Cast(tree);
    if (TreeView.GetSelectedNode() === null) {
        setJSNotification(chooseCategoryFirst);
    }
    else {
        mode = '-1';
        var answer = confirm(deleteCategory.replace("@1", TreeView.GetSelectedNode().GetText()));
        oid = TreeView.GetSelectedNode().name;
        if (answer) {
            deletedNodeParent = TreeView.GetSelectedNode().parent;
            nodeMode = "delete";
            cbp.PerformCallback();
            grdMembersOfNode.PerformCallback();
        }
    }

}

function hidepopup(s, e) {
    EditTreeViewPopup.Hide();
}

function OnBeginCallBackMembersOfNodeGrid(s, e) {

    if (CategoryTreeView.GetSelectedNode() === null) {
        setJSNotification(chooseCategoryFirst);
        return;
    }

    e.customArgs.categoryid = CategoryTreeView.GetSelectedNode().name;

    if (s.IsEditing()) {
        var comboBox = Component.GetName('NodeComboBox');
        if (comboBox !== null) {
            e.customArgs.NodeComboBoxID = comboBox.GetValue();
        }
        comboBox = Component.GetName('RootComboBoxHidden');
        if (comboBox !== null) {
            e.customArgs.RootComboBoxID = comboBox.GetValue();
        }
        comboBox = Component.GetName('ObjectComboBoxHidden');
        if (comboBox !== null) {
            e.customArgs.ObjectComboBoxID = comboBox.GetValue();
        }
    }
}

function OnRowChangedMembersOfNodeGrid(s, e) {
    if (!s.IsEditing()) {
        AnalyticTreeID = grdMembersOfNode.GetRowKey(grdMembersOfNode.GetFocusedRowIndex());
    }
}

function EditTreeViewPopupValidation(s, e) {
    if (ValidateModalFormSingle()) {
        mode = 2;
        cbp.PerformCallback();
        EditTreeViewPopup.Hide();
    } else {
        setJSError(markedFieldsAreRequired);
    }
}

function popUpCallbackPanel_OnBeginCallback(s, e) {
    if (mode !== 2) {
        if (mode === 1) {
            if (CategoryTreeView.GetSelectedNode().parent !== null)
                e.customArgs.parent = CategoryTreeView.GetSelectedNode().parent.name;
            else
                e.customArgs.parent = null;
        }
        else if (mode === 0)
            e.customArgs.parent = CategoryTreeView.GetSelectedNode().name;
        else if (mode === 3)
            e.customArgs.parent = null;
    }
    else {
        e.customArgs.parent = HiddenParent.GetValue();
    }
    e.customArgs.oid = oid;
    e.customArgs.mode = mode;
    e.customArgs.descr = txtdescr.GetValue();
    e.customArgs.code = txtcode.GetValue();
    if (Component.GetName('txtpoints') !== null) {
        e.customArgs.points = txtpoints.GetValue();
    }
}

function popUpCallbackPanel_OnEndCallback(s, e) {
    if (s.cp_success) {
        unexpandable = s.cp_unexpandable;
        parentunexpandable = s.cp_parentunexpandable;
        TreeViewPartial.PerformCallback();
        TreeViewPopupPartial.PerformCallback();

    }
}

function CategoryTreeViewPartialBeginCallback(s, e) {
    CategoryTreeViewSelectedNode = CategoryTreeView.GetSelectedNode();
    CategoryTreeViewSelectedNodeRoute = [];
    var cNode = CategoryTreeViewSelectedNode;
    while (cNode !== null) {
        CategoryTreeViewSelectedNodeRoute.push(cNode);
        cNode = cNode.parent;
    }
}

function CategoryTreeViewPartialEndCallback(s, e) {
    if (typeof (CategoryTreeViewSelectedNodeRoute) !== "undefined" && CategoryTreeViewSelectedNodeRoute !== null && CategoryTreeViewSelectedNodeRoute.length > 0) {
        if (deletedNodeParent !== null && nodeMode === "delete") {
            if (!parentunexpandable) {
                CategoryTreeViewSelectedNodeRoute.pop().SetExpanded(true);
            }
            if (CategoryTreeViewSelectedNodeRoute.length === 1) {
                CategoryTreeViewSelectedNodeRoute = null;
            }
        }
        else if (!unexpandable) {
            nodeRouteToExpand = CategoryTreeViewSelectedNodeRoute;
            ExpandNodeRoute();
        }
    }
}
