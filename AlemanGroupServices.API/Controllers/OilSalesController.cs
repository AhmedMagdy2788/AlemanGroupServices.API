using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OilSalesController : ControllerBase
    {
        private readonly MySQLDBContext _context;

        public OilSalesController(MySQLDBContext context)
        {
            _context = context;
        }

        // GET: api/OilSales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OilSaleDto>>> Gettbloilssales()
        {
            if (_context.Tbloilssales == null)
            {
                return NotFound();
            }

            var oilsSalesDtoList = await (
                from oilsSales in _context.Tbloilssales
                join stations in _context.TblStations on oilsSales.StationId equals stations.Id
                join oils in _context.TblProducts on oilsSales.OilId equals oils.Id
                orderby oilsSales.Date
                select new OilSaleDto
                {
                    Id = oilsSales.Id,
                    Date = oilsSales.Date,
                    StationName = stations.Name,
                    OilName = oils.Product_Name,
                    Quantity = oilsSales.Quantity,
                })
                .ToListAsync();

            return oilsSalesDtoList;
        }

        // GET: api/OilSales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OilSaleDto>> GetOilSale(int id)
        {
            if (_context.Tbloilssales == null)
            {
                return NotFound();
            }
            var oilSale = await (
                from oilsSales in _context.Tbloilssales
                where oilsSales.Id == id
                join stations in _context.TblStations on oilsSales.StationId equals stations.Id
                join oils in _context.TblProducts on oilsSales.OilId equals oils.Id
                orderby oilsSales.Date
                select new OilSaleDto
                {
                    Id = oilsSales.Id,
                    Date = oilsSales.Date,
                    StationName = stations.Name,
                    OilName = oils.Product_Name,
                    Quantity = oilsSales.Quantity,
                })
                .FirstOrDefaultAsync();

            if (oilSale == null)
            {
                return NotFound();
            }

            return oilSale;
        }

        [HttpGet("GetStationOilSale{stationName}")]
        public async Task<ActionResult<IEnumerable<OilSaleDto>>> GetStationOilSale(string stationName)
        {
            if (_context.Tbloilssales == null)
            {
                return NotFound();
            }
            var oilSaleDtoList = await (
                from oilsSales in _context.Tbloilssales
                join stations in _context.TblStations on oilsSales.StationId equals stations.Id
                where stations.Name == stationName
                join oils in _context.TblProducts on oilsSales.OilId equals oils.Id
                orderby oilsSales.Date
                select new OilSaleDto
                {
                    Id = oilsSales.Id,
                    Date = oilsSales.Date,
                    StationName = stations.Name,
                    OilName = oils.Product_Name,
                    Quantity = oilsSales.Quantity,
                })
                .ToListAsync();

            if (oilSaleDtoList == null)
            {
                return NotFound();
            }

            return oilSaleDtoList;
        }

        [HttpGet("GetStationOilSaleIntervalDate{stationName}/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<OilSaleDto>>> GetStationOilSaleIntervalDate(string stationName, DateTime startDate, DateTime endDate)
        {
            if (_context.Tbloilssales == null)
            {
                return NotFound();
            }
            var oilSaleDtoList = await (
                from oilsSales in _context.Tbloilssales
                where oilsSales.Date >= startDate && oilsSales.Date <= endDate
                join stations in _context.TblStations on oilsSales.StationId equals stations.Id
                where stations.Name == stationName
                join oils in _context.TblProducts on oilsSales.OilId equals oils.Id
                orderby oilsSales.Date
                select new OilSaleDto
                {
                    Id = oilsSales.Id,
                    Date = oilsSales.Date,
                    StationName = stations.Name,
                    OilName = oils.Product_Name,
                    Quantity = oilsSales.Quantity,
                })
                .ToListAsync();

            if (oilSaleDtoList == null)
            {
                return NotFound();
            }

            return oilSaleDtoList;
        }

        // PUT: api/OilSales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOilSale(int id, OilSaleDto oilSaleDto)
        {
            if (id != oilSaleDto.Id)
            {
                return BadRequest();
            }
            var station = await _context.TblStations.Where(s => s.Name == oilSaleDto.StationName).FirstOrDefaultAsync();
            if (station == null) { return NotFound($"There is no station with Name '{oilSaleDto.StationName}'."); }

            var product = await _context.TblProducts.Where(p => p.Product_Name == oilSaleDto.OilName).FirstOrDefaultAsync();
            if (product == null) { return NotFound($"There is no prouct with Name '{oilSaleDto.OilName}'"); }

            var oilSale = new OilSale
            {
                Id = id,
                Date = oilSaleDto.Date,
                StationId = station.Id,
                OilId = product.Id,
                Quantity = oilSaleDto.Quantity,
            };
            _context.Entry(oilSale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OilSaleExists(id))
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

        // POST: api/OilSales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OilSale>> PostOilSale(OilSaleDto oilSaleDto)
        {
            if (_context.Tbloilssales == null)
            {
                return Problem("Entity set 'MySQLDBContext.Tbloilssales'  is null.");
            }
            var station = await _context.TblStations.Where(s => s.Name == oilSaleDto.StationName).FirstOrDefaultAsync();
            if (station == null) { return NotFound($"There is no station with Name '{oilSaleDto.StationName}'."); }

            var product = await _context.TblProducts.Where(p => p.Product_Name == oilSaleDto.OilName).FirstOrDefaultAsync();
            if (product == null) { return NotFound($"There is no prouct with Name '{oilSaleDto.OilName}'"); }

            var oilSale = new OilSale
            {
                Id = oilSaleDto.Id ?? 0,
                Date = oilSaleDto.Date,
                StationId = station.Id,
                OilId = product.Id,
                Quantity = oilSaleDto.Quantity,
            };

            _context.Tbloilssales.Add(oilSale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOilSale", new { id = oilSale.Id }, oilSale);
        }

        // DELETE: api/OilSales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOilSale(int id)
        {
            if (_context.Tbloilssales == null)
            {
                return NotFound();
            }
            var oilSale = await _context.Tbloilssales.FindAsync(id);
            if (oilSale == null)
            {
                return NotFound();
            }

            _context.Tbloilssales.Remove(oilSale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OilSaleExists(int id)
        {
            return (_context.Tbloilssales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
