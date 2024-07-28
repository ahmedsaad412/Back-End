using CustomerService.Data;
using CustomerService.DTO;
using CustomerService.DTO.Page;
using CustomerService.Extensions;
using CustomerService.IService;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Service
{
    public class MyCustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public MyCustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PageDTO<T>> GetPage<T>(PagingOptions pagingOptions) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            var page = await query.ToPagedAsync(pagingOptions);
            return page;
        }

        #region demo
        public async Task<DataGridDTO> GetCustomers()
        {
            List<CustomersDTO> customers = await GetCustomersList();
            int totalNumber = customers.Count();
            DataGridDTO Data = new DataGridDTO { Data = customers, TotalNumber = totalNumber };
            return Data;
        }

        //public async Task<DataGridDTO> GridCustomers(GridParameterDTO options)
        //{
        //    if (options != null)
        //    {
        //        int page = options.Page;
        //        int take = options.Take;
        //        string orderBy = options.OrderBy;
        //        OrderType orderType = options.OrderType;
        //        dynamic response;
        //        if (orderType.ToString() == "desc")
        //        {
        //            response = await _context.Customers
        //            .Select(x => new CustomersDTO
        //            {
        //                Id = x.Id,
        //                FirstName = x.FirstName,
        //                LastName = x.LastName,
        //                Email = x.Email,
        //                PhoneNumber = x.PhoneNumber,
        //            }).OrderBy(orderBy + " desc").Skip((page - 1) * take).Take(take).ToListAsync();

        //        }
        //        else if (orderType.ToString() == "asc" || orderType.ToString() == null || orderType.ToString() == "")
        //        {
        //            response =await _context.Customers
        //            .Select(x => new CustomersDTO
        //            {
        //                Id = x.Id,
        //                FirstName = x.FirstName,
        //                LastName = x.LastName,
        //                Email = x.Email,
        //                PhoneNumber = x.PhoneNumber,
        //            }).OrderBy(options.OrderBy + " asc").Skip((page - 1) * take).Take(take).ToListAsync();
        //        }

        //        else
        //        {
        //            throw new Exception("Order Type must be 'asc' or 'desc' ");
        //        }
        //        int totalNumber = GetCustomersCount();
        //        DataGridDTO Data = new DataGridDTO { Data = response, TotalNumber = totalNumber };
        //        return Data;
        //    }
        //    else
        //    {
        //        throw new ArgumentNullException(nameof(options), "Options cannot be null.");
        //    }

        //}


        public async Task<List<CustomersDTO>> GetCustomersList()
        {
            List<CustomersDTO> allCustomers = await _context.ASCustomers.Select(x => new CustomersDTO
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
            }).ToListAsync();
            return allCustomers;
        }
        public int GetCustomersCount()
        {
            int totalNumbers = _context.ASCustomers.ToList().Count();
            return totalNumbers;
        }
        #endregion

        #region demo 2
        //public async Task<PageDTO<CustomersDTO>> GetCustomerPage(int skip, int take, string orderBy)
        //{
        //    var page = await _context.Customers.Select(x=>new CustomersDTO
        //    {
        //        FirstName=x.FirstName,
        //        Email=x.Email,
        //        LastName=x.LastName,
        //        PhoneNumber=x.PhoneNumber,
        //        Id=x.Id
        //    }).ToPagedAsync(skip, take, orderBy);
        //    return page;
        //}


        //public async Task<List<CustomersDTO>> GetSortedListByPropertyName(string orderBy)
        //{
        //    var sortedList =await _context.Customers.Select(x => new CustomersDTO
        //    {
        //        FirstName = x.FirstName,
        //        Email = x.Email,
        //        LastName = x.LastName,
        //        PhoneNumber = x.PhoneNumber,
        //        Id = x.Id
        //    }).OrderBy<CustomersDTO>(orderBy).ToListAsync();
        //    return sortedList;
        //} 
        #endregion
    }
}
