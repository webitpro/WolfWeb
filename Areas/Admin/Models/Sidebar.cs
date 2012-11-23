using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(SidebarMetadata))]
    public partial class Sidebar
    {
        [Bind(Exclude = "ID,WebPage,WebPageID")]
        public class SidebarMetadata
        {
            [Required(ErrorMessage = "Source is required")]
            public string Source { get; set; }

            [Required(ErrorMessage = "Thumbnail is required")]
            public string Thumb { get; set; }

            public bool IsOverlay { get; set; }
        }
    }
}