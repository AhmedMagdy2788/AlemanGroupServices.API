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
    public class PumpsNozelsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public PumpsNozelsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/PumpsNozels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PumpsNozels>>> Gettblpumpsnozels()
        {
          if (_context.tblpumpsnozels == null)
          {
              return NotFound();
          }
            return await _context.tblpumpsnozels.ToListAsync();
        }

        // GET: api/PumpsNozels/5
        [HttpGet("{pumpNo}")]
        public async Task<ActionResult<IEnumerable<PumpsNozels>>> GetPumpsNozels(uint pumpNo)
        {
          if (_context.tblpumpsnozels == null)
          {
              return NotFound();
          }
            var pumpsNozels = await _context.tblpumpsnozels.Where(e=> e.PumpNo == pumpNo).ToListAsync();

            if (pumpsNozels == null)
            {
                return NotFound();
            }

            return pumpsNozels;
        }

        [HttpGet("GetPumpNozelById{pumpNo}/{nozelNo}")]
        public async Task<ActionResult<PumpsNozels>> GetPumpNozelById(uint pumpNo, int nozelNo)
        {
            if (_context.tblpumpsnozels == null)
            {
                return NotFound();
            }
            var pumpsNozel = await _context.tblpumpsnozels.Where(e => e.PumpNo == pumpNo && e.NozelNo == nozelNo).FirstOrDefaultAsync();

            if (pumpsNozel == null)
            {
                return NotFound();
            }

            return pumpsNozel;
        }

        // PUT: api/PumpsNozels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPumpsNozels(uint id, PumpsNozels pumpsNozels)
        //{
        //    if (id != pumpsNozels.PumpNo)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(pumpsNozels).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PumpsNozelsExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/PumpsNozels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PumpsNozels>> PostPumpsNozels(PumpsNozels pumpsNozels)
        {
          if (_context.tblpumpsnozels == null)
          {
              return Problem("Entity set 'MySQLDBContext.tblpumpsnozels'  is null.");
          }
            _context.tblpumpsnozels.Add(pumpsNozels);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PumpsNozelsExists(pumpsNozels.PumpNo, pumpsNozels.NozelNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetPumpNozelById", new { pumpNo = pumpsNozels.PumpNo, nozelNo = pumpsNozels.NozelNo }, pumpsNozels);
        }

        // DELETE: api/PumpsNozels/5
        [HttpDelete("{pumpNo}/{NozelNo}")]
        public async Task<IActionResult> DeletePumpsNozels(uint pumpNo, int nozelNo)
        {
            if (_context.tblpumpsnozels == null)
            {
                return NotFound();
            }
            var pumpsNozels = await _context.tblpumpsnozels.Where(e=> e.PumpNo == pumpNo && e.NozelNo == nozelNo).FirstOrDefaultAsync();
            if (pumpsNozels == null)
            {
                return NotFound();
            }

            _context.tblpumpsnozels.Remove(pumpsNozels);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PumpsNozelsExists(uint pumpNo , int nozelNo)
        {
            return (_context.tblpumpsnozels?.Any(e => e.PumpNo == pumpNo && e.NozelNo == nozelNo)).GetValueOrDefault();
        }
    }
}
