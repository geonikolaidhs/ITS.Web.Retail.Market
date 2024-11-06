(function() {
  var Document;

  Document = (function($) {
    return {
      grdOnEndCallback: function() {
        var filterSelected;
        Helpers.ToggleSelectAll();
        GridCustomizationPopUp.DisplayCustomizationWindowColor();
        if (filterSelected === true) {
          Component.GetName(gridName).UnselectRows();
          filterSelected = false;
        }
      }
    };
  })($);

}).call(this);

//# sourceMappingURL=DocumentRC.js.map
