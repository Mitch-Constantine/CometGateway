<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/cometd.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js"></script>
    <script src="../../Scripts/jquery.cometd.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.json-2.2.js" type="text/javascript"></script>
    <script src="../../Scripts/Views/Home/Index.js"></script>

    <h2><%: ViewData["Message"] %></h2>
    <p>
    </p>
</asp:Content>
