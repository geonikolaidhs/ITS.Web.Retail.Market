'@mixin'['CustomerAuthentication'] = {
    "1.Wait for UserName to Appear": function() {
      act.waitFor("#UserName");
    },
    '2.Hover over input "UserName"': function() {
        act.hover("#UserName");
    },
    '3.Type in input "UserName"': function() {
        act.type("#UserName", "Customer");
    },
    "4.Check if username input exists": function() {
        eq($("#UserName").length > 0, 1, "Username input exists");
    },
    '5.Hover over password input "Password"': function() {
        act.hover("#Password");
    },
    '6.Type in password input "Password"': function() {
        act.type("#Password", "Aa123456");
    },
    "7.Check if input password exists": function() {
        eq($("#Password").length > 0, 1, "Password exists");
    },
    '8.Hover over div "#Login"': function() {
        act.hover("#Login");
    },
    "9.Check if Login Button Exists": function() {
        eq($("#Login").length > 0, 1, "Login exists");
    },
    '10.Click span "Είσοδος"': function() {
        var actionTarget = function() {
            return $("#Login_CD").find(".dx-vam");
        };
        act.click(actionTarget);
    },
    "11.Wait for Menu Label to Appear": function() {
      act.waitFor("#menuLabel");
    }
};
