using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountersPumpsDetailsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public CountersPumpsDetailsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/CountersPumpsDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountersPumpsDetail>>> Gettblcounterspumpsdetails()
        {
            if (_context.tblcounterspumpsdetails == null)
            {
                return NotFound();
            }
            return await _context.tblcounterspumpsdetails.ToListAsync();
        }

        // GET: api/CountersPumpsDetails/5
        [HttpGet("{Date}/{pumpNo}/{nozelNo}")]
        public async Task<ActionResult<CountersPumpsDetail>> GetCountersPumpsDetail(DateTime date, uint pumpNo, int nozelNo)
        {
            if (_context.tblcounterspumpsdetails == null)
            {
                return NotFound();
            }
            var countersPumpsDetail = await _context.tblcounterspumpsdetails.Where(e => e.Date == date && e.PumpNo == pumpNo && e.NozelNo == nozelNo).FirstOrDefaultAsync();

            if (countersPumpsDetail == null)
            {
                return NotFound();
            }

            return countersPumpsDetail;
        }

        [HttpGet("GetPumpCountersAtGenralDate{Date}/{pumpNo}")]
        public async Task<ActionResult<IEnumerable<CountersPumpsDetail>>> GetPumpCountersAtGenralDate(DateTime date, uint pumpNo)
        {
            if (_context.tblcounterspumpsdetails == null)
            {
                return NotFound();
            }
            var pumpsNozels = await _context.tblpumpsnozels.Where(n => n.PumpNo == pumpNo).ToListAsync();
            if (!pumpsNozels.Any()) return NotFound($"there are no Nozels installed on pump no '{pumpNo}'");
            List<CountersPumpsDetail> pumpcounters = new List<CountersPumpsDetail>();
            foreach (var nozel in pumpsNozels)
            {
                Console.WriteLine($"get counters for pump no '{pumpNo}' , nozel no '{nozel.NozelNo} at Date '{date}' .. ######################################################");
                var countersPumpsDetail = await getPumpscountersAtGeneralDate(date, pumpNo, nozel.NozelNo);
                if (countersPumpsDetail != null) pumpcounters.Add(countersPumpsDetail); 
            }

            if (!pumpcounters.Any())
            {
                return NotFound($"There are no counters installed on pump no '{pumpNo}' ");
            }

            return pumpcounters;
        }

        // PUT: api/CountersPumpsDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Date}/{pumpNo}/{nozelNo}")]
        public async Task<IActionResult> PutCountersPumpsDetail(DateTime date, uint pumpNo, int nozelNo, CountersPumpsDetail countersPumpsDetail)
        {
            if (date != countersPumpsDetail.Date && pumpNo != countersPumpsDetail.PumpNo && nozelNo != countersPumpsDetail.NozelNo)
            {
                return BadRequest();
            }

            _context.Entry(countersPumpsDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountersPumpsDetailExists(date, pumpNo, nozelNo))
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

        // POST: api/CountersPumpsDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountersPumpsDetail>> PostCountersPumpsDetail(CountersPumpsDetail countersPumpsDetail)
        {
            if (_context.tblcounterspumpsdetails == null)
            {
                return Problem("Entity set 'MySQLDBContext.tblcounterspumpsdetails'  is null.");
            }
            _context.tblcounterspumpsdetails.Add(countersPumpsDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CountersPumpsDetailExists(countersPumpsDetail.Date, countersPumpsDetail.PumpNo, countersPumpsDetail.NozelNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCountersPumpsDetail", new
            {
                date = countersPumpsDetail.Date,
                pumpNo = countersPumpsDetail.PumpNo,
                nozelNo = countersPumpsDetail.NozelNo
            }, countersPumpsDetail);
        }

        // DELETE: api/CountersPumpsDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountersPumpsDetail(DateTime date, uint pumpNo, int nozelNo)
        {
            if (_context.tblcounterspumpsdetails == null)
            {
                return NotFound();
            }
            var countersPumpsDetail = await _context.tblcounterspumpsdetails.Where(e => e.Date == date && e.PumpNo == pumpNo && e.NozelNo == nozelNo).FirstOrDefaultAsync();
            if (countersPumpsDetail == null)
            {
                return NotFound();
            }

            _context.tblcounterspumpsdetails.Remove(countersPumpsDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // this function returns the most recent CountersPumpsDetail object that matches the given parameters.
        private async Task<CountersPumpsDetail?> getPumpscountersAtGeneralDate(DateTime date, uint pumpNo, int nozelNo)
        {
            //First, it gets a list of all CountersPumpsDetail objects where the Date is less than or equal to the given Date, and the PumpNo and NozelNo properties match the given values.
            var countersList = await _context.tblcounterspumpsdetails.Where(c => c.Date <= date && c.PumpNo == pumpNo && c.NozelNo == nozelNo).ToListAsync();
            //If the countersList is null, it returns null.
            if (!countersList.Any()) return null;
            //If the countersList is not null, it finds the maximum Date in the list using the Max() method.
            var maxDate = countersList.Max(c => c.Date);
            //It then finds the first object in the list where the Date matches the maxDate.
            var counterAtMaxDate = countersList.Where(c => c.Date == maxDate).FirstOrDefault();
            //It get all the counter installation on all pumps
            var counterInstallationList = await _context.tblcounterspumpsdetails.Where(c => c.CounterNo == counterAtMaxDate!.CounterNo).ToListAsync();
            //it get the Date of last installation on or befor the given Date for the counter
            var maxInstallationDate = counterInstallationList.Max(c => c.Date);
            //It then finds the last counter installation that occurred on the maximum Date
            var lastCounterInstallation = counterInstallationList.Where(c => c.Date == maxInstallationDate).FirstOrDefault();
            //checks if the last counter installation is belongs to the specified pump number.
            if (lastCounterInstallation!.PumpNo != pumpNo) return null;
            return counterAtMaxDate;
        }

        private bool CountersPumpsDetailExists(DateTime date, uint pumpNo, int nozelNo)
        {
            return (_context.tblcounterspumpsdetails?.Any(e => e.Date == date && e.PumpNo == pumpNo && e.NozelNo == nozelNo)).GetValueOrDefault();
        }
    }
}
