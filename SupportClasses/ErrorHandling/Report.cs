using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIT.Temp
{

    public static class ErrorHandler
    {
        public static class Report
        {
            public static void Exception(Exception ex, string location)
            {
                if (!string.IsNullOrEmpty(Config.ActiveConfiguration.Mail.ReportEmail))
                {
                    WebIT.Lib.Utils.Email.sendEmail(
                        Config.ActiveConfiguration.Mail.ReportEmail,
                        Config.ActiveConfiguration.Mail.ReportEmail,
                        "Exception Report",
                        location + ex.Message,
                        true,
                        Config.ActiveConfiguration.Mail.Host,
                        Config.ActiveConfiguration.Mail.Port);
                }
            }
        }
    }
}