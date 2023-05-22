using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Principal;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CusotmersAccountsController : ControllerBase
    {
        private readonly IStationUnitOfWork
            _stationunitOfWork;
        public CusotmersAccountsController(IStationUnitOfWork stationunitOfWork)
        {
            _stationunitOfWork = stationunitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try {
                return Ok(_stationunitOfWork.CustomersAccountsRepository.GetAll());
            }catch (Exception ex) { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("GetByAccountNo")]
        public IActionResult GetByAccountNo(int accountNo)
        {
            try { 
                return Ok(_stationunitOfWork.CustomersAccountsRepository.GetById(accountNo));
            } catch (Exception ex) { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("GetByAccountNoAsync")]
        public async Task<IActionResult> GetByAccountNoAsync(int accountNo)
        {
            try { 
                return Ok(await _stationunitOfWork.CustomersAccountsRepository.GetByIdAsync(accountNo));
            } catch (Exception ex) { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("GetByAccountInterface")]
        public IActionResult GetByAccountInterface(String accountInterface)
        {
            try
            {
                var result = _stationunitOfWork.CustomersAccountsRepository.FindAll(
                    b => b.Accounts_Interfaces == accountInterface);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCustomersAccountsOrderedByAccountInterface")]
        public IActionResult GetCustomersAccountsOrderedByAccountInterface()
        {
            try
            {
                return Ok(_stationunitOfWork.CustomersAccountsRepository.FindAll(
                b =>true
                    , null, null, b => b.Accounts_Interfaces));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddCustomersAccounts")]
        public IActionResult AddCustomersAccounts([FromBody] tblCustomersAccounts customerAccount)
        {
            try
            {
                var customerTemp = _stationunitOfWork.CustomersAccountsRepository.Add(customerAccount);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddRangeOfCustomersAccounts")]
        public IActionResult AddRangeOfCustomersAccounts([FromBody] List<tblCustomersAccounts> customers)
        {
            try
            {
                List<tblCustomersAccounts> fullCusomters = new List<tblCustomersAccounts>();
                customers.ForEach(b => fullCusomters.Add(b));
                var booksIEnumable = _stationunitOfWork.CustomersAccountsRepository.AddRange(fullCusomters);
                _stationunitOfWork.complete();
                return Ok(booksIEnumable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateCustomerAccount")]
        public IActionResult UpdateCustomerAccount([FromBody] tblCustomersAccounts customerAccount)
        {
            try
            {
                var customerTemp = _stationunitOfWork.CustomersAccountsRepository.Update(customerAccount);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("DeleteCustomerAccount")]
        public IActionResult DeleteCustomerAccount(int account_No)
        {
            try
            {
                tblCustomersAccounts? customerAccount = _stationunitOfWork.CustomersAccountsRepository.Find(b => b.Account_No == account_No);
                if (customerAccount == null) return Ok(false);
                var customerTemp = _stationunitOfWork.CustomersAccountsRepository.Delete(customerAccount);
                _stationunitOfWork.complete();
                return Ok(customerTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
