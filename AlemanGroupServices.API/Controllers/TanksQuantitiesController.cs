using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TanksQuantitiesController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public TanksQuantitiesController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationUnitOfWork.TblTanksQuantityRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetById{Date}/{tankNo}")]
        public ActionResult<TblTanksQuantity> GetById(DateTime date, uint tankNo)
        {
            try
            {
                var entity = _stationUnitOfWork.TblTanksQuantityRepository.Find((b) => b.Registeration_Date == date && b.Tank_No == tankNo);

                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("getStationTanksQuantity")]
        public async Task<ActionResult<TblTanksQuantity>> getStationTanksQuantity(String stationName, DateTime date)
        {
            try
            {
                string sql = $"CALL getStationTanksQuantity('{date.ToString("yyyy-MM-dd")}', '{stationName}');";
                Console.WriteLine(sql);
                var entity = await _stationUnitOfWork.DataAccess.LoadData<TblTanksQuantity, dynamic>(sql, new { });

                if (entity == null)
                {
                    return NotFound();
                }

                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetByTankNo")]
        public IActionResult GetByTankNo(int tankNo)
        {
            try
            {
                return Ok(_stationUnitOfWork.TblTanksQuantityRepository.FindAll(b => b.Tank_No == tankNo));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
        
        [HttpGet("GetByDate")]
        public IActionResult GetByDate(DateTime date)
        {
            try
            {
                return Ok(_stationUnitOfWork.TblTanksQuantityRepository.FindAll(b => b.Registeration_Date == date));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetTanksQuantityByStationName")]
        public async Task<ActionResult<TblTanksQuantity>> GetTanksQuantityByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL getTanksQuantityByStationName('{stationName}');";
                Console.WriteLine(sql);
                var entity = await _stationUnitOfWork.DataAccess.LoadData<TblTanksQuantity, dynamic>(sql, new { });

                if (entity == null)
                {
                    return NotFound();
                }

                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetExpandedTanksQuantityByStationName")]
        public async Task<ActionResult<dynamic>> GetExpandedTanksQuantityByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL GetExpandedTanksQuantityByStationName('{stationName}');";
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

        [HttpGet("GetExpandedTanksQuantityByStationNameAndDate")]
        public async Task<ActionResult<dynamic>> GetExpandedTanksQuantityByStationNameAndDate(DateTime date, String stationName)
        {
            try
            {
                string sql = $"Call GetExpandedTanksQuantityByStationNameAndDate('{stationName}', '{date.ToString("yyyy-MM-dd")}');";
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

        [HttpPost("AddTankQuantity")]
        public IActionResult AddTankQuantity([FromBody] TblTanksQuantity tank)
        {
            try
            {
                var tankTemp = _stationUnitOfWork.TblTanksQuantityRepository.Add(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfTankQuantities")]
        public IActionResult AddRangeOfTankQuantities([FromBody] List<TblTanksQuantity> tanks)
        {
            try
            {
                List<TblTanksQuantity> fulltanks = new List<TblTanksQuantity>();
                tanks.ForEach(b => fulltanks.Add(b));
                var tanksIEnumable = _stationUnitOfWork.TblTanksQuantityRepository.AddRange(tanks);
                _stationUnitOfWork.complete();
                return Ok(tanksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdateTankQuantity")]
        public IActionResult UpdateTankQuantity([FromBody] TblTanksQuantity tank)
        {
            try
            {
                var tankTemp = _stationUnitOfWork.TblTanksQuantityRepository.Update(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("DeleteTankQuantity")]
        public IActionResult DeleteTankQuantity(DateTime date, uint tankNo)
        {
            try
            {
                TblTanksQuantity? tank = _stationUnitOfWork.TblTanksQuantityRepository.Find(b => b.Tank_No == tankNo && b.Registeration_Date == date);
                if (tank == null) return Ok(false);
                var tankTemp = _stationUnitOfWork.TblTanksQuantityRepository.Delete(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
