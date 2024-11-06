var VariableValuesDisplay = (function () {
    return {
        OnDataViewsCategoryChanged: function (s, e) {
            CustomDataView.PerformCallback({ categoryOid: Component.GetName("CustomDataViewsCategory").GetValue() });
        },
        DisplayVariableValues: function (s, e) {
            $("#gridOids").val(selectedItemsArray.toString());
            $("#customDataViewOid").val(Component.GetName("CustomDataView").GetValue());
            $("#varvalues-form").submit();
        },
        ShowVariableValuesPopUp: function () {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            }
            else {
                $('#variableValuesButton').modal({
                    keyboard: false
                })
            }
        },
        VariableValuesPopUpClose: function () {
            VariableValuesPopUp.Hide();
        }
    };
})();


$(document).on("click",".btn.showVar", function () {
    VariableValuesDisplay.ShowVariableValuesPopUp();
})