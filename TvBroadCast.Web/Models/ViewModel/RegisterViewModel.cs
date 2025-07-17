using System.ComponentModel.DataAnnotations;

namespace TvBroadCast.Web.Models.ViewModel
{
    public class RegisterViewModel
    {

        public string UserName { get; set; }


        [Required(ErrorMessage = "Email is required!!!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address!!!")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required!!!")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password doesn't match requirements!!!")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password doesn't mact!!!")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm your password!!!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }



    }
}
