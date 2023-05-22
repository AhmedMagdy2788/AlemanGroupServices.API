using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsPumpsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public StationsPumpsController(MySQLDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationsPumps>>> Gettblstationspumps()
        {
          if (_context.tblstationspumps == null)
          {
              return NotFound();
          }
            return await _context.tblstationspumps.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StationsPumps>> GetTblstationspump(uint id)
        {
            if (_context.tblstationspumps == null)
            {
                return NotFound();
            }

            var tblstationspump = await _context.tblstationspumps.FindAsync(id);

            if (tblstationspump == null)
            {
                return NotFound();
            }

            return tblstationspump;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblstationspump(uint id, StationsPumps tblstationspump)
        {
            if (id != tblstationspump.PumpNo)
            {
                return BadRequest();
            }

            _context.Entry(tblstationspump).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblstationspumpExists(id))
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

        [HttpPost]
        public async Task<ActionResult<StationsPumps>> PostTblstationspump(StationsPumps tblstationspump)
        {
            if (_context.tblstationspumps == null)
            {
                return Problem("Entity set 'MySQLDBContext.tblstationspumps'  is null.");
            }
            _context.tblstationspumps.Add(tblstationspump);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblstationspump", new { id = tblstationspump.PumpNo }, tblstationspump);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblstationspump(uint id)
        {
            if (_context.tblstationspumps == null)
            {
                return NotFound();
            }
            var tblstationspump = await _context.tblstationspumps.FindAsync(id);
            if (tblstationspump == null)
            {
                return NotFound();
            }

            _context.tblstationspumps.Remove(tblstationspump);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblstationspumpExists(uint id)
        {
            return (_context.tblstationspumps?.Any(e => e.PumpNo == id)).GetValueOrDefault();
        }
    }
}
