using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models.ViewModels {

    public class ChangePasswordViewModel {

        [Required(ErrorMessage = "Old Password is Required")]
        public string MainPassword { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Re Enter New Password")]
        public string ReNewPassword { get; set; }
    }
}