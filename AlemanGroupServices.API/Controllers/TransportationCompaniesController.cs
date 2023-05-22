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
    public class TransportationCompaniesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TransportationCompaniesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/TransportationCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransportationCompany>>> GetTbltransportationcompanies()
        {
          if (_context.Tbltransportationcompanies == null)
          {
              return NotFound();
          }
            return await _context.Tbltransportationcompanies.ToListAsync();
        }

        // GET: api/TransportationCompanies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransportationCompany>> GetTransportationCompany(uint id)
        {
          if (_context.Tbltransportationcompanies == null)
          {
              return NotFound();
          }
            var transportationCompany = await _context.Tbltransportationcompanies.FindAsync(id);

            if (transportationCompany == null)
            {
                return NotFound();
            }

            return transportationCompany;
        }

        // PUT: api/TransportationCompanies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransportationCompany(uint id, TransportationCompany transportationCompany)
        {
            if (id != transportationCompany.Id)
            {
                return BadRequest();
            }

            _context.Entry(transportationCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransportationCompanyExists(id))
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

        // POST: api/TransportationCompanies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransportationCompany>> PostTransportationCompany(TransportationCompany transportationCompany)
        {
          if (_context.Tbltransportationcompanies == null)
          {
              return Problem("Entity set 'MySQLDBContext.Tbltransportationcompanies'  is null.");
          }
            _context.Tbltransportationcompanies.Add(transportationCompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransportationCompany", new { id = transportationCompany.Id }, transportationCompany);
        }

        // DELETE: api/TransportationCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransportationCompany(uint id)
        {
            if (_context.Tbltransportationcompanies == null)
            {
                return NotFound();
            }
            var transportationCompany = await _context.Tbltransportationcompanies.FindAsync(id);
            if (transportationCompany == null)
            {
                return NotFound();
            }

            _context.Tbltransportationcompanies.Remove(transportationCompany);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransportationCompanyExists(uint id)
        {
            return (_context.Tbltransportationcompanies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
