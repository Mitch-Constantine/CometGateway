using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CometGateway.Server.TelnetDemo.Common
{
    public static class HtmlExtension
    {
        public static string IncludePageScript(this HtmlHelper htmlHelper)
        {
            var view = htmlHelper.ViewContext.View as WebFormView;
            Debug.Assert(view != null);
            Debug.Assert(view.ViewPath.StartsWith("~/Views"));
            Debug.Assert(view.ViewPath.EndsWith(".aspx"));

            var viewScriptPath = Regex.Replace(view.ViewPath.Replace("~/", ""), @"\.aspx$", ".js");
            return "bbbcb" + htmlHelper.IncludeScript(new[] { viewScriptPath });
        }

        public static string IncludeScript(this HtmlHelper htmlHelper, params string[] scriptNames)
        {
            var applicationPath = htmlHelper.ViewContext.HttpContext.Request.ApplicationPath;
            var scriptIncludes = scriptNames
                .Select(scriptName => GetIncludeScript(applicationPath, scriptName))
                .ToArray();

            return String.Join("", scriptIncludes);
        }

        static string GetIncludeScript(string applicationPath, string scriptName)
        {
            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "text/javascript");
            string scriptPath = CombinePathSegments(applicationPath, "Scripts", scriptName);
            tagBuilder.MergeAttribute("src", scriptPath);
            return tagBuilder.ToString();
        }

        static string CombinePathSegments(params string[] pathSegments)
        {
            return pathSegments.Aggregate(CombinePath);
        }

        static string CombinePath(string path1, string path2)
        {
            return path1.EndsWith("/") ?
                    path1 + path2 :
                    path1 + "/" + path2;
        }
    }
}