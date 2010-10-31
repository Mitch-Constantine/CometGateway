using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CometGateway.Server.TelnetDemo.Common
{
    public static class IncludeScripts
    {
        public static string IncludePageScript(this HtmlHelper htmlHelper)
        {
            return GetPageScript(htmlHelper);
        }

        public static string IncludePageTestScript(this HtmlHelper htmlHelper)
        {
            return GetPageScript(htmlHelper, "Test");
        }

        public static string IncludeScript(this HtmlHelper htmlHelper, params string[] scriptNames)
        {
            var scriptIncludes = scriptNames
                .Select(scriptName => GetIncludeScript(GetApplicationPath(htmlHelper), scriptName))
                .ToArray();

            return String.Join("", scriptIncludes);
        }

        public static string IncludeCss(this HtmlHelper htmlHelper, string cssPath)
        {
            return GetIncludeCss(GetApplicationPath(htmlHelper), cssPath);
        }

        static string GetIncludeScript(string applicationPath, string scriptName)
        {
            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "text/javascript");
            string scriptPath = CombinePathSegments(applicationPath, "Scripts", scriptName);
            tagBuilder.MergeAttribute("src", scriptPath);
            return tagBuilder.ToString();
        }

        static string GetApplicationPath(HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.Request.ApplicationPath;
        }

        private static string GetPageScript(HtmlHelper htmlHelper, string pageSuffix = "")
        {
            var view = htmlHelper.ViewContext.View as WebFormView;
            Debug.Assert(view != null);
            Debug.Assert(view.ViewPath.StartsWith("~/Views"));
            Debug.Assert(view.ViewPath.EndsWith(".aspx"));

            var viewScriptPath = Regex.Replace(view.ViewPath.Replace("~/", ""), @"\.aspx$", pageSuffix + ".js");
            return htmlHelper.IncludeScript(new[] { viewScriptPath });
        }

        static string GetIncludeCss(string applicationPath, string scriptName)
        {
            var tagBuilder = new TagBuilder("link");
            string scriptPath = CombinePathSegments(applicationPath, "Content", scriptName);
            tagBuilder.MergeAttribute("href", scriptPath);
            tagBuilder.MergeAttribute("rel", "stylesheet");
            tagBuilder.MergeAttribute("type", "text/css");
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