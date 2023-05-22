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
    public class DigitalCountersReadsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public DigitalCountersReadsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/DigitalCountersReads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DigitalCountersReads>>> Gettbldigitalcountersreads()
        {
          if (_context.tbldigitalcountersreads == null)
          {
              return NotFound();
          }
            return await _context.tbldigitalcountersreads.ToListAsync();
        }

        // GET: api/DigitalCountersReads/5
        [HttpGet("{Date}/{counterNo}")]
        public async Task<ActionResult<DigitalCountersReads>> GetDigitalCountersReads(DateTime date, uint counterNo)
        {
          if (_context.tbldigitalcountersreads == null)
          {
              return NotFound();
          }
            var digitalCounterRead = await _context.tbldigitalcountersreads.Where(e=> e.Registeration_Date == date && e.Counter_No == counterNo).FirstOrDefaultAsync();    

            if (digitalCounterRead == null)
            {
                return NotFound();
            }

            return digitalCounterRead;
        }

        [HttpGet("{Date}")]
        public async Task<ActionResult<IEnumerable<DigitalCountersReads>>> GetDigitalCountersReads(DateTime date)
        {
          if (_context.tbldigitalcountersreads == null)
          {
              return NotFound();
          }
            var digitalCountersReads = await _context.tbldigitalcountersreads.Where(e=> e.Registeration_Date == date).ToListAsync();    

            if (digitalCountersReads == null)
            {
                return NotFound();
            }

            return digitalCountersReads;
        }

        [HttpGet("{counterNo}")]
        public async Task<ActionResult<IEnumerable<DigitalCountersReads>>> GetDigitalCountersReads(uint counterNo)
        {
          if (_context.tbldigitalcountersreads == null)
          {
              return NotFound();
          }
            var digitalCountersReads = await _context.tbldigitalcountersreads.Where(e=> e.Counter_No == counterNo).ToListAsync();    

            if (digitalCountersReads == null)
            {
                return NotFound();
            }

            return digitalCountersReads;
        }

        [HttpGet("orderedByDate")]
        public async Task<ActionResult<IEnumerable<DigitalCountersReads>>> GettbldigitalcountersreadsOrderedByDate()
        {
            if (_context.tbldigitalcountersreads == null)
            {
                return NotFound();
            }
            return await _context.tbldigitalcountersreads.OrderBy(e=> e.Registeration_Date).ToListAsync();
        }

        [HttpGet("orderedByCounterNo")]
        public async Task<ActionResult<IEnumerable<DigitalCountersReads>>> GettbldigitalcountersreadsOrderedByCounterNo()
        {
            if (_context.tbldigitalcountersreads == null)
            {
                return NotFound();
            }
            return await _context.tbldigitalcountersreads.OrderBy(e=> e.Counter_No).ToListAsync();
        }
        
        // PUT: api/DigitalCountersReads/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Date}/{counterNo}")]
        public async Task<IActionResult> PutDigitalCountersReads(DateTime date, uint counterNo, DigitalCountersReads digitalCountersReads)
        {
            if (date != digitalCountersReads.Registeration_Date || counterNo != digitalCountersReads.Counter_No)
            {
                return BadRequest();
            }

            _context.Entry(digitalCountersReads).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DigitalCountersReadsExists(date, counterNo))
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

        // POST: api/DigitalCountersReads
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DigitalCountersReads>> PostDigitalCountersReads(DigitalCountersReads digitalCountersReads)
        {
          if (_context.tbldigitalcountersreads == null)
          {
              return Problem("Entity set 'MySQLDBContext.tbldigitalcountersreads'  is null.");
          }
            _context.tbldigitalcountersreads.Add(digitalCountersReads);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DigitalCountersReadsExists(digitalCountersReads.Registeration_Date, digitalCountersReads.Counter_No))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDigitalCountersReads", new { date = digitalCountersReads.Registeration_Date , counterNo = digitalCountersReads.Counter_No }, digitalCountersReads);
        }

        [HttpPost("AddRangeOfDigitalCounterReadings")]
        public async Task<ActionResult> AddRangeOfDigitalCounterReadings([FromBody] List<DigitalCountersReads> digitalCountersReadsList)
        {
            if (_context.tbldigitalcountersreads == null)
            {
                return Problem("Entity set 'MySQLDBContext.tbldigitalcountersreads'  is null.");
            }

            foreach (var digitalCountersReads in digitalCountersReadsList)
            {
                _context.tbldigitalcountersreads.Add(digitalCountersReads);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE: api/DigitalCountersReads/5
        [HttpDelete("{Date}/{counterNo}")]
        public async Task<IActionResult> DeleteDigitalCountersReads(DateTime date, uint counterNo)
        {
            if (_context.tbldigitalcountersreads == null)
            {
                return NotFound();
            }
            var digitalCountersReads = await _context.tbldigitalcountersreads.Where(e=> e.Registeration_Date == date && e.Counter_No == counterNo).FirstOrDefaultAsync();
            if (digitalCountersReads == null)
            {
                return NotFound();
            }

            _context.tbldigitalcountersreads.Remove(digitalCountersReads);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DigitalCountersReadsExists(DateTime date, uint counterNo)
        {
            return (_context.tbldigitalcountersreads?.Any(e => e.Registeration_Date == date && e.Counter_No == counterNo)).GetValueOrDefault();
        }
    }
}
