
var Coupon = (function () {

    return {
        ShowMassivelyCreateCouponsForm: function (url) {
            window.location = url;
        },
        CancelMassivelyCreateCoupons: function () {
            location.href = document.referrer;
        },
        MassivelyCreateCoupons: function () {
            if (ValidateModalFormSingle() === false) {
                setJSError(markedFieldsAreRequired);
            }
            else {
                var form_to_submit = Component.GetCorrectForm(document.forms[0]);
                $.ajax({
                    url: form_to_submit.action,
                    type: "POST",
                    data: $("#form0").serialize()
                })
                .done(function (ajaxResult) {
                    if (ajaxResult.success) {
                        location.href = document.referrer;
                    }
                    setJSNotification(ajaxResult.message);
                });
            }
        },
        CancelEdit: function () {
            LoadEditPopup.Hide();
        },
        UsePaddingOnValueChanged: function (component, event) {
            if (component.GetValue() === null) {
                component.SetValue(false);
            }

            var selectedValue = UsePadding.GetValue();

            if (selectedValue) {
                $('.padding-related-fields').show();
            }
            else {
                $('.padding-related-fields').hide();
            }
        },
        HandleChangeCouponAmountIsAppliedAs: function (component, event) {
            var selectedValue = component.GetValue();

            if (selectedValue === 'PAYMENT_METHOD') {
                $('#DiscountType-Container').hide('1000');
                $('#PaymentMethod-Container').show('1000');
            }
            else if (selectedValue === 'DISCOUNT') {
                $('#PaymentMethod-Container').hide('1000');
                $('#DiscountType-Container').show('1000');
            }
            else {
                $('#DiscountType-Container').hide('1000');
                $('#PaymentMethod-Container').hide('1000');
            }

        }
    };

})();

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}