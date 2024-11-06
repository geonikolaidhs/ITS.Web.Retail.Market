Document = do($) ->
	    
	{   
	    grdOnEndCallback: () ->	       
	        Helpers.ToggleSelectAll()
	        GridCustomizationPopUp.DisplayCustomizationWindowColor()
	        
	        if filterSelected == true            
                Component.GetName(gridName).UnselectRows()
                filterSelected = false           
            
	        return
	}