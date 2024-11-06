$(document).ready(function () {
    setInterval(StoreControllerStatus.StatusesUpdate, 5000);
});

var StoreControllerStatus = (function () {
    var textSeparator = ";";
    IsAllSelected = function() {
        for (var i = 1; i < lstStore.GetItemCount() ; i++) {
            if (!lstStore.GetItem(i).selected) {
                return false;
            }
        }
        return true;
    };
    UpdateSelectAllItemState = function () {
         
        if (IsAllSelected()) {
            lstStore.SelectIndices([0]);
        }
        else {
            lstStore.UnselectIndices([0]);
        }
    };

    UpdateText = function () {
        var selectedItems = lstStore.GetSelectedItems();
        cmbStore.SetText(GetSelectedItemsText(selectedItems));
    };

    GetValuesByTexts = function (texts) {
        var actualValues = [];
        var item;
        for (var i = 0; i < texts.length; i++) {
            item = lstStore.FindItemByText(texts[i]);
            if (item !== null)
                actualValues.push(item.value);
        }
        return actualValues;
    };

    GetSelectedItemsText = function (items) {
        var texts = [];
        for (var i = 0; i < items.length; i++) {
            if (items[i].index !== 0) {
                texts.push(items[i].text);
            }
        }
        return texts.join(textSeparator);
    };

    return {
        CommandsBeginCallback: function (s, e) {
            e.customArgs.command = cmbCommand.GetValue();
            e.customArgs.stores = $.toJSON(lstStore.GetSelectedValues());
            parameters = "";
            e.customArgs.parameters = parameters;
        },

        OnListBoxSelectionChanged: function (listBox, args) {
            if (args.index === 0) {
                if (args.isSelected)
                    listBox.SelectAll();
                else
                    listBox.UnselectAll();
            }

            UpdateSelectAllItemState();
            UpdateText();
        },
        SynchronizeListBoxValues: function (dropDown, args) {
            lstStore.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts(texts);
            lstStore.SelectValues(values);
            UpdateSelectAllItemState();
            UpdateText();
        },
        SendCommandButtonBasicClick: function (s, e) {

            if (lstStore.GetSelectedItems().length === 0) {
                setJSNotification(pleaseSelectAnObject);
                return;
            }

            StoreControllerCommands.PerformCallback(); // Αποστολή εντολών στον Controller

        },
        StatusesUpdate: function (s, e) {
            grdStoreControllerSettings.PerformCallback();
        }

    };
})();
