using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace WebIT.Temp
{
    public static partial class Config
    {
        public static int IsTCRequired
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["IsTCRequired"]); }
        }

        public static class Header
        {
            public static class Image
            {
                public static int Width { get { return Convert.ToInt32(ConfigurationManager.AppSettings["HeaderImageWidth"]); } }
                public static int Height { get { return Convert.ToInt32(ConfigurationManager.AppSettings["HeaderImageHeight"]); } }
            }

            public static class Flash
            {
                public static int Width { get { return Convert.ToInt32(ConfigurationManager.AppSettings["HeaderFlashWidth"]); } }
                public static int Height { get { return Convert.ToInt32(ConfigurationManager.AppSettings["HeaderFlashHeight"]); } }
            }
        }

        public static class Sidebar
        {
            public static class Thumbnail
            {
                public static int Width { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SidebarThumbnailWidth"]); } }
                public static int Height { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SidebarThumbnailHeight"]); } }
            }
        }

        public static class DocLib
        {
            public static class Thumbnail
            {
                public static int Width { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DocLibThumbnailWidth"]); } }
                public static int Height { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DocLibThumbnailHeight"]); } }
            }
        }

        public static class Employee
        {
            public static class Photo
            {
                public static int Width { get { return Convert.ToInt32(ConfigurationManager.AppSettings["EmployeePhotoWidth"]); } }
                public static int Height { get { return Convert.ToInt32(ConfigurationManager.AppSettings["EmployeePhotoHeight"]); } }
            }
        }

        public static class Navigation
        {
            public static class Section
            {
                public static string[] EmptySymbols { get { return WebIT.Lib.Utils.Array.FromString(ConfigurationManager.AppSettings["NavigationSectionEmptySymbols"].ToString(), ","); } }
                public static string EmptyHeaderSymbol { get { return ConfigurationManager.AppSettings["NavigationSectionEmptyHeader"].ToString(); } }
            }
        }

        public static eConfig Section
        {
            get { return (eConfig)ConfigurationManager.GetSection("eConfig"); }
        }

        public static DeploymentFor ActiveConfiguration
        {
            get { return Section.ActiveConfiguration; }
        }

        public static class Email
        {
            public static class Form
            {
                public static class ContactUs
                {
                    public static string Subject { get { return ConfigurationManager.AppSettings["ContactUsSubject"].ToString(); } }
                    public static string Recipient { get { return ConfigurationManager.AppSettings["ContactUsRecipient"].ToString(); } }
                }

                public static class WebConferenceRequest
                {
                    public static string Subject { get { return ConfigurationManager.AppSettings["WebConferenceRequestSubject"].ToString(); } }
                    public static string Recipient { get { return ConfigurationManager.AppSettings["WebConferenceRequestRecipient"].ToString(); } }
                }
            }
        }
    }

    public class eConfig : ConfigurationSection
    {
        [ConfigurationProperty("environment", IsRequired = true, DefaultValue = "development")]
        public string Environment
        {
            get { return this["environment"] as string; }
        }

        public DeploymentFor ActiveConfiguration
        {
            get
            {
                switch (Environment)
                {
                    case "development":
                        return this.Development;
                    case "staging":
                        return this.Staging;
                    case "production":
                        return this.Production;
                }
                return null;
            }
        }

        [ConfigurationProperty("development", IsRequired = false)]
        public DeploymentFor Development
        {
            get { return this["development"] as DeploymentFor; }
        }

        [ConfigurationProperty("staging", IsRequired = false)]
        public DeploymentFor Staging
        {
            get { return this["staging"] as DeploymentFor; }
        }

        [ConfigurationProperty("production", IsRequired = false)]
        public DeploymentFor Production
        {
            get { return this["production"] as DeploymentFor; }
        }
    }

    public class DeploymentFor : ConfigurationElement
    {
        [ConfigurationProperty("controllerExtension", IsRequired = true, DefaultValue = "")]
        public string ControllerExtension
        {
            get { return this["controllerExtension"] as string; }
        }

        [ConfigurationProperty("minifyFiles", IsRequired = false, DefaultValue = false)]
        public bool MinifyFiles
        {
            get { return (bool)this["minifyFiles"]; }
        }

        [ConfigurationProperty("database", IsRequired = false, DefaultValue = "development")]
        public string Database
        {
            get { return this["database"] as string; }
        }

        [ConfigurationProperty("mail", IsRequired = false)]
        public MailConfiguration Mail
        {
            get { return this["mail"] as MailConfiguration; }
        }

    }

    public class MailConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("host", IsRequired = false, DefaultValue = "localhost")]
        public string Host
        {
            get { return this["host"] as string; }
        }

        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 25)]
        public int Port
        {
            get { return (int)this["port"]; }
        }

        [ConfigurationProperty("from", IsRequired = false, DefaultValue = "admin@site.local")]
        public string From
        {
            get { return this["from"] as string; }
        }

        [ConfigurationProperty("reportEmail", IsRequired = false, DefaultValue = "admin@site.local")]
        public string ReportEmail
        {
            get { return this["reportEmail"] as string; }
        }

    }
}