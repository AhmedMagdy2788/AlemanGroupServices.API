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
    public class StationsCountersController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public StationsCountersController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/StationsCounters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationsCounter>>> Gettblstationscounters()
        {
          if (_context.tblstationscounters == null)
          {
              return NotFound();
          }
            return await _context.tblstationscounters.ToListAsync();
        }

        // GET: api/StationsCounters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StationsCounter>> GetStationsCounter(uint id)
        {
          if (_context.tblstationscounters == null)
          {
              return NotFound();
          }
            var stationsCounter = await _context.tblstationscounters.FindAsync(id);

            if (stationsCounter == null)
            {
                return NotFound();
            }

            return stationsCounter;
        }

        // PUT: api/StationsCounters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStationsCounter(uint id, StationsCounter stationsCounter)
        {
            if (id != stationsCounter.CounterNo)
            {
                return BadRequest();
            }

            _context.Entry(stationsCounter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationsCounterExists(id))
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

        // POST: api/StationsCounters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StationsCounter>> PostStationsCounter(StationsCounter stationsCounter)
        {
          if (_context.tblstationscounters == null)
          {
              return Problem("Entity set 'MySQLDBContext.tblstationscounters'  is null.");
          }
            _context.tblstationscounters.Add(stationsCounter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStationsCounter", new { id = stationsCounter.CounterNo }, stationsCounter);
        }

        // DELETE: api/StationsCounters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStationsCounter(uint id)
        {
            if (_context.tblstationscounters == null)
            {
                return NotFound();
            }
            var stationsCounter = await _context.tblstationscounters.FindAsync(id);
            if (stationsCounter == null)
            {
                return NotFound();
            }

            _context.tblstationscounters.Remove(stationsCounter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StationsCounterExists(uint id)
        {
            return (_context.tblstationscounters?.Any(e => e.CounterNo == id)).GetValueOrDefault();
        }
    }
}
