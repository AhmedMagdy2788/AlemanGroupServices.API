using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using System.Data;

namespace AlemanGroupServices.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketingCompaniesAccountsController : ControllerBase
{
    private readonly MySQLDBContext _context;

    public MarketingCompaniesAccountsController(MySQLDBContext context)
    {
        _context = context;
    }

    [HttpGet("GetAccountsName")]
    public async Task<ActionResult<IEnumerable<MarketingCompaniesAccountsPair>>> GetAccountsName()
    {
        try
        {
            if (_context.Tblmarketingcompaniesaccounts == null)
            {
                return NotFound();
            }
            var mCAccountDtosList = await _context.Tblmarketingcompaniesaccounts
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
            foreach (var mCAcountDto in mCAccountDtosList)
            {
                result.Add(new MarketingCompaniesAccountsPair
                {
                    AccountNo = mCAcountDto.AccountNo,
                    accountName = getAccountName(mCAcountDto)
                });
            }
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.ToString());
        }
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

    // GET: api/MarketingCompaniesAccounts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MarketingCompaniesAccountsDto>>> GetTblmarketingcompaniesaccounts()
    {
        try
        {
            if (_context.Tblmarketingcompaniesaccounts == null)
            {
                return NotFound();
            }
            var subcompaniesPairs = _context.Tblsubcompanies.Select(sc => new { sc.Id, sc.Subcompany_name }).ToList();

            var result = await (from a in _context.Tblmarketingcompaniesaccounts
                                join b in _context.Tblaccountsinterfaces on a.AccountInterfaceId equals b.Id
                                join c in _context.Tblmarketingcompnies on a.MarketingCompanyId equals c.Id
                                orderby a.AccountNo // add this line to sort by AccountNo
                                select new MarketingCompaniesAccountsDto
                                {
                                    AccountNo = a.AccountNo,
                                    Date = a.Date,
                                    AccountsInterface = b.accounts_interfaces,
                                    InitialDept = a.InitialDept,
                                    Subcompany_Name = _context.Tblsubcompanies
                                    .Where(p => p.Id == b.subcompany_id)
                                    .Select(p => p.Subcompany_name)
                                    .FirstOrDefault()!,
                                    MarketingCompany = c.Marketing_comany
                                }
                         ).ToListAsync();
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.ToString());
        }
    }

    // GET: api/MarketingCompaniesAccounts/5
    [HttpGet("{accountNo}")]
    public async Task<ActionResult<MarketingCompaniesAccountsDto>> GetMarketingCompaniesAccounts(int accountNo)
    {
        try
        {
            if (_context.Tblmarketingcompaniesaccounts == null)
            {
                return NotFound();
            }

            var result = await (from a in _context.Tblmarketingcompaniesaccounts
                                where a.AccountNo == accountNo
                                join b in _context.Tblaccountsinterfaces on a.AccountInterfaceId equals b.Id
                                join c in _context.Tblmarketingcompnies on a.MarketingCompanyId equals c.Id
                                select new MarketingCompaniesAccountsDto
                                {
                                    AccountNo = a.AccountNo,
                                    Date = a.Date,
                                    AccountsInterface = b.accounts_interfaces,
                                    InitialDept = a.InitialDept,
                                    Subcompany_Name = _context.Tblsubcompanies.Where(p => p.Id == b.subcompany_id).Select(p => p.Subcompany_name).FirstOrDefault()!,
                                    MarketingCompany = c.Marketing_comany
                                }).FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex)
        {
            return Problem(ex.ToString());
        }
    }

    // PUT: api/MarketingCompaniesAccounts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkaccountNo=2123754
    [HttpPut("{accountNo}")]
    public async Task<IActionResult> PutMarketingCompaniesAccounts(int accountNo, MarketingCompaniesAccountsDto marketingCompaniesAccountsDto)
    {
        if (accountNo != marketingCompaniesAccountsDto.AccountNo)
        {
            return BadRequest();
        }

        var marketingCompany = await _context.Tblmarketingcompnies.Where(x => x.Marketing_comany == marketingCompaniesAccountsDto.MarketingCompany).FirstOrDefaultAsync();
        if (marketingCompany == null)
        {
            return NotFound($"there is no Marketing Company whith name '{marketingCompaniesAccountsDto.MarketingCompany}'");
        }

        var subcompnay = await _context.Tblsubcompanies.Where(sc => sc.Subcompany_name == marketingCompaniesAccountsDto.Subcompany_Name).FirstOrDefaultAsync();
        if (subcompnay == null)
        {
            return NotFound($"there is no subcompany whith name '{marketingCompaniesAccountsDto.Subcompany_Name}'");
        }

        var accountInterface = await _context.Tblaccountsinterfaces.Where(ai => ai.subcompany_id == subcompnay.Id && ai.accounts_interfaces == marketingCompaniesAccountsDto.AccountsInterface).FirstOrDefaultAsync();
        if (accountInterface == null)
        {
            return NotFound($"there is no account interface with subcompany name '{marketingCompaniesAccountsDto.Subcompany_Name}' and interface called '{marketingCompaniesAccountsDto.AccountsInterface}'");
        }

        _context.Entry(new MarketingCompaniesAccounts
        {
            AccountNo = marketingCompaniesAccountsDto.AccountNo,
            Date = marketingCompaniesAccountsDto.Date,
            AccountInterfaceId = accountInterface.Id,
            MarketingCompanyId = marketingCompany.Id,
            InitialDept = marketingCompaniesAccountsDto.InitialDept
        }).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MarketingCompaniesAccountsExists(accountNo))
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

    // POST: api/MarketingCompaniesAccounts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkaccountNo=2123754
    [HttpPost]
    public async Task<ActionResult<MarketingCompaniesAccountsDto>> PostMarketingCompaniesAccounts(MarketingCompaniesAccountsDto marketingCompaniesAccountsDto)
    {
        try
        {
            if (_context.Tblmarketingcompaniesaccounts == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblmarketingcompaniesaccounts'  is null.");
            }

            var marketingCompany = await _context.Tblmarketingcompnies.Where(x => x.Marketing_comany == marketingCompaniesAccountsDto.MarketingCompany).FirstOrDefaultAsync();
            if (marketingCompany == null)
            {
                return NotFound($"there is no Marketing Company whith name '{marketingCompaniesAccountsDto.MarketingCompany}'");
            }
            var subcompnay = await _context.Tblsubcompanies.Where(sc => sc.Subcompany_name == marketingCompaniesAccountsDto.Subcompany_Name).FirstOrDefaultAsync();
            if (subcompnay == null)
            {
                return NotFound($"there is no subcompany whith name '{marketingCompaniesAccountsDto.Subcompany_Name}'");
            }

            var accountInterface = await _context.Tblaccountsinterfaces.Where(ai => ai.subcompany_id == subcompnay.Id && ai.accounts_interfaces == marketingCompaniesAccountsDto.AccountsInterface).FirstOrDefaultAsync();
            if (accountInterface == null)
            {
                return NotFound($"there is no account interface with subcompany name '{marketingCompaniesAccountsDto.Subcompany_Name}' and interface called '{marketingCompaniesAccountsDto.AccountsInterface}'");
            }
            _context.Tblmarketingcompaniesaccounts.Add(new MarketingCompaniesAccounts
            {
                AccountNo = marketingCompaniesAccountsDto.AccountNo,
                Date = marketingCompaniesAccountsDto.Date,
                AccountInterfaceId = accountInterface.Id,
                MarketingCompanyId = marketingCompany.Id,
                InitialDept = marketingCompaniesAccountsDto.InitialDept
            });
            await _context.SaveChangesAsync();

            return Ok(marketingCompaniesAccountsDto);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("addRangeOfMarketingcompaniesAccountsDtos")]
    public async Task<IActionResult> AddRangeOfMarketingcompaniesAccountsDtos(List<MarketingCompaniesAccountsDto> marketingCompaniesAccountsDtoList)
    {
        try
        {
            if (_context.Tblmarketingcompaniesaccounts == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblmarketingcompaniesaccounts'  is null.");
            }
            foreach (var marketingCompaniesAccountsDto in marketingCompaniesAccountsDtoList)
            {
                var marketingCompany = await _context.Tblmarketingcompnies.Where(x => x.Marketing_comany == marketingCompaniesAccountsDto.MarketingCompany).FirstOrDefaultAsync();
                if (marketingCompany == null)
                {
                    return NotFound($"there is no Marketing Company whith name '{marketingCompaniesAccountsDto.MarketingCompany}'");
                }
                var subcompnay = await _context.Tblsubcompanies.Where(sc => sc.Subcompany_name == marketingCompaniesAccountsDto.Subcompany_Name).FirstOrDefaultAsync();
                if (subcompnay == null)
                {
                    return NotFound($"there is no subcompany whith name '{marketingCompaniesAccountsDto.Subcompany_Name}'");
                }

                var accountInterface = await _context.Tblaccountsinterfaces.Where(ai => ai.subcompany_id == subcompnay.Id && ai.accounts_interfaces == marketingCompaniesAccountsDto.AccountsInterface).FirstOrDefaultAsync();
                if (accountInterface == null)
                {
                    return NotFound($"there is no account interface with subcompany name '{marketingCompaniesAccountsDto.Subcompany_Name}' and interface called '{marketingCompaniesAccountsDto.AccountsInterface}'");
                }
                _context.Tblmarketingcompaniesaccounts.Add(new MarketingCompaniesAccounts
                {
                    AccountNo = marketingCompaniesAccountsDto.AccountNo,
                    Date = marketingCompaniesAccountsDto.Date,
                    AccountInterfaceId = accountInterface.Id,
                    MarketingCompanyId = marketingCompany.Id,
                    InitialDept = marketingCompaniesAccountsDto.InitialDept
                });
            }
            await _context.SaveChangesAsync();

            return Ok();

        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    // DELETE: api/MarketingCompaniesAccounts/5
    [HttpDelete("{accountNo}")]
    public async Task<IActionResult> DeleteMarketingCompaniesAccounts(int accountNo)
    {
        if (_context.Tblmarketingcompaniesaccounts == null)
        {
            return NotFound();
        }
        var marketingCompaniesAccounts = await _context.Tblmarketingcompaniesaccounts.FindAsync(accountNo);
        if (marketingCompaniesAccounts == null)
        {
            return NotFound();
        }

        _context.Tblmarketingcompaniesaccounts.Remove(marketingCompaniesAccounts);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MarketingCompaniesAccountsExists(int accountNo)
    {
        return (_context.Tblmarketingcompaniesaccounts?.Any(e => e.AccountNo == accountNo)).GetValueOrDefault();
    }
}
