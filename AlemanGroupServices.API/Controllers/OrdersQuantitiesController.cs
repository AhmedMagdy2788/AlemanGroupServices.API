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
    public class OrdersQuantitiesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public OrdersQuantitiesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdersQuantities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersQuantityDto>>> Gettblordersquantity()
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return NotFound();
                }
                return await _context.Tblordersquantity
                    .Join(_context.TblProducts,
                    orderQuantity => orderQuantity.ProductId,
                    product => product.Id,
                    (orderQuantity, product) => new OrdersQuantityDto
                    {
                        OrderNo = orderQuantity.OrderNo,
                        ProductName = product.Product_Name,
                        Quantity = orderQuantity.Quantity,
                    })
                    .OrderBy(x => x.OrderNo)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // GET: api/OrdersQuantities/5
        [HttpGet("{orderNo}/{productName}")]
        public async Task<ActionResult<OrdersQuantityDto>> GetOrdersQuantity(uint orderNo, string productName)
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return NotFound();
                }
                Tblproduct? product = _context.TblProducts.Where(p => p.Product_Name == productName).FirstOrDefault();
                if (product == null) return NotFound("there is no product with that Name");
                var ordersQuantityDto = await _context.Tblordersquantity
                    .Where(oq => oq.OrderNo == orderNo && oq.ProductId == product.Id)
                    .Join(_context.TblProducts,
                    orderQuantity => orderQuantity.ProductId,
                    product => product.Id,
                    (orderQuantity, product) => new OrdersQuantityDto
                    {
                        OrderNo = orderQuantity.OrderNo,
                        ProductName = product.Product_Name,
                        Quantity = orderQuantity.Quantity,
                    })
                    .FirstOrDefaultAsync();

                if (ordersQuantityDto == null)
                {
                    return NotFound($"there is no orderQuantity stored with order no '{orderNo}' and product Name '{productName}'");
                }
                return ordersQuantityDto;

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{orderNo}")]
        public async Task<ActionResult<IEnumerable<OrdersQuantityDto>>> GetOrdersQuantity(uint orderNo)
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return NotFound();
                }
                var ordersQuantityDtoList = await _context.Tblordersquantity
                    .Where(oq => oq.OrderNo == orderNo)
                    .Join(_context.TblProducts,
                    orderQuantity => orderQuantity.ProductId,
                    product => product.Id,
                    (orderQuantity, product) => new OrdersQuantityDto
                    {
                        OrderNo = orderQuantity.OrderNo,
                        ProductName = product.Product_Name,
                        Quantity = orderQuantity.Quantity,
                    })
                    .ToListAsync();

                if (ordersQuantityDtoList == null)
                {
                    return NotFound($"there is no orderQuantity stored with order no '{orderNo}'.'");
                }

                return ordersQuantityDtoList;

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT: api/OrdersQuantities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{orderNo}/{productName}")]
        public async Task<IActionResult> PutOrdersQuantity(uint orderNo, string productName, OrdersQuantityDto ordersQuantityDto)
        {
            try
            {
                if (orderNo != ordersQuantityDto.OrderNo || productName != ordersQuantityDto.ProductName)
                {
                    return BadRequest();
                }
                Tblproduct? product = _context.TblProducts.Where(p => p.Product_Name == productName).FirstOrDefault();
                if (product == null) return NotFound("there is no product with that Name");
                var ordersQuantity = await _context.Tblordersquantity
                    .Where(oq => oq.OrderNo == orderNo && oq.ProductId == product.Id)
                    .Join(_context.TblProducts,
                    orderQuantity => orderQuantity.ProductId,
                    product => product.Id,
                    (orderQuantity, product) => new OrdersQuantityDto
                    {
                        OrderNo = orderQuantity.OrderNo,
                        ProductName = product.Product_Name,
                        Quantity = orderQuantity.Quantity,
                    })
                    .FirstOrDefaultAsync();

                if (ordersQuantity == null)
                {
                    return NotFound($"there is no orderQuantity stored with order no '{orderNo}' and product Name '{productName}'");
                }

                _context.Entry(ordersQuantity).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersQuantityExists(orderNo, product.Id))
                    {
                        return NotFound($"there is no orderQuantity stored with order no '{orderNo}' and product Name '{productName}'");
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

        // POST: api/OrdersQuantities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdersQuantity>> PostOrdersQuantity(OrdersQuantityDto ordersQuantityDto)
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblordersquantity'  is null.");
                }

                Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == ordersQuantityDto.ProductName)
                       .FirstOrDefault();
                if (product == null) return NotFound("there is no product with that Name");
                var ordersQuantity = new OrdersQuantity
                {
                    OrderNo = ordersQuantityDto.OrderNo,
                    ProductId = product.Id,
                    Quantity = ordersQuantityDto.Quantity,
                };

                _context.Tblordersquantity.Add(ordersQuantity);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (OrdersQuantityExists(ordersQuantity.OrderNo, ordersQuantity.ProductId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetOrdersQuantity",
                    new
                    {
                        orderNo = ordersQuantity.OrderNo,
                        prductName = ordersQuantityDto.ProductName
                    }
                    , ordersQuantity);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        [HttpPost("PostOrdersQuantityRange")]
        public async Task<ActionResult<IEnumerable<OrdersQuantityDto>>> PostOrdersQuantityRange([FromBody] List<OrdersQuantityDto> ordersQuantityDtoList)
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return Problem("Entity set 'MySQLDBContext.Tblordersquantity'  is null.");
                }

                List<Tblproduct> productsList = _context.TblProducts.ToList();
                var correctDataList = new List<OrdersQuantityDto>();
                var dbDuplicates = new List<OrdersQuantityDto>();
                var givenListDuplicates = new List<OrdersQuantityDto>();
                var incorrectData = new List<OrdersQuantityDto>();
                foreach (var orderQunatityDto in ordersQuantityDtoList)
                {

                    Tblproduct? product = productsList.Where(p => p.Product_Name == orderQunatityDto.ProductName).FirstOrDefault();
                    if (product == null)
                    {
                        incorrectData.Add(orderQunatityDto);
                    }
                    else if (OrdersQuantityExists(orderQunatityDto.OrderNo, product.Id))
                    {
                        dbDuplicates.Add(orderQunatityDto);
                    }
                    else if (correctDataList
                        .Any(e =>
                        e.OrderNo == orderQunatityDto.OrderNo &&
                        e.ProductName == orderQunatityDto.ProductName
                        ))
                    {
                        givenListDuplicates.Add(orderQunatityDto);
                    }
                    else
                    {
                        var ordersQuantity = new OrdersQuantity
                        {
                            OrderNo = orderQunatityDto.OrderNo,
                            ProductId = product.Id,
                            Quantity = orderQunatityDto.Quantity,
                        };
                        correctDataList.Add(orderQunatityDto);
                        _context.Tblordersquantity.Add(ordersQuantity);
                    }
                }

                await _context.SaveChangesAsync();

                if (dbDuplicates.Count > 0 || incorrectData.Count > 0 || givenListDuplicates.Count > 0) return BadRequest(new
                {
                    errorMessage = $"There are {dbDuplicates.Count} withdrawals already exist in the database,{givenListDuplicates.Count} duplicates in the given list, and {incorrectData.Count} incorrect data",
                    duplicateOrderQuantityList = dbDuplicates,
                    givenDuplicateProductDistributionList = givenListDuplicates,
                    incorrectOrderQuantityList = incorrectData
                });

                return CreatedAtAction("Gettblordersquantity", new { }, ordersQuantityDtoList);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // DELETE: api/OrdersQuantities/5
        [HttpDelete("{orderNo}/{productName}")]
        public async Task<IActionResult> DeleteOrdersQuantity(uint orderNo, string productName)
        {
            try
            {
                if (_context.Tblordersquantity == null)
                {
                    return NotFound();
                }
                Tblproduct? product = _context.TblProducts
                       .Where(p => p.Product_Name == productName)
                       .FirstOrDefault();
                if (product == null) return NotFound("there is no product with that Name");
                var ordersQuantity = await _context.Tblordersquantity
                    .Where(oq => oq.ProductId == product.Id && oq.OrderNo == orderNo)
                    .FirstOrDefaultAsync();
                if (ordersQuantity == null)
                {
                    return NotFound($"there is no orderQuantity stored with order no '{orderNo}' and product Name '{productName}'");
                }

                _context.Tblordersquantity.Remove(ordersQuantity);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool OrdersQuantityExists(uint orderNo, int productId)
        {
            return (_context.Tblordersquantity?.Any(e => e.OrderNo == orderNo && e.ProductId == productId)).GetValueOrDefault();
        }
    }
}
