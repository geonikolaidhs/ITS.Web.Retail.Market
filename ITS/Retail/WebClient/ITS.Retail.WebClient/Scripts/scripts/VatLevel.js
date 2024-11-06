function FixPercentage(s, e) {
    var value = parseFloat(Factor.GetValue().replace(",","."))*100;
    $("#factor_label").text(" (" + value + "%)");
    $("#factor_label").css("color", "red");
}

function VatOnBeginCallback(s, e) {
    if (s.IsEditing()) {
        //e.customArgs.IsDefault = IsDefaultCheckbox.GetValue();
        e.customArgs.Fsupplier = Component.GetName('OwnersComboBox') === null ? null : Component.GetName('OwnersComboBox').GetValue();
    }
}

//function VatFactorBeginCallback(s, e) {
//    if (s.IsEditing()) {
//        e.customArgs['IsDefault'] = IsDefaultCheckbox.GetValue();
//        var comboBox = Component.GetName('VatLevel!Key');
//        if (comboBox != null) {
//            e.customArgs['VatLevelId'] = comboBox.GetValue();
//        }

//        comboBox = Component.GetName('VatCategory!Key');
//        if (comboBox != null) {
//            e.customArgs['VatCategoryId'] = comboBox.GetValue();
//        }
//    }
//}