using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly IStationUnitOfWork
            _stationunitOfWork;
        public StationsController(IStationUnitOfWork stationunitOfWork)
        {
            _stationunitOfWork = stationunitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                string sql = "select * from tblstations_view;";
                var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("StationName")]
        public async Task<IActionResult> GetStationName(string stationName)
        {
            try
            {
                string sql = $"select * from tblstations_view where Station_Id = '{stationName}';";
                var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.GetById(stationName));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByOwnerCompany")]
        public async Task<IActionResult> GetByOwnerCompany(String ownerCompany)
        {
            try
            {
                string sql = $"select * from tblstations_view where owner_company = '{ownerCompany}';";
                var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.FindAll(
                //b => b.Owner_company == ownerCompany));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByPartnerCompany")]
        public async Task<IActionResult> GetByPartnerCompany(string partnerCompany)
        {
            try
            {
                string sql = $"select * from tblstations_view where partner_ship = '{partnerCompany}';";
                var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.FindAll(
                //b => b.Partner_ship == partnerCompany));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetstationsOrderedByID")]
        public async Task<IActionResult> GetstationsOrderedByID()
        {
            try
            {
                string sql = $"select * from tblstations_view ORDER By station_id;";
                var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.FindAll(b => true
                //, null, null, b => b.Station_name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddStation")]
        public async Task<IActionResult> Addstation([FromBody] StationDto station)
        {
            try
            {
                //    string sql =
                //"insert into tblstations_view(Station_Id, station_id, location, owner_company, partner_ship) " +
                //$"values (@Station_name, @Station_id, @Location, @Owner_company, @Partner_ship)";
                string sql = "CALL add_station(@Station_name, @Location, @Owner_company, @Partner_ship)";
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData<StationDto>(sql, station);
                return Ok(station);
                //var stationTemp = _stationunitOfWork.StationRepository.Add(station);
                //_stationunitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddRangeOfstations")]
        public async Task<IActionResult> AddRangeOfstations([FromBody] List<StationDto> stations)
        {
            try
            {
                string sql =
            "insert into tblstations(Station_Id, location, owner_company, partner_ship) ";
                stations.ForEach(b => sql += $"values ('{b.Station_name}', {b.Station_id}, '{b.Location}', '{b.Owner_company}', null),");
                sql = sql.Remove(sql.Length - 1, 1);
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData< List<StationDto>>(sql,stations);
                return Ok(stations);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("Updatestation")]
        public async Task<IActionResult> Updatestation([FromBody] StationDto station)
        {
            try
            {
                //string sql = "UPDATE tblstations SET  station_id= @Station_id, location = @Location, owner_company = @Owner_company, partner_ship = @Partner_ship WHERE Station_name = @Station_name;";
                string sql = "CALL update_station(@Station_name, @Station_id, @Location, @Owner_company, @Partner_ship)";
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData<StationDto>(sql, station);
                return Ok(station);
                //var stationTemp = _stationunitOfWork.StationRepository.Update(station);
                //_stationunitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("Deletestation")]
        public async Task<IActionResult> Deletestation(String stationName)
        {
            try
            {
                string sql = $"DELETE FROM tblstations WHERE Station_Id = '{stationName}'";
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData<string>(sql, stationName);
                return Ok(stationName);
                //StationDto? station = _stationunitOfWork.StationRepository.Find(b => b.Station_name == stationName);
                //if (station == null) return Ok(false);
                //var stationTemp = _stationunitOfWork.StationRepository.Delete(station);
                //_stationunitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
