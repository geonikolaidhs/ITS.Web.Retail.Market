

function OnInitPopupEditCallbackPanel(s, e) {
    Component.OnInitPopupEditCallbackPanel(s, e);
}


var Variable = (function () {
    (function ($) {
        $.fn.getCursorPosition = function () {
            var el = $(this).get(0);
            var pos = 0;
            if ('selectionStart' in el) {
                pos = el.selectionStart;
            }
            else if ('selection' in document) {
                el.focus();
                var Sel = document.selection.createRange();
                var SelLength = document.selection.createRange().text.length;
                Sel.moveStart('character', -el.value.length);
                pos = Sel.text.length - SelLength;
            }
            return pos;
        }
    })(jQuery);

    InsertAtCursorPos = function(text) {
        var content = Component.GetName('Expression').GetValue() === null ? "" : Component.GetName('Expression').GetValue();
        var position = $("#Expression_I").getCursorPosition();
        var newContent = position !== 0 ? content.substr(0, position) + text + content.substr(position) : content + text;
        Component.GetName('Expression').SetValue(newContent);
    };

    AddFieldValuetoExpression =  function (name,char1,char2) {
        var field = Component.GetName(name);
        var source = Component.GetName('Source').GetValue();
        if (field.GetValue() !== null) {
            if (source == "FIELD") {
                Component.GetName('Expression').SetValue(char1 + field.GetValue() + char2);
            }
            else if (source == "FORMULA") {
                InsertAtCursorPos(char1 + field.GetValue() + char2);
            }
        }
        return true;
    };

    return{
        ComboBoxButtonOnClick: function(s,e) {
            if (e.buttonIndex === 0) {
                s.SetText("");
                s.SetValue(null);
            }
            else if (e.buttonIndex === 1) {
                var startchar;
                var endchar;
                if (s.name == "Variables")
                {
                    startchar = "[";
                    endchar = "]";
                }
                if (s.name == "EntityFields") {
                    startchar = endchar = "$";
                }
                AddFieldValuetoExpression(s.name, startchar, endchar);
            }
        },
        OnVariableCategoryChanged: function (s,e) {
            EntityFieldsCallBackPanel.PerformCallback({
                category: s.GetValue(),
                variableSource: Source.GetValue()
            });
        },
        OnVariableTargetCategoryChanged: function (s, e) {
            TargetFieldsCallBackPanel.PerformCallback(
                //{
                //    category: s.GetValue(),
                //    variableSource: Source.GetValue()
                //}
                );
        },
        OnVariableDestinationChanged: function (s, e) {
            if (s.lastSuccessValue === "FIELD") {
                $(".EntityFieldsComboBoxContainer").attr('colspan', 4);
                Component.GetName('Expression').SetValue("");
                Component.GetName('Expression').SetEnabled(false);
                Component.GetName('Variables').SetClientVisible(false);
            }
            else if (s.lastSuccessValue === "FORMULA") {
                $(".EntityFieldsComboBoxContainer").attr('colspan', 2);
                Component.GetName('Expression').SetEnabled(true);
                Component.GetName('Variables').SetClientVisible(true);
                Component.GetByID("EntityFields_B1").SetClientVisible = true;
            }
            EntityFieldsCallBackPanel.PerformCallback({
                category: Category.GetValue(),
                variableSource: Source.GetValue()
            });
        },
        RecalculateActionTypes: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            } else {
                ActionTypeRecalculateForm.Show();
                ActionTypeRecalculateForm.PerformCallback();
            }
        },
        ActionTypeRecalculateFormBeginCallback: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            } else {
                e.customArgs['Variables'] = selectedItemsArray.toString();
            }
        },
        ExportButtonOnClick: function (s, e) {
            if (selectedItemsArray.length === 0) {
                setJSNotification(pleaseSelectAnObject);
            }
            else {
                window.location.href = $("#HOME").val() + "Variable/ExportVariables?VariablesOids=" + selectedItemsArray.toString();
            }
        },
        onFileUploadComplete_UploadControl: function (s, e) {
            if (e.errorText !== "" && e.errorText !== null) {
                setJSError(e.errorText);
            }
            else {
                Dialog.Hide();
                grdVariables.PerformCallback();
            }
        },
        ImportButtonOnClick: function (s, e) {
            DialogCallbackPanel.PerformCallback();
        }
    }
})();