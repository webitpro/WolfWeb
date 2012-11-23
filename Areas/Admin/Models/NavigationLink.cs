using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(NavigationLinkMetadata))]
    public partial class NavigationLink
    {
        [Bind(Exclude = "ID,Section,SectionID")]
        public class NavigationLinkMetadata
        {

            [Required(ErrorMessage = "Section ID is required")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = true, ErrorMessage = "Section ID must be numeric value")]
            public int WebPageID { get; set; }

            [Required(ErrorMessage = "Title is required")]
            [StringLength(50, ErrorMessage = "Title length cannot exceed 50 characters")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, ErrorMessage = "Title cannot be numeric value")]
            public string Title { get; set; }

            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = true, ErrorMessage = "Position must be numeric value")]
            public int Position { get; set; }

        }
    }
}