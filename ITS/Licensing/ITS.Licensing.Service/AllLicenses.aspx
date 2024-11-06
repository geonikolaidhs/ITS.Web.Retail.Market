<%@ Page Language="C#" MasterPageFile="~/LicenseServer.Master" AutoEventWireup="true" CodeBehind="AllLicenses.aspx.cs" Inherits="ITS.Licensing.Service.WebForm1" Title="Untitled Page" %>
<%@ Register assembly="DevExpress.Web.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Xpo.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Xpo" tagprefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" 
        DataSourceID="XpoDataSource1" KeyFieldName="Oid" EnableTheming="True" 
    Theme="Aqua">
        <Columns>
            <dx:GridViewCommandColumn VisibleIndex="0">
                <EditButton Visible="True">
                </EditButton>
                <NewButton Visible="True">
                </NewButton>
                <DeleteButton Visible="True">
                </DeleteButton>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Oid" ReadOnly="True" VisibleIndex="1">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Number" VisibleIndex="2">
                <EditItemTemplate>
                    <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" Width="170px">
                    </dx:ASPxTextBox>
                </EditItemTemplate>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="NumberOfLicenses" 
                VisibleIndex="3">
                <EditItemTemplate>
                    <dx:ASPxSpinEdit ID="ASPxSpinEdit1" runat="server" Height="21px" Number="0" />
                </EditItemTemplate>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Application!Key" VisibleIndex="4">
                <EditItemTemplate>
                    <dx:ASPxComboBox ID="ASPxComboBox1" runat="server" 
                        DataSourceID="XpoDataSource2" ValueType="System.Int32">
                    </dx:ASPxComboBox>
                </EditItemTemplate>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataDateColumn FieldName="FinalDate" 
                VisibleIndex="5">
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="StartDate" VisibleIndex="6">
            </dx:GridViewDataDateColumn>
        </Columns>
        <Settings ShowFilterRow="True" ShowGroupPanel="True" />
    </dx:ASPxGridView>
    <dx:XpoDataSource ID="XpoDataSource1" runat="server" 
        TypeName="ITS.Licensing.LicenseModel.SerialNumber">
    </dx:XpoDataSource>

    <dx:XpoDataSource ID="XpoDataSource2" runat="server" 
        TypeName="ITS.Licensing.LicenseModel.ApplicationInfo">
    </dx:XpoDataSource>

    </asp:Content>
