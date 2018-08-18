using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belt1.Models
{
    public class LoginValidate
    {
        [Required(ErrorMessage = "Email Address is required to login")]
        [EmailAddress(ErrorMessage = "Email Address is not registered")]
        public string login_email { get; set; }
        [Required(ErrorMessage = "Password is required to login")]
        [MinLength(8, ErrorMessage = "Passwords must be at least 8 characters")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must have a minimum of 8 characters contain at least 1 number, 1 letter and a special character")]
        // [Compare("password", ErrorMessage = "The passwords do not match.")]
        public string login_password { get; set; }
    }
}