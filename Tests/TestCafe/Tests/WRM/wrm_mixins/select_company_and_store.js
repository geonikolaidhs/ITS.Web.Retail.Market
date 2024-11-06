"@mixin"["SelectCompanyAndStore"] = {
    "1.Assert Company Menu Exists": function() {
        eq($("#LayoutCompanySelection").length > 0, true, "Select Company Menu Exists");
    },
    "2.Hover over Select Company Menu": function() {
        act.hover("#LayoutCompanySelection");
    },
    "3.Click Select Company Menu": function() {
        act.click("#LayoutCompanySelection");
    },    
    "4.Wait for Company Selection to Show": function() {
        act.waitFor("#LayoutCompanySelection ul li:first-child");
    },
    "5.Assert First Company Exists": function() {
        eq($("#LayoutCompanySelection ul li:first-child").length > 0, true, "First Company Exists");
    },
    "6.Click First Company": function() {
        act.click("#LayoutCompanySelection ul li:first-child");
    },
    "7.Wait for Store Selection to Show": function() {
        act.waitFor("#LayoutStoreSelection");
    },
    "8.Assert Store Menu Exists": function() {
        eq($("#LayoutStoreSelection").length > 0, true, "Select Store Menu Exists");
    },
    "9.Hover over Select Store Menu": function() {
        act.hover("#LayoutStoreSelection");
    },
    "10.Click Select Store Menu": function() {
        act.click("#LayoutStoreSelection");
    },
    "11.Wait for Store Item Selection to Show": function() {
        act.wait(500);
    },
    "12.Click First Store": function() {
        act.click("#LayoutStoreSelection ul li:first-child");
    }
};
