using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebIT.Temp.Models;
using System.Configuration;
using WebIT.Temp;

namespace WebIT.Lib
{
    public static partial class Utils
    {
        //database
        public static class DB
        {
            public static DBDataContext GetContext()
            {
                return new DBDataContext(ConfigurationManager.ConnectionStrings[Config.ActiveConfiguration.Database].ToString());
            }
        }
    }
}