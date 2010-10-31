using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CometGateway.Server.TelnetDemo.Models;

namespace CometGateway.Server.TelnetDemo.Common
{
    public static class EnableQUnit
    {
        public static string EnableQUnitTesting(this HtmlHelper htmlHelper)
        {
            object objTestModeEnabled = false;
            htmlHelper.ViewData.TryGetValue(TestModeConstants.ENABLE_TEST_MODE, out objTestModeEnabled);
            var testModeEnabled = (bool) (objTestModeEnabled ?? false);
              
            if (!testModeEnabled)
                return "";

            const string htmlSnippet = @"
    <div id='testResults'>
        <h1 id='qunit-header'></h1>
        <h2 id='qunit-banner'></h2>
        <h2 id='qunit-userAgent'></h2>
        <ol id='qunit-tests'></ol>
        <div id='qunit-fixture'></div>
    </div>
    <script type='text/javascript'>var testmode = true;</script>
    <script type='text/javascript'>
        $('body').contents().hide();
        $('#testResults').show();
    </script>";

            var htmlPieces = new[]
            {
                htmlHelper.IncludeCss("QUnit.css"),
                htmlHelper.IncludeScript("qunit.js"),
                htmlHelper.IncludePageTestScript(),
                htmlSnippet
            };

            return String.Join("\r\n",htmlPieces );
        }
    }
}