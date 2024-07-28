using System.Reflection.Metadata.Ecma335;

namespace CustomerService.DTO
{
    public class DataGridDTO
    {
       
        public List<CustomersDTO> Data { get; set; }
        public int TotalNumber { get; set; }
    }
}
