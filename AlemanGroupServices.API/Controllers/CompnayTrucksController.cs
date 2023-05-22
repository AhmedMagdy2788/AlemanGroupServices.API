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
    public class CompnayTrucksController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public CompnayTrucksController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/CompnayTrucks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyTruck>>> GetTblstationtrucks()
        {
            try
            {
                if (_context.Tblstationtrucks == null)
                {
                    return NotFound();
                }
                return await _context.Tblstationtrucks.ToListAsync();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/CompnayTrucks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyTruck>> GetCompnayTruck(uint id)
        {
            try
            {
                if (_context.Tblstationtrucks == null)
                {
                    return NotFound();
                }
                var compnayTruck = await _context.Tblstationtrucks.FindAsync(id);

                if (compnayTruck == null)
                {
                    return NotFound();
                }

                return compnayTruck;

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT: api/CompnayTrucks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompnayTruck(uint id, CompanyTruck compnayTruck)
        {
            try
            {
                if (id != compnayTruck.Id)
                {
                    return BadRequest();
                }

                _context.Entry(compnayTruck).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompnayTruckExists(id))
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

        // POST: api/CompnayTrucks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyTruck>> PostCompnayTruck(CompanyTruck compnayTruck)
        {
            try
            {
                if (_context.Tblstationtrucks == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblstationtrucks'  is null.");
                }
                _context.Tblstationtrucks.Add(compnayTruck);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCompnayTruck", new { id = compnayTruck.Id }, compnayTruck);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/CompnayTrucks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompnayTruck(uint id)
        {
            try
            {
                if (_context.Tblstationtrucks == null)
                {
                    return NotFound();
                }
                var compnayTruck = await _context.Tblstationtrucks.FindAsync(id);
                if (compnayTruck == null)
                {
                    return NotFound();
                }

                _context.Tblstationtrucks.Remove(compnayTruck);
                await _context.SaveChangesAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool CompnayTruckExists(uint id)
        {
            return (_context.Tblstationtrucks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
