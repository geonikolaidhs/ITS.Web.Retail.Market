(function() {
  var GridCustomizationPopUp;

  GridCustomizationPopUp = (function($) {
    var GetHiddenColumnsCounter;
    GetHiddenColumnsCounter = function() {
      while (index < Component.GetName(gridName).GetColumnCount() && index >= 0) {
        if (Component.GetName(gridName).GetColumn(index).visible === false) {
          count++;
        }
        index++;
      }
      return count;
    };
    return {
      DisplayCustomizationWindowColor: function() {
        if (GetHiddenColumnsCounter === 0) {
          return $('.btCustomizationWindow').removeClass('color-red');
        } else {
          return $('.btCustomizationWindow').addClass('color-red');
        }
      }
    };
  })($);

}).call(this);

//# sourceMappingURL=GridCustomizationPopUp.js.map
