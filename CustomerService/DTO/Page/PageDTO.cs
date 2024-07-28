namespace CustomerService.DTO.Page
{
    public class PageDTO<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItemCount { get; set; }
    }
}
