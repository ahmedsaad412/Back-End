using CustomerService.DTO;

namespace CustomerService.IService
{
    public interface ICustomerService
    {
        #region demo
        //Task<DataGridDTO> GetCustomers();
        //int GetCustomersCount();
        //Task<DataGridDTO> GridCustomers(GridParameterDTO options); 
        #endregion

        Task<PageDTO<CustomersDTO>> GetCustomerPage(int skip , int take ,string orderBy);
        Task<List<CustomersDTO>> GetSortedListByPropertyName(string orderBy);

        Task<PageDTO<T>> GetGenricPage<T>(int skip , int take ,string orderBy) where T :class;
    }
}
