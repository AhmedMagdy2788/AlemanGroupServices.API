using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsInterfacesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public AccountsInterfacesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/AccountsInterfaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountsInterfacesDto>>> GetTblaccountsinterfaces()
        {
            try
            {
                if (_context.Tblaccountsinterfaces == null)
                {
                    return NotFound("AccountsInterfaces table does not exist");
                }
                return await _context.Tblaccountsinterfaces
                    .Join(_context.Tblsubcompanies, accountInterface => accountInterface.subcompany_id, subcompany => subcompany.Id, (accountInterface, subcompany) => new AccountsInterfacesDto
                    {
                        Id = accountInterface.Id,
                        subcompany_name = subcompany.Subcompany_name,
                        accounts_interfaces = accountInterface.accounts_interfaces,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        // GET: api/AccountsInterfaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountsInterfacesDto>> GetAccountsInterfaces(int id)
        {
            try
            {
                if (_context.Tblaccountsinterfaces == null)
                {
                    return NotFound();
                }
                var accountsInterfaces = await _context.Tblaccountsinterfaces
                    .Where(ai => ai.Id == id)
                    .Join(_context.Tblsubcompanies, accountInterface => accountInterface.subcompany_id, subcompany => subcompany.Id, (accountInterface, subcompany) => new AccountsInterfacesDto
                    {
                        Id = accountInterface.Id,
                        subcompany_name = subcompany.Subcompany_name,
                        accounts_interfaces = accountInterface.accounts_interfaces,
                    }).FirstOrDefaultAsync();

                if (accountsInterfaces == null)
                {
                    return NotFound();
                }

                return accountsInterfaces;
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountsInterfaces(int id, AccountsInterfacesDto accountsInterfacesDto)
        {
            try
            {
                if (id != accountsInterfacesDto.Id)
                {
                    return BadRequest();
                }
                var subcompany = await _context.Tblsubcompanies.Where(ai => ai.Subcompany_name == accountsInterfacesDto.subcompany_name).FirstOrDefaultAsync();
                if (subcompany == null)
                {
                    return NotFound($"there is no subcopany with name '{accountsInterfacesDto.subcompany_name}'");
                }
                _context.Entry(new AccountsInterfaces
                {
                    Id = accountsInterfacesDto.Id,
                    subcompany_id = subcompany.Id,
                    accounts_interfaces = accountsInterfacesDto.accounts_interfaces,
                }).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountsInterfacesExists(id))
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
                return Problem(ex.ToString());
            }
        }

        // POST: api/AccountsInterfaces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AccountsInterfaces>> PostAccountsInterfaces(AccountsInterfacesDto accountsInterfacesDto)
        {
            try
            {
                if (_context.Tblaccountsinterfaces == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblaccountsinterfaces'  is null.");
                }
                var subcompany = await _context.Tblsubcompanies.Where(ai => ai.Subcompany_name == accountsInterfacesDto.subcompany_name).FirstOrDefaultAsync();
                if (subcompany == null)
                {
                    return NotFound($"there is no subcopany with name '{accountsInterfacesDto.subcompany_name}'");
                }
                _context.Tblaccountsinterfaces.Add(new AccountsInterfaces
                {
                    Id = accountsInterfacesDto.Id,
                    subcompany_id = subcompany.Id,
                    accounts_interfaces = accountsInterfacesDto.accounts_interfaces,
                });
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAccountsInterfaces", new { id = accountsInterfacesDto.Id }, accountsInterfacesDto);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        // DELETE: api/AccountsInterfaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountsInterfaces(int id)
        {
            try
            {
                if (_context.Tblaccountsinterfaces == null)
                {
                    return NotFound();
                }
                var accountsInterfaces = await _context.Tblaccountsinterfaces.FindAsync(id);
                if (accountsInterfaces == null)
                {
                    return NotFound();
                }

                _context.Tblaccountsinterfaces.Remove(accountsInterfaces);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        private bool AccountsInterfacesExists(int id)
        {
            return (_context.Tblaccountsinterfaces?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
