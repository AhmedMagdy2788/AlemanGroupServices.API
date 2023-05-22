using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDriversController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public CompanyDriversController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/CompanyDrivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDriver>>> GetTblstationdrivers()
        {
            try
            {
                if (_context.Tblstationdrivers == null)
                {
                    return NotFound();
                }
                return await _context.Tblstationdrivers.ToListAsync();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/CompanyDrivers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDriver>> GetCompanyDriver(uint id)
        {
            try
            {
                if (_context.Tblstationdrivers == null)
                {
                    return NotFound();
                }
                var companyDriver = await _context.Tblstationdrivers.FindAsync(id);

                if (companyDriver == null)
                {
                    return NotFound();
                }

                return companyDriver;

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT: api/CompanyDrivers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyDriver(uint id, CompanyDriver companyDriver)
        {
            try
            {
                if (id != companyDriver.Id)
                {
                    return BadRequest();
                }

                _context.Entry(companyDriver).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyDriverExists(id))
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

        // POST: api/CompanyDrivers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyDriver>> PostCompanyDriver(CompanyDriver companyDriver)
        {
            try
            {
                if (_context.Tblstationdrivers == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblstationdrivers'  is null.");
                }
                _context.Tblstationdrivers.Add(companyDriver);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCompanyDriver", new { id = companyDriver.Id }, companyDriver);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/CompanyDrivers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyDriver(uint id)
        {
            try
            {
                if (_context.Tblstationdrivers == null)
                {
                    return NotFound();
                }
                var companyDriver = await _context.Tblstationdrivers.FindAsync(id);
                if (companyDriver == null)
                {
                    return NotFound();
                }

                _context.Tblstationdrivers.Remove(companyDriver);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool CompanyDriverExists(uint id)
        {
            return (_context.Tblstationdrivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
