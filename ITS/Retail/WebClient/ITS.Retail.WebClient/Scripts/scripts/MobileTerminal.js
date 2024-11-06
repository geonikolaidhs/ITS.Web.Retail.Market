var MobileTerminal, OnInitPopupEditCallbackPanel;

OnInitPopupEditCallbackPanel = function(s, e) {
  Component.OnInitPopupEditCallbackPanel(s, e);
};

MobileTerminal = (function() {
  return {
    CancelEdit: function(s, e) {
      LoadEditPopup.Hide();
    }
  };
})();

//# sourceMappingURL=MobileTerminal.js.map
