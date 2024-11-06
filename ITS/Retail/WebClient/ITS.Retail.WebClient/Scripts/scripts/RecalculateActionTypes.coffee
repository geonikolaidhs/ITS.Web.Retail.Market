# CoffeeScript
RecalculateActionTypes = do ->
    AjaxRecalculate = () ->
        $.ajax
            type: 'POST'
            url: RecalculateActionTypesForm.action
            data: $('#RecalculateActionTypesForm').serialize()
            cache: false
            success: (data) ->
                if data.error?
                    setJSNotification data.error
                else
                    ActionTypeRecalculateForm.Hide()
                return
        return false
            
    {
         Recalculate: (s, e) ->
            AjaxRecalculate()
            return
         Cancel: (s,e) ->
            ActionTypeRecalculateForm.Hide()
            return
    }
    
    