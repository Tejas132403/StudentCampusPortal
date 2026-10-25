using System.ComponentModel.DataAnnotations;

namespace StudentCampusPortal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enrollment number is required")]
        [Display(Name = "Enrollment No")]
        public string EnrollmentNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
