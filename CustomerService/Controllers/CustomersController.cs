using Azure;
using CustomerService.DTO;
using CustomerService.Entities;
using CustomerService.IService;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;

        }

        [HttpGet("AllCustomers")]
        public async Task<ActionResult<Page<Customer>>> GetGenricPaged()
        {
            var page = await _customerService.GetCustomers();
            return Ok(page);
        }
        [HttpPost("GetCustomerPage")]
        public async Task<ActionResult<Page<Customer>>> GetCustomerPage([FromBody] PagingOptions pagingOptions)
        {


            var page = await _customerService.GetPage<Customer>(pagingOptions);
            return Ok(page);
        }

        //[HttpPost("GetUserPage")]
        //public async Task<ActionResult<Page<User>>> GetUserPage([FromBody] PagingOptions pagingOptions)
        //{
        //    var page = await _customerService.GetPage<User>(pagingOptions);
        //    return Ok(page);
        //}
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
        #region demo 2 
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

        //[HttpGet("UseGenric")]
        //public async Task<ActionResult<Page<Customer>>> GetGenricPaged(int skip, [Required] int take, string orderBy)
        //{
        //    var page = await _customerService.GetGenricPage<Customer>(skip, take, orderBy);
        //    return Ok(page);
        //} 

        #endregion
    }
}
