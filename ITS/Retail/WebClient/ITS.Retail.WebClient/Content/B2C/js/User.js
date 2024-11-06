var User = {    
    ChangePasswordOnComplete: function (s, e) {
        ResetPasswordCallbackPanel.PerformCallback();
        Layout.notifyUser();
    },
}