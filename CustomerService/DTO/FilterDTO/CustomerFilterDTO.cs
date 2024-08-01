namespace CustomerService.DTO.FilterDTO
{
    public class CustomerFilterDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? DateFrom { get; set; } 
        public string? DateTo { get; set; }   
    }
}
