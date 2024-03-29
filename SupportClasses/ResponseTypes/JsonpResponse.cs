﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebIT.Temp
{
    public class JsonpResponse : System.Web.Mvc.JsonResult
    {

        public string JSONPCallback { get; set; }
        public JsonpResponse(string jsonp, object data)
        {
            JSONPCallback = jsonp;
            Data = data;
        }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";

            JavaScriptSerializer jss = new JavaScriptSerializer();
            if (Data != null)
            {
                response.Write(JSONPCallback + "(" + jss.Serialize(Data) + ")");
            }
        }
    }
}