using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DigitalCountersController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public DigitalCountersController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                string sql = "select * from tbldigitalcountersreads;";
                var readingsRows= await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, new { });
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(DateTime registerationDate, int counterNo)
        {
            try
            {
                string sql = $"select * from tbldigitalcountersreads where counter_no = '{counterNo}' AND registeration_date = '{registerationDate}';";
                var readingsRows= await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, new { });
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.GetById(counterNo));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByDate")]
        public async Task<IActionResult> GetByDate(DateTime registeration_date)
        {
            try
            {
                string sql = $"select * from tbldigitalcountersreads where registeration_date = @registeration_date;";
                var readingsRows= await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, registeration_date);
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.FindAll(
                //b => b.Owner_Company_Name == ownerCompany));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByCounterNo")]
        public async Task<IActionResult> GetByCounterNo(int counterNo)
        {
            try
            {
                string sql = $"select * from tbldigitalcountersreads where counter_no = '{counterNo}';";
                var readingsRows= await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, new { });
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.FindAll(
                //b => b.Partner_Ship_Name == partnerCompany));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetstationsOrderedByDate")]
        public async Task<IActionResult> GetstationsOrderedByDate()
        {
            try
            {
                string sql = $"select * from tbldigitalcountersreads ORDER By registeration_date;";
                var readingsRows= await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, new { });
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.FindAll(b => true
                //, null, null, b => b.Name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetstationsOrderedByCounterNo")]
        public async Task<IActionResult> GetstationsOrderedByCounterNo()
        {
            try
            {
                string sql = $"select * from tbldigitalcountersreads ORDER By counter_no;";
                var readingsRows = await _stationUnitOfWork.DataAccess.LoadData<DigitalCountersReads, dynamic>(sql, new { });
                return Ok(readingsRows);
                //return Ok(_stationUnitOfWork.StationRepository.FindAll(b => true
                //, null, null, b => b.Name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddCounterReading")]
        public async Task<IActionResult> AddCounterReading([FromBody] DigitalCountersReads counterReading)
        {
            try
            {
                string sql =
            "insert into tbldigitalcountersreads(registeration_date, counter_no, counter_reading) " +
            $"values (@registeration_date, @counter_no, @counter_reading)";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<DigitalCountersReads>(sql, counterReading);
                return Ok(counterReading);
                //var stationTemp = _stationUnitOfWork.StationRepository.Add(station);
                //_stationUnitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddRangeOfCounterReadings")]
        public async Task<IActionResult> AddRangeOfCounterReadings([FromBody] List<DigitalCountersReads> counterReadings)
        {
            try
            {
                string sql =
            "insert into tbldigitalcountersreads(registeration_date, counter_no, counter_reading) values ";
                counterReadings.ForEach(b => sql += $"('{b.Registeration_Date}', {b.Counter_No}, {b.Counter_Reading}),");
                sql = sql.Remove(sql.Length - 1, 1);
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<List<DigitalCountersReads>>(sql, counterReadings);
                return Ok(counterReadings);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdateCounterReading")]
        public async Task<IActionResult> Updatestation([FromBody] DigitalCountersReads counterReading)
        {
            try
            {
                string sql = "UPDATE tbldigitalcountersreads SET  counter_reading=@counter_reading WHERE counter_no = @counter_no AND registeration_date = @registeration_date;";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<DigitalCountersReads>(sql, counterReading);
                return Ok(counterReading);
                //var stationTemp = _stationUnitOfWork.StationRepository.Update(station);
                //_stationUnitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("Deletestation")]
        public async Task<IActionResult> Deletestation([FromBody] DigitalCountersReads counterReading)
        {
            try
            {
                string sql = $"DELETE FROM tbldigitalcountersreads WHERE counter_no = @counter_no AND registeration_date = @registeration_date;";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<DigitalCountersReads>(sql, counterReading);
                return Ok(counterReading);
                //DigitalCountersReads? station = _stationUnitOfWork.StationRepository.Find(b => b.Name == counterNo);
                //if (station == null) return Ok(false);
                //var stationTemp = _stationUnitOfWork.StationRepository.Delete(station);
                //_stationUnitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
