using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace pms.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name required")]
        public string first_name { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name required")]
        public string last_name { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Job Description")]
        public string jop_description { get; set; }

        [Display(Name = "Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type is required")]
        [DataType(DataType.Text)]
        public string type { get; set; }

        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Confirm password and password do not match")]
        public string ConfirmPassword { get; set; }
        

    }
}