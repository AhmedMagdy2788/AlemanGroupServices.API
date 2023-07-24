using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly MySQLDBContext _context;
        private readonly IStationUnitOfWork
            _stationunitOfWork;
        public StationsController(IStationUnitOfWork stationunitOfWork, MySQLDBContext context)
        {
            _stationunitOfWork = stationunitOfWork;
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stations = await _context.TblStations.ToListAsync();
                return Ok(stations);
                //string sql = "select * from tblstations_view;";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                //return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("StationName")]
        public async Task<IActionResult> GetStationName(string stationName)
        {
            try
            {
                var station = await _context.TblStations.Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return NotFound($"There is no staion with name {stationName}.");
                }
                return Ok(station);
                //string sql = $"select * from tblstations_view where Station_Id = '{stationName}';";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                //return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.GetById(stationName));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetByOwnerCompany")]
        public async Task<IActionResult> GetByOwnerCompany(String ownerCompany)
        {
            try
            {
                var company = await _context.Tblsubcompanies
                    .Where(sc => sc.Subcompany_name == ownerCompany)
                    .FirstOrDefaultAsync();
                if (company == null)
                {
                    return NotFound($"There is no subcompany with name {ownerCompany}.");
                }
                var stations = await _context.TblStations
                    .Where(s => s.Owner_company_Id == company.Id)
                    .ToListAsync();
                return Ok(stations);
                //string sql = $"select * from tblstations_view where owner_company = '{ownerCompany}';";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                //return Ok(stations);
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
                var company = await _context.Tblmarketingcompnies
                    .Where(mc => mc.Marketing_comany == partnerCompany)
                    .FirstOrDefaultAsync();
                if (company == null)
                {
                    return NotFound($"There is no Marketing company with name {partnerCompany}.");
                }
                var stations = await _context.TblStations
                    .Where(s => s.Partner_ship_Id == company.Id)
                    .ToListAsync();
                return Ok(stations);
                //string sql = $"select * from tblstations_view where partner_ship = '{partnerCompany}';";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                //return Ok(stations);
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
                var stations = await _context.TblStations
                    .OrderBy(s => s.Station_Id)
                    .ToListAsync();
                return Ok(stations);
                //string sql = $"select * from tblstations_view ORDER By station_id;";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationDto, dynamic>(sql, new { });
                //return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.FindAll(b => true
                //, null, null, b => b.Station_name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddStation")]
        public async Task<IActionResult> Addstation([FromBody] StationDto stationDto)
        {
            try
            {
                if (_context.TblStations == null)
                {
                    return Problem("Entity set 'MySQLDBContext.TblStations'  is null.");
                }
                var subcompany = _context.Tblsubcompanies.Where(sc => sc.Subcompany_name == stationDto.Owner_company).FirstOrDefault();
                if (subcompany == null) { return NotFound($"There is no Owner Company with name {stationDto.Owner_company}"); }
                MarketingCompny? marketingCompny = null;
                if (stationDto.Partner_ship != null)
                {
                    marketingCompny = _context.Tblmarketingcompnies.Where(mc => mc.Marketing_comany == stationDto.Partner_ship).FirstOrDefault();
                    if (marketingCompny == null)
                    {
                        return NotFound($"There is no Partner company with name {stationDto.Partner_ship}");
                    }
                }
                var addedStation = _context.TblStations.Add(new Station
                {
                    Station_Name = stationDto.Station_name,
                    Location = stationDto.Location,
                    Owner_company_Id = subcompany.Id,
                    Partner_ship_Id = marketingCompny != null ? marketingCompny.Id : null
                }).Entity;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (await isStationExists(stationDto.Station_name))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok(addedStation);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddRangeOfstations")]
        public async Task<IActionResult> AddRangeOfstations([FromBody] List<StationDto> stations)
        {
            try
            {
                if (_context.TblStations == null)
                {
                    return Problem("Entity set 'MySQLDBContext.TblStations' is null.");
                }

                var addedStations = new List<Station>();
                var addedStationsDto = new List<StationDto>();
                var duplicateStationsDto = new List<StationDto>();
                var conflictStationsDto = new List<StationDto>();
                var incompatableStationsDto = new List<StationDto>();

                foreach (var stationDto in stations)
                {
                    if (addedStationsDto.Any(e => e.Station_name == stationDto.Station_name))
                    {
                        duplicateStationsDto.Add(stationDto);
                        continue;
                    }
                    var subcompany = _context.Tblsubcompanies.Where(sc => sc.Subcompany_name == stationDto.Owner_company).FirstOrDefault();

                    if (subcompany == null)
                    {
                        incompatableStationsDto.Add(stationDto);
                        continue;
                    }

                    MarketingCompny? marketingCompny = null;

                    if (stationDto.Partner_ship != null)
                    {
                        marketingCompny = _context.Tblmarketingcompnies.Where(mc => mc.Marketing_comany == stationDto.Partner_ship).FirstOrDefault();

                        if (marketingCompny == null)
                        {
                            incompatableStationsDto.Add(stationDto);
                            continue;
                        }
                    }

                    var existingStation = _context.TblStations.Where(s => s.Station_Name == stationDto.Station_name).FirstOrDefault();

                    if (existingStation != null)
                    {
                        conflictStationsDto.Add(stationDto);
                        continue;
                    }

                    var addedStation = _context.TblStations.Add(new Station
                    {
                        Station_Name = stationDto.Station_name,
                        Location = stationDto.Location,
                        Owner_company_Id = subcompany.Id,
                        Partner_ship_Id = marketingCompny != null ? marketingCompny.Id : null
                    }).Entity;
                    if (addedStation != null)
                    {
                        addedStations.Add(addedStation);
                        addedStationsDto.Add(stationDto);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    foreach (var addedStation in addedStationsDto)
                    {
                        if (await isStationExists(addedStation.Station_name))
                        {
                            conflictStationsDto.Add(addedStation);
                        }
                    }
                }

                var result = new
                {
                    AddedStations = addedStationsDto.Join(addedStations,
                    dto => dto.Station_name,
                    station => station.Station_Name,
                    (dto, station) => new StationDto
                    {
                        Station_id = station.Station_Id,
                        Station_name = dto.Station_name,
                        Location = dto.Location,
                        Owner_company = dto.Owner_company,
                        Partner_ship = dto.Partner_ship,
                    }).ToList(),
                    DuplicateStations = duplicateStationsDto,
                    ConflictStations = conflictStationsDto,
                    IncompatableStations = incompatableStationsDto
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
            //try
            //{
            //    string sql =
            //"insert into tblstations(Station_Id, location, owner_company, partner_ship) ";
            //    stations.ForEach(b => sql += $"values ('{b.Station_name}', {b.Station_id}, '{b.Location}', '{b.Owner_company}', null),");
            //    sql = sql.Remove(sql.Length - 1, 1);
            //    Console.WriteLine(sql);
            //    await _stationunitOfWork.DataAccess.SaveData<List<StationDto>>(sql, stations);
            //    return Ok(stations);
            //}
            //catch (Exception ex) { return BadRequest(ex.ToString()); }
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

        private async Task<bool> isStationExists(string station_name)
        {
            var station = _context.TblStations.Where(s => s.Station_Name == station_name).FirstOrDefault();
            if (station != null) return true;
            return false;
        }
    }
}
