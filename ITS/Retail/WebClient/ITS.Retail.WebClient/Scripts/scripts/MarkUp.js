
function RetrieveValueChanges(s, e) {
    if (!grdMarkUp.InCallback()) {
        grdMarkUp.PerformCallback();
    }
}

function sensitivityKeyPress(s, e) {

    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    //enter pressed
    if (unicode == 13) {
        RetrieveValueChanges();
    }
}

function grdMarkUpBeginCallback(s, e) {
    e.customArgs.AllValues = AllValues.GetValue();
    e.customArgs.sensitivity = sensitivity.GetValue()*QUANTITY_MULTIPLIER;  
}

function markupKeyPress(s, e) {
    unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (unicode == 13) {
        SetMarkUp(s.uniqueID);
    }
    
}

function markupLostFocus(s, e) {
    s.ParseValue();
    SetMarkUp(s.uniqueID);
}

function SetMarkUp(id) {
    $.ajax({
        type: 'POST',
        url:  $('#HOME').val() + 'Document/SetMarkUp',
        data: {
            'inputID': id,
            'markUpValue': Math.round(ASPxClientControl.GetControlCollection().GetByName(id).GetValue() * QUANTITY_MULTIPLIER)
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success === true) {
                Component.GetName(id).SetValue(data.markup * 100);
                selected = Component.GetName(id.replace('markup', 'selected'));
                selected.SetValue(data.selected);
                unit_price = ASPxClientControl.GetControlCollection().GetByName(id.replace('markup', 'unit_price'));
                unit_price.SetValue(data.unit_price);
                unit_price.Focus();
            }
        },
        error: commonError
    });
}

function unit_priceLostFocus(s, e) {
    s.ParseValue();
    SetUnitPrice(s.uniqueID);
}

function unit_priceKeyPress(s, e) {
    unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    if (unicode == 13) {        
        SetUnitPrice(s.uniqueID);
    }
}

function SetUnitPrice(id) {
    $.ajax({
        type: 'POST',
        url: $('#HOME').val() + 'Document/SetUnitPrice',
        data: {
            'inputID': id,
            'unitPriceValue': Math.round(Component.GetName(id).GetValue() * QUANTITY_MULTIPLIER * QUANTITY_MULTIPLIER)
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success === true) {
                Component.GetName(id).SetValue(data.unit_price);
                selected = Component.GetName(id.replace('unit_price', 'selected'));
                selected.SetValue(data.selected);
                var markup = ASPxClientControl.GetControlCollection().GetByName(id.replace('unit_price', 'markup'));
                markup.SetValue(data.markup * 100);
                Component.GetName(data.focus_on).Focus();
            }
        },
        error: commonError
    });
}

function SaveMarkUps(s, e) {
    var saveMarkups = ASPxClientControl.GetControlCollection().GetByName('SaveMarkUpsCheck');
    var path = $('#HOME').val() + 'Document/SaveMarkUps';
    $.ajax({
        type: 'POST',
        url: path,
        data: {           
            'saveMarkUps': saveMarkups !== null ? saveMarkups.GetValue() : false
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.success) {
                MarkUpPopUp.Hide();
            }
            UpdateTemporaryFilterForm();
        },
        error: commonError
    });
}

function CancelMarkUps(s, e) {
    MarkUpPopUp.Hide();
    UpdateTemporaryFilterForm();
}

function grdMarkUpValueChanged(s, e) {
    var path = $('#HOME').val() + 'Document/SetSelected';
    $.ajax({
        type: 'POST',
        url: path,
        data: {
            'inputID': s.uniqueID,
            'selected': Component.GetName(s.uniqueID).GetValue()
        },
        cache: false,
        dataType: 'json',
        success: function (data) {            
        },
        error: commonError
    });
}