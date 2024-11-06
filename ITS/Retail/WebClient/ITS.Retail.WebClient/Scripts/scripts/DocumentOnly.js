function OnInitPopupEditCallbackPanel(s, e) {

    if (selectedItemsArray.length === 0 && isFromTransformation === null && hasReturnedFromOrderItemsForm === null) {
        //AddNewItem Situation
        e.customArgs.ID = "00000000-0000-0000-0000-000000000000";
        e.customArgs.Recover = false;
        e.customArgs.Division = DocumentsDivision;
    }
    else if (isFromTransformation === true) {
        if (selectedItemsArray.length === 0) {
            e.customArgs.ID = "00000000-0000-0000-0000-000000000000";
        }
        else {
            e.customArgs.ID = selectedItemsArray[0];
            selectedItemsArray = [];
        }

        e.customArgs.LoadFromSession = isFromTransformation;
        e.customArgs.Division = DocumentsDivision;
        isFromTransformation = null;
    }
    else if ($("#HasReturnedFromOrderItemsForm").length === 1 && $("#HasReturnedFromOrderItemsForm").val() === 'True' && hasReturnedFromOrderItemsForm === true) {
        e.customArgs.ID = $("#Oid").val();
        e.customArgs.Division = $("#Mode").val() === "" ? DocumentsDivision : $("#Mode").val();
        e.customArgs.HasReturnedFromOrderItemsForm = true;
        hasReturnedFromOrderItemsForm = null;
    }
    else if (isTemporaryObject === true) {
        isTemporaryObject = null;
        e.customArgs.ID = selectedItemsArray[0];
        e.customArgs.RestoreTemporary = selectedItemsArray[0];
        e.customArgs.Division = DocumentsDivision;

    }
    else {
        var result = CheckSelectedDocumentForEditing(selectedItemsArray[0]);
        if (result.returnValue) {
            e.customArgs.ID = selectedItemsArray[0];
            if (result.warnForCrashed && confirm(documentHasCrashed) === false) {
                e.customArgs.Recover = false;
            }
            else if (result.warnForCrashed) {
                e.customArgs.Recover = true;
            }
        }
    }
}