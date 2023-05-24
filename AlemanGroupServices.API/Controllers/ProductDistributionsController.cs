using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Mysqlx;
using System;

namespace AlemanGroupServices.API.Controllers
{
    //# ProductDistributionsController

    //This controller provides endpoints for managing product distributions.

        [Route("api/[controller]")]
    [ApiController]
    public class ProductDistributionsController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public ProductDistributionsController(MySQLDBContext context)
        {
            _context = context;
        }

        // `GET /api/ProductDistributions`
        //        Returns a list of all product distributions.
        // # Response
        //      * `200 OK`: A successful response will return a JSON array of `ProductDistributionDto` objects.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDistributionDto>>> GetTblproductsdistribution()
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return NotFound();
                }
                var result = await (
                    from productsDistribution in _context.Tblproductsdistribution
                    join product in _context.TblProducts on productsDistribution.ProductId equals product.Id
                    join destination in _context.Tbldestinationregions on productsDistribution.DestinationId equals destination.Id
                    orderby productsDistribution.OrderNo
                    select new ProductDistributionDto
                    {
                        Date = productsDistribution.Date,
                        OrderNo = productsDistribution.OrderNo,
                        ProductName = product.Product_Name,
                        DestinationName = destination.Name,
                        Quantity = productsDistribution.Quantity
                    }
                    ).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        // `GET /api/ProductDistributions/GetByDestinationNameWithIntervalTime`
        //        Returns a list of product distributions for a specific destination name within a specified date range.
        // ### Parameters ###
        //      * `destinationName` (string, required): The name of the destination to filter by.
        //      * `startDate` (DateTime, required): The start date of the date range to filter by.
        //      * `endDate` (DateTime, required): The end date of the date range to filter by.
        // ### Response ###
        //      * `200 OK`: A successful response will return a JSON array of `ProductDistributionDto` objects.
        [HttpGet("GetByDestinationNameWithIntervalTime")]
        public async Task<ActionResult<IEnumerable<ProductDistributionDto>>> GetByDestinationNameTillDate(string destinationName, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return NotFound();
                }
                var result = await (
                    from productsDistribution in _context.Tblproductsdistribution
                    join product in _context.TblProducts on productsDistribution.ProductId equals product.Id
                    join destination in _context.Tbldestinationregions on productsDistribution.DestinationId equals destination.Id
                    where productsDistribution.Date >= startDate && productsDistribution.Date <= endDate && destination.Name == destinationName
                    orderby productsDistribution.OrderNo
                    select new ProductDistributionDto
                    {
                        Date = productsDistribution.Date,
                        OrderNo = productsDistribution.OrderNo,
                        ProductName = product.Product_Name,
                        DestinationName = destination.Name,
                        Quantity = productsDistribution.Quantity
                    }
                    ).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetByDestinationNameTillDateGroupedByProductName")]
        public async Task<ActionResult<IEnumerable<ProductDistributionGroupedByProductNameTillDateDto>>> GetByDestinationNameTillDateGroupedByProductName(string destinationName, DateTime tillDate)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return NotFound();
                }
                var result = await (
                    from productsDistribution in _context.Tblproductsdistribution
                    join product in _context.TblProducts on productsDistribution.ProductId equals product.Id
                    join destination in _context.Tbldestinationregions on productsDistribution.DestinationId equals destination.Id
                    where productsDistribution.Date <= tillDate && destination.Name == destinationName
                    group productsDistribution by new { product.Product_Name } into grouped
                    orderby grouped.Key.Product_Name
                    select new ProductDistributionGroupedByProductNameTillDateDto
                    {
                        TillDate = tillDate,
                        DestinationName= destinationName,
                        GroupProductName = grouped.Key.Product_Name,
                        QuantitiesSum = grouped.Sum(x => x.Quantity)
                    }
                ).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/ProductDistributions/5
        [HttpGet("{orderNo}/{productName}/{destinationName}")]
        public async Task<ActionResult<ProductDistributionDto>> GetProductDistribution(uint orderNo, string productName, string destinationName)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return NotFound();
                }
                var product = _context.TblProducts.Where(p => p.Product_Name == productName).FirstOrDefault();
                if (product == null) { return NotFound($"There is no product with name '{productName}'"); }

                var destination = _context.Tbldestinationregions.Where(dr => dr.Name == destinationName).FirstOrDefault();
                if (destination == null) { return NotFound($"There is no destination name '{destinationName}'"); }

                var productDistribution = await _context.Tblproductsdistribution
                    .Where(pd => pd.OrderNo == orderNo && pd.ProductId == product.Id && pd.DestinationId == destination.Id).FirstOrDefaultAsync();

                if (productDistribution == null)
                {
                    return NotFound();
                }

                return new ProductDistributionDto
                {
                    Date = productDistribution.Date,
                    OrderNo = productDistribution.OrderNo,
                    ProductName = product.Product_Name,
                    DestinationName = destination.Name,
                    Quantity = productDistribution.Quantity
                };
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT: api/ProductDistributions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{orderNo}/{productName}/{destinationName}")]
        public async Task<IActionResult> PutProductDistribution(uint orderNo, string productName, string destinationName, ProductDistributionDto productDistributionDto)
        {
            try
            {
                if (orderNo != productDistributionDto.OrderNo || productName != productDistributionDto.ProductName || destinationName != productDistributionDto.DestinationName)
                {
                    return BadRequest();
                }

                var product = _context.TblProducts.Where(p => p.Product_Name == productDistributionDto.ProductName).FirstOrDefault();
                if (product == null) { return NotFound($"There is no product with name '{productName}'"); }

                var destination = _context.Tbldestinationregions.Where(dr => dr.Name == productDistributionDto.DestinationName).FirstOrDefault();
                if (destination == null) { return NotFound($"There is no destination name '{destinationName}'"); }

                var productDistribution = new ProductDistribution
                {
                    Date = productDistributionDto.Date,
                    OrderNo = productDistributionDto.OrderNo,
                    ProductId = product.Id,
                    DestinationId = destination.Id,
                    Quantity = productDistributionDto.Quantity
                };

                _context.Entry(productDistribution).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDistributionExists(
                        productDistribution.OrderNo,
                        productDistribution.ProductId,
                        productDistribution.DestinationId
                        ))
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
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // POST: api/ProductDistributions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDistributionDto>> PostProductDistribution(ProductDistributionDto productDistributionDto)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblproductsdistribution'  is null.");
                }
                var product = _context.TblProducts.Where(p => p.Product_Name == productDistributionDto.ProductName).FirstOrDefault();
                if (product == null) { return NotFound($"There is no product with name '{productDistributionDto.ProductName}'"); }

                var destination = _context.Tbldestinationregions.Where(dr => dr.Name == productDistributionDto.DestinationName).FirstOrDefault();
                if (destination == null) { return NotFound($"There is no destination name '{productDistributionDto.DestinationName}'"); }

                var productDistribution = new ProductDistribution
                {
                    Date = productDistributionDto.Date,
                    OrderNo = productDistributionDto.OrderNo,
                    ProductId = product.Id,
                    DestinationId = destination.Id,
                    Quantity = productDistributionDto.Quantity
                };


                _context.Tblproductsdistribution.Add(productDistribution);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (ProductDistributionExists(
                        productDistribution.OrderNo,
                        productDistribution.ProductId,
                        productDistribution.DestinationId
                        ))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetProductDistribution",
                    new
                    {
                        orderNo = productDistributionDto.OrderNo,
                        productName = productDistributionDto.ProductName,
                        destinationName = productDistributionDto.DestinationName
                    },
                    productDistributionDto);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("PostProductDistributionRange")]
        public async Task<ActionResult<IEnumerable<ProductDistributionDto>>> PostProductDistributionRange(List<ProductDistributionDto> productDistributionDtoList)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblproductsdistribution'  is null.");
                }
                var productsList = _context.TblProducts.ToList();
                var destinationsList = _context.Tbldestinationregions.ToList();

                var correctDataList = new List<ProductDistributionDto>();
                var dbDuplicates = new List<ProductDistributionDto>();
                var givenListDuplicates = new List<ProductDistributionDto>();
                var incorrectData = new List<ProductDistributionDto>();

                foreach (var productDistributionDto in productDistributionDtoList)
                {
                    Tblproduct? product = productsList.Where(p => p.Product_Name == productDistributionDto.ProductName).FirstOrDefault();
                    DestinationRegion? destination = destinationsList.Where(d => d.Name == productDistributionDto.DestinationName).FirstOrDefault();
                    if (product == null || destination == null)
                    {
                        incorrectData.Add(productDistributionDto);
                    }
                    else if (ProductDistributionExists(productDistributionDto.OrderNo, product.Id, destination.Id))
                    {
                        dbDuplicates.Add(productDistributionDto);
                    }
                    else if (correctDataList
                        .Any(e =>
                        e.OrderNo == productDistributionDto.OrderNo &&
                        e.ProductName == productDistributionDto.ProductName &&
                        e.DestinationName == productDistributionDto.DestinationName
                        ))
                    {
                        givenListDuplicates.Add(productDistributionDto);
                    }
                    else
                    {
                        var productDistribution = new ProductDistribution
                        {
                            Date = productDistributionDto.Date,
                            OrderNo = productDistributionDto.OrderNo,
                            ProductId = product.Id,
                            DestinationId = destination.Id,
                            Quantity = productDistributionDto.Quantity
                        };
                        correctDataList.Add(productDistributionDto);
                        _context.Tblproductsdistribution.Add(productDistribution);
                    }
                }

                await _context.SaveChangesAsync();

                if (dbDuplicates.Count > 0 || incorrectData.Count > 0 || givenListDuplicates.Count > 0) return BadRequest(new
                {
                    errorMessage = $"There are {dbDuplicates.Count} douplicates prodcuts Distribution already exist in the database, {givenListDuplicates.Count} duplicates in the given list and {incorrectData.Count} incorrect data",
                    dbDuplicateProductDistributionList = dbDuplicates,
                    givenDuplicateProductDistributionList = givenListDuplicates,
                    incorrectProductDistributionList = incorrectData
                });

                return CreatedAtAction("Gettblordersquantity", new { }, productDistributionDtoList);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/ProductDistributions/5
        [HttpDelete("{orderNo}/{productName}/{destinationName}")]
        public async Task<IActionResult> DeleteProductDistribution(uint orderNo, string productName, string destinationName)
        {
            try
            {
                if (_context.Tblproductsdistribution == null)
                {
                    return NotFound();
                }

                var product = _context.TblProducts.Where(p => p.Product_Name == productName).FirstOrDefault();
                if (product == null) { return NotFound($"There is no product with name '{productName}'"); }

                var destination = _context.Tbldestinationregions.Where(dr => dr.Name == destinationName).FirstOrDefault();
                if (destination == null) { return NotFound($"There is no destination name '{destinationName}'"); }

                var productDistribution = await _context.Tblproductsdistribution
                    .Where(pd => pd.OrderNo == orderNo && pd.ProductId == product.Id && pd.DestinationId == destination.Id)
                    .FirstOrDefaultAsync();
                if (productDistribution == null)
                {
                    return NotFound();
                }

                _context.Tblproductsdistribution.Remove(productDistribution);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool ProductDistributionExists(uint orderNo, int productId, int destinationId)
        {
            return (_context.Tblproductsdistribution?
                .Any(e => e.OrderNo == orderNo && e.ProductId == productId && e.DestinationId == destinationId))
                .GetValueOrDefault();
        }
    }
}

/*
## Endpoints

### `GET /api/ProductDistributions/GetByDestinationNameTillDateGroupedByProductName`

Returns a list of product distributions for a specific destination name grouped by product name until a specified date.

#### Parameters

- `destinationName` (string, required): The name of the destination to filter by.
- `tillDate` (DateTime, required): The end date to filter by.

#### Response

- `200 OK`: A successful response will return a JSON array of `ProductDistributionGroupedByProductNameTillDateDto` objects.

### `GET /api/ProductDistributions/{orderNo}/{productName}/{destinationName}`

Returns a single product distribution for a specific order number, product name, and destination name.

#### Parameters

- `orderNo` (uint, required): The order number to filter by.
- `productName` (string, required): The name of the product to filter by.
- `destinationName` (string, required): The name of the destination to filter by.

#### Response

- `200 OK`: A successful response will return a JSON object representing the requested product distribution.
- `404 Not Found`: If no product distribution is found for the specified order number, product name, and destination name.

### `PUT /api/ProductDistributions/{orderNo}/{productName}/{destinationName}`

Updates an existing product distribution for a specific order number, product name, and destination name.

#### Parameters

- `orderNo` (uint, required): The order number to update.
- `productName` (string, required): The name of the product to update.
- `destinationName` (string, required): The name of the destination to update.
- `
*/