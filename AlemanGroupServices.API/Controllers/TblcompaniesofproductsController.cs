using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblcompaniesofproductsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public TblcompaniesofproductsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/Tblcompaniesofproducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tblcompaniesofproduct>>> GetTblcompaniesofproducts()
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            return await _context.Tblcompaniesofproducts.ToListAsync();
        }

        [HttpGet("GetWithProductName&SourceComapnyName")]
        public ActionResult<IEnumerable<CompanyOfProductDto>> GetWithProductNameAndSourceComapnyName()
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var query =
                from companiesOfProductsTbl in _context.Tblcompaniesofproducts
                join prductsTbl in _context.TblProducts
                on companiesOfProductsTbl.Product_Id equals prductsTbl.Id
                join marketingCompaniesTbl in _context.Tblmarketingcompnies
                on companiesOfProductsTbl.Source_Company_Id equals marketingCompaniesTbl.Id
                select new CompanyOfProductDto
                {
                    Date = companiesOfProductsTbl.Date,
                    Product_Name = prductsTbl.Product_Name,
                    Source_Company_Name = marketingCompaniesTbl.Name
                };
            return Ok(query.ToList());
        }

        // GET: api/Tblcompaniesofproducts/5
        [HttpGet("{product_id}/{source_company_id}")]
        public ActionResult<Tblcompaniesofproduct> GetTblcompaniesofproduct(int product_id, Guid source_company_id)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var tblcompaniesofproduct = _context.Tblcompaniesofproducts.Where(p => p.Product_Id == product_id && p.Source_Company_Id == source_company_id).FirstOrDefault();

            if (tblcompaniesofproduct == null)
            {
                return NotFound();
            }

            return tblcompaniesofproduct;
        }

        // GET: api/Tblcompaniesofproducts/5
        [HttpGet("GetTblcompaniesofproductWithNames{Product_Name}/{source_company_name}")]
        public ActionResult<CompanyOfProductDto> GetTblcompaniesofproductWithNames(string product_name, string source_company_name)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var product = _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefault();
            if (product == null)
            {
                return NotFound($" there is no product wiht Name '{product_name}'");
            }

            var source_compnay = _context.Tblmarketingcompnies.Where(m => m.Name == source_company_name).FirstOrDefault();
            if (source_compnay == null)
            {
                return NotFound($" there is no Marketing company wiht Name '{source_company_name}'");
            }

            var tblcompaniesofproduct = _context.Tblcompaniesofproducts.Where(p => p.Product_Id == product.Id && p.Source_Company_Id == source_compnay.Id).FirstOrDefault();

            if (tblcompaniesofproduct == null)
            {
                return NotFound();
            }

            return new CompanyOfProductDto { Date = tblcompaniesofproduct.Date, Product_Name = product.Product_Name, Source_Company_Name = source_compnay.Name };
        }

        // GET: api/Tblcompaniesofproducts/5
        [HttpGet("Get_product_sources{Product_Name}")]
        public ActionResult<IEnumerable<CompanyOfProductDto>> Get_product_sources(string product_name)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var product = _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefault();
            if (product == null)
            {
                return NotFound($" there is no product wiht Name '{product_name}'");
            }

            var tblcompaniesofproduct = _context.Tblcompaniesofproducts
                .Where(p => p.Product_Id == product.Id)
                .Join(_context.Tblmarketingcompnies,
                source_company => source_company.Source_Company_Id,
                marketing_company => marketing_company.Id,
                (source_company, marketing_company) => new CompanyOfProductDto
                {
                    Date = source_company.Date,
                    Product_Name = product.Product_Name,
                    Source_Company_Name = marketing_company.Name
                }
                ).ToList();

            if (tblcompaniesofproduct == null)
            {
                return NotFound();
            }

            return Ok(tblcompaniesofproduct);
        }

        // PUT: api/Tblcompaniesofproducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{product_id}/{source_company_id}")]
        public async Task<IActionResult> PutTblcompaniesofproduct(int product_id, Guid source_company_id, Tblcompaniesofproduct tblcompaniesofproduct)
        {
            if (product_id != tblcompaniesofproduct.Product_Id && source_company_id == tblcompaniesofproduct.Source_Company_Id)
            {
                return BadRequest();
            }

            _context.Entry(tblcompaniesofproduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblcompaniesofproductExists(product_id, source_company_id))
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

        [HttpPut("update_with_names_Dto{product_Name}/{source_company_Name}")]
        public async Task<IActionResult> PutTblcompaniesofproductDto(string product_name, string source_company_name, CompanyOfProductDto companyOfProductDto)
        {
            if (product_name != companyOfProductDto.Product_Name && source_company_name == companyOfProductDto.Source_Company_Name)
            {
                return BadRequest();
            }
            var product = await _context.TblProducts.Where(p => p.Product_Name == companyOfProductDto.Product_Name).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($" there is no product wiht Name '{companyOfProductDto.Product_Name}'");
            }

            var source_compnay = await _context.Tblmarketingcompnies.Where(m => m.Name == companyOfProductDto.Source_Company_Name).FirstOrDefaultAsync();
            if (source_compnay == null)
            {
                return NotFound($" there is no Marketing company wiht Name '{companyOfProductDto.Source_Company_Name}'");
            }
            var updatedEntity = new Tblcompaniesofproduct
            {
                Date = companyOfProductDto.Date,
                Product_Id = product.Id,
                Source_Company_Id = source_compnay.Id,
            };

            _context.Entry(updatedEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblcompaniesofproductExists(updatedEntity.Product_Id, updatedEntity.Source_Company_Id))
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

        // POST: api/Tblcompaniesofproducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tblcompaniesofproduct>> PostTblcompaniesofproduct(Tblcompaniesofproduct tblcompaniesofproduct)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblcompaniesofproducts'  is null.");
            }
            _context.Tblcompaniesofproducts.Add(tblcompaniesofproduct);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblcompaniesofproductExists(tblcompaniesofproduct.Product_Id, tblcompaniesofproduct.Source_Company_Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblcompaniesofproduct", new { product_id = tblcompaniesofproduct.Product_Id, source_company_id = tblcompaniesofproduct.Source_Company_Id }, tblcompaniesofproduct);
        }

        [HttpPost("add_with_Names")]
        public async Task<ActionResult<Tblcompaniesofproduct>> PostCompaniesOfProductDto(CompanyOfProductDto tblcompaniesofproductDto)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tblcompaniesofproducts'  is null.");
            }
            var product = await _context.TblProducts.Where(p => p.Product_Name == tblcompaniesofproductDto.Product_Name).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($" there is no product wiht Name '{tblcompaniesofproductDto.Product_Name}'");
            }

            var source_compnay = await _context.Tblmarketingcompnies.Where(m => m.Name == tblcompaniesofproductDto.Source_Company_Name).FirstOrDefaultAsync();
            if (source_compnay == null)
            {
                return NotFound($" there is no Marketing company wiht Name '{tblcompaniesofproductDto.Source_Company_Name}'");
            }
            var addedEntity = new Tblcompaniesofproduct
            {
                Date = tblcompaniesofproductDto.Date,
                Product_Id = product.Id,
                Source_Company_Id = source_compnay.Id,
            };
            _context.Tblcompaniesofproducts.Add(addedEntity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblcompaniesofproductExists(addedEntity.Product_Id, addedEntity.Source_Company_Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblcompaniesofproduct", new { product_id = addedEntity.Product_Id, source_company_id = addedEntity.Source_Company_Id }, addedEntity);
        }

        // DELETE: api/Tblcompaniesofproducts/5
        [HttpDelete("{product_id}/{source_company_id}")]
        public async Task<IActionResult> DeleteTblcompaniesofproduct(int product_id, Guid source_company_id)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var tblcompaniesofproduct = await _context.Tblcompaniesofproducts.Where(p => p.Product_Id == product_id && p.Source_Company_Id == source_company_id).FirstOrDefaultAsync();
            if (tblcompaniesofproduct == null)
            {
                return NotFound();
            }

            _context.Tblcompaniesofproducts.Remove(tblcompaniesofproduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete_with_names{Product_Name}/{source_company_name}")]
        public async Task<IActionResult> DeleteTblcompaniesofproductDto(string product_name, string source_company_name)
        {
            if (_context.Tblcompaniesofproducts == null)
            {
                return NotFound();
            }
            var product = await _context.TblProducts.Where(p => p.Product_Name == product_name).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound($" there is no product wiht Name '{product_name}'");
            }

            var source_compnay = await _context.Tblmarketingcompnies.Where(m => m.Name == source_company_name).FirstOrDefaultAsync();
            if (source_compnay == null)
            {
                return NotFound($" there is no Marketing company wiht Name '{source_company_name}'");
            }
            var tblcompaniesofproduct = await _context.Tblcompaniesofproducts.Where(p => p.Product_Id == product.Id && p.Source_Company_Id == source_compnay.Id).FirstOrDefaultAsync();
            if (tblcompaniesofproduct == null)
            {
                return NotFound();
            }

            _context.Tblcompaniesofproducts.Remove(tblcompaniesofproduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblcompaniesofproductExists(int product_id, Guid source_company_id)
        {
            return (_context.Tblcompaniesofproducts?.Any(e => e.Product_Id == product_id && e.Source_Company_Id == source_company_id)).GetValueOrDefault();
        }
    }
}
