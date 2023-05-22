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
    public class TblwarehousesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblwarehousesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/Tblwarehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblwarehouse>>> GetTblwarehouses()
        {
          if (_context.Tblwarehouses == null)
          {
              return NotFound();
          }
            return await _context.Tblwarehouses.ToListAsync();
        }

        // GET: api/Tblwarehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tblwarehouse>> GetTblwarehouse(string id)
        {
          if (_context.Tblwarehouses == null)
          {
              return NotFound();
          }
            var tblwarehouse = await _context.Tblwarehouses.FindAsync(id);

            if (tblwarehouse == null)
            {
                return NotFound();
            }

            return tblwarehouse;
        }

        // PUT: api/Tblwarehouses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblwarehouse(string id, Tblwarehouse tblwarehouse)
        {
            if (id != tblwarehouse.Warehouse)
            {
                return BadRequest();
            }

            _context.Entry(tblwarehouse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblwarehouseExists(id))
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

        // POST: api/Tblwarehouses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tblwarehouse>> PostTblwarehouse(Tblwarehouse tblwarehouse)
        {
          if (_context.Tblwarehouses == null)
          {
              return Problem("Entity set 'MySQLDBContext.Tblwarehouses'  is null.");
          }
            _context.Tblwarehouses.Add(tblwarehouse);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblwarehouseExists(tblwarehouse.Warehouse))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblwarehouse", new { id = tblwarehouse.Warehouse }, tblwarehouse);
        }

        // DELETE: api/Tblwarehouses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblwarehouse(string id)
        {
            if (_context.Tblwarehouses == null)
            {
                return NotFound();
            }
            var tblwarehouse = await _context.Tblwarehouses.FindAsync(id);
            if (tblwarehouse == null)
            {
                return NotFound();
            }

            _context.Tblwarehouses.Remove(tblwarehouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblwarehouseExists(string id)
        {
            return (_context.Tblwarehouses?.Any(e => e.Warehouse == id)).GetValueOrDefault();
        }
    }
}
