using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebIT.Temp
{
    public static partial class UrlHelperExtensions
    {
        public enum ContentDirectory
        {
            css,
            data,
            images,
            script,
            xml
        }

        public static string Content(this UrlHelper helper, ContentDirectory dir, string fileWithExt)
        {
            return helper.Content(String.Format("~/Content/{0}/{1}", dir.ToString(), fileWithExt));
        }

        public static string Content(this UrlHelper helper, string filePath)
        {
            return helper.Content(String.Format("~/Content/{0}", filePath));
        }



        public static string Image(this UrlHelper helper, string file)
        {
            return helper.Content(String.Format("~/Content/{0}/{1}", ContentDirectory.images.ToString(), file));
        }

        public static string CSS(this UrlHelper helper, string file)
        {
            if (Config.ActiveConfiguration.MinifyFiles)
            {
                return helper.Content(String.Format("~/Content/{0}/{1}.min.css", ContentDirectory.css.ToString(), file));
            }
            else
            {
                return helper.Content(String.Format("~/Content/{0}/{1}.css", ContentDirectory.css.ToString(), file));
            }
        }

        public static string Script(this UrlHelper helper, string file)
        {
            if (Config.ActiveConfiguration.MinifyFiles)
            {
                return helper.Content(String.Format("~/Content/{0}/{1}.min.js", ContentDirectory.script.ToString(), file));
            }
            else
            {
                return helper.Content(String.Format("~/Content/{0}/{1}.js", ContentDirectory.script.ToString(), file));
            }
        }

        public static string Xml(this UrlHelper helper, string file)
        {
            return helper.Content(String.Format("~/Content/{0}/{1}.xml", ContentDirectory.xml.ToString(), file));
        }

        public static string Data(this UrlHelper helper, string fileWithExt)
        {
            return helper.Content(String.Format("~/Content/{0}/{1}", ContentDirectory.data.ToString(), fileWithExt));
        }

        public static string UploadPath(this UrlHelper helper, string fileWithExt)
        {
            return helper.RequestContext.HttpContext.Server.MapPath(String.Format("~/Content/{0}/{1}", ContentDirectory.data.ToString(), fileWithExt));
        }

        public static string JQueryScript(this UrlHelper helper, JQuery.Script script)
        {
            if (JQuery.Scripts.ContainsKey(script))
            {
                return JQuery.Scripts[script];
            }
            return null;
        }
    }
}