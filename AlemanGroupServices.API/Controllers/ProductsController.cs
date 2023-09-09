using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public ProductsController(MySQLDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: api/TblproductsWithCategoryName
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTblProductsWithCategoryName")]
        public async Task<ActionResult<IEnumerable<ProductWithCategoryNameDTO>>> GetTblProductsWithCategoryName()
        {
            if (_context.TblProducts == null)
            {
                return NotFound();
            }
            var result = await _context.TblProducts
                .Join(_context.Tblmainproducts,
                    p => p.Category_Id,
                    c => c.CategoryId,
                    (p, c) => new ProductWithCategoryNameDTO
                    {
                        Id = p.Id,
                        Product_Name = p.Product_Name,
                        Products_Category = c.Products_Category
                    })
                .ToListAsync();
            return result;
        }

        [HttpGet("GetTblProductsWithCategoryId")]
        public async Task<ActionResult<IEnumerable<PureProductDto>>> GetTblProductsWithCategoryId()
        {
            if (_context.TblProducts == null)
            {
                return NotFound();
            }
            return await _context.TblProducts.Select(p => new PureProductDto
            {
                Id = p.Id,
                Product_Name = p.Product_Name,
                Category_Id = p.Category_Id
            }).ToListAsync();
        }

        [HttpGet("Get_Products_By_Category_name{category_name}")]
        public async Task<ActionResult<IEnumerable<Tblproduct>>> GetTblProductsWithBuyPrice(string category_name)
        {
            if (_context.TblProducts == null)
            {
                return NotFound();
            }
            var category = _context.Tblmainproducts.FirstOrDefault(c=> c.Products_Category == category_name);
            if(category == null)
            {
                return NotFound($"there is no category Name like this '{category_name}'");
            }
            var products = await _context.TblProducts
                .Where(p => p.Category_Id == category.CategoryId)
                .ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tblproduct>> GetTblproduct(int id)
        {
          if (_context.TblProducts == null)
          {
              return NotFound();
          }
            var tblproduct = await _context.TblProducts.FindAsync(id);

            if (tblproduct == null)
            {
                return NotFound();
            }

            return tblproduct;
        }

        [HttpGet("GetProductByIdWithCategoryName{id}")]
        public async Task<ActionResult<ProductWithCategoryNameDTO>> GetProductByIdWithCategoryName(int id)
        {
            if (_context.TblProducts == null)
            {
                return NotFound();
            }
            var tblproduct = await _context.TblProducts.FindAsync(id);

            if (tblproduct == null)
            {
                return NotFound($"there is no product with id = '{id}'");
            }
            var category = await _context.Tblmainproducts.FindAsync(tblproduct.Category_Id);

            if(category == null)
            {
                return NotFound("there is not category with id = 'id'");
            }


            var productWithCategoryName = new ProductWithCategoryNameDTO
            {
                Id = tblproduct.Id,
                Product_Name = tblproduct.Product_Name,
                Products_Category = category.Products_Category
            };

            return productWithCategoryName;
        }

        /// <summary>
        /// PUT: api/Tblproducts/5
        /// </summary>
        /// <param Name="id"></param>
        /// <param Name="tblproduct"></param>
        /// <returns></returns>
       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblproduct(int id, Tblproduct tblproduct)
        {
            if (id != tblproduct.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblproduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblproductExists(id))
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

        /// <summary>
        /// POST: api/Tblproducts
        /// </summary>
        /// <param Name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Tblproduct>> PostTblproduct(PureProductDto product)
        {
          if (_context.TblProducts == null)
          {
              return Problem("Entity set 'MySQLDBContext.TblProducts'  is null.");
          }
          Tblproduct tblproduct = new Tblproduct 
          { 
              Id = product.Id , 
              Category_Id = product.Category_Id, 
              Product_Name = product.Product_Name
          };
            _context.TblProducts.Add(tblproduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblproduct", new { id = tblproduct.Id }, tblproduct);
        }

        [HttpPost("Add_Product_With_Category_Name")]
        public async Task<ActionResult<ProductWithCategoryNameDTO>> PostTblProductWithCategoryName(ProductWithCategoryNameDTO tblproductDTO)
        {
            // Find the Tblmainproduct with the matching Products_Category
            var tblmainproduct = await _context.Tblmainproducts
                .FirstOrDefaultAsync(c => c.Products_Category == tblproductDTO.Products_Category);
            
            // If no matching Tblmainproduct is found, return an error
            if (tblmainproduct == null)
            {
                return BadRequest("Invalid Products_Category");
            }

            // Create a new Tblproduct and set its properties
            var tblproduct = new Tblproduct
            {
                Product_Name = tblproductDTO.Product_Name,
                Category_Id = tblmainproduct.CategoryId
            };

            // Add the new Tblproduct to the database
            _context.TblProducts.Add(tblproduct);
            await _context.SaveChangesAsync();
            tblproductDTO.Id = tblproduct.Id;
            // Return the newly created Tblproduct
            return CreatedAtAction("GetProductByIdWithCategoryName", new { id = tblproduct.Id }, tblproductDTO);
        }

        /// <summary>
        /// DELETE: api/Tblproducts/5
        /// </summary>
        /// <param Name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblproduct(int id)
        {
            if (_context.TblProducts == null)
            {
                return NotFound();
            }
            var tblproduct = await _context.TblProducts.FindAsync(id);
            if (tblproduct == null)
            {
                return NotFound();
            }

            _context.TblProducts.Remove(tblproduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblproductExists(int id)
        {
            return (_context.TblProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
