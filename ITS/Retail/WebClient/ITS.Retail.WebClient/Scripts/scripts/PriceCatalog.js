var $tree_dialog,
    deletedNodeParent = null,
    selectedNode;

//--------------------tree functions--------------

//item category tree dialog
$(document).ready(function () {
   
    $("#btnClearCategoriesSupplier").hide();
    $("#btnClearCategoriesCustomer").hide();
});

function clearCategory() {
    SelectedNodeOid = -1;
    SelectedNodeText = '';
    $("#categoryfilterid").text(SelectedNodeOid);
    $("#categoryfiltertext").text(SelectedNodeText);
    $("#btnClearCategories").hide();
}

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}



function newButtonClick(s, e) {
    nodeMode = "new";
    
    Component.EmptyCallbackPanels();
    priceCatalogTreeViewCbPanel.PerformCallback();
    $('#PriceCatalogInfo').hide();
    pcGeneralPageControl.AdjustSize();
    PopupEditCallbackPanel.PerformCallback({ ID: "00000000-0000-0000-0000-000000000000" });

}

function editButtonClick(s, e) {
    mode = '1';
    nodeMode = "edit";

    if (Component.GetName('priceCatalogTreeView') !== null && priceCatalogTreeView.GetSelectedNode() !== null) {        

        if (UserHasOwnerPermition(priceCatalogTreeView.GetSelectedNode().name)) {
            Component.EmptyCallbackPanels();
            priceCatalogTreeViewCbPanel.PerformCallback();
            $('#PriceCatalogInfo').hide();
            pcGeneralPageControl.AdjustSize();
            PopupEditCallbackPanel.PerformCallback({ ID: priceCatalogTreeView.GetSelectedNode().name });

            return true;
        }
        else {
            setJSNotification(youCannotEditThisElement);
        }
    }
    else {
        setJSNotification(choosePriceCatalogFirst);
    }
}


function deleteButtonClick(s, e) {
    if (Component.GetName('priceCatalogTreeView') !== null && priceCatalogTreeView.GetSelectedNode() !== null) {
        pcGeneralPageControl.AdjustSize();
        var answer = confirm(deletePriceCatalog.replace("@1", priceCatalogTreeView.GetSelectedNode().GetText()));
        oid = priceCatalogTreeView.GetSelectedNode().name;
        if (answer) {            
            var path = $("#HOME").val() + "PriceCatalog/DeletePriceCatalog";
            $.ajax({
                type: 'POST',
                url: path,
                data: {
                    'PriceCatalogID': priceCatalogTreeView.GetSelectedNode().name
                },
                cache: false,
                dataType: 'json',
                success: function (data) {
                    if (data && typeof data.Error === "undefined") {
                        deletedNodeParent = priceCatalogTreeView.GetSelectedNode().parent;
                        nodeMode = "delete";
                        priceCatalogTreeViewCbPanel.PerformCallback();
                        $('#PriceCatalogInfo').hide();
                        pcGeneralPageControl.AdjustSize();
                    }
                }
            });
        }
    }
    else {
        setJSNotification(choosePriceCatalogFirst);
    }
}

function OnBeginCallBackPriceCatalogInfoCbp(s, e) {
    pcGeneralPageControl.AdjustSize();
    e.customArgs.PriceCatalogID = priceCatalogTreeView.GetSelectedNode().name;
    
}

function OnEndCallbackPriceCatalogInfoCbp(s, e) {
    InitializePriceCatalogFilters();
}

function priceCatalogTreeViewGetData(s, e) {
    
    priceCatalogInfoCbp.PerformCallback();
    $('#PriceCatalogInfo').show();
    $("#btnClearCategories").hide();
    pcGeneralPageControl.AdjustSize();
    
}

function priceCatalogTreeViewExpanded(s, e) {

    if (typeof (PriceCatalogTreeViewSelectedNodeRoute) !== "undefined" && PriceCatalogTreeViewSelectedNodeRoute !== null && PriceCatalogTreeViewSelectedNodeRoute.length > 0) {
        if (deletedNodeParent !== null && nodeMode == "delete") {

            PriceCatalogTreeViewSelectedNodeRoute.pop().SetExpanded(true);
            if (PriceCatalogTreeViewSelectedNodeRoute.length === 1) {
                PriceCatalogTreeViewSelectedNodeRoute = null;
            }
        }
    }
    pcGeneralPageControl.AdjustSize();
}

function TreeViewGetData(s, e) {
    if (s.GetSelectedNode() === null) {
        s.SetSelectedNode(s.GetNode(0));
    }
    if (SelectedNodeOid == s.GetSelectedNode().name) {
        if (document.getElementById("categoryfilterid") !== null) {
            $("#categoryfilterid").text(SelectedNodeOid);
        }
        if (document.getElementById('categoryfiltertext') !== null) {
            $("#categoryfiltertext").text(SelectedNodeText);
        }
        if (document.getElementById('btnClearCategories') !== null) {
            $("#btnClearCategories").show();
        }
        if (Component.GetName('pcCategoriesPopup') !== null) {
            pcCategoriesPopup.Hide();
        }
    }
    SelectedNodeOid = s.GetSelectedNode().name;
    SelectedNodeText = s.GetSelectedNode().GetText();
    if (Component.GetName('pcGeneralPageControl') !== null) {
        pcGeneralPageControl.AdjustSize();
    }
}

//function treeViewCbPanelOnEndCallback(s, e) {
//    pcGeneralPageControl.AdjustSize();
//}

function OnBeginCallbackPriceCatalogDetailGrid(s, e) {
   
        e.customArgs.Fcode = Component.GetName('Fcode').GetValue();
        e.customArgs.Fname = Component.GetName('Fname').GetValue();
        e.customArgs.Fbuyer = Component.GetName('Fbuyer').GetValue();
        e.customArgs.Fseasonality = Component.GetName('Fseasonality').GetValue();
        e.customArgs.Fmothercode = Component.GetName('Fmothercode').GetValue();
        e.customArgs.Fbarcode = Component.GetName('Fbarcode').GetValue();
        e.customArgs.Factive = Component.GetName('Factive').GetValue();
        e.customArgs.FcreatedOn = Component.GetName('FcreatedOn').GetText();
        e.customArgs.FitemSupplier = Component.GetName('FitemSupplier').GetValue();
        e.customArgs.FupdatedOn = Component.GetName('FupdatedOn').GetText();
        e.customArgs.Fcategory = Component.GetName('categoryfilterid').GetValue();
        pcGeneralPageControl.AdjustSize();
}

//function OnEndCallbackPriceCatalogDetailGrid(s, e) {
  
//    if (Component.GetName("pcFeatures") !== null) {
//        //refresh sto esoteriko pageControl
//        pcFeatures.AdjustSize();  
//    }
//    pcGeneralPageControl.AdjustSize();

//}



function OnTabChangedPcFeatures(s, e) {
    pcGeneralPageControl.AdjustSize();
}

function OnBeginCallbackTreeViewCbPanel(s, e) {
    PriceCatalogTreeViewSelectedNode = priceCatalogTreeView.GetSelectedNode();
    PriceCatalogTreeViewSelectedNodeRoute = [];
    var cNode = PriceCatalogTreeViewSelectedNode;
    while (cNode !== null) {
        PriceCatalogTreeViewSelectedNodeRoute.push(cNode);
        cNode = cNode.parent;
    }
}

function OnEndCallbackTreeViewCbPanel(s, e) {

    if (typeof (PriceCatalogTreeViewSelectedNodeRoute) !== "undefined" && PriceCatalogTreeViewSelectedNodeRoute !== null && PriceCatalogTreeViewSelectedNodeRoute.length > 0) {
        if (deletedNodeParent !== null && nodeMode == "delete") {

            PriceCatalogTreeViewSelectedNodeRoute.pop().SetExpanded(true);
            if (PriceCatalogTreeViewSelectedNodeRoute.length === 1) {
                PriceCatalogTreeViewSelectedNodeRoute = null;
            }
        }
    }

    pcGeneralPageControl.AdjustSize();    
}