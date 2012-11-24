using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(TabMetadata))]
    public partial class Tab
    {
        [Bind(Exclude = "ID")]
        public class TabMetadata
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(50, ErrorMessage = "Title length cannot exceed 50 characters")]
            [Empty(Allowed = false, ErrorMessage = "Title cannot be empty")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, ErrorMessage = "Title cannot be numeric value")]
            public string Title { get; set; }

            [Required(ErrorMessage = "URLTitle is required")]
            [StringLength(50, ErrorMessage = "URL Title length cannot exceed 50 characters")]
            public string URLTitle { get; set; }

            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = true, ErrorMessage = "Position must be numeric value")]
            public int Position { get; set; }


            public int? FAQPageID { get; set; }
        }
    }
}