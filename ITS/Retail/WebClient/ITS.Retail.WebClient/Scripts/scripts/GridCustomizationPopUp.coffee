GridCustomizationPopUp = do($) ->
	GetHiddenColumnsCounter = ->
    while index < Component.GetName(gridName).GetColumnCount() and index >= 0
      if Component.GetName(gridName).GetColumn(index).visible == false
        count++
      index++
    return count      
	{
		DisplayCustomizationWindowColor: ->
    		if GetHiddenColumnsCounter == 0 
    			$('.btCustomizationWindow').removeClass('color-red');
    		else
    			$('.btCustomizationWindow').addClass('color-red');
	} 
	
	