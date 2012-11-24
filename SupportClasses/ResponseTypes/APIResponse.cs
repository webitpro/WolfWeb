using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebIT.Temp
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public object Result { get; set; }

        public APIResponse()
        {
            Success = false;
            Errors = new List<string>();
            Result = null;
        }

        public void setErrors(ICollection<ModelState> dic)
        {
            List<string> errorList = new List<string>();
            foreach (ModelState item in dic)
            {
                if (item.Errors.Count() > 0)
                {
                    errorList.Add(item.Errors.First().ErrorMessage);
                }
            }
            Errors = errorList;
        }


    }
}