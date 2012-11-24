using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(WebPageMetadata))]
    public partial class WebPage
    {
        [Bind(Exclude = "ID")]
        public class WebPageMetadata
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(50, ErrorMessage = "Title length cannot exceed 50 characters")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, ErrorMessage = "Title cannot be numeric value")]
            public string Title { get; set; }

            [Required(ErrorMessage = "URLTitle is required")]
            [StringLength(50, ErrorMessage = "URL Title length cannot exceed 50 characters")]
            public string URLTitle { get; set; }

            [Required(ErrorMessage = "Template ID must be numeric value")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = true, ErrorMessage = "Template ID must be numeric value")]
            public int TemplateID { get; set; }

            public bool IsHomePage { get; set; }
        }
    }
}