namespace CustomerService.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; }
        public int CommercialId  { get; set; }
        public DateOnly BirthDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

}
