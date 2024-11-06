"@fixture Application Log Page";
"@page ./ApplicationLog/Index";
"@auth admin:admin";
'@require ./../../wrm_mixins/authentication/admin_authentication.js';

"@test"["Search Application Log"] = {
    '1.Log In': '@mixin AdminAuthentication',
    "2.Check if Filters Exist": function() {
        eq($("#created").length > 0, true, "Created");
        eq($("#user").length > 0, true, "Created By User");
        eq($("#controller").length > 0, true, "Controller");
        eq($("#ip").length > 0, true, "IP");
        eq($("#btnSearch").length > 0, true, "Search Input");
        eq($("#clear_log").length > 0, true, "Delete All");
    },
    '3.Click span "Αναζήτηση"': function() {
        var actionTarget = function() {
            return $("#btnSearch_CD").find(".dx-vam");
        };
        act.click(actionTarget);
    },
    "4.Wait for Options to Appear": function() {
        act.waitFor(".btn.options");
    },
    '5.Click div "Επιλογές"': function() {
        act.click(".btn.options");
    },
    '6.Click div "Εμφάνιση/Απόκρυψη..."': function() {
        act.click("#gear");
    },
    "7.Click div Options Main": function() {
        act.click(".fast_menu_options.hidden");
    },
    '8.Click div "Επιλογές"': function() {
        act.click(".btn.options");
    }
};
