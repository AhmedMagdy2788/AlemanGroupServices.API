using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TanksContentTypeController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public TanksContentTypeController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationUnitOfWork.TankcontentTypeRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("{Date}/{tankNo}")]
        public ActionResult<Tbltankscontentstype> GetById(DateTime date, uint tankNo)
        {
            try
            {
                var entity = _stationUnitOfWork.TankcontentTypeRepository.Find((b) => b.Date == date && b.Tank_No == tankNo);

                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
            
        }

        [HttpGet("getTankcontentType{Date}/{tankNo}")]
        public async Task<ActionResult<Tbltankscontentstype>> getTankcontentType(DateTime date, uint tankNo)
        {
            try { 
                string sql = $"CALL getTankcontentType('{date.ToString("yyyy-MM-dd")}', {tankNo});";
                Console.WriteLine(sql);
                var entity = await _stationUnitOfWork.DataAccess.LoadData<Tbltankscontentstype, dynamic>(sql, new { });

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
                return Ok(_stationUnitOfWork.TankcontentTypeRepository.FindAll(b=> b.Tank_No == tankNo));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetTanksContentTypeByStationName")]
        public async Task<ActionResult<Tbltankscontentstype>> GetTanksContentTypeByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL getTanksContentTypeByStationName('{stationName}');";
                Console.WriteLine(sql);
                var entity = await _stationUnitOfWork.DataAccess.LoadData<Tbltankscontentstype, dynamic>(sql, new { });

                if (entity == null)
                {
                    return NotFound();
                }

                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetExpandedTanksContentTypeByStationNameAndDate")]
        public async Task<IActionResult> GetExpandedTanksContentTypeByStationNameAndDate(DateTime date, string stationName)
        {
            try
            {
                string sql = $"Call GetExpandedTanksContentTypeByStationNameAndDate('{stationName}', '{date.ToString("yyyy-MM-dd")}');";
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


        [HttpGet("GetTanksProducts")]
        public async Task<IActionResult> GetTanksProducts()
        {
            try
            {
                string sql = $"select * from  aleman_db.tblpumptype;";
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

        [HttpPost("AddtankContentType")]
        public IActionResult AddtankContentType([FromBody] Tbltankscontentstype tank)
        {
            try
            {
                var tankTemp = _stationUnitOfWork.TankcontentTypeRepository.Add(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfTankContentType")]
        public IActionResult AddRangeOfTankContentType([FromBody] List<Tbltankscontentstype> tanks)
        {
            try
            {
                List<Tbltankscontentstype> fulltanks = new List<Tbltankscontentstype>();
                tanks.ForEach(b => fulltanks.Add(b));
                var tanksIEnumable = _stationUnitOfWork.TankcontentTypeRepository.AddRange(tanks);
                _stationUnitOfWork.complete();
                return Ok(tanksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdatetankContentType")]
        public IActionResult UpdatetankContentType([FromBody] Tbltankscontentstype tank)
        {
            try
            {
                var tankTemp = _stationUnitOfWork.TankcontentTypeRepository.Update(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("DeleteTankContentType")]
        public IActionResult DeleteTankContentType(DateTime date, uint tankNo)
        {
            try
            {
                Tbltankscontentstype? tank = _stationUnitOfWork.TankcontentTypeRepository.Find(b => b.Tank_No == tankNo && b.Date == date);
                if (tank == null) return Ok(false);
                var tankTemp = _stationUnitOfWork.TankcontentTypeRepository.Delete(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
