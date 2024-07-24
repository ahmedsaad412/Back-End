namespace CustomerService.DTO
{
    public class PageDTO<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItemCount { get; set; }
    }
}
