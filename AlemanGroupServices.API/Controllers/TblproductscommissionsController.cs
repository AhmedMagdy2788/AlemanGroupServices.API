using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblproductscommissionsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblproductscommissionsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/Tblproductscommissions
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblproductscommission>>> GetTblProductCommissions()
        {
          if (_context.Tblproductscommission == null)
          {
              return NotFound();
          }
            return await _context.Tblproductscommission.ToListAsync();
        }

        [HttpGet("GetWithName")]
        public async Task<ActionResult<IEnumerable<ProductsCommissionDto>>> GetTblProductCommissionsWithName()
        {
            if (_context.Tblproductscommission == null)
            {
                return NotFound();
            }
            return await _context.Tblproductscommission
                .Join(
                _context.TblProducts,
                product_commission=> product_commission.Product_Id,
                product=> product.Id,
                (product_commission, product) => new ProductsCommissionDto 
                {
                    Date = product_commission.Date,
                    Product_Name = product.Product_Name,
                    Product_Commission = product_commission.Produc_Commission
                }
                )
                .ToListAsync();
        }

        [HttpGet("Get_product_commissions_with_product_name{Product_Name}")]
        public async Task<ActionResult<IEnumerable<ProductsCommissionDto>>> Get_product_commissions_with_product_name(string product_name)
        {
            if (_context.Tblproductscommission == null)
            {
                return NotFound();
            }
            var product = await _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefaultAsync();
            if (product == null) { return NotFound($"there is no product with Name '{product_name}'"); }
            return await _context.Tblproductscommission
                .Where(p => p.Product_Id == product.Id)
                .Join(
                _context.TblProducts,
                product_commission => product_commission.Product_Id,
                product => product.Id,
                (product_commission, product) => new ProductsCommissionDto
                {
                    Date = product_commission.Date,
                    Product_Name = product.Product_Name,
                    Product_Commission = product_commission.Produc_Commission
                }
                )
                .ToListAsync();
        }

        // GET: api/Tblproductscommissions/5
        [HttpGet("Get_With_Ids{Date}/{product_id}")]
        public async Task<ActionResult<Tblproductscommission>> GetTblproductscommission(DateTime date, int product_id)
        {
          if (_context.Tblproductscommission == null)
          {
              return NotFound();
          }
            var tblproductscommission = await _context.Tblproductscommission.Where(e => e.Date == date && e.Product_Id == product_id).FirstOrDefaultAsync();

            if (tblproductscommission == null)
            {
                return NotFound();
            }

            return tblproductscommission;
        }

        [HttpGet("get_With_Product_Name_At_Date{Date}/{Product_Name}")]
        public async Task<ActionResult<Tblproductscommission>> GetProductsCommissionWithNameAtDate(DateTime date, string product_name)
        {
            if (_context.Tblproductscommission == null)
            {
                return NotFound();
            }
            var product = await _context.TblProducts.Where(p=> p.Product_Name == product_name).FirstOrDefaultAsync(); 
            if (product == null) { return  NotFound($"there is no product with Name '{product_name}'"); }
            DateTime? maxDate = null;
            var products = _context.Tblproductscommission
                .Where(p => p.Date <= date && p.Product_Id == product.Id);
            if (products.Any())
            {
                maxDate = products.Max(p => p.Date);
            }
            if (maxDate == null)
            {
                return NotFound($"there is no commission recored stored for product '{product_name}' before this Date '{date}'");
            }
            var tblproductscommission = await _context.Tblproductscommission
                .Where(e => e.Date == maxDate && e.Product_Id == product.Id)
                .FirstOrDefaultAsync();

            if (tblproductscommission == null)
            {
                return NotFound();
            }

            return tblproductscommission;
        }

        // PUT: api/Tblproductscommissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update_wiht_ids{Date}/{product_id}")]
        public async Task<IActionResult> PutTblproductscommission(DateTime date, int product_id, Tblproductscommission tblproductscommission)
        {
            if (date != tblproductscommission.Date && product_id != tblproductscommission.Product_Id)
            {
                return BadRequest();
            }

            _context.Entry(tblproductscommission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblproductscommissionExists(date, product_id))
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

        [HttpPut("update_with_product_name{Date}/{Product_Name}")]
        public async Task<IActionResult> PutTblproductscommission(DateTime date, string product_name, ProductsCommissionDto tblproductscommissionDto)
        {
            if (date != tblproductscommissionDto.Date && product_name != tblproductscommissionDto.Product_Name)
            {
                return BadRequest();
            }
            var product = await _context.TblProducts.
                Where(p => p.Product_Name == product_name)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($"there is no product with Name '{tblproductscommissionDto.Product_Name}'");
            }

            _context.Entry(new Tblproductscommission 
                { 
                    Date = tblproductscommissionDto.Date,
                    Product_Id = product.Id,
                    Produc_Commission = tblproductscommissionDto.Product_Commission
                })
                .State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblproductscommissionExists(date, product.Id))
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

        // POST: api/Tblproductscommissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tblproductscommission>> PostTblproductscommission(Tblproductscommission tblproductscommission)
        {
          if (_context.Tblproductscommission == null)
          {
              return Problem("Entity set 'MySQLDBContext.TblProductCommissions'  is null.");
          }
            _context.Tblproductscommission.Add(tblproductscommission);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductscommissionExists(tblproductscommission.Date, tblproductscommission.Product_Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblproductscommission", new { id = tblproductscommission.Date }, tblproductscommission);
        }

        [HttpPost("add_commission_with_product_name")]
        public async Task<ActionResult<ProductsCommissionDto>> PostTblproductscommissionDto(ProductsCommissionDto tblproductscommissionDto)
        {
          if (_context.Tblproductscommission == null)
          {
              return Problem("Entity set 'MySQLDBContext.TblProductCommissions'  is null.");
          }
          var product = await _context.TblProducts.
                Where(p=> p.Product_Name == tblproductscommissionDto.Product_Name)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($"there is no product with Name '{tblproductscommissionDto.Product_Name}'");
            }

            var tblproductscommission = new Tblproductscommission
            {
                Date = tblproductscommissionDto.Date,
                Product_Id = product.Id,
                Produc_Commission = tblproductscommissionDto.Product_Commission
            };

            _context.Tblproductscommission.Add(tblproductscommission) ;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductscommissionExists(tblproductscommissionDto.Date, product.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblproductscommission", new { date = tblproductscommission.Date , product_id = product.Id}, tblproductscommission);
        }

        // DELETE: api/Tblproductscommissions/5
        //
        [HttpDelete("{Date}/{product_id}")]
        public async Task<IActionResult> DeleteTblproductscommission(DateTime date, int product_id)
        {
            if (_context.Tblproductscommission == null)
            {
                return NotFound();
            }
            var tblproductscommission = await _context.Tblproductscommission.Where(e=> e.Date == date && e.Product_Id == product_id).FirstOrDefaultAsync();
            if (tblproductscommission == null)
            {
                return NotFound();
            }

            _context.Tblproductscommission.Remove(tblproductscommission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete_with_product_name_and_date{Date}/{Product_Name}")]
        public async Task<IActionResult> DeleteTblproductscommissionWithProductName(DateTime date, string product_name)
        {
            if (_context.Tblproductscommission == null)
            {
                return NotFound();
            }
            var product = await _context.TblProducts.
                Where(p => p.Product_Name == product_name)
                .FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($"there is no product with Name '{product_name}'");
            }
            var tblproductscommission = await _context.Tblproductscommission.Where(e => e.Date == date && e.Product_Id == product.Id).FirstOrDefaultAsync();
            if (tblproductscommission == null)
            {
                return NotFound();
            }

            _context.Tblproductscommission.Remove(tblproductscommission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblproductscommissionExists(DateTime date, int product_id)
        {
            return (_context.Tblproductscommission?.Any(e => e.Date == date && e.Product_Id == product_id)).GetValueOrDefault();
        }
    }
}
