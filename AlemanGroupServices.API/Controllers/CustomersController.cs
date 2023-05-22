using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IStationUnitOfWork
            _stationunitOfWork;
        public CustomersController(IStationUnitOfWork stationunitOfWork)
        {
            _stationunitOfWork = stationunitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationunitOfWork.CustomerRepository.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_stationunitOfWork.CustomerRepository.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                return Ok(await _stationunitOfWork.CustomerRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByName")]
        public IActionResult GetByName(String customer_name)
        {
            try
            {
                return Ok(_stationunitOfWork.CustomerRepository.Find(
                    b => b.Name == customer_name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetStationCustomers")]
        public async Task<IActionResult> GetStationCustomers(string station_name)
        {
            try
            {
                string sql = $"CALL getStationCustomers( '{station_name}');";
                var customers = await _stationunitOfWork.DataAccess.LoadData<Tblcustomer, dynamic>(sql, new { });
                return Ok(customers);

                //List<int> ids = await getStationCusotmersIds(Station_Id);
                //return Ok(_stationunitOfWork.CustomerRepository.FindAll(
                //    b => ids.Contains(b.Id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddCustomer")]
        public IActionResult AddCustomer([FromBody] Tblcustomer customer)
        {
            try
            {
                var customerTemp = _stationunitOfWork.CustomerRepository.Add(customer);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddRangeOfCustomers")]
        public IActionResult AddRangeOfCustomers([FromBody] List<Tblcustomer> customers)
        {
            try
            {
                List<Tblcustomer> fullCusomters = new List<Tblcustomer>();
                customers.ForEach(b => fullCusomters.Add(b));
                var booksIEnumable = _stationunitOfWork.CustomerRepository.AddRange(fullCusomters);
                _stationunitOfWork.complete();
                return Ok(booksIEnumable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateCustomer")]
        public IActionResult UpdateCustomer([FromBody] Tblcustomer customer)
        {
            try
            {
                var customerTemp = _stationunitOfWork.CustomerRepository.Update(customer);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteCustomer")]
        public IActionResult DeleteCustomer( int customerId)
        {
            try
            {
                Tblcustomer? customer = _stationunitOfWork.CustomerRepository.Find(b=> b.Id == customerId);
                if (customer == null)return Ok(false);
                var customerTemp = _stationunitOfWork.CustomerRepository.Delete(customer);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Task<List<int>> getStationCusotmersIds(string Station_Id)
        //{
        //    try
        //    {
        //        //string sql = "select * from tblstations;";
        //        //var stations = await _stationunitOfWork.DataAccess.LoadData<Station, dynamic>(sql, new { });
        //        List<tblCustomersAccounts?> customerAccounts = _stationunitOfWork.CustomersAccountsRepository.FindAll(b => b.Accounts_Interfaces == Station_Id).ToList();
            
        //        List<int> ids = new();
        //        foreach (var account in customerAccounts)
        //        {
        //            ids.Add(account!.Customer_No);
        //        }
        //        return Task.FromResult(ids);
        //    }
        //    catch (Exception)
        //    {
        //        return Task.FromResult<List<int>>(result: null);
        //    }
        //}

    }
}
