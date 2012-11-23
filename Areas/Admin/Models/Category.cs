using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category
    {
        [Bind(Exclude="ID")]
        public class CategoryMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
        }
    }
}