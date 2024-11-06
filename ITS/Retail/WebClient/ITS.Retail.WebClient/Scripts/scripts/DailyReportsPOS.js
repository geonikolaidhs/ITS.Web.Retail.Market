function ShowPopups() {
    if (selectedItemsArray.length === 0) {
        setJSNotification(pleaseSelectAnObjectToView);
    }
    else if (selectedItemsArray.length > 1) {
        setJSNotification(pleaseSelectOnlyOneObjectToView);
        }
    else if (selectedItemsArray.length == 1) {   
        ASPxClientControl.GetControlCollection().GetByName("dailyReportPOS_" + selectedItemsArray[0].replaceAll("-", "_")).Show();
        grdDailyGrid.UnselectAllRowsOnPage();
    }
}





