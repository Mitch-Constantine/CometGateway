<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@Import Namespace="CometGateway.Server.TelnetDemo.Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Telnet Demo Main Page</title>
    <%=Html.IncludeScript(
        "jquery-1.4.2.min.js", 
        "jquery-ui-1.8.5.custom.min.js",
        "cometd.js",
        "jquery.json-2.2.js",
        "jquery.cometd.js",
        "cometdwrapper.js",
        "jquery.scrollTo-1.4.2.js"
    )%>
    <%=Html.IncludeCss("jquery-ui-1.8.6.custom.css")%>
    <%=Html.IncludeCss("Site.css")%>
    <%=Html.IncludePageScript() %>
</head>
<body>
    <%string applicationPath = Request.ApplicationPath.EndsWith("/") ? 
          Request.ApplicationPath.Substring(0, Request.ApplicationPath.Length-1) :
          Request.ApplicationPath; %>
    <%=Html.Hidden("applicationPath", applicationPath)%>
    <%=Html.Hidden("hdnServer", ViewData["server"])%>
    <%=Html.Hidden("hdnPort", ViewData["port"])%>

    <div class="contentWrapper">
        <div id="textReceived" class="preHolder"></div>
        <input type="text" id="txtTextTyped" class="inputText"/>
        <button type="button" id="btnLineReady" class="SendButton default">Send</button>
    </div>

    <div id="connectDialog" style="display:none" class="dialog">
        <span class="header">Connect to server</span>
        <p><label for='txtServer'>Server:</label><input type='text' id='txtServer' /></p>
        <p><label for='txtPort'>Port:</label><input type='text' id='txtPort' /></p>
        <p><button id='btnConnect' class='default'>Connect</button></p>
    </div>

    <div id="cancelDialog" style="display:none" class="dialog">
        <span class="header">Connecting...</span>
        <p>Connecting, press Cancel button to stop</p>
        <p><button id='btnCancelConnect'>Cancel</button></p>
    </div>
    
    <div id="errorOccurred" style="display:none" class="dialog">
        <span class="header">Unexpected error</span>
        <p>An unexpected error has occurred:</p>
        <div id='errorMsg'></div>
        <p><button id='btnClose'>OK</button></p>
    </div>
    
    <%=Html.EnableQUnitTesting() %>

</body>
</html>
