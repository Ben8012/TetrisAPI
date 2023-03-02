using System.ComponentModel.DataAnnotations;

namespace DemoUser.Models.Forms.FormsUser
{
    public class FormUpdate
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

    }
}
