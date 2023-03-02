using System.ComponentModel.DataAnnotations;

namespace DemoUser.Models.Froms.FromsUser
{
    public class FormInsert
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
