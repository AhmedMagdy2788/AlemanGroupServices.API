using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblwarehousecostsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblwarehousecostsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/Tblwarehousecosts
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblwarehousecost>>> GetTblwarehousecosts()
        {
          if (_context.Tblwarehousecosts == null)
          {
              return NotFound();
          }
            return await _context.Tblwarehousecosts.ToListAsync();
        }

        [HttpGet("Get_warehouse_costs_with_products_name")]
        public async Task<ActionResult<IEnumerable<WarehouseCostDto>>> Get_warehouse_costs_with_products_name()
        {
            if (_context.Tblwarehousecosts == null)
            {
                return NotFound();
            }
            return await _context.Tblwarehousecosts
                .Join(
                _context.TblProducts,
                warehouse_cost => warehouse_cost.Product_Id,
                product => product.Id,
                (warehouse_cost, product) => new WarehouseCostDto
                {
                    Date = warehouse_cost.Date,
                    Product_Name = product.Product_Name,
                    Warehouse = warehouse_cost.Warehouse,
                    Warehouse_Expenses = warehouse_cost.Warehouse_Expenses,
                    Expenses_On_Customer = warehouse_cost.Expenses_On_Customer
                }
                )
                .ToListAsync();
        }

        [HttpGet("Get_by_ids{Date}/{product_id}/{warhouse}")]
        public async Task<ActionResult<Tblwarehousecost>> GetTblwarehousecostByIds(DateTime date, int product_id, string warehouse)
        {
          if (_context.Tblwarehousecosts == null)
          {
              return NotFound();
          }
            var tblwarehousecost = await _context.Tblwarehousecosts.Where(p=> p.Date == date && p.Product_Id == product_id && p.Warehouse == warehouse).FirstOrDefaultAsync();

            if (tblwarehousecost == null)
            {
                return NotFound();
            }

            return tblwarehousecost;
        }

        [HttpGet("Get_at_date_with_ids{Date}/{Product_Name}/{warehouse}")]
        public async Task<ActionResult<WarehouseCostDto>> GetTblwarehousecostAtDateWithIds(DateTime date, string product_name, string warehouse)
        {
            if (_context.Tblwarehousecosts == null)
            {
                return NotFound();
            }

            var product = await _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefaultAsync();
            if (product == null) 
            { 
                return NotFound($"there is no product with name '{product_name}'"); 
            }

            DateTime? maxDate = null;
            var products = _context.Tblwarehousecosts
                .Where(p => p.Date <= date && p.Product_Id == product.Id && p.Warehouse == warehouse);
            if (products.Any())
            {
                maxDate = products.Max(p => p.Date);
            }
            if (maxDate == null)
            {
                return NotFound($"there is no warehouse costs recored stored for product '{product_name}' before this Date '{date}'");
            }

            var tblwarehousecost = await _context.Tblwarehousecosts.Where(p => p.Date == maxDate && p.Product_Id == product.Id && p.Warehouse == warehouse).FirstOrDefaultAsync();

            if (tblwarehousecost == null)
            {
                return NotFound();
            }

            return new WarehouseCostDto 
            { 
                Date = tblwarehousecost.Date,
                Product_Name = product_name,
                Warehouse = warehouse,
                Warehouse_Expenses = tblwarehousecost.Warehouse_Expenses,
                Expenses_On_Customer = tblwarehousecost.Expenses_On_Customer
            };
        }

        [HttpGet("Get_For_prduct_and_warhouse{Product_Name}/{warehouse}")]
        public async Task<ActionResult<IEnumerable<Tblwarehousecost>>> Get_For_prduct_and_warhouse( string product_name, string warehouse)
        {
            if (_context.Tblwarehousecosts == null)
            {
                return NotFound();
            }

            var product = await _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($"there is no product with name '{product_name}'");
            }

            var tblwarehousecost = await _context.Tblwarehousecosts.Where(p =>  p.Product_Id == product.Id && p.Warehouse == warehouse).ToListAsync();

            if (tblwarehousecost == null)
            {
                return NotFound();
            }

            return tblwarehousecost;
        }

        // PUT: api/Tblwarehousecosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Date}/{product_id}/{warehouse}")]
        public async Task<IActionResult> PutTblwarehousecost(DateTime date, int product_id, string warehouse, Tblwarehousecost tblwarehousecost)
        {
            if (date != tblwarehousecost.Date || product_id != tblwarehousecost.Product_Id || warehouse != tblwarehousecost.Warehouse)
            {
                return BadRequest();
            }

            _context.Entry(tblwarehousecost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(TblwarehousecostExists(tblwarehousecost.Date, tblwarehousecost.Product_Id, tblwarehousecost.Warehouse))
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

        // POST: api/Tblwarehousecosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tblwarehousecost>> PostTblwarehousecost(WarehouseCostDto warehouseCostDto)
        {
          if (_context.Tblwarehousecosts == null)
          {
              return Problem("Entity set 'MySQLDBContext.Tblwarehousecosts'  is null.");
          }
          var product = await _context.TblProducts.Where(p=> p.Product_Name == warehouseCostDto.Product_Name).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($"there is no product exist with name '{warehouseCostDto.Product_Name}'");
            }
            var tblwarehousecost = new Tblwarehousecost 
            { 
                Date = warehouseCostDto.Date,
                Product_Id = product.Id,
                Warehouse = warehouseCostDto.Warehouse,
                Warehouse_Expenses = warehouseCostDto.Warehouse_Expenses,
                Expenses_On_Customer = warehouseCostDto.Expenses_On_Customer
            };
            _context.Tblwarehousecosts.Add(tblwarehousecost);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblwarehousecostExists(tblwarehousecost.Date, tblwarehousecost.Product_Id, tblwarehousecost.Warehouse))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblwarehousecost", new { date = tblwarehousecost.Date, product_id = tblwarehousecost.Product_Id, warehouse = tblwarehousecost.Warehouse }, tblwarehousecost);
        }

        // DELETE: api/Tblwarehousecosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblwarehousecost(DateTime date, int product_id, string warehouse)
        {
            if (_context.Tblwarehousecosts == null)
            {
                return NotFound();
            }
            var tblwarehousecost = await _context.Tblwarehousecosts.Where(wc=> wc.Date == date && wc.Product_Id == product_id && wc.Warehouse == warehouse).FirstOrDefaultAsync();
            if (tblwarehousecost == null)
            {
                return NotFound();
            }

            _context.Tblwarehousecosts.Remove(tblwarehousecost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblwarehousecostExists(DateTime date, int product_id, string warehouse)
        {
            return (_context.Tblwarehousecosts?.Any(e => e.Date == date && e.Product_Id == product_id && e.Warehouse == warehouse)).GetValueOrDefault();
        }
    }
}
