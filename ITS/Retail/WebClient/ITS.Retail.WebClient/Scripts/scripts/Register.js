$(document).ready(function () {  
    $('#password_message').hide();
    $('#password_confirm_message').hide();
    $('#pos_password_message').hide();
    $('#pos_password_confirm_message').hide();
});

function MatchingPasswords(s, e) 
{
    if ((PasswordConfirm.GetValue() != Password.GetValue()) && PasswordConfirm.GetValue() !== null) 
    {
        $('#password_message').show();
        $('#password_confirm_message').show();
    }
}

function MatchingPOSPasswords(s, e) {
    if ((POSPasswordConfirm.GetValue() != POSPassword.GetValue()) && POSPasswordConfirm.GetValue() !== null) {
        $('#pos_password_message').show();
        $('#pos_password_confirm_message').show();
    }
}

function ClearMessages(s, e){
    $('#MessageLabel').html('');
    $('#password_message').hide();
    $('#password_confirm_message').hide();
}

function ClearPOSMessages(s, e) {
    $('#pos_password_message').hide();
    $('#pos_password_confirm_message').hide();
}

function CharFilter(s, e) 
{
    var unicode = e.htmlEvent.charCode ? e.htmlEvent.charCode : e.htmlEvent.keyCode;
    var actualkey = String.fromCharCode(unicode);
    if (actualkey == " ") {
        $('#MessageLabel').html(invalidCharacter);
        e.htmlEvent.preventDefault();
    }
    else {
        $('#MessageLabel').html('');
        e.htmlEvent.returnValue = true;
    }
}

function OnClickBtnRegister(s, e) {
    if ($.trim(Username.GetValue()) === "" || $.trim(Password.GetValue()) === "" || $.trim(PasswordConfirm.GetValue()) === "" ||
        $.trim(FullName.GetValue()) === "" || $.trim(TaxCode.GetValue()) === "") {
        setJSError(fillInAllTheFields);
    }
    else {
        var form_to_submit = Component.GetCorrectForm(document.forms[0]);
        form_to_submit.submit();
    }
}