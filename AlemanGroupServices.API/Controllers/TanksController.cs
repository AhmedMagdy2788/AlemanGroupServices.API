using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TanksController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public TanksController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationUnitOfWork.TankRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByTankNo")]
        public IActionResult GetByTankNo(int tankNo)
        {
            try
            {
                return Ok(_stationUnitOfWork.TankRepository.GetById(tankNo));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetStationTanksNoNamePairs")]
        public async Task<IActionResult> GetStationTanksNoNamePairs(string stationName)
        {
            try
            {
                string sql = $"CALL getStationTanksNoNamePairs('{stationName}');";
                var tanksNoNamePairs = await _stationUnitOfWork.DataAccess.LoadData<TanksPairs, dynamic>(sql, new { });
                return Ok(tanksNoNamePairs);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetBytankNoAsync")]
        public async Task<IActionResult> GetByIdAsync(uint tankNo)
        {
            try
            {
                return Ok(await _stationUnitOfWork.TankRepository.GetByIdAsync(tankNo));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByStationName")]
        public async Task<IActionResult> GetByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL getStationTanks('{stationName}');";
                var tanks = await _stationUnitOfWork.DataAccess.LoadData<TankDTO, string>(sql, stationName);
                return Ok(tanks);
                //return Ok(_stationUnitOfWork.TankRepository.FindAll(
                //b => b.Station_id == 1));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetStationTanksOrderedByName")]
        public async Task<IActionResult> GetTanksOrderedByName(string stationName)
        {
            try
            {
                int stationId = await getStationId(stationName);
                return Ok(_stationUnitOfWork.TankRepository.FindAll(b => b.Station_id == stationId
                , null, null, b => b.Tank_Name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetStationTanksOrderedByNo")]
        public IActionResult GetTanksOrderedByNo(string stationName)
        {
            try
            {
                return Ok(_stationUnitOfWork.TankRepository.FindAll(b => b.Station_id == 1
                , null, null, b => b.Tank_No));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("Addtank")]
        public async Task<IActionResult> Addtank([FromBody] TankDTO tank)
        {
            try
            {
                int stationId = await getStationId(tank.Station_Name);
                var tankTemp = _stationUnitOfWork.TankRepository.Add(
                    new Tbltank
                    {
                        Tank_Name = tank.Tank_Name,
                        Station_id = stationId,
                        Max_Capacity = tank.Max_Capacity,
                    });
                _stationUnitOfWork.complete();
                return Ok(new TankDTO { Tank_No = tankTemp.Tank_No, Max_Capacity = tankTemp.Max_Capacity, Station_Name = tank.Station_Name, Tank_Name = tankTemp.Tank_Name });
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOftanks")]
        public async Task<IActionResult> AddRangeOftanks([FromBody] List<TankDTO> tanks)
        {
            try
            {
                var stationNameIdPairs = await getStationsNameIdPairs();
                List<Tbltank> fulltanks = new List<Tbltank>();
                tanks.ForEach(b => fulltanks.Add(new Tbltank { Tank_Name = b.Tank_Name, Max_Capacity = b.Max_Capacity, Station_id = stationNameIdPairs[b.Station_Name] }));
                var tanksIEnumable = _stationUnitOfWork.TankRepository.AddRange(fulltanks);
                _stationUnitOfWork.complete();
                return Ok(tanksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("Updatetank")]
        public async Task<IActionResult> Updatetank([FromBody] TankDTO tank)
        {
            try
            {
                int stationId = await getStationId(tank.Station_Name);
                var tankTemp = _stationUnitOfWork.TankRepository.Update(new Tbltank
                {
                    Tank_No = tank.Tank_No,
                    Tank_Name = tank.Tank_Name,
                    Max_Capacity = tank.Max_Capacity,
                    Station_id = stationId
                });
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("Deletetank")]
        public IActionResult Deletetank(uint tankNo)
        {
            try
            {
                Tbltank? tank = _stationUnitOfWork.TankRepository.Find(b => b.Tank_No == tankNo);
                if (tank == null) return Ok(false);
                var tankTemp = _stationUnitOfWork.TankRepository.Delete(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        private async Task<Dictionary<string, int>> getStationsNameIdPairs()
        {
            string sql = $"select station_id, Station_Id from tblstations";
            var stationIdNameList = await _stationUnitOfWork.DataAccess.LoadData<StationIdNamePairs, dynamic>(sql, new { });

            Dictionary<string, int> stationMap = stationIdNameList.ToDictionary(
                s => s.Station_name,
                s => s.Station_id
            );
            return stationMap;
        }

        private async Task<Dictionary<int, string>> getStationsIdNamePairs()
        {
            string sql = $"select station_id, Station_Id from tblstations";
            var stationIdNameList = await _stationUnitOfWork.DataAccess.LoadData<StationIdNamePairs, dynamic>(sql, new { });

            Dictionary<int, string> stationMap = stationIdNameList.ToDictionary(
                s => s.Station_id,
                s => s.Station_name
            );
            return stationMap;
        }

        private async Task<int> getStationId(string stationName)
        {
            string sql = $"select station_id from tblstations where Station_Id = '{stationName}'";
            var stationId = await _stationUnitOfWork.DataAccess.LoadData<int, string>(sql, stationName);
            return stationId[0];
        }
    }
}
