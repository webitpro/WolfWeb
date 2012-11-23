using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(MediaMetadata))]
    public partial class Media
    {
        [Bind(Exclude="ID")]
        public class MediaMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, ErrorMessage = "Name cannot be numeric value")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Source is required")]
            public string Path { get; set; }

            public string Type { get; set; }
        }
    }
}