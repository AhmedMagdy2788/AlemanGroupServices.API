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
    public class DestinationRegionsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public DestinationRegionsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/DestinationRegions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DestinationRegion>>> GetTbldestinationregions()
        {
          if (_context.Tbldestinationregions == null)
          {
              return NotFound();
          }
            return await _context.Tbldestinationregions.ToListAsync();
        }

        // GET: api/DestinationRegions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DestinationRegion>> GetDestinationRegion(int id)
        {
          if (_context.Tbldestinationregions == null)
          {
              return NotFound();
          }
            var destinationRegion = await _context.Tbldestinationregions.FindAsync(id);

            if (destinationRegion == null)
            {
                return NotFound();
            }

            return destinationRegion;
        }

        // PUT: api/DestinationRegions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestinationRegion(int id, DestinationRegion destinationRegion)
        {
            if (id != destinationRegion.Id)
            {
                return BadRequest();
            }

            _context.Entry(destinationRegion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationRegionExists(id))
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

        // POST: api/DestinationRegions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DestinationRegion>> PostDestinationRegion(DestinationRegion destinationRegion)
        {
          if (_context.Tbldestinationregions == null)
          {
              return Problem("Entity set 'MySQLDBContext.Tbldestinationregions'  is null.");
          }
            _context.Tbldestinationregions.Add(destinationRegion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDestinationRegion", new { id = destinationRegion.Id }, destinationRegion);
        }

        // DELETE: api/DestinationRegions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestinationRegion(int id)
        {
            if (_context.Tbldestinationregions == null)
            {
                return NotFound();
            }
            var destinationRegion = await _context.Tbldestinationregions.FindAsync(id);
            if (destinationRegion == null)
            {
                return NotFound();
            }

            _context.Tbldestinationregions.Remove(destinationRegion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DestinationRegionExists(int id)
        {
            return (_context.Tbldestinationregions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
