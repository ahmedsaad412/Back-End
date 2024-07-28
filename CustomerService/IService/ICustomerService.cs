using CustomerService.DTO;
using CustomerService.DTO.Page;

namespace CustomerService.IService
{
    public interface ICustomerService
    {
        #region demo
        Task<DataGridDTO> GetCustomers();
        //int GetCustomersCount();
        //Task<DataGridDTO> GridCustomers(GridParameterDTO options); 
        #endregion

        #region demo 2
        //Task<PageDTO<CustomersDTO>> GetCustomerPage(int skip , int take ,string orderBy);
        //Task<List<CustomersDTO>> GetSortedListByPropertyName(string orderBy); 
        #endregion

        Task<PageDTO<T>> GetPage<T>(PagingOptions pagingOptions) where T : class;
    }
}
