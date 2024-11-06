var parameters = "";

function SelectAllEntitiesCheckBox_OnValueChanged(s, e) {
    var controls = ASPxClientControl.GetControlCollection();
    for (var name in controls.elements) {
        if (endsWith(name, "_EntityCheckBox")) {
            var control = controls.GetByName(name);
            control.SetChecked(s.GetChecked());
        }
    }
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

