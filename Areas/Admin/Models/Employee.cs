using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace WebIT.Temp.Models
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        [Bind(Exclude = "ID, WebPage, WebPageID")]
        public class EmployeeMetadata
        {
            [Required(ErrorMessage = "First Name is required")]
            [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false, 
                ErrorMessage = "First Name cannot be numeric value only.")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false,
                ErrorMessage = "Last Name cannot be numeric value only.")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Title is required")]
            [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false,
                ErrorMessage = "Title cannot be numeric value only.")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]            
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric, xOperator = false,
                ErrorMessage = "Description cannot be numeric value only.")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Email Address is required.")]
            [EmailFormat(ErrorMessage = "Email format is not valid")]
            public string EmailAddress { get; set; }

            public string Photo { get; set; }
        }
    }
}