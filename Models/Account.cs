using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebIT.Temp;

namespace WebIT.Temp.Models
{
    [MetadataType(typeof(AccountMetadata))]
    public partial class Account
    {
        [Bind(Exclude = "ID")]
        public class AccountMetadata
        {


            [Required(ErrorMessage = "First Name is required")]
            [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric,
                xOperator = false,
                ErrorMessage = "First Name cannot be numeric value")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
            [ExpressionType(xType = WebIT.Lib.Type.xPression.Numeric,
                xOperator = false,
                ErrorMessage = "Last Name cannot be numeric value")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [StringLength(250, ErrorMessage = "Email cannot exceed 250 characters")]
            [EmailFormat(ErrorMessage = "Email format is not valid")]
            public string Email { get; set; }

            public string Password { get; set; }

        }
    }
}