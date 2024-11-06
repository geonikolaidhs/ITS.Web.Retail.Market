<%@ Page Title="" Language="C#" MasterPageFile="~/LicenseServer.Master" AutoEventWireup="true" CodeBehind="Applications.aspx.cs" Inherits="ITS.Licensing.Service.Applications" %>
<%@ Register assembly="DevExpress.Xpo.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
        DataSourceID="XpoDataSource1" KeyFieldName="Oid" Theme="Aqua">
        <Columns>
            <dx:GridViewCommandColumn VisibleIndex="0">
                <editbutton visible="True">
                </editbutton>
                <newbutton visible="True">
                </newbutton>
                <deletebutton visible="True">
                </deletebutton>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Oid" ReadOnly="True" VisibleIndex="1">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="ApplicationOid" VisibleIndex="2">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="3">
            </dx:GridViewDataTextColumn>
        </Columns>
    </dx:ASPxGridView>
    <dx:XpoDataSource ID="XpoDataSource1" runat="server" 
        TypeName="ITS.Licensing.LicenseModel.ApplicationInfo">
    </dx:XpoDataSource>
</asp:Content>
