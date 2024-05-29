using System.ComponentModel.DataAnnotations;

namespace ChocoLuxAPI.DTO
{
    public class LoginDto
    {
        [Required]
        [Display(Name = "Email / Username")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
