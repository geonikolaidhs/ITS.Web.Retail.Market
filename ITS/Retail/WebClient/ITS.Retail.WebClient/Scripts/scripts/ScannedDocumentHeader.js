function PreviewImage(s, e) {    
    for (var i = 0; i < selectedItemsArray.length; i++) {
        Component.GetName("ScannedDocumentPopup_" + selectedItemsArray[i].replaceAll("-", "_")).Show();
    }    
}

