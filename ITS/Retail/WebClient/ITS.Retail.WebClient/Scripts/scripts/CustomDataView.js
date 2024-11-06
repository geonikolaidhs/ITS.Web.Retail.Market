var CustomDataView = {
    OnDataViewsCategoryChanged: function (s, e) {
        grdCustomDataViews.PerformCallback();
    },
    grdOnBeginCallback: function (s, e) {
        e.customArgs.CustomDataViewsCategory = Component.GetName("CustomDataViewsCategory").GetValue();
    },
    OnCustomDataViewInit: function(s, e) {
        var initialOrgs = $('#Roles_initial').val();
        var values = initialOrgs.split(',');
        lstRoles.SelectValues(values);
    }
};  