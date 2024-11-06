var parameters = "";
var textSeparator = ";";

$(document).ready(function () {
    setInterval(grdPOSStatusesUpdate, 5000);
    setInterval(grdPOSCommandResultsUpdate, 5000);
});

function grdPOSCommandResultsUpdate() {
    $(function () {

        grdPosCommandsResults.PerformCallback();

        //try {
        //    var commandResults = $(".CommandResultClass");
        //    if (commandResults.length > 0) {
        //        for (var i = 0; i < commandResults.length; i++) {
        //            var html = "";
        //            var rows = $(commandResults[i]).html().split("|");
        //            if (rows.length > 0) {
        //                for (var i = 0; i < rows.length; i++) {
        //                    html += "<div class='rowDiv'>";
        //                    var rowCells = rows[i].split(",");
        //                    if (rowCells.length > 0) {
        //                        for (var i = 0; i < rowCells.length; i++) {
        //                            html += "<span class='rowcell'>" + rowCells[i] + "</span>";
        //                        }
        //                    }
        //                    html += "</div>";
        //                    //$(commandResults[i]).html(html)
        //                }
        //            }
        //        }
        //    }
        //    else {
        //        alert("noresults");
        //    }

        //} catch (error) {
        //    console.log(error);
        //}
        //return false;


    });
}

function grdPOSStatusesUpdate() {
    $(function () {
        grdPOSStatuses.PerformCallback();
    });
}


function grdPosCommandsResultsEndCallback(s, e) {
    console.log("aa");



}

//function grdCallBack(e) {
//    grdPOSStatuses.PerformCallback(e);
//}


function OnListBoxSelectionChanged(listBox, args, posList) {
    if (args.index === 0) {
        if (args.isSelected)
            listBox.SelectAll();
        else
            listBox.UnselectAll();
    }

    UpdateSelectAllItemState(posList);
    UpdateText(posList);
}

function UpdateSelectAllItemState(posList) {
    var lstPos;

    if (posList == "basic") {
        lstPos = lstPosBasic;
    }
    else {
        lstPos = lstPosAdvanced;
    }

    if (IsAllSelected(posList)) {
        lstPos.SelectIndices([0]);
    }
    else {
        lstPos.UnselectIndices([0]);
    }
}

function IsAllSelected(posList) {
    var lstPos;
    if (posList == "basic") {
        lstPos = lstPosBasic;
    }
    else {
        lstPos = lstPosAdvanced;
    }

    for (var i = 1; i < lstPos.GetItemCount(); i++)
        if (!lstPos.GetItem(i).selected)
            return false;
    return true;
}

function UpdateText(posList) {
    var lstPos;
    if (posList == "basic") {
        lstPos = lstPosBasic;
        cmbPos = cmbPosBasic;
    }
    else if (posList == "sql") {
        lstPos = lstSqlPosBasic;
        cmbPos = cmbSqlPosBasic;
    }
    else {
        lstPos = lstPosAdvanced;
        cmbPos = cmbPosAdvanced;
    }

    var selectedItems = lstPos.GetSelectedItems();
    cmbPos.SetText(GetSelectedItemsText(selectedItems));
}

function SynchronizeListBoxValues(dropDown, args, posList) {
    var lstPos;
    if (posList == "basic") {
        lstPos = lstPosBasic;
    }
    else {
        lstPos = lstPosAdvanced;
    }

    lstPos.UnselectAll();
    var texts = dropDown.GetText().split(textSeparator);
    var values = GetValuesByTexts(texts);
    lstPos.SelectValues(values);
    UpdateSelectAllItemState(posList);
    UpdateText(posList);
}

function GetSelectedItemsText(items) {
    var texts = [];
    for (var i = 0; i < items.length; i++)
        if (items[i].index !== 0)
            texts.push(items[i].text);
    return texts.join(textSeparator);
}

function GetValuesByTexts(texts, posList) {
    var lstPos;
    if (posList == "basic") {
        lstPos = lstPosBasic;
    }
    else {
        lstPos = lstPosAdvanced;
    }
    var actualValues = [];
    var item;
    for (var i = 0; i < texts.length; i++) {
        item = lstPos.FindItemByText(texts[i]);
        if (item !== null)
            actualValues.push(item.value);
    }
    return actualValues;
}

function SendCommandButtonAdvancedClick(s, e) {

    if (lstPosAdvanced.GetSelectedItems().length === 0) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    if (cmbCommandAdvanced.GetValue() == "RELOAD_ENTITIES") {
        DialogCallbackPanel.PerformCallback(cmbCommandAdvanced.GetValue());
    }
    else {
        PosCommandsAdvanced.PerformCallback();
    }
}

function SendCommandButtonBasicClick(s, e) {

    if (lstPosBasic.GetSelectedItems().length === 0) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    DialogCallbackPanel.PerformCallback(cmbCommandBasic.GetValue());
}

function SendCommandSqlButtonBasicClick(s, e) {

    if (lstSqlPosBasic.GetSelectedItems().length === 0) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    if (cmbtypebasic.GetSelectedItems().length === 0) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    if (cmbtypebasic.GetSelectedItems().length === 2) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }

    var cmdType = cmbtypebasic.GetSelectedItems()[0].text;
    var cmd = $("#commandTextArea").val();
    if (cmd.length < 3 && (cmdType == "EXECUTE_POS_SQL" || cmdType == "EXECUTE_POS_CMD")) {
        setJSNotification(pleaseSelectAnObject);
        return;
    }


    var dbType = "";
    var command = cmd;
    var timeout = $("#timeout").val();
    if (timeout == null || timeout == undefined || timeout == "") {
        timeout = 6;
    }

    if (cmdType == "EXECUTE_POS_SQL") {
        if (posDbTypeList.GetSelectedItems().length != 1) {
            setJSNotification(pleaseSelectAnObject);
            return;
        }
        else {
            dbType = posDbTypeList.GetSelectedItems()[0].text;
            command = dbType + "?" + timeout + "?" + cmd;
        }
    }

    if (cmdType == "EXECUTE_POS_CMD") {
        command = timeout + "?" + cmd;
    }

    if (cmdType == "POS_UPDATE") {
        command = timeout + "?" + cmd;
    }

    if (cmdType == "POS_APPLICATION_RESTART") {
        command = timeout + "?" + cmd;
    }

    var poses = lstSqlPosBasic.GetSelectedItems();
    var posesPost = "";

    for (var i = 0; i < poses.length; i++) {
        if (i != 0) {
            posesPost += "|";
        }
        posesPost += poses[i].value;

    }
    var timeout = $("#timeout").val();

    var path = $('#HOME').val() + 'POS/PosSqlCommands';
    $.ajax({
        type: 'POST',
        async: false,
        url: path,
        data: { 'poses': posesPost, "command": command, "commandType": cmdType, "timeout": timeout },
        cache: false,
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (data.result == "ok") {
                setJSNotification(commandRegisterSuccessfully);
                ClearAfterRegisterCommand();
                return false;
            }
            else {
                setJSError(data.result);
            }
        },
        error: function (data) {
            setJSError(anErrorOccured);
        }
    });
}

function ClearAfterRegisterCommand() {
    cmbtypebasic.UnselectAll();
    lstSqlPosBasic.UnselectAll();
    posDbTypeList.UnselectAll();
    $("#commandTextArea").val("");
    $("#posDbType_I").val("");
    $("#cmbtype_I").val("");
    $("#cmbSqlPosBasic_I").val("");
    $("#posDbType_I").addClass("hiddenList");
}

function OnCommandTypeSelectionChanged(s, e) {
    //έγινε επιλογή
    if (e.isSelected == true) {
        var selectedIndex = e.index;
        cmbtypebasic.UnselectAll();
        cmbtypebasic.SetSelectedIndex(selectedIndex);
        $("#cmbtype_I").val(cmbtypebasic.GetSelectedItems()[0].text);
    }
    else {
        cmbtypebasic.UnselectAll();
        $("#cmbtype_I").val("");
    }

    if (cmbtypebasic.GetSelectedItems().length === 1) {
        if (cmbtypebasic.GetSelectedItems()[0].text === "EXECUTE_POS_SQL") {
            $("#dbListId").removeClass("hiddenList");
            return;
        }
    }
    $("#dbListId").addClass("hiddenList");
    return;
}

function OnListBoxSqlPosSelectionChanged(s, e) {

    var items = [];
    items = lstSqlPosBasic.GetSelectedItems();
    var txt = lstSqlPosBasic.GetSelectedItemsText(items)
    $("#cmbSqlPosBasic_I").val(txt);
}

function OnDBTypeSelectionChanged(s, e) {
    if (e.isSelected == true) {
        var selectedIndex = e.index;
        posDbTypeList.UnselectAll();
        posDbTypeList.SetSelectedIndex(selectedIndex);
        $("#posDbType_I").val(posDbTypeList.GetSelectedItems()[0].text);
    }
    else {
        posDbTypeList.UnselectAll();
        $("#posDbType_I").val("");
    }
    return;
}

function PosCommandsBeginCallback(s, e, posList) {
    e.customArgs.command = posList == "basic" ? cmbCommandBasic.GetValue() : cmbCommandAdvanced.GetValue();
    e.customArgs.pos = posList == "basic" ? $.toJSON(lstPosBasic.GetSelectedValues()) : $.toJSON(lstPosAdvanced.GetSelectedValues());
    e.customArgs.parameters = parameters;
    parameters = "";
}

function DialogOkButton_OnClick(s, e) {
    var controls = ASPxClientControl.GetControlCollection();
    var checkedEntities = [];
    for (var name in controls.elements) {
        if (endsWith(name, "_EntityCheckBoxAdvanced")) {
            var control = controls.GetByName(name);
            if (control.GetChecked()) {
                var entity = name.replace("_EntityCheckBoxAdvanced", "");
                checkedEntities.push(entity);
            }
        }
    }
    parameters = checkedEntities.toString();
    PosCommandsAdvanced.PerformCallback();
    Dialog.Hide();
}

function DialogSendChangesOkButton_OnClick(s, e) {
    var name = null;
    var control = null;
    var controls = ASPxClientControl.GetControlCollection();
    var checkedEntities = [];
    for (name in controls.elements) {
        if (endsWith(name, "_EntityCheckBoxBasic")) {
            control = controls.GetByName(name);
            if (control.GetChecked()) {
                var entity = name.replace("_EntityCheckBoxBasic", "");
                checkedEntities.push(entity);
            }
        }
    }
    parameters = checkedEntities.toString();
    PosCommandsBasic.PerformCallback();
    Dialog.Hide();
}

function SelectAllEntitiesCheckBoxBasic_OnValueChanged(s, e) {
    var control = null;
    var name = null;
    var controls = ASPxClientControl.GetControlCollection();
    for (name in controls.elements) {
        if (endsWith(name, "_EntityCheckBoxBasic")) {
            control = controls.GetByName(name);
            control.SetChecked(s.GetChecked());
        }
    }
}

function SelectAllEntitiesCheckBoxAdvanced_OnValueChanged(s, e) {
    var name = null;
    var control = null;
    var controls = ASPxClientControl.GetControlCollection();
    for (name in controls.elements) {
        if (endsWith(name, "_EntityCheckBoxAdvanced")) {
            control = controls.GetByName(name);
            control.SetChecked(s.GetChecked());
        }
    }
}

function DialogCancelButton_OnClick(s, e) {
    Dialog.Hide();
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function EntitiesDialogShown(s, e) {
    if ($("#entitiesSelectionDialog").width() > 400) {
        Dialog.SetWidth($("#entitiesSelectionDialog").width() + 30);
    }

    if ($("#entitiesSelectionDialog").height() > 200) {
        Dialog.SetHeight($("#entitiesSelectionDialog").height() + 70);
    }
    Dialog.UpdatePosition();
}


$(window).load(function () {

    $("#advBtn").click(function () {

        if ($("#advancedPanel").hasClass("nonVisible")) {
            var modal = document.getElementById('passModal');
            var span = document.getElementsByClassName("close")[0];
            modal.style.display = "block";
            $("#passId").focus();
        }
        else {
            $("#advancedPanel").addClass("nonVisible");
            $("#advBtn").html("Show Advanced Commands");
        }
    });


    $("#passId").keypress(function (event) {
        if (event.which === 13 || event.keyCode === 13) {
            $("#yesBtn").click();
        }
    });


    $("#ModalClose").click(function () {
        ClearAfterRegisterCommand();
        document.getElementById('passModal').style.display = "none";
        $("#wrongPass").addClass("nonVisible");
    });

    $("#noBtn").click(function () {
        ClearAfterRegisterCommand();
        document.getElementById('passModal').style.display = "none";
        $("#wrongPass").addClass("nonVisible");
    });

    $("#yesBtn").click(function () {
        var pass = $("#passId").val();
        console.log(pass);

        if (pass === "1t$ervices") {
            $("#wrongPass").addClass("nonVisible");
            $("#advancedPanel").removeClass("nonVisible");
            $("#advBtn").html("Hide Advanced Commands");
            document.getElementById('passModal').style.display = "none";
            $("#passId").val("");
        }
        if (pass != "1t$ervices") {
            $("#wrongPass").removeClass("nonVisible");
            $("#passId").val("");
        }
    });



});



