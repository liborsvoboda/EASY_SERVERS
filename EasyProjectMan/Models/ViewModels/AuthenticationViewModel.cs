using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.ViewModels {

    public class AuthenticationViewModel {

        [Required(ErrorMessage = "Email address is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Addresss")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
    }
}