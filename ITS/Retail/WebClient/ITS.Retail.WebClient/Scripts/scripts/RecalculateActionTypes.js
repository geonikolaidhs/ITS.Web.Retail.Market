var RecalculateActionTypes;

RecalculateActionTypes = (function() {
  var AjaxRecalculate;
  AjaxRecalculate = function() {
    $.ajax({
      type: 'POST',
      url: RecalculateActionTypesForm.action,
      data: $('#RecalculateActionTypesForm').serialize(),
      cache: false,
      success: function(data) {
        if (data.error != null) {
          setJSNotification(data.error);
        } else {
          ActionTypeRecalculateForm.Hide();
        }
      }
    });
    return false;
  };
  return {
    Recalculate: function(s, e) {
      AjaxRecalculate();
    },
    Cancel: function(s, e) {
      ActionTypeRecalculateForm.Hide();
    }
  };
})();

//# sourceMappingURL=RecalculateActionTypes.js.map
