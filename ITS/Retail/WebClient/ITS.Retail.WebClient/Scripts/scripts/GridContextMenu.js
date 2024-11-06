function CustomizationWindowBtn_Click(s, e) {
    var grid = ASPxClientGridView.Cast(s);

    if (e.objectType == "header") {
        if (grid.IsCustomizationWindowVisible()) {
            grid.HideCustomizationWindow();
        }
        else {
            grid.ShowCustomizationWindow(e.htmlEvent.srcElement);

        }

    } 
}