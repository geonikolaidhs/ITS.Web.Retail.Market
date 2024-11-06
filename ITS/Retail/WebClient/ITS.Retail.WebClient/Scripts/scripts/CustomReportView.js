function OnClick_btnPreview(s, e) {
    reportCallbackPanel.PerformCallback();
    toolbarHideFiltersOnly();
}

var performPrint = false;

function OnClick_btnPrint(s, e) {
    OnClick_btnPreview(btnPreview);
    performPrint = true;
}

function OnBeginCallback_CustomReportCallbackPanel(s, e) {
    var controls = ASPxClientControl.GetControlCollection();
    for (var name in controls.elements) {
        if (endsWith(name, "_reportParam")) {
            var control = controls.GetByName(name);
            if (control.ChangeDate)  //control is date edit
            {
                e.customArgs[control.name] = control.GetText();
            }
            else if (control.GetNumber)  //control is spin edit
            {
                e.customArgs[control.name] = control.GetNumber() * 10000;
            }
            else if (control.GetSelectedValues) //control is ListBox
            {
                e.customArgs[control.name] = control.GetSelectedValues().toString();
            }
            else {
                e.customArgs[control.name] = control.GetValue();
            }

        }
    }

    $.each($("input[type='hidden']"), function (index, value) {
        if (endsWith($(value)[0].id, "_reportParam")) {
            e.customArgs[$(value)[0].id] = $(value).val();
        }
    });
    e.customArgs.Oid = $('#Oid')[0].value;
    e.customArgs.ObjectOid = $('#ObjectOid')[0].value;
    e.customArgs.Duplicates = $('#Duplicates')[0].value;
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function OnBeginCallback_CustomReportViewer(s, e) {
    e.customArgs.Oid = $('#Oid')[0].value;
    e.customArgs.ObjectOid = $('#ObjectOid')[0].value;
    e.customArgs.Duplicates = $('#Duplicates')[0].value;
}

function OnEndCallback_CustomReportViewer(s, e) {
    if (performPrint) {
        performPrint = false;
        ReportToolbar.reportViewer.Print();
    }
}

function OnBeforeExportRequest_CustomReportViewer(s, e) {
    e.customArgs.Oid = $('#Oid')[0].value;
    e.customArgs.ObjectOid = $('#ObjectOid')[0].value;
    e.customArgs.Duplicates = $('#Duplicates')[0].value;
}

function OnCheckChanged_SelectAllCheckbox(s, e) {
    var listboxName = s.name.split('_')[0] + "_reportParam";
    var listbox = ASPxClientControl.GetControlCollection().GetByName(listboxName);
    if (s.GetValue()) {
        listbox.SelectAll();
    }
    else {
        listbox.UnselectAll();
    }

}

function OnSelectedIndexChanged_ParameterListBox(s,e) {
    var selectAllCheckboxName = s.name.split('_')[0] + "_selectAllCheckbox";
    var checkBox = ASPxClientControl.GetControlCollection().GetByName(selectAllCheckboxName);
    var totalItemsCount = s.GetItemCount();
    var selectedItemsCount = s.GetSelectedIndices().length;
    if(totalItemsCount === selectedItemsCount)
    {
        checkBox.SetValue(true);
    }
    else
    {
        checkBox.SetValue(false);
    }
}

function WebDocumentViewer_CustomizeParameterEditors(s,e)
{
    if (e.info.editor.header == 'dxrd-combobox')
    {
        if (typeof (e.info.valuesArray) == "undefined" || e.info.valuesArray.length > 15) {
            e.info.editor.header = 'combobox-with-paging';
        }
    }
}