$(function () {
    $('.info_banner .login-logo-container #logo').resizeToParent(".info_banner");
});

function login_submit() {
    if ($('#UserName').val() == '') {
        setJSError(usernameIsNull);
        return false;
    }
    if ($('#Password').val() == '') {
        setJSError(passwordIsNull);
        return false;
    }
}
