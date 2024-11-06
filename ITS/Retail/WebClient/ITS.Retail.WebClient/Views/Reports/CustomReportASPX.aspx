<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ITS.Retail.Common.XtraReportBaseExtension>" %>

<%@ Register Assembly="DevExpress.XtraReports.v15.2.Web, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

<%--<!DOCTYPE html>--%>

<%--<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Custom Report Aspx</title>
    <script src="../Scripts/jquery-1.11.0.js"></script>
    <script src="../Scripts/jquery-migrate-1.2.1.js"></script>
    <script src="../Scripts/scripts/globalize.js"></script>
    <script src="../Scripts/scripts/jquery.cookie.js"></script>
    <script src="../Scripts/jquery-ui.js"></script>
    <script src="../Scripts/noty/jquery.noty.js"></script>
    <script src="../Scripts/noty/layouts/bottomRight.js"></script>
    <script src="../Scripts/noty/layouts/topCenter.js"></script>            
    <script src="../Scripts/noty/themes/default.js"></script>                        
    <script src="../Scripts/modernizr.js"></script>                        
    <script src="../Scripts/scripts/jquery.modaldialog.js"></script>
    <%
        //System.Web.Optimization.Scripts.Render("~/bundles/mainpage.js");
        
        Html.DevExpress().GetStyleSheets
            (
            new StyleSheet { ExtensionSuite = ExtensionSuite.All, Theme = DevExpressHelper.Theme }
            );
        Html.DevExpress().GetScripts(
            new Script { ExtensionSuite = ExtensionSuite.All }
            );
    %>
</head>
<body>--%>
    <div>
        <% Html.DevExpress().DocumentViewer(settings =>
           {
               settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
               settings.Name = "CustomReportViewer";
               settings.Report = Model;
               settings.CallbackRouteValues = new { Controller = "Reports", Action = "CustomReportASPX" };
               settings.ExportRouteValues = new { Controller = "Reports", Action = "CustomExportReportViewer" };
           }).Render();
        %>
    </div>
<%--</body>
</html>--%>
