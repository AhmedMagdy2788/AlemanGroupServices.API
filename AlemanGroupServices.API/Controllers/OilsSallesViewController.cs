using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OilsSallesViewController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        private readonly MySQLDBContext _context;

        public OilsSallesViewController(IStationUnitOfWork stationUnitOfWork, MySQLDBContext context)
        {
            _stationUnitOfWork = stationUnitOfWork;
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var oilsSalles = await _context.Oils_sales_view.ToListAsync();
                return Ok(oilsSalles);
                //string sql = "SELECT * FROM aleman_db.Oils_sales_view ;";
                //var readingsRows = await _stationUnitOfWork.DataAccess.LoadData<OilsSalesView, dynamic>(sql, new { });
                //return Ok(readingsRows);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetStationOilsSalles")]
        public async Task<IActionResult> GetStationOilsSalles(string stationName)
        {
            try
            {
                
                var oilsSalles = await _context.Oils_sales_view.Where(os=> os.Station_Name == stationName).ToListAsync();
                return Ok(oilsSalles);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetStationOilsSallesAtDate")]
        public async Task<IActionResult> GetStationOilsSallesAtDate(string stationName, DateTime date)
        {
            try
            {

                var oilsSalles = await _context.Oils_sales_view.Where(os => os.Station_Name == stationName && os.Date == date).ToListAsync();
                return Ok(oilsSalles);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetStationOilsSallesSummationAtDate")]
        public async Task<IActionResult> GetStationOilsSallesSummationAtDate(string stationName, DateTime date)
        {
            try
            {

                var oilsSalles = await _context.Oils_sales_view.Where(os => os.Station_Name == stationName && os.Date == date).SumAsync(os=> os.Total_Price);
                return Ok(oilsSalles);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
