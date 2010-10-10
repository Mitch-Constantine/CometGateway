<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@Import Namespace="CometGateway.Server.TelnetDemo.Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Telnet Main Page</title>
    <%=Html.IncludeScript("qunit.js") %>
    <script type="text/javascript">
        test("sampleTest", function () {
            ok(2 == 3);
            ok(2 == 2);
            equals(2, 2);
            equals(2, 3);
        });
    </script>
    <link href="../../Content/QUnit.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <pre>
    
    </pre>
    <input type="text" id="Message" />
    <button id="btnMessage">Send</button>
    
    <h1 id="qunit-header">QUnit example</h1>
    <h2 id="qunit-banner"></h2>
    <h2 id="qunit-userAgent"></h2>
    <ol id="qunit-tests"></ol>
    <div id="qunit-fixture">test markup, will be hidden</div>

</body>
</html>
