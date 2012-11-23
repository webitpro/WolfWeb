using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebIT.Temp
{
    public static class JQuery
    {
        public enum Script
        {
            Base,
            UI,
            Templates,
            Cycle,
            SwfObject
        }

        static public readonly Dictionary<Script, string> Scripts = new Dictionary<Script, string>
        {
            {Script.Base, "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.1.min.js" },
            {Script.UI, "http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.17/jquery-ui.min.js" },
            {Script.Templates, "http://ajax.aspnetcdn.com/ajax/jquery.templates/beta1/jquery.tmpl.min.js" },
            {Script.Cycle, "http://ajax.aspnetcdn.com/ajax/jquery.cycle/2.99/jquery.cycle.all.min.js" },
            {Script.SwfObject, "http://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js" }
        };
    }
}