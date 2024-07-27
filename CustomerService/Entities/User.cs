using System.ComponentModel.DataAnnotations;

namespace CustomerService.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";

        [Required]

        public string Email { get; set; } = "";
        [Required]
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

}
