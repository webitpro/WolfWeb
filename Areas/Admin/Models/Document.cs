using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(DocumentMetadata))]
    public partial class Document
    {
        [Bind(Exclude="ID, Category, CategoryID, WebPageID, WebPage")]
        public class DocumentMetadata
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, 
                ErrorMessage = "Title cannot be numeric value only.")]
            public string Title { get; set; }

            public string Source { get; set; }

            public string Thumb { get; set; }
        }
    }
}