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
    public class MCAccountProductsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public MCAccountProductsController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/MCAccountProducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MCAccountProductDto>>> Gettblmarketingcompaccountsproducts()
        {
            try
            {
                if (_context.Tblmarketingcompaccountsproducts == null)
                {
                    return NotFound();
                }
                return await _context.Tblmarketingcompaccountsproducts
                    .Join(_context.Tblmainproducts,
                    mcap => mcap.MainProductId,
                    mp => mp.CategoryId,
                    (mcap, mp) => new MCAccountProductDto
                    {
                        AccountNo = mcap.AccountNo,
                        MainProductName = mp.Products_Category,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        // GET: api/MCAccountProducts/5
        [HttpGet("{accountNo}")]
        public async Task<ActionResult<IEnumerable<MCAccountProductDto>>> GetMCAccountProduct(int accountNo)
        {
            try
            {
                if (_context.Tblmarketingcompaccountsproducts == null)
                {
                    return NotFound();
                }
                var mCAccountProduct = await _context.Tblmarketingcompaccountsproducts
                    .Where(mcap => mcap.AccountNo == accountNo)
                    .Join(_context.Tblmainproducts,
                        mcap => mcap.MainProductId,
                        mp => mp.CategoryId,
                        (mcap, mp) => new MCAccountProductDto
                        {
                            AccountNo = mcap.AccountNo,
                            MainProductName = mp.Products_Category,
                        })
                    .ToListAsync();

                if (mCAccountProduct == null)
                {
                    return NotFound();
                }

                return mCAccountProduct;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // POST: api/MCAccountProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MCAccountProduct>> PostMCAccountProduct(MCAccountProductDto mCAccountProductDto)
        {
            try
            {
                if (_context.Tblmarketingcompaccountsproducts == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblmarketingcompaccountsproducts'  is null.");
                }

                var category = await _context.Tblmainproducts.Where(mcap => mcap.Products_Category == mCAccountProductDto.MainProductName).FirstOrDefaultAsync();
                if (category == null)
                {
                    return NotFound($"there is no product category called '{mCAccountProductDto.MainProductName}'");
                }
                var mCAccountProduct = new MCAccountProduct { AccountNo = mCAccountProductDto.AccountNo, MainProductId = category.CategoryId };
                Console.WriteLine(mCAccountProduct.ToString());
                _context.Tblmarketingcompaccountsproducts.Add(mCAccountProduct);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (MCAccountProductExists(mCAccountProduct.AccountNo, mCAccountProduct.MainProductId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("PostRangeMCAccountProduct")]
        public async Task<ActionResult<MCAccountProduct>> PostRangeMCAccountProduct(List<MCAccountProductDto> mCAccountProductDtoList)
        {
            try
            {
                if (_context.Tblmarketingcompaccountsproducts == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblmarketingcompaccountsproducts'  is null.");
                }
                foreach (var mCAccountProductDto in mCAccountProductDtoList)
                {
                    var category = await _context.Tblmainproducts.Where(mcap => mcap.Products_Category == mCAccountProductDto.MainProductName).FirstOrDefaultAsync();
                    if (category == null)
                    {
                        return NotFound($"there is no product category called '{mCAccountProductDto.MainProductName}'");
                    }
                    var mCAccountProduct = new MCAccountProduct { AccountNo = mCAccountProductDto.AccountNo, MainProductId = category.CategoryId };
                    await _context.Tblmarketingcompaccountsproducts.AddAsync(mCAccountProduct);
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        if (MCAccountProductExists(mCAccountProduct.AccountNo, mCAccountProduct.MainProductId))
                        {
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        // DELETE: api/MCAccountProducts/5
        [HttpDelete("{accounNo}/{mainProductName}")]
        public async Task<IActionResult> DeleteMCAccountProduct(int accounNo, string mainProductName)
        {
            try
            {
                if (_context.Tblmarketingcompaccountsproducts == null)
                {
                    return NotFound();
                }
                var category = await _context.Tblmainproducts.Where(mcap => mcap.Products_Category == mainProductName).FirstOrDefaultAsync();
                if (category == null)
                {
                    return NotFound($"there is no product category called '{mainProductName}'");
                }
                var mCAccountProduct = await _context.Tblmarketingcompaccountsproducts
                    .Where(e => e.AccountNo == accounNo && e.MainProductId == category.CategoryId)
                    .FirstOrDefaultAsync();
                if (mCAccountProduct == null)
                {
                    return NotFound();
                }

                _context.Tblmarketingcompaccountsproducts.Remove(mCAccountProduct);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool MCAccountProductExists(uint accounNo, int mainProductId)
        {
            return (_context.Tblmarketingcompaccountsproducts?.Any(e => e.AccountNo == accounNo && e.MainProductId == mainProductId)).GetValueOrDefault();
        }
    }
}
