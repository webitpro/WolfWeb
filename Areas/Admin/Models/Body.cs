using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(BodyMetadata))]
    public partial class Body
    {
        [Bind(Exclude = "ID,WebPage,WebPageID")]
        public class BodyMetadata
        {
            [Required(ErrorMessage = "HTML is required")]
            public string HTML { get; set; }
        }

    }
}