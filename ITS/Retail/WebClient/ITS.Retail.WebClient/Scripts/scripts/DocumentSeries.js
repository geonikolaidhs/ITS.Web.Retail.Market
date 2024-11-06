function StoresComboBoxValueChanged(s,e) {
    DocumentSeriesCbPanel.PerformCallback();
    POSComboBox.PerformCallback({ storeOid: StoresComboBox.GetValue() });
}

function eModuleValueChanged(s, e) {
    if(s.GetValue() != "POS"){
        POSComboBox.SetEnabled(false);
        POSComboBox.SetValue(null);
    }
    else {
        POSComboBox.SetEnabled(true);
    }
    var docNumberElement = Component.GetName("DocumentSequence.DocumentNumber");
    if (docNumberElement != null)
    {
        docNumberElement.SetEnabled(("HEADQUARTERS" == s.GetValue()));
    }
    //POSComboBox.PerformCallback({ storeOid: StoresComboBox.GetValue() });
}

function DocumentSeriesCbPanelOnBeginCallback(s, e) {
    e.customArgs.Store = StoresComboBox.GetValue();
}

function DocumentSeriesCbPanelOnEndCallback(s, e) {
    DocumentSeriesCbPanel.SetEnabled(!IsCancelingSeries.GetValue());
}

function IsCancelingSeriesValueChanged(s, e) {
    if (IsCancelingSeries.GetValue()) {
        DocumentSeriesComboBox.SetValue();
    }
    DocumentSeriesCbPanel.SetEnabled(!IsCancelingSeries.GetValue());    
}

function IsCancelingSeriesCheckChanged(s, e) {
    if (!IsCancelingSeries.GetValue()) {
        DocumentSeriesCbPanel.SetVisible(true);
        $("#SeriesComboBoxName").show();
    }
    else {
        DocumentSeriesCbPanel.SetVisible(false);
        $("#SeriesComboBoxName").hide();
    }
}

function clearDocumentSeriesComboBox(s, e) {
    if (e.buttonIndex === 0) {        
        clearComboBox(s, e);
    } 
}
