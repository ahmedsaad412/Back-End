namespace CustomerService.DTO
{
    public class GridParameterDTO
    {
        public int Page { get; set; }
        public int Take { get; set; }
        public string OrderBy { get; set; }
        public OrderType OrderType { get; set; }
    }
    public enum OrderType
    {
        asc = 1,
        desc = 2
    }
}
