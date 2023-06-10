using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MainproductsController : ControllerBase
{
    private readonly MySQLDBContext _context;

    public MainproductsController(MySQLDBContext context)
    {
        _context = context;
    }

    // GET: api/Tblmainproducts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tblmainproduct>>> Gettblmainproducts()
    {
      if (_context.Tblmainproducts == null)
      {
          return NotFound();
      }
        return await _context.Tblmainproducts.ToListAsync();
    }

    // GET: api/Tblmainproducts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tblmainproduct>> GetTblmainproduct(int id)
    {
        if (_context.Tblmainproducts == null)
        {
            return NotFound();
        }
        var tblmainproduct = await _context.Tblmainproducts.FindAsync(id);

        if (tblmainproduct == null)
        {
            return NotFound();
        }
        return tblmainproduct;
    }

    // PUT: api/Tblmainproducts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTblmainproduct(int id, Tblmainproduct tblmainproduct)
    {
        if (id != tblmainproduct.CategoryId)
        {
            return BadRequest();
        }

        _context.Entry(tblmainproduct).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TblmainproductExists(id))
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

    // POST: api/Tblmainproducts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Tblmainproduct>> PostTblmainproduct(Tblmainproduct tblmainproduct)
    {
        if (_context.Tblmainproducts == null)
        {
            return Problem("Entity set 'MySQLDBContext.Tblmainproducts'  is null.");
        }
        _context.Tblmainproducts.Add(tblmainproduct);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTblmainproduct", new { id = tblmainproduct.CategoryId }, tblmainproduct);
    }

    // DELETE: api/Tblmainproducts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTblmainproduct(int id)
    {
        if (_context.Tblmainproducts == null)
        {
            return NotFound();
        }
        var tblmainproduct = await _context.Tblmainproducts.FindAsync(id);
        if (tblmainproduct == null)
        {
            return NotFound();
        }
        _context.Tblmainproducts.Remove(tblmainproduct);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TblmainproductExists(int id)
    {
        return (_context.Tblmainproducts?.Any(e => e.CategoryId == id)).GetValueOrDefault();
    }
}
