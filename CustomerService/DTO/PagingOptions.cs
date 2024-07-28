namespace CustomerService.DTO
{
    public class PagingOptions
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string SortProperty { get; set; } 
        public string SortDirection { get; set; } 
        public string? SearchProperty { get; set; } 
        public string? SearchText { get; set; } 

    }
}
