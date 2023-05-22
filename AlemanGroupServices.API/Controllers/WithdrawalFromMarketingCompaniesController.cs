using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawalFromMarketingCompaniesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public WithdrawalFromMarketingCompaniesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/WithdrawalFromMarketingCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WithdrawalFromMarketingCompanyDto>>> Gettblstationswithdrawals()
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return NotFound();
                }
                var accountsPairs = await getAccountsNamesIdsPairs();
                var result = await (from withdrawals in _context.tblstationswithdrawals 
                                    join transportationcompanies in _context.Tbltransportationcompanies on withdrawals.TransportationId equals transportationcompanies.Id
                                    join trucks in _context.Tblstationtrucks on withdrawals.TruckId equals trucks.Id
                                    join drivers in _context.Tblstationdrivers on withdrawals.DriverId equals drivers.Id
                                    orderby withdrawals.OrderNO
                                    select new WithdrawalFromMarketingCompanyDto 
                                    {
                                        OrderNO = withdrawals.OrderNO,
                                        OrderDate = withdrawals.OrderDate,
                                        AccountNo = withdrawals.AccountNo,
                                        Wareahouse = withdrawals.Wareahouse,
                                        MCInvoiceNo = withdrawals.MCInvoiceNo,
                                        WarehouseInvoiceNo = withdrawals.WarehouseInvoiceNo,
                                        TransportationName = transportationcompanies.TransportContractors,
                                        TruckNumber = trucks.CompanyTruckNo,
                                        DriverName = drivers.DriverName
                                    }).ToListAsync();
                foreach (var row in result)
                {
                    row.AccountName = accountsPairs.Where(p => p.AccountNo == row.AccountNo).Select(p => p.accountName).FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/WithdrawalFromMarketingCompanies
        [HttpGet("GetAccountWithdrawals{accountNo}")]
        public async Task<ActionResult<IEnumerable<WithdrawalFromMarketingCompanyDto>>> GetAccountWithdrawals(uint accountNo)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return NotFound();
                }
                var accountsPairs = await getAccountsNamesIdsPairs();
                var result = await (from withdrawals in _context.tblstationswithdrawals
                                    where withdrawals.AccountNo == accountNo
                                    join transportationcompanies in _context.Tbltransportationcompanies on withdrawals.TransportationId equals transportationcompanies.Id
                                    join trucks in _context.Tblstationtrucks on withdrawals.TruckId equals trucks.Id
                                    join drivers in _context.Tblstationdrivers on withdrawals.DriverId equals drivers.Id
                                    orderby withdrawals.OrderNO
                                    select new WithdrawalFromMarketingCompanyDto 
                                    {
                                        OrderNO = withdrawals.OrderNO,
                                        OrderDate = withdrawals.OrderDate,
                                        AccountNo = withdrawals.AccountNo,
                                        Wareahouse = withdrawals.Wareahouse,
                                        MCInvoiceNo = withdrawals.MCInvoiceNo,
                                        WarehouseInvoiceNo = withdrawals.WarehouseInvoiceNo,
                                        TransportationName = transportationcompanies.TransportContractors,
                                        TruckNumber = trucks.CompanyTruckNo,
                                        DriverName = drivers.DriverName
                                    }).ToListAsync();
                foreach (var row in result)
                {
                    row.AccountName = accountsPairs.Where(p => p.AccountNo == row.AccountNo).Select(p => p.accountName).FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/WithdrawalFromMarketingCompanies
        [HttpGet("GetAccountWithdrawalsInterval{accountNo}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<WithdrawalFromMarketingCompanyDto>>> GetAccountWithdrawalsInterval(uint accountNo, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return NotFound();
                }
                var accountsPairs = await getAccountsNamesIdsPairs();
                var result = await (from withdrawals in _context.tblstationswithdrawals
                                    where withdrawals.AccountNo == accountNo && withdrawals.OrderDate >= startDate && withdrawals.OrderDate <= endDate
                                    join transportationcompanies in _context.Tbltransportationcompanies on withdrawals.TransportationId equals transportationcompanies.Id
                                    join trucks in _context.Tblstationtrucks on withdrawals.TruckId equals trucks.Id
                                    join drivers in _context.Tblstationdrivers on withdrawals.DriverId equals drivers.Id
                                    orderby withdrawals.OrderNO
                                    select new WithdrawalFromMarketingCompanyDto 
                                    {
                                        OrderNO = withdrawals.OrderNO,
                                        OrderDate = withdrawals.OrderDate,
                                        AccountNo = withdrawals.AccountNo,
                                        Wareahouse = withdrawals.Wareahouse,
                                        MCInvoiceNo = withdrawals.MCInvoiceNo,
                                        WarehouseInvoiceNo = withdrawals.WarehouseInvoiceNo,
                                        TransportationName = transportationcompanies.TransportContractors,
                                        TruckNumber = trucks.CompanyTruckNo,
                                        DriverName = drivers.DriverName
                                    }).ToListAsync();
                foreach (var row in result)
                {
                    row.AccountName = accountsPairs.Where(p => p.AccountNo == row.AccountNo).Select(p => p.accountName).FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        

        // GET: api/WithdrawalFromMarketingCompanies/5
        [HttpGet("{orderNo}")]
        public async Task<ActionResult<WithdrawalFromMarketingCompanyDto>> GetWithdrawalFromMarketingCompany(uint orderNo)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return NotFound();
                }
                var withdrawalFromMarketingCompany = await _context.tblstationswithdrawals.FindAsync(orderNo);

                if (withdrawalFromMarketingCompany == null)
                {
                    return NotFound();
                }

                // get the transportation company that transport the order
                var transportationCompany = await _context.Tbltransportationcompanies
                    .Where(tc => tc.Id == withdrawalFromMarketingCompany.TransportationId)
                    .FirstOrDefaultAsync();
                if (transportationCompany == null) return NotFound($"there is no transportation company with Id '{withdrawalFromMarketingCompany.TransportationId}'");
                //get the company truck for the order
                var companyTruck = await _context.Tblstationtrucks.Where(t => t.Id == withdrawalFromMarketingCompany.TruckId).FirstOrDefaultAsync();
                if (companyTruck == null) return NotFound($"there is no company truck with Id '{withdrawalFromMarketingCompany.TruckId}'");

                // get the driver that drive the order truck
                var drivier = await _context.Tblstationdrivers.Where(d => d.Id == withdrawalFromMarketingCompany.DriverId).FirstOrDefaultAsync();
                if (drivier == null) NotFound($"There is no driver with id '{withdrawalFromMarketingCompany.DriverId}'");

                var accountsPairs = await getAccountsNamesIdsPairs();
                var withdrawalFromMarketingCompanyDto = new WithdrawalFromMarketingCompanyDto
                {
                    OrderNO = withdrawalFromMarketingCompany.OrderNO,
                    OrderDate = withdrawalFromMarketingCompany.OrderDate,
                    AccountNo = withdrawalFromMarketingCompany.AccountNo,
                    AccountName = accountsPairs.Where(p=> p.AccountNo == withdrawalFromMarketingCompany.AccountNo).Select(p=> p.accountName).FirstOrDefault(),
                    Wareahouse = withdrawalFromMarketingCompany.Wareahouse,
                    MCInvoiceNo = withdrawalFromMarketingCompany.MCInvoiceNo,
                    WarehouseInvoiceNo = withdrawalFromMarketingCompany.WarehouseInvoiceNo,
                    TransportationName = transportationCompany.TransportContractors,
                    TruckNumber = companyTruck.CompanyTruckNo,
                    DriverName = drivier!.DriverName
                };

                return withdrawalFromMarketingCompanyDto;

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private async Task<List<MarketingCompaniesAccountsPair>> getAccountsNamesIdsPairs()
        {
            var accounts = await _context.Tblmarketingcompaniesaccounts
                    .Join(_context.Tblaccountsinterfaces,
                    marketingCompanyAccount => marketingCompanyAccount.AccountInterfaceId,
                    accountInterface => accountInterface.Id,
                    (MCAccount, AInterface) => new { MCAccount, AInterface })
                    .Join(_context.Tblmarketingcompnies,
                    MCAccountAInterface => MCAccountAInterface.MCAccount.MarketingCompanyId,
                    marketingCompany => marketingCompany.Id,
                    (MCAccountAInterface, marketingCompany) => new MarketingCompaniesAccountsDto
                    {
                        AccountNo = MCAccountAInterface.MCAccount.AccountNo,
                        Date = MCAccountAInterface.MCAccount.Date,
                        AccountsInterface = MCAccountAInterface.AInterface.accounts_interfaces,
                        InitialDept = MCAccountAInterface.MCAccount.InitialDept,
                        Subcompany_Name = _context.Tblsubcompanies.Where(p => p.Id == MCAccountAInterface.AInterface.subcompany_id).Select(p => p.Subcompany_name).FirstOrDefault()!,
                        MarketingCompany = marketingCompany.Marketing_comany
                    }).ToListAsync();
            var result = new List<MarketingCompaniesAccountsPair>();
            foreach (var mCAcountDto in accounts)
            {
                result.Add(new MarketingCompaniesAccountsPair
                {
                    AccountNo = mCAcountDto.AccountNo,
                    accountName = getAccountName(mCAcountDto)
                });
            }
            return result;
        }
        private string getAccountName(MarketingCompaniesAccountsDto account)
        {
            var accountProducts = _context.Tblmarketingcompaccountsproducts
                .Where(p => p.AccountNo == account.AccountNo)
                .Join(_context.Tblmainproducts,
                mCAccountProduct => mCAccountProduct.MainProductId,
                mainProduct => mainProduct.CategoryId,
                (mCAccountProduct, mainProduct) => mainProduct.Products_Category)
                .ToList();
            string accountProductsSTR = string.Join(", ", accountProducts);
            return $"{account.AccountsInterface}, {account.MarketingCompany}, ({accountProductsSTR})";
        }

        // PUT: api/WithdrawalFromMarketingCompanies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkorderNo=2123754
        [HttpPut("{orderNo}")]
        public async Task<IActionResult> PutWithdrawalFromMarketingCompany(uint orderNo, WithdrawalFromMarketingCompanyDto withdrawalFromMarketingCompanyDto)
        {
            try
            {
                if (orderNo != withdrawalFromMarketingCompanyDto.OrderNO)
                {
                    return BadRequest();
                }

                // get the transportation company that transport the order
                var transportationCompany = await _context.Tbltransportationcompanies
                    .Where(tc => tc.TransportContractors == withdrawalFromMarketingCompanyDto.TransportationName)
                    .FirstOrDefaultAsync();
                if (transportationCompany == null) return NotFound($"there is no transportation company with name '{withdrawalFromMarketingCompanyDto.TransportationName}'");
                //get the company truck for the order
                var companyTruck = await _context.Tblstationtrucks.Where(t => t.CompanyTruckNo == withdrawalFromMarketingCompanyDto.TruckNumber).FirstOrDefaultAsync();
                if (companyTruck == null) return NotFound($"there is no company truck with nomber '{withdrawalFromMarketingCompanyDto.TruckNumber}'");

                // get the driver that drive the order truck
                var drivier = await _context.Tblstationdrivers.Where(d => d.DriverName == withdrawalFromMarketingCompanyDto.DriverName).FirstOrDefaultAsync();
                if (drivier == null) NotFound($"There is no driver with name '{withdrawalFromMarketingCompanyDto.DriverName}'");

                var withdrawalFromMarketingCompany = new WithdrawalFromMarketingCompany
                {
                    OrderNO = withdrawalFromMarketingCompanyDto.OrderNO,
                    OrderDate = withdrawalFromMarketingCompanyDto.OrderDate,
                    AccountNo = withdrawalFromMarketingCompanyDto.AccountNo,
                    Wareahouse = withdrawalFromMarketingCompanyDto.Wareahouse,
                    MCInvoiceNo = withdrawalFromMarketingCompanyDto.MCInvoiceNo,
                    WarehouseInvoiceNo = withdrawalFromMarketingCompanyDto.WarehouseInvoiceNo,
                    TransportationId = transportationCompany.Id,
                    TruckId = companyTruck.Id,
                    DriverId = drivier!.Id
                };

                _context.Entry(withdrawalFromMarketingCompany).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WithdrawalFromMarketingCompanyExists(orderNo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // POST: api/WithdrawalFromMarketingCompanies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkorderNo=2123754
        [HttpPost]
        public async Task<ActionResult<WithdrawalFromMarketingCompany>> PostWithdrawalFromMarketingCompany(WithdrawalFromMarketingCompanyDto withdrawalFromMarketingCompanyDto)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return Problem("Entity set 'MySQLDBContext.tblstationswithdrawals'  is null.");
                }
                // get the transportation company that transport the order
                var transportationCompany = await _context.Tbltransportationcompanies
                    .Where(tc=> tc.TransportContractors == withdrawalFromMarketingCompanyDto.TransportationName)
                    .FirstOrDefaultAsync();
                if (transportationCompany == null)  return NotFound($"there is no transportation company with name '{withdrawalFromMarketingCompanyDto.TransportationName}'");
                //get the company truck for the order
                var companyTruck = await _context.Tblstationtrucks.Where(t => t.CompanyTruckNo == withdrawalFromMarketingCompanyDto.TruckNumber).FirstOrDefaultAsync();
                if (companyTruck == null) return NotFound($"there is no company truck with nomber '{withdrawalFromMarketingCompanyDto.TruckNumber}'");

                // get the driver that drive the order truck
                var drivier = await _context.Tblstationdrivers.Where(d=> d.DriverName == withdrawalFromMarketingCompanyDto.DriverName).FirstOrDefaultAsync();
                if (drivier == null) NotFound($"There is no driver with name '{withdrawalFromMarketingCompanyDto.DriverName}'");

                var withdrawalFromMarketingCompany = new WithdrawalFromMarketingCompany
                {
                    OrderNO = withdrawalFromMarketingCompanyDto.OrderNO,
                    OrderDate = withdrawalFromMarketingCompanyDto.OrderDate,
                    AccountNo = withdrawalFromMarketingCompanyDto.AccountNo,
                    Wareahouse = withdrawalFromMarketingCompanyDto.Wareahouse,
                    MCInvoiceNo = withdrawalFromMarketingCompanyDto.MCInvoiceNo,
                    WarehouseInvoiceNo = withdrawalFromMarketingCompanyDto.WarehouseInvoiceNo,
                    TransportationId = transportationCompany.Id,
                    TruckId = companyTruck.Id,
                    DriverId = drivier!.Id
                };
                _context.tblstationswithdrawals.Add(withdrawalFromMarketingCompany);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetWithdrawalFromMarketingCompany", new { orderNo = withdrawalFromMarketingCompany.OrderNO }, withdrawalFromMarketingCompany);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("PostWithdrawalFromMarketingCompanyRange")]
        public async Task<ActionResult<IEnumerable<WithdrawalFromMarketingCompany>>> PostWithdrawalFromMarketingCompanyRange(IEnumerable<WithdrawalFromMarketingCompanyDto> withdrawalFromMarketingCompanyDtos)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return Problem("Entity set 'MySQLDBContext.tblstationswithdrawals'  is null.");
                }
                var duplicates = new List<WithdrawalFromMarketingCompanyDto>();
                foreach (var withdrawalFromMarketingCompanyDto in withdrawalFromMarketingCompanyDtos)
                {
                    // check if there are any duplicates withdrawals in the list based on the order no
                    var duplicatesList = _context.tblstationswithdrawals.Where(w => w.OrderNO == withdrawalFromMarketingCompanyDto.OrderNO).ToList();
                    if (duplicatesList.Count > 0) duplicates.Add(withdrawalFromMarketingCompanyDto);
                    else
                    {
                        // get the transportation company that transport the order
                        var transportationCompany = await _context.Tbltransportationcompanies
                            .Where(tc => tc.TransportContractors == withdrawalFromMarketingCompanyDto.TransportationName)
                            .FirstOrDefaultAsync();
                        if (transportationCompany == null) return NotFound($"there is no transportation company with name '{withdrawalFromMarketingCompanyDto.TransportationName}'");
                        //get the company truck for the order
                        var companyTruck = await _context.Tblstationtrucks.Where(t => t.CompanyTruckNo == withdrawalFromMarketingCompanyDto.TruckNumber).FirstOrDefaultAsync();
                        if (companyTruck == null) return NotFound($"there is no company truck with nomber '{withdrawalFromMarketingCompanyDto.TruckNumber}'");

                        // get the driver that drive the order truck
                        var drivier = await _context.Tblstationdrivers.Where(d => d.DriverName == withdrawalFromMarketingCompanyDto.DriverName).FirstOrDefaultAsync();
                        if (drivier == null) NotFound($"There is no driver with name '{withdrawalFromMarketingCompanyDto.DriverName}'");

                        var withdrawalFromMarketingCompany = new WithdrawalFromMarketingCompany
                        {
                            OrderNO = withdrawalFromMarketingCompanyDto.OrderNO,
                            OrderDate = withdrawalFromMarketingCompanyDto.OrderDate,
                            AccountNo = withdrawalFromMarketingCompanyDto.AccountNo,
                            Wareahouse = withdrawalFromMarketingCompanyDto.Wareahouse,
                            MCInvoiceNo = withdrawalFromMarketingCompanyDto.MCInvoiceNo,
                            WarehouseInvoiceNo = withdrawalFromMarketingCompanyDto.WarehouseInvoiceNo,
                            TransportationId = transportationCompany.Id,
                            TruckId = companyTruck.Id,
                            DriverId = drivier!.Id
                        };
                        _context.tblstationswithdrawals.Add(withdrawalFromMarketingCompany);
                    }
                }
                await _context.SaveChangesAsync();

                if (duplicates.Count > 0) return BadRequest(new {errorMessage = $"There are {duplicates.Count} withdrawals already exist in the database." , duplicatesWithdrawalsList = duplicates }
                    );

                return CreatedAtAction("GetWithdrawalFromMarketingCompanies", new { }, withdrawalFromMarketingCompanyDtos);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/WithdrawalFromMarketingCompanies/5
        [HttpDelete("{orderNo}")]
        public async Task<IActionResult> DeleteWithdrawalFromMarketingCompany(uint orderNo)
        {
            try
            {
                if (_context.tblstationswithdrawals == null)
                {
                    return NotFound();
                }
                var withdrawalFromMarketingCompany = await _context.tblstationswithdrawals.FindAsync(orderNo);
                if (withdrawalFromMarketingCompany == null)
                {
                    return NotFound();
                }

                _context.tblstationswithdrawals.Remove(withdrawalFromMarketingCompany);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool WithdrawalFromMarketingCompanyExists(uint orderNo)
        {
            return (_context.tblstationswithdrawals?.Any(e => e.OrderNO == orderNo)).GetValueOrDefault();
        }
    }
}
