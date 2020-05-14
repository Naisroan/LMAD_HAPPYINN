<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="NDEV_HAPPY_INN.mods.home" %>

<asp:Content ID="c_styles" ContentPlaceHolderID="cph_styles" runat="server">

    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/home/home.css") %>" />

</asp:Content>

<asp:Content ID="c_scripts" ContentPlaceHolderID="cph_scripts" runat="server">

    <script type="text/javascript" src="<%: ResolveUrl("~/Content/mods/home/home.js") %>"></script>

</asp:Content>

<asp:Content ID="c_body" ContentPlaceHolderID="cph_body" runat="server">
    
    <%--<asp:LinkButton
        ID="btnPrueba"
        runat="server"
        CssClass="btn btn-primary btn-icon-split"
    >
        <span class="icon text-white-50">
            <i class="fas fa-flag"></i>
        </span>
        <span class="text">Test</span>
    </asp:LinkButton>--%>

</asp:Content>
