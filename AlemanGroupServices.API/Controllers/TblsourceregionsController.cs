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
    public class TblsourceregionsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblsourceregionsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/Tblsourceregions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblsourceregion>>> GetTblsourceregions()
        {
          if (_context.Tblsourceregions == null)
          {
              return NotFound();
          }
            return await _context.Tblsourceregions.ToListAsync();
        }

        // GET: api/Tblsourceregions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tblsourceregion>> GetTblsourceregion(int id)
        {
          if (_context.Tblsourceregions == null)
          {
              return NotFound();
          }
            var tblsourceregion = await _context.Tblsourceregions.FindAsync(id);

            if (tblsourceregion == null)
            {
                return NotFound();
            }

            return tblsourceregion;
        }

        // PUT: api/Tblsourceregions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblsourceregion(int id, Tblsourceregion tblsourceregion)
        {
            if (id != tblsourceregion.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblsourceregion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblsourceregionExists(id))
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

        // POST: api/Tblsourceregions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tblsourceregion>> PostTblsourceregion(Tblsourceregion tblsourceregion)
        {
          if (_context.Tblsourceregions == null)
          {
              return Problem("Entity set 'MySQLDBContext.Tblsourceregions'  is null.");
          }
            _context.Tblsourceregions.Add(tblsourceregion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblsourceregion", new { id = tblsourceregion.Id }, tblsourceregion);
        }

        // DELETE: api/Tblsourceregions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblsourceregion(int id)
        {
            if (_context.Tblsourceregions == null)
            {
                return NotFound();
            }
            var tblsourceregion = await _context.Tblsourceregions.FindAsync(id);
            if (tblsourceregion == null)
            {
                return NotFound();
            }

            _context.Tblsourceregions.Remove(tblsourceregion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblsourceregionExists(int id)
        {
            return (_context.Tblsourceregions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
