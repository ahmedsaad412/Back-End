using CustomerService.DTO;
using CustomerService.DTO.FilterDTO;
using CustomerService.DTO.Page;
using CustomerService.Entities;

namespace CustomerService.IService
{
    public interface ICustomerService
    { 
        Task<DataGridDTO> GetCustomers();
        //int GetCustomersCount();
        //Task<DataGridDTO> GridCustomers(GridParameterDTO options); 
 

  
        //Task<PageDTO<CustomersDTO>> GetCustomerPage(int skip , int take ,string orderBy);
        //Task<List<CustomersDTO>> GetSortedListByPropertyName(string orderBy); 


        //Task<PageDTO<T>> GetPage<T>(PagingOptions pagingOptions) where T : class;
        Task<PageDTO<Customer>> GetCustomerPage(PagingOptions pagingOptions );
    }
}
