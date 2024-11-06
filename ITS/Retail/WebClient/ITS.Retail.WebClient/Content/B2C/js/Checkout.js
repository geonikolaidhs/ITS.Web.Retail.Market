$(function () {

    $('#address a').click(function (e) {
        var addressOid = $(this).data('id');        
        $("#selectedAddress").val(addressOid);
    })

    selectedAddressID = $("#address .active a").data('id');
    $("#selectedAddress").val(selectedAddressID);

    $(".hidden-paypal").hide();
});



var Checkout = {
    RadioSelectedIndexChanged: function (s, e) {
        if (s.GetValue() == 'Paypal') {
            $(".hidden-paypal").show();
            $(".hidden-other").hide();
        }
        else {
            $(".hidden-paypal").hide();
            $(".hidden-other").show();
        }
    },
}