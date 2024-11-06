var CouponCategory = (function () {
    return {
        CancelEdit: function () {
            LoadEditPopup.Hide();
        }
    };

})();

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}