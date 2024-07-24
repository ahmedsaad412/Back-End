using System.ComponentModel.DataAnnotations;

namespace CustomerService.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        [RegularExpression("^[01]?[- .]?\\(?[2-9]\\d{2}\\)?[- .]?\\d{3}[- .]?\\d{4}$")]
        public string PhoneNumber { get; set; } 
        public bool IsDeleted { get; set; } = false;
    }

}
