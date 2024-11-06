OnInitPopupEditCallbackPanel = (s,e) ->
    Component.OnInitPopupEditCallbackPanel(s, e)
    return
    
    
MobileTerminal = do ->
    {
        CancelEdit : (s, e) ->
            LoadEditPopup.Hide()
            return
       
    }