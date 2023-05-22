using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblproductsbuypricesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblproductsbuypricesController(MySQLDBContext context)
        {
            _context = context;
        }

        #region Get Products Purchase Prices
        /// <summary>
        /// Get Products Purchase Prices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductsPurchasePriceDto>>> GetTblProductsBuyPrice()
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return NotFound();
            }
            return await _context.TblProductsBuyPrice.Select(p => new ProductsPurchasePriceDto
            {
                Date = p.Date,
                Product_Id = p.Product_Id,
                Product_Purchase_Price = p.Product_Purchase_Price
            }).ToListAsync();
        }

        [HttpGet("ProductsPurchasePricesWithProductName")]
        public async Task<ActionResult<IEnumerable<ProductsPurchasePriceWithProductNameDto>>> ProductsPurchasePricesWithProductName()
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return NotFound();
            }
            return await _context.TblProductsBuyPrice.Join(_context.TblProducts,
                    PurchasePrice => PurchasePrice.Product_Id,
                    product => product.Id,
                    (PurchasePrice, product) => new ProductsPurchasePriceWithProductNameDto
                    {
                        Date = PurchasePrice.Date,
                        Product_Name = product.Product_Name,
                        Product_Purchase_Price = PurchasePrice.Product_Purchase_Price
                    }).ToListAsync();
        }

        [HttpGet("GetByPrimaryKeys{Date}/{product_id}")]
        public IActionResult GetByPrimaryKeys(DateTime date, int product_id)
        {
            // Find the Tblproductsbuyprices with the specified Date and Product_Id
            var tblproductsbuyprices = _context.TblProductsBuyPrice
                                               .Find(date, product_id);

            // If the Tblproductsbuyprices was not found, return a 404 Not Found response
            if (tblproductsbuyprices == null)
            {
                return NotFound();
            }

            // Return the Tblproductsbuyprices
            return Ok(tblproductsbuyprices);
        }
        
        [HttpGet("GetByDateAndProductName{Date}/{Product_Name}")]
        public ActionResult<ProductsPurchasePriceWithProductNameDto> GetByDateAndProductName(DateTime date, string product_name)
        {

            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == product_name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this name '{product_name}'");
            }

            // Find the Tblproductsbuyprices with the specified Date and Product_Id
            var tblproductsbuyprices = _context.TblProductsBuyPrice
                                               .Find(date, product.Id);

            // If the Tblproductsbuyprices was not found, return a 404 Not Found response
            if (tblproductsbuyprices == null)
            {
                return NotFound();
            }
            var result = new ProductsPurchasePriceWithProductNameDto 
            {
                Product_Name = product_name,
                Date = date,
                Product_Purchase_Price= tblproductsbuyprices.Product_Purchase_Price,
            };
            // Return the Tblproductsbuyprices
            return Ok(result);
        }

        [HttpGet("getPriceAtDate{Date}/{Product_Name}")]
        public IActionResult GetTblproductsbuypricesAtDate(DateTime date, string product_name)
        {
            Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == product_name)
                       .FirstOrDefault();
            if (product == null)
                return NotFound($"there is no product with this name '{product_name}'");
            Console.WriteLine($"{product.Product_Name} is found with id = {product.Id}");
            // Find the maximum Date that is equal to or before the specified Date
            DateTime maxDate = _context.TblProductsBuyPrice
                                  .Where(p => p.Date <= date && p.Product_Id == product.Id)
                                  .Max(p => p.Date);
            Console.WriteLine($"the Date that equivilant for Date '{date}' is '{maxDate}'");
            // Find the Tblproductsbuyprices with Product_Id equal to 1 and Date equal to the maximum Date
            var tblproductsbuyprices = _context.TblProductsBuyPrice.
                Where(p => p.Product_Id == product.Id && p.Date == maxDate)
                .ToList();

            // If the Tblproductsbuyprices was not found, return a 404 Not Found response
            if (tblproductsbuyprices == null)
                return NotFound();

            // Return the Tblproductsbuyprices
            return Ok(tblproductsbuyprices);
        }

        [HttpGet("{productName}")]
        public ActionResult<Tblproductsbuyprices> GetRowsWithProductName(string productName)
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return NotFound();
            }
            Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == productName)
                       .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this name '{productName}'");
            }
            var tblproductsbuyprices = _context.TblProductsBuyPrice
                                       .Where(p => p.Product_Id == product.Id)
                                       .Join(_context.TblProducts,
                                            p => p.Product_Id,
                                            c => c.Id,
                                            (p, c) => new ProductsPurchasePriceWithProductNameDto
                                            {
                                                Date = p.Date,
                                                Product_Name = c.Product_Name,
                                                Product_Purchase_Price = p.Product_Purchase_Price
                                            })
                                       .ToList();

            if (tblproductsbuyprices == null)
            {
                return NotFound();
            }

            return Ok(tblproductsbuyprices);
        }
        #endregion

        #region Add Prodcuts Purchase Prices
        /// <summary>
        /// Add Prodcuts Purchase Prices
        /// </summary>
        /// <param name="productsPurchasePriceDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Tblproductsbuyprices>> PostTblproductsbuyprice(ProductsPurchasePriceDto productsPurchasePriceDto)
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return Problem("Entity set 'MySQLDBContext.TblProductsBuyPrice'  is null.");
            }
            _context.TblProductsBuyPrice.Add(new Tblproductsbuyprices
            {
                Date = productsPurchasePriceDto.Date,
                Product_Id = productsPurchasePriceDto.Product_Id,
                Product_Purchase_Price = productsPurchasePriceDto.Product_Purchase_Price
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductsbuypriceExists(productsPurchasePriceDto.Date, productsPurchasePriceDto.Product_Id)) return Conflict();
                else throw;
            }

            return CreatedAtAction("GetTblproductsbuyprice", new { id = productsPurchasePriceDto.Date }, productsPurchasePriceDto);
        }

        [HttpPost("Add_with_product_name")]
        public async Task<ActionResult<ProductsPurchasePriceWithProductNameDto>> Add_with_product_name(ProductsPurchasePriceWithProductNameDto priceWithProductNameDto)
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return Problem("Entity set 'MySQLDBContext.TblProductsBuyPrice'  is null.");
            }
            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == priceWithProductNameDto.Product_Name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this name '{priceWithProductNameDto.Product_Name}'");
            }

            Tblproductsbuyprices addedPriceObject = new Tblproductsbuyprices
            {
                Date = priceWithProductNameDto.Date,
                Product_Id = product.Id,
                Product_Purchase_Price = priceWithProductNameDto.Product_Purchase_Price
            };
            _context.TblProductsBuyPrice.Add(addedPriceObject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductsbuypriceExists(priceWithProductNameDto.Date, product.Id)) return Conflict();
                else throw;
            }

            return CreatedAtAction("GetByDateAndProductName", new { date = priceWithProductNameDto.Date, product_name = priceWithProductNameDto.Product_Name }, priceWithProductNameDto);
        }
        #endregion

        #region Update Products Purchase Prices

        [HttpPut("{id},{productId}")]
        public async Task<IActionResult> PutTblproductsbuyprice(DateTime id, int productId, Tblproductsbuyprices tblproductsbuyprice)
        {
            if (id != tblproductsbuyprice.Date && productId != tblproductsbuyprice.Product_Id)
            {
                return BadRequest();
            }

            _context.Entry(tblproductsbuyprice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblproductsbuypriceExists(id, productId))
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
        #endregion

        #region Delete Products Purchase Prices

        [HttpDelete("{Date}/{product_id}")]
        public async Task<IActionResult> DeleteTblproductsbuyprice(DateTime date, int product_id)
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return NotFound();
            }
            var tblproductsbuyprice = await _context.TblProductsBuyPrice.FindAsync(date, product_id);
            if (tblproductsbuyprice == null)
            {
                return NotFound();
            }

            _context.TblProductsBuyPrice.Remove(tblproductsbuyprice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteByDateAndProductName{Date}/{Product_Name}")]
        public async Task<IActionResult> DeleteByDateAndProductName(DateTime date, string product_name)
        {
            if (_context.TblProductsBuyPrice == null)
            {
                return NotFound();
            }

            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == product_name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this name '{product_name}'");
            }

            var tblproductsbuyprice = await _context.TblProductsBuyPrice
                .FindAsync(date, product.Id);
            if (tblproductsbuyprice == null)
            {
                return NotFound();
            }

            _context.TblProductsBuyPrice.Remove(tblproductsbuyprice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        private bool TblproductsbuypriceExists(DateTime date, int product_id)
        {
            return (_context.TblProductsBuyPrice?.Any(e => e.Date == date && e.Product_Id == product_id)).GetValueOrDefault();
        }
    }
}
