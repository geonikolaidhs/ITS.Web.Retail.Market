"@mixin"["SearchDocumentsSales"] = {
  "1.Assert Menu Exists": function() {
      eq($("#menuLabel").length > 0, true, "Menu does not exists");
  },
  "2.Hover over Menu": function() {
      act.hover("#menuLabel");
  },
  "3.Click Menu": function() {
      act.click("#menuLabel");
  },
  "4.Wait for Sale Documents to Appear": function() {
      act.waitFor("#salesDocuments");
  },
  "5. Assert Sales Documents": function() {
      eq($("#salesDocuments").length > 0, true, "Sales Documents does not exists");
  },
  "6.Hover over Sales Documents": function() {
      act.hover("#salesDocuments");
  },
  "7.Click Sales Documents": function() {
      act.click("#salesDocuments");
  },
  "8. Assert List Sales Document": function() {
      eq($("#listSalesDocument").length > 0, true, "List Sales Documents does not exists");
  },
  "9.Hover List Sales Document": function() {
      act.hover("#listSalesDocument");
  },
  "10.Click on Sales Document List": function() {
      act.click("#listSalesDocument");
  },
  "11.Wait for Search Form to Show": function() {
      act.waitFor("#FilterPanel_RPHT");
  },
  "12.Check if Filter Buttons and Input exists": function() {
      eq($("#FilterPanel_RPHT").length > 0, true, "Up Filter Button");
      eq($("#btnClear").length > 0, true, "Clear Filter Button");
      eq($("#sell_from").length > 0, true, "Select Store");
      eq($("#typecombo").length > 0, true, "Select Type Combo");
      eq($("#seriescombo").length > 0, true, "Select Series Combo");
      eq($("#txtnumberFrom").length > 0, true, "Document Number From");
      eq($("#txtnumberTo").length > 0, true, "Document Number To");
      eq($("#Fromdate").length > 0, true, "Date From");
      eq($("#todate").length > 0, true, "Date To");
      eq($("#FromExecdate").length > 0, true, "Executed From");
      eq($("#toExecdate").length > 0, true, "Execute To");
      eq($("#TransformationStatus").length > 0, true, "Has Been Transformed");
      eq($("#statuscombo").length > 0, true, "Status Combo");
      eq($("#Users").length > 0, true, "Created From User");
      // eq($("#checkComboBox").length > 0, 1, "Created From Cashier");
      eq($("#btnSearch").length > 0, true, "Search Button");
  },
  "13.Hover over From Date Input": function() {
      act.hover("#Fromdate");
  },
  '14.Click input "Ημερομηνία..."': function() {
      act.click("#Fromdate_I");
  },
  "15.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "16.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "17.Hover over To Date Input": function() {
      act.hover("#todate");
  },
  '18.Click input "Ημερομηνία..."': function() {
      act.click("#todate_I");
  },
  "19.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "20.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "21.Hover over div Search Button": function() {
      act.hover("#btnSearch");
  },
  '22.Click Search Button': function() {
      var actionTarget = function() {
          return $("#btnSearch_CD").find(".dx-vam");
      };
      act.click(actionTarget);
  },
  "23.Wait For Results to Appear": function() {
      act.waitFor("#grdDocument_DXDataRow0");
  },
};

"@mixin"["SearchDocumentsPurchase"] = {
  "1.Assert Menu Exists": function() {
      eq($("#menuLabel").length > 0, true, "Menu does not exists");
  },
  "2.Hover over Menu": function() {
      act.hover("#menuLabel");
  },
  "3.Click Menu": function() {
      act.click("#menuLabel");
  },
  "4.Wait for Purchase Documents to Appear": function() {
      act.waitFor("#purchaseDocuments");
  },
  "5. Assert Purchase Documents": function() {
      eq($("#purchaseDocuments").length > 0, true, "Purchase Documents does not exists");
  },
  "6.Hover over Purchase Documents": function() {
      act.hover("#purchaseDocuments");
  },
  "7.Click Purchase Documents": function() {
      act.click("#purchaseDocuments");
  },
  "8. Assert List Purchase Document": function() {
      eq($("#listPurchaseDocument").length > 0, true, "List Purchase Documents does not exists");
  },
  "9.Hover List Purchase Document": function() {
      act.hover("#listPurchaseDocument");
  },
  "10.Click on Purchase Document List": function() {
      act.click("#listPurchaseDocument");
  },
  "11.Wait for Search Form to Show": function() {
      act.waitFor("#FilterPanel_RPHT");
  },
  "12.Check if Filter Buttons and Input exists": function() {
      eq($("#FilterPanel_RPHT").length > 0, true, "Up Filter Button");
      eq($("#btnClear").length > 0, true, "Clear Filter Button");
      eq($("#sell_from").length > 0, true, "Select Store");
      eq($("#typecombo").length > 0, true, "Select Type Combo");
      eq($("#seriescombo").length > 0, true, "Select Series Combo");
      eq($("#txtnumberFrom").length > 0, true, "Document Number From");
      eq($("#txtnumberTo").length > 0, true, "Document Number To");
      eq($("#Fromdate").length > 0, true, "Date From");
      eq($("#todate").length > 0, true, "Date To");
      eq($("#FromExecdate").length > 0, true, "Executed From");
      eq($("#toExecdate").length > 0, true, "Execute To");
      eq($("#TransformationStatus").length > 0, true, "Has Been Transformed");
      eq($("#statuscombo").length > 0, true, "Status Combo");
      eq($("#Users").length > 0, true, "Created From User");
      // eq($("#checkComboBox").length > 0, 1, "Created From Cashier");
      eq($("#btnSearch").length > 0, true, "Search Button");
  },
  "13.Hover over From Date Input": function() {
      act.hover("#Fromdate");
  },
  '14.Click input "Ημερομηνία..."': function() {
      act.click("#Fromdate_I");
  },
  "15.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "16.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "17.Hover over To Date Input": function() {
      act.hover("#todate");
  },
  '18.Click input "Ημερομηνία..."': function() {
      act.click("#todate_I");
  },
  "19.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "20.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "21.Hover over div Search Button": function() {
      act.hover("#btnSearch");
  },
  '22.Click Search Button': function() {
      var actionTarget = function() {
          return $("#btnSearch_CD").find(".dx-vam");
      };
      act.click(actionTarget);
  },
  "23.Wait For Results to Appear": function() {
      act.waitFor("#grdDocument_DXDataRow0");
  },
};

"@mixin"["SearchDocumentsStore"] = {
  "1.Assert Menu Exists": function() {
      eq($("#menuLabel").length > 0, true, "Menu does not exists");
  },
  "2.Hover over Menu": function() {
      act.hover("#menuLabel");
  },
  "3.Click Menu": function() {
      act.click("#menuLabel");
  },
  "4.Wait for Store Documents to Appear": function() {
      act.waitFor("#storeDocuments");
  },
  "5. Assert Store Documents": function() {
      eq($("#storeDocuments").length > 0, true, "Store Documents does not exists");
  },
  "6.Hover over Store Documents": function() {
      act.hover("#storeDocuments");
  },
  "7.Click Store Documents": function() {
      act.click("#storeDocuments");
  },
  "8. Assert List Store Document": function() {
      eq($("#listStoreDocument").length > 0, true, "List Store Documents does not exists");
  },
  "9.Hover List Store Document": function() {
      act.hover("#listStoreDocument");
  },
  "10.Click on Store Document List": function() {
      act.click("#listStoreDocument");
  },
  "11.Wait for Search Form to Show": function() {
      act.waitFor("#FilterPanel_RPHT");
  },
  "12.Check if Filter Buttons and Input exists": function() {
      eq($("#FilterPanel_RPHT").length > 0, true, "Up Filter Button");
      eq($("#btnClear").length > 0, true, "Clear Filter Button");
      eq($("#sell_from").length > 0, true, "Select Store");
      eq($("#typecombo").length > 0, true, "Select Type Combo");
      eq($("#seriescombo").length > 0, true, "Select Series Combo");
      eq($("#txtnumberFrom").length > 0, true, "Document Number From");
      eq($("#txtnumberTo").length > 0, true, "Document Number To");
      eq($("#Fromdate").length > 0, true, "Date From");
      eq($("#todate").length > 0, true, "Date To");
      eq($("#FromExecdate").length > 0, true, "Executed From");
      eq($("#toExecdate").length > 0, true, "Execute To");
      eq($("#TransformationStatus").length > 0, true, "Has Been Transformed");
      eq($("#statuscombo").length > 0, true, "Status Combo");
      eq($("#Users").length > 0, true, "Created From User");
      // eq($("#checkComboBox").length > 0, 1, "Created From Cashier");
      eq($("#btnSearch").length > 0, true, "Search Button");
  },
  "13.Hover over From Date Input": function() {
      act.hover("#Fromdate");
  },
  '14.Click input "Ημερομηνία..."': function() {
      act.click("#Fromdate_I");
  },
  "15.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "16.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "17.Hover over To Date Input": function() {
      act.hover("#todate");
  },
  '18.Click input "Ημερομηνία..."': function() {
      act.click("#todate_I");
  },
  "19.Press key combination CTRL+A": function() {
      act.press("ctrl+a");
  },
  "20.Press key BACKSPACE": function() {
      act.press("backspace");
  },
  "21.Hover over div Search Button": function() {
      act.hover("#btnSearch");
  },
  '22.Click Search Button': function() {
      var actionTarget = function() {
          return $("#btnSearch_CD").find(".dx-vam");
      };
      act.click(actionTarget);
  },
  "23.Wait For Results to Appear": function() {
      act.waitFor("#grdDocument_DXDataRow0");
  },
};
