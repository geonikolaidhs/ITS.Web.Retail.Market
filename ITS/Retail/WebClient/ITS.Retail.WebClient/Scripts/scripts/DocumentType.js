var clopy = false;

function CopySelectedDocumentType() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToCopy);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToCopy);
    }
    else if (selectedItemsArray.length == 1) {        
        clopy = true;
        grdDocumentType.AddNewRow();
        return false;
    }
}

function DocumentTypeOnBeginCallback(s, e)
{
    if(clopy !== null && typeof clopy !== "undefined" && clopy === true){
        e.customArgs.COPY = objectToBeCopied;
        clopy = false;
    }
}

function OnActionTypeEntityInit(s,e) {
    var initialOrgs = $('#DocStatuses_initial').val();
    var values = initialOrgs.split(',');
    lstDocStatus.SelectValues(values);
}





