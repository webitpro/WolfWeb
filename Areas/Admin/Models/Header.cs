using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(HeaderMetadata))]
    public partial class Header
    {
    
       [Bind(Exclude = "ID,WebPage,WebPageID")]
        public class HeaderMetadata
        {
            [Required(ErrorMessage = "Movie is required")]            
            public string MoviePath { get; set; }

            [Required(ErrorMessage = "Image is required")]
            public string ImagePath { get; set; }
        }
    }
}