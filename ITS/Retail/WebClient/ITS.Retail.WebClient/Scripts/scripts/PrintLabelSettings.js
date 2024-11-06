function OnValueChangedPrintingTypeRadioList(s, e) {
    if (s.GetValue() == "com") {
        $("#comSettings").show();
    }
    else if (s.GetValue() == "windowsdriver") {
        $("#comSettings").hide();
    }
}

