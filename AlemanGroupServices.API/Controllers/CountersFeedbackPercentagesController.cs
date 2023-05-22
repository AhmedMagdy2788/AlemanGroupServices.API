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
    public class CountersFeedbackPercentagesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public CountersFeedbackPercentagesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/CountersFeedbackPercentages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountersFeedbackPercentage>>> Gettblcountersfeedback()
        {
          if (_context.tblcountersfeedback == null)
          {
              return NotFound();
          }
            return await _context.tblcountersfeedback.ToListAsync();
        }

        // GET: api/CountersFeedbackPercentages/5
        [HttpGet("{Date}/{counterNO}")]
        public async Task<ActionResult<CountersFeedbackPercentage>> GetCountersFeedbackPercentage(DateTime date, uint counterNO)
        {
          if (_context.tblcountersfeedback == null)
          {
              return NotFound();
          }
            var countersFeedbackPercentage = await _context.tblcountersfeedback.Where(e => e.Date == date && e.CounterNo == counterNO).FirstOrDefaultAsync();

            if (countersFeedbackPercentage == null)
            {
                return NotFound();
            }

            return countersFeedbackPercentage;
        }

        // GET: api/CountersFeedbackPercentages/5
        [HttpGet("GetCountersFeedbackPercentageAtGernerDate{Date}/{counterNO}")]
        public async Task<ActionResult<CountersFeedbackPercentage>> GetCountersFeedbackPercentageAtGernerDate(DateTime date, uint counterNO)
        {
          if (_context.tblcountersfeedback == null)
          {
              return NotFound();
          }
            var countersFeedbackPercentage = await getCountersFeedbackPercentageAtGeneralDate(date, counterNO);

            if (countersFeedbackPercentage == null)
            {
                return NotFound();
            }

            return countersFeedbackPercentage;
        }
        // GET: api/CountersFeedbackPercentages/5
        [HttpGet("{counterNO}")]
        public async Task<ActionResult<IEnumerable<CountersFeedbackPercentage>>> GetCounterFeedbackPercentages( uint counterNO)
        {
          if (_context.tblcountersfeedback == null)
          {
              return NotFound();
          }
            var countersFeedbackPercentage = await _context.tblcountersfeedback.Where(e =>e.CounterNo == counterNO).ToListAsync();

            if (countersFeedbackPercentage == null)
            {
                return NotFound();
            }

            return countersFeedbackPercentage;
        }

        // PUT: api/CountersFeedbackPercentages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Date}/{counterNO}")]
        public async Task<IActionResult> PutCountersFeedbackPercentage(DateTime date, uint counterNO, CountersFeedbackPercentage countersFeedbackPercentage)
        {
            if (date != countersFeedbackPercentage.Date || counterNO != countersFeedbackPercentage.CounterNo)
            {
                return BadRequest();
            }

            _context.Entry(countersFeedbackPercentage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountersFeedbackPercentageExists(date, counterNO))
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

        // POST: api/CountersFeedbackPercentages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountersFeedbackPercentage>> PostCountersFeedbackPercentage(CountersFeedbackPercentage countersFeedbackPercentage)
        {
          if (_context.tblcountersfeedback == null)
          {
              return Problem("Entity set 'MySQLDBContext.tblcountersfeedback'  is null.");
          }
            _context.tblcountersfeedback.Add(countersFeedbackPercentage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CountersFeedbackPercentageExists(countersFeedbackPercentage.Date, countersFeedbackPercentage.CounterNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCountersFeedbackPercentage", new { date = countersFeedbackPercentage.Date, counterNo = countersFeedbackPercentage.CounterNo }, countersFeedbackPercentage);
        }

        // DELETE: api/CountersFeedbackPercentages/5
        [HttpDelete("{Date}/{counterNO}")]
        public async Task<IActionResult> DeleteCountersFeedbackPercentage(DateTime date, uint counterNO)
        {
            if (_context.tblcountersfeedback == null)
            {
                return NotFound();
            }
            var countersFeedbackPercentage = await _context.tblcountersfeedback.Where(e=> e.Date == date && e.CounterNo == counterNO).FirstOrDefaultAsync();
            if (countersFeedbackPercentage == null)
            {
                return NotFound();
            }

            _context.tblcountersfeedback.Remove(countersFeedbackPercentage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountersFeedbackPercentageExists(DateTime date, uint counterNO)
        {
            return (_context.tblcountersfeedback?.Any(e => e.Date == date && e.CounterNo == counterNO)).GetValueOrDefault();
        }

        // this function returns the most recent CountersFeedbackPercentage object that matches the given parameters.
        private async Task<CountersFeedbackPercentage?> getCountersFeedbackPercentageAtGeneralDate(DateTime date, uint counterNo)
        {
            //First, it gets a list of all CountersFeedbackPercentage objects where the Date is less than or equal to the given Date, and the CounterNo property match the given value.
            var countersList = await _context.tblcountersfeedback.Where(c => c.Date <= date && c.CounterNo == counterNo).ToListAsync();
            //If the countersList is null, it returns null.
            if (!countersList.Any()) return null;
            //If the countersList is not null, it finds the maximum Date in the list using the Max() method.
            var maxDate = countersList.Max(c => c.Date);
            //It then finds the first object in the list where the Date matches the maxDate.
            var counterAtMaxDate = countersList.Where(c => c.Date == maxDate).FirstOrDefault();
            return counterAtMaxDate;
        }
    }
}
