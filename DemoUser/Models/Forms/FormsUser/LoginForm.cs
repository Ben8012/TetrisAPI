using System.ComponentModel.DataAnnotations;

namespace DemoUser.Models.Forms.FormsUser
{
    public class LoginForm
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
