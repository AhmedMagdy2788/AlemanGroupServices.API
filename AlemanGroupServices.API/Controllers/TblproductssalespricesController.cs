using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblproductssalespricesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblproductssalespricesController(MySQLDBContext context)
        {
            _context = context;
        }

        #region Get Products Sales Prices
        // GET: api/Tblproductssalesprices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductsSalePriceDto>>> GetTblproductssalesprice()
        {
            if (_context.Tblproductssalesprices == null)
            {
                return NotFound();
            }
            //return await _context.Tblproductssalesprices.ToListAsync();
            return await _context.Tblproductssalesprices.Select(p => new ProductsSalePriceDto
            {
                Date = p.Date,
                Product_Id = p.Product_Id,
                Product_Selling_Price = p.Product_Selling_Price
            }).ToListAsync();
        }

        [HttpGet("ProductsSalePricesWithProductName")]
        public async Task<ActionResult<IEnumerable<ProductsSalePriceWithProductNameDto>>> ProductsSalePricesWithProductName()
        {
            if (_context.Tblproductssalesprices == null)
            {
                return NotFound();
            }
            return await _context.Tblproductssalesprices.Join(_context.TblProducts,
                    PurchasePrice => PurchasePrice.Product_Id,
                    product => product.Id,
                    (PurchasePrice, product) => new ProductsSalePriceWithProductNameDto
                    {
                        Date = PurchasePrice.Date,
                        Product_Name = product.Product_Name,
                        Product_Selling_Price = PurchasePrice.Product_Selling_Price
                    }).ToListAsync();
        }

        [HttpGet("{Date}/{product_id}")]
        public IActionResult GetTblproductssalesprice(DateTime date, int product_id)
        {
            // Find the Tblproductssalesprice with the specified Date and Product_Id
            var tblproductssaleprices = _context.Tblproductssalesprices
                                               .Find(date, product_id);

            // If the Tblproductssalesprice was not found, return a 404 Not Found response
            if (tblproductssaleprices == null)
            {
                return NotFound();
            }

            // Return the Tblproductssalesprice
            return Ok(tblproductssaleprices);
        }

        [HttpGet("GetByDateAndProductName{Date}/{Product_Name}")]
        public ActionResult<ProductsSalePriceWithProductNameDto> GetByDateAndProductName(DateTime date, string product_name)
        {

            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == product_name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this Name '{product_name}'");
            }

            // Find the Tblproductssalesprices with the specified Date and Product_Id
            var tblproductssaleprices = _context.Tblproductssalesprices
                                               .Find(date, product.Id);

            // If the Tblproductssalesprices was not found, return a 404 Not Found response
            if (tblproductssaleprices == null)
            {
                return NotFound();
            }
            var result = new ProductsSalePriceWithProductNameDto
            {
                Product_Name = product_name,
                Date = date,
                Product_Selling_Price = tblproductssaleprices.Product_Selling_Price,
            };
            // Return the Tblproductssalesprices
            return Ok(result);
        }

        [HttpGet("getPriceAtDate{Date}/{Product_Name}")]
        public IActionResult GetTblproductssalespriceAtDate(DateTime date, string product_name)
        {
            Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == product_name)
                       .FirstOrDefault();
            if (product == null)
                return NotFound($"there is no product with this Name '{product_name}'");
            Console.WriteLine($"{product.Product_Name} is found with id = {product.Id}");
            // Find the maximum Date that is equal to or before the specified Date
            DateTime maxDate = _context.Tblproductssalesprices
                                  .Where(p => p.Date <= date && p.Product_Id == product.Id)
                                  .Max(p => p.Date);
            Console.WriteLine($"the Date that equivilant for Date '{date}' is '{maxDate}'");
            // Find the Tblproductssalesprice with Product_Id equal to 1 and Date equal to the maximum Date
            var tblproductssaleprices = _context.Tblproductssalesprices.
                Where(p => p.Product_Id == product.Id && p.Date == maxDate)
                .ToList();

            // If the Tblproductssalesprice was not found, return a 404 Not Found response
            if (tblproductssaleprices == null)
                return NotFound();

            // Return the Tblproductssalesprice
            return Ok(tblproductssaleprices);
        }

        [HttpGet("{productName}")]
        public ActionResult<Tblproductssalesprice> GetRowsWithProductName(string productName)
        {
            if (_context.Tblproductssalesprices == null)
            {
                return NotFound();
            }
            Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == productName)
                       .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this Name '{productName}'");
            }
            var tblproductssaleprices = _context.Tblproductssalesprices
                .Where(p => p.Product_Id == product.Id)
                .Join(_context.TblProducts,
                    p => p.Product_Id,
                    c => c.Id,
                    (p, c) => new ProductsSalePriceWithProductNameDto
                    {
                        Date = p.Date,
                        Product_Name = c.Product_Name,
                        Product_Selling_Price = p.Product_Selling_Price
                    })
                .ToList();

            if (tblproductssaleprices == null)
            {
                return NotFound();
            }

            return Ok(tblproductssaleprices);
        }
        #endregion

        #region Update Products Sale Prices
        [HttpPut("{id},{productId}")]
        public async Task<IActionResult> PutTblproductssaleprice(DateTime id, int productId, Tblproductssalesprice tblproductssaleprice)
        {
            if (id != tblproductssaleprice.Date && productId != tblproductssaleprice.Product_Id)
            {
                return BadRequest();
            }

            _context.Entry(tblproductssaleprice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblproductssalepriceExists(id, productId))
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

        #region Add Products Sale Prices
        [HttpPost]
        public async Task<ActionResult<Tblproductssalesprice>> PostTblproductssaleprice(ProductsSalePriceDto productsSalePriceDto)
        {
            if (_context.Tblproductssalesprices == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblproductssalesprice'  is null.");
            }
            _context.Tblproductssalesprices.Add(new Tblproductssalesprice
            {
                Date = productsSalePriceDto.Date,
                Product_Id = productsSalePriceDto.Product_Id,
                Product_Selling_Price = productsSalePriceDto.Product_Selling_Price
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductssalepriceExists(productsSalePriceDto.Date, productsSalePriceDto.Product_Id)) return Conflict();
                else throw;
            }

            return CreatedAtAction("GetTblproductssalesprice", new { date = productsSalePriceDto.Date, product_Id = productsSalePriceDto.Product_Id }, productsSalePriceDto);
        }

        [HttpPost("Add_with_product_name")]
        public async Task<ActionResult<Tblproductssalesprice>> Add_with_product_name(ProductsSalePriceWithProductNameDto priceWithProductNameDto)
        {
            if (_context.Tblproductssalesprices == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblproductssalesprice'  is null.");
            }
            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == priceWithProductNameDto.Product_Name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this Name '{priceWithProductNameDto.Product_Name}'");
            }

            Tblproductssalesprice addedPriceObject = new Tblproductssalesprice
            {
                Date = priceWithProductNameDto.Date,
                Product_Id = product.Id,
                Product_Selling_Price = priceWithProductNameDto.Product_Selling_Price
            };
            _context.Tblproductssalesprices.Add(addedPriceObject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblproductssalepriceExists(priceWithProductNameDto.Date, product.Id)) return Conflict();
                else throw;
            }

            return CreatedAtAction("GetByDateAndProductName", new { date = priceWithProductNameDto.Date, product_name = priceWithProductNameDto.Product_Name }, priceWithProductNameDto);
        }

        [HttpPost("Add_Range_of_sales_prices_with_product_name")]
        public ActionResult<IEnumerable<ProductsSalePriceWithProductNameDto>> AddRangeProductsSalesPrice([FromBody] List<ProductsSalePriceWithProductNameDto> productsSalesPricesDto)
        {
            // Validate the input
            if (productsSalesPricesDto == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // Remove duplicates
            productsSalesPricesDto = productsSalesPricesDto
                .GroupBy(x => new { x.Date, x.Product_Name })
                .Select(g => g.Last())
                .ToList();

            // Map the DTO objects to entities
            var productsSalesPrices = productsSalesPricesDto.Select(dto =>
            {
                // Look up the Product_Id value
                var productId = _context.TblProducts
                    .Where(p => p.Product_Name == dto.Product_Name)
                    .Select(p => p.Id)
                    .FirstOrDefault();

                return new Tblproductssalesprice
                {
                    Date = dto.Date,
                    Product_Id = productId,
                    Product_Selling_Price = dto.Product_Selling_Price
                };
            }).ToList();

            // Check for existing entities
            var newProductsSalesPrices = new List<Tblproductssalesprice>();
            foreach (var productSalesPrice in productsSalesPrices)
            {
                var existingEntity = _context.Tblproductssalesprices.Find(productSalesPrice.Date, productSalesPrice.Product_Id);
                if (existingEntity == null)
                {
                    // Add the new entity to the list of entities to be added
                    newProductsSalesPrices.Add(productSalesPrice);
                }
            }

            // Add the products sales prices to the database

            _context.Tblproductssalesprices.AddRange(newProductsSalesPrices);
            _context.SaveChanges();

            return Ok(newProductsSalesPrices);
        }
        #endregion

        #region Delete Products Sale Prices
        [HttpDelete("{Date}/{Product_Name}")]
        public async Task<IActionResult> DeleteTblproductssaleprice(DateTime date, string product_name)
        {
            if (_context.Tblproductssalesprices == null)
            {
                return NotFound();
            }

            Tblproduct? product = _context.TblProducts
                        .Where(p => p.Product_Name == product_name)
                        .FirstOrDefault();
            if (product == null)
            {
                return NotFound($"there is no product with this Name '{product_name}'");
            }

            var tblproductssaleprice = _context.Tblproductssalesprices
                .Where(p => p.Date == date && p.Product_Id == product.Id)
                .FirstOrDefault();

            if (tblproductssaleprice == null)
            {
                return NotFound();
            }

            _context.Tblproductssalesprices.Remove(tblproductssaleprice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        private bool TblproductssalepriceExists(DateTime id, int product_id)
        {
            return (_context.Tblproductssalesprices?.Any(e => e.Date == id && e.Product_Id == product_id)).GetValueOrDefault();
        }
    }
}
