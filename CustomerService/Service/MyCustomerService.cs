﻿using CustomerService.Data;
using CustomerService.DTO;
using CustomerService.DTO.FilterDTO;
using CustomerService.DTO.Page;
using CustomerService.Entities;
using CustomerService.Extensions;
using CustomerService.IService;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CustomerService.Service
{
    public class MyCustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public MyCustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        #region demo

        public async Task<PageDTO<Customer>> GetCustomerPage(PagingOptions pagingOptions)
        {
            CustomerFilterDTO filter = DeserializeFilter(pagingOptions.Filters) ?? new CustomerFilterDTO();
            var query = BuildQuery(filter);
            var page = await query.PageAsync(pagingOptions);
            return page;
        }
        private IQueryable<Customer> BuildQuery(CustomerFilterDTO filter)
        {
            var query = _context.ASCustomers.AsQueryable();      
                query = ApplyContainFilter(query, filter.FirstName, filter.LastName);         
                query = ApplyEqualFilter(query, filter.PhoneNumber);
                query = ApplyDateFilters(query, filter.DateFrom, filter.DateTo);
            return query;
            #region 
            //var query = _context.ASCustomers.AsQueryable();
            //if(!string.IsNullOrWhiteSpace(filter.FirstName)|| !string.IsNullOrWhiteSpace(filter.LastName))
            //    query = ApplyContainFilter(query, filter.FirstName, filter.LastName);
            //if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            //    query = ApplyEqualFilter(query, filter.PhoneNumber);
            //if (!string.IsNullOrWhiteSpace(filter.DateFrom) || !string.IsNullOrWhiteSpace(filter.DateTo))
            //    query = ApplyDateFilters(query, filter.DateFrom, filter.DateTo);
            //return query; 
            #endregion
        }

        private IQueryable<Customer> ApplyContainFilter(IQueryable<Customer> query, string? firstName, string? lastName)
        {
            
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(c => c.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(c => c.LastName.Contains(lastName));
            }

            return query;
        }

        private IQueryable<Customer> ApplyEqualFilter(IQueryable<Customer> query, string? phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                query = query.Where(c => c.PhoneNumber == phoneNumber);
            return query;
        }

        private IQueryable<Customer> ApplyDateFilters(IQueryable<Customer> query, string? dateFromStr, string? dateToStr)
        {
            if (DateOnly.TryParse(dateFromStr, out DateOnly dateFrom))
            {
                query = query.Where(c => c.BirthDate >= dateFrom);
            }

            if (DateOnly.TryParse(dateToStr, out DateOnly dateTo))
            {
                query = query.Where(c => c.BirthDate <= dateTo);
            }

            return query;
        }
        private CustomerFilterDTO? DeserializeFilter(string filters)
        {
            CustomerFilterDTO? filter = string.IsNullOrWhiteSpace(filters)
                ? new CustomerFilterDTO()
                : JsonSerializer.Deserialize<CustomerFilterDTO>(filters, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return filter;
        }


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
        public async Task<DataGridDTO> GetCustomers()
        {
            List<CustomersDTO> customers = await GetCustomersList();
            int totalNumber = customers.Count();
            DataGridDTO Data = new DataGridDTO { Data = customers, TotalNumber = totalNumber };
            return Data;
        }
        //public async Task<PageDTO<T>> GetPage<T>(PagingOptions pagingOptions) where T : class
        //{
        //    IQueryable<T> query = _context.Set<T>();
        //    var page = await query.ToPagedAsync(pagingOptions);
        //    return page;
        //}
        //public int GetCustomersCount()
        //{
        //    int totalNumbers = _context.ASCustomers.ToList().Count();
        //    return totalNumbers;
        //}
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

        #region code refactor for GetCustomerPage method 
        //public async Task<PageDTO<Customer>> GetCustomerPage(PagingOptions pagingOptions)
        //{
        //    var filter = DeserializeFilter(pagingOptions.Filters);
        //    var query = BuildQuery(filter);

        //    var page = await query.PageAsync(pagingOptions);
        //    return page;
        //}

        //private IQueryable<Customer> BuildQuery(CustomerFilterDTO filter)
        //{
        //    var query = _context.ASCustomers.AsQueryable();

        //    query = ApplyNameFilter(query, filter.FirstName, filter.LastName);
        //    query = ApplyPhoneNumberFilter(query, filter.PhoneNumber);
        //    query = ApplyDateFilters(query, filter.DateFrom, filter.DateTo);

        //    return query;
        //}

        //private IQueryable<Customer> ApplyNameFilter(IQueryable<Customer> query, string? firstName, string? lastName)
        //{
        //    if (!string.IsNullOrWhiteSpace(firstName))
        //    {
        //        query = query.Where(c => c.FirstName.Contains(firstName));
        //    }

        //    if (!string.IsNullOrWhiteSpace(lastName))
        //    {
        //        query = query.Where(c => c.LastName.Contains(lastName));
        //    }

        //    return query;
        //}

        //private IQueryable<Customer> ApplyPhoneNumberFilter(IQueryable<Customer> query, string? phoneNumber)
        //{
        //    if (!string.IsNullOrWhiteSpace(phoneNumber))
        //    {
        //        query = query.Where(c => c.PhoneNumber == phoneNumber);
        //    }

        //    return query;
        //}

        //private IQueryable<Customer> ApplyDateFilters(IQueryable<Customer> query, string? dateFromStr, string? dateToStr)
        //{
        //    if (DateOnly.TryParse(dateFromStr, out DateOnly dateFrom))
        //    {
        //        query = query.Where(c => c.BirthDate >= dateFrom);
        //    }

        //    if (DateOnly.TryParse(dateToStr, out DateOnly dateTo))
        //    {
        //        query = query.Where(c => c.BirthDate <= dateTo);
        //    }

        //    return query;
        //} 
        #endregion
    }
}
