using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyReposrtController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public DailyReposrtController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("getPreviousDailyDate")]
        public async Task<ActionResult<dynamic>> getPreviousDailyDate(DateTime date, String stationName)
        {
            try
            {
                string sql = $"CALL getPreviousDailyDate('{date.ToString("yyyy-MM-dd")}', '{stationName}');";
                Console.WriteLine(sql);
                var entity = await _stationUnitOfWork.DataAccess.LoadData<dynamic, dynamic>(sql, new { });
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
