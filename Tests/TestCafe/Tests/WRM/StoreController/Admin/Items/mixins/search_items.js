"@mixin"["SearchItems"] = {
    "1.Assert Menu Exists": function() {
      eq($("#menuLabel").length > 0, true, "Menu does not exists");
    },
    "2.Hover over Menu": function() {
      act.hover("#menuLabel");
    },
    "3.Click Menu": function() {
      act.click("#menuLabel");
    },
    "4.Wait for Menu Item to Appear": function() {
      act.waitFor("#menuItem");
    },
    "5. Assert Menu Item": function() {
      eq($("#menuItem").length > 0, true, "Menu Item does not exists");
    },
    "6.Hover over Menu Item": function() {
      act.hover("#menuItem");
    },
    "7.Click Menu Item": function() {
        act.click("#menuItem");
    },
    "8. Assert Menu List": function() {
      eq($("#menuItemList").length > 0, true, " Menu List does not exists");
    },
    "9.Hover over Menu List": function() {
      act.hover("#menuItemList");
    },
    "10.Click on Menu List": function() {
      act.click("#menuItemList");
    },
    "11.Wait for Search Form to Show": function() {
      act.waitFor("#FilterPanel_RPHT");
    },
    "12.Assert Search Button Exists": function() {
        eq($("#btnSearch").length > 0, true, "Search Button Exists");
    },
    "13.Hover over Search Button": function() {
        act.hover("#btnSearch");
    },
    "14.Click Search Button": function() {
        act.click("#btnSearch");
    },
    "15.Wait For Results to Appear": function() {
      act.waitFor("#grdItems_DXDataRow0");
    },
};
