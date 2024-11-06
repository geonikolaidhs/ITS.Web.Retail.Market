var Wizard = {
    RefreshCustomDataViewParameters: function () {
        CustomDataViewParametersCallbackPanel.PerformCallback({ customDataViewOid: CustomDataView.GetValue() });
    },
    ClearCustomDataViewSelector: function (s,e) {
        clearComboBox(s, e);
        CustomDataViewParametersCallbackPanel.PerformCallback({ customDataViewOid: '' });
    }
};