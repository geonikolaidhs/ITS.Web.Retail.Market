var Profile = {    
    UpdateProfileOnBegin: function (s, e) {
        fullnameProfileForm = FirstName.GetValue()+' ' + LastName.GetValue();
    },
    UpdateProfileOnComplete: function (s, e) {
        ProfileCallbackPanel.PerformCallback();
        Layout.notifyUser();
        
        $(".current-user-name").html(fullnameProfileForm);
    },
}