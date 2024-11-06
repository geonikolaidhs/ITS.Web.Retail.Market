$(document).ready(function () {
    $('#settings ul li ul li.subtitle').each(function () {
        var oid = $(this).children('h4').data("id");
        if (oid == category) {       
            $(this).parent().find('span').addClass('maximaze');
            $(this).parent().find('.options li').slideToggle();
            $(this).css('background-color', '#06A7FF');
            $(this).css('color', '#fff');
        }
    });
});
