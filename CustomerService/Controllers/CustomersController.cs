using Azure;
using CustomerService.Data;
using CustomerService.DTO;
using CustomerService.Entities;
using CustomerService.Extensions;
using CustomerService.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;

        public CustomersController(ICustomerService customerService,ApplicationDbContext context )
        {
            _customerService = customerService;
            _context = context;
        }

        #region demo
        //[HttpGet]
        //public async Task<ActionResult<DataGridDTO>> GetCustomers()
        //{
        //    var data = await _customerService.GetCustomers();
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(data);
        //}

        //[HttpGet("GridViewTable")]
        //public async Task<ActionResult<DataGridDTO>> GetCustomer()
        //{
        //    GridParameterDTO parameters = new GridParameterDTO { Page = 1, Take = 3, OrderBy = "LastName", OrderType = OrderType.asc };
        //    var data = await _customerService.GridCustomers(parameters);
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(data);
        //} 
        #endregion

        //[HttpGet("paged")]
        //public async Task<ActionResult<Page<CustomersDTO>>> GetPaged(int skip , [Required] int take, string orderBy)
        //{
        //    var page = await _customerService.GetCustomerPage(skip,take,orderBy);
        //    return Ok(page);
        //}
        //[HttpGet("SortedList")]
        //public async Task<ActionResult<List<CustomersDTO>>> SortedList( string orderBy)
        //{
        //    var page = await _customerService.GetSortedListByPropertyName(orderBy);
        //    return Ok(page);
        //}

        [HttpGet("UseGenric")]
        public async Task<ActionResult<Page<Customer>>> GetGenricPaged(int skip, [Required] int take, string orderBy)
        {
            var page = await _customerService.GetGenricPage<Customer>(skip, take, orderBy);
            return Ok(page);
        }
    }
    }
