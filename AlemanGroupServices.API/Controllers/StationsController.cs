using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public StationsController(IStationUnitOfWork stationunitOfWork, MySQLDBContext context, IMapper mapper)
        {
            _stationunitOfWork = stationunitOfWork;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stations = await _context.TblStations.ToListAsync();
                return Ok(stations);
            }
            catch (Exception ex) { return Problem(ex.ToString()); }
        }

        [HttpGet("StationName")]
        public async Task<IActionResult> GetStationName(string stationName)
        {
            try
            {
                var station = await _context.TblStations.Where(s => s.Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return NotFound($"There is no staion with Name {stationName}.");
                }
                return Ok(station);
            }
            catch (Exception ex) { return Problem(ex.ToString()); }

        }

        [HttpGet("GetByOwnerCompany")]
        public async Task<IActionResult> GetByOwnerCompany(String ownerCompany)
        {
            try
            {
                var company = await _context.Tblsubcompanies
                    .Where(sc => sc.Name == ownerCompany)
                    .FirstOrDefaultAsync();
                if (company == null)
                {
                    return NotFound($"There is no subcompany with Name {ownerCompany}.");
                }
                var stations = await _context.TblStations
                    .Where(s => s.Owner_Company_Id == company.Id)
                    .Join(_context.Tblsubcompanies,
                    station => station.Owner_Company_Id,
                    subcompany => subcompany.Id,
                    (station, subcompany) => new { station, subcompany })
                    .Join(_context.Tblmarketingcompnies,
                    sc => sc.station.Partner_Ship_Id,
                    marketingCompany => marketingCompany.Id,
                    (sc, marketingCompany) => new StationResponseDto
                    {
                        Id = sc.station.Id,
                        Name = sc.station.Name,
                        Location = sc.station.Location,
                        Owner_Company_Name = sc.subcompany.Name,
                        Partner_Ship_Name = marketingCompany.Name
                    }
                    )
                    .ToListAsync();
                return Ok(stations);
            }
            catch (Exception ex) { return Problem(ex.ToString()); }

        }

        [HttpGet("GetByPartnerCompany")]
        public async Task<IActionResult> GetByPartnerCompany(string partnerCompany)
        {
            try
            {
                var company = await _context.Tblmarketingcompnies
                    .Where(mc => mc.Name == partnerCompany)
                    .FirstOrDefaultAsync();
                if (company == null)
                {
                    return NotFound($"There is no Marketing company with Name {partnerCompany}.");
                }
                var stations = await _context.TblStations
                    .Where(s => s.Partner_Ship_Id == company.Id)
                    .ToListAsync();
                return Ok(stations);
                //string sql = $"select * from tblstations_view where partner_ship = '{partnerCompany}';";
                //var stations = await _stationunitOfWork.DataAccess.LoadData<StationResponseDto, dynamic>(sql, new { });
                //return Ok(stations);
                //return Ok(_stationunitOfWork.StationRepository.FindAll(
                //b => b.Partner_Ship_Name == partnerCompany));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetstationsOrderedByID")]
        public async Task<IActionResult> GetstationsOrderedByID()
        {
            try
            {
                var stations = await _context.TblStations
                    .OrderBy(s => s.Id)
                    .Join(_context.Tblsubcompanies,
                    station => station.Owner_Company_Id,
                    subcompany => subcompany.Id,
                    (station, subcompany) => new { station, subcompany })
                    .Join(_context.Tblmarketingcompnies,
                    sc => sc.station.Partner_Ship_Id,
                    marketingCompany => marketingCompany.Id,
                    (sc, marketingCompany) => new StationResponseDto
                    {
                        Id = sc.station.Id,
                        Name = sc.station.Name,
                        Location = sc.station.Location,
                        Owner_Company_Name = sc.subcompany.Name,
                        Partner_Ship_Name = marketingCompany.Name
                    }
                    )
                    .ToListAsync();
                //var sttionsDtos = _mapper.Map<List<StationResponseDto>>(stations);
                return Ok(stations);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddStation")]
        public async Task<IActionResult> Addstation([FromBody] StationResponseDto stationDto)
        {
            try
            {
                if (_context.TblStations == null)
                {
                    return Problem("Entity set 'MySQLDBContext.TblStations'  is null.");
                }
                var subcompany = _context.Tblsubcompanies.Where(sc => sc.Name == stationDto.Owner_Company_Name).FirstOrDefault();
                if (subcompany == null) { return NotFound($"There is no Owner Company with Name {stationDto.Owner_Company_Name}"); }
                MarketingCompany? marketingCompny = null;
                if (stationDto.Partner_Ship_Name != null)
                {
                    marketingCompny = _context.Tblmarketingcompnies.Where(mc => mc.Name == stationDto.Partner_Ship_Name).FirstOrDefault();
                    if (marketingCompny == null)
                    {
                        return NotFound($"There is no Partner company with Name {stationDto.Partner_Ship_Name}");
                    }
                }
                var addedStation = _context.TblStations.Add(new Station
                {
                    Name = stationDto.Name,
                    Location = stationDto.Location,
                    Owner_Company_Id = subcompany.Id,
                    Partner_Ship_Id = marketingCompny != null ? marketingCompny.Id : null
                }).Entity;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (await isStationExists(stationDto.Name))
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
        public async Task<IActionResult> AddRangeOfstations([FromBody] List<StationResponseDto> stations)
        {
            try
            {
                if (_context.TblStations == null)
                {
                    return Problem("Entity set 'MySQLDBContext.TblStations' is null.");
                }

                var addedStations = new List<Station>();
                var addedStationsDto = new List<StationResponseDto>();
                var duplicateStationsDto = new List<StationResponseDto>();
                var conflictStationsDto = new List<StationResponseDto>();
                var incompatableStationsDto = new List<StationResponseDto>();

                foreach (var stationDto in stations)
                {
                    if (addedStationsDto.Any(e => e.Name == stationDto.Name))
                    {
                        duplicateStationsDto.Add(stationDto);
                        continue;
                    }
                    var subcompany = _context.Tblsubcompanies.Where(sc => sc.Name == stationDto.Owner_Company_Name).FirstOrDefault();

                    if (subcompany == null)
                    {
                        incompatableStationsDto.Add(stationDto);
                        continue;
                    }

                    MarketingCompany? marketingCompny = null;

                    if (stationDto.Partner_Ship_Name != null)
                    {
                        marketingCompny = _context.Tblmarketingcompnies.Where(mc => mc.Name == stationDto.Partner_Ship_Name).FirstOrDefault();

                        if (marketingCompny == null)
                        {
                            incompatableStationsDto.Add(stationDto);
                            continue;
                        }
                    }

                    var existingStation = _context.TblStations.Where(s => s.Name == stationDto.Name).FirstOrDefault();

                    if (existingStation != null)
                    {
                        conflictStationsDto.Add(stationDto);
                        continue;
                    }

                    var addedStation = _context.TblStations.Add(new Station
                    {
                        Name = stationDto.Name,
                        Location = stationDto.Location,
                        Owner_Company_Id = subcompany.Id,
                        Partner_Ship_Id = marketingCompny != null ? marketingCompny.Id : null
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
                        if (await isStationExists(addedStation.Name))
                        {
                            conflictStationsDto.Add(addedStation);
                        }
                    }
                }

                var result = new
                {
                    AddedStations = addedStationsDto.Join(addedStations,
                    dto => dto.Name,
                    station => station.Name,
                    (dto, station) => new StationResponseDto
                    {
                        Id = station.Id,
                        Name = dto.Name,
                        Location = dto.Location,
                        Owner_Company_Name = dto.Owner_Company_Name,
                        Partner_Ship_Name = dto.Partner_Ship_Name,
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
            //"insert into tblstations(Id, location, owner_company, partner_ship) ";
            //    stations.ForEach(b => sql += $"values ('{b.Name}', {b.Id}, '{b.Location}', '{b.Owner_Company_Name}', null),");
            //    sql = sql.Remove(sql.Length - 1, 1);
            //    Console.WriteLine(sql);
            //    await _stationunitOfWork.DataAccess.SaveData<List<StationResponseDto>>(sql, stations);
            //    return Ok(stations);
            //}
            //catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("Updatestation")]
        public async Task<IActionResult> Updatestation([FromBody] StationResponseDto station)
        {
            try
            {
                //string sql = "UPDATE tblstations SET  station_id= @Id, location = @Location, owner_company = @Owner_Company_Name, partner_ship = @Partner_Ship_Name WHERE Name = @Name;";
                string sql = "CALL update_station(@Name, @Id, @Location, @Owner_Company_Name, @Partner_Ship_Name)";
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData<StationResponseDto>(sql, station);
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
                string sql = $"DELETE FROM tblstations WHERE Id = '{stationName}'";
                Console.WriteLine(sql);
                await _stationunitOfWork.DataAccess.SaveData<string>(sql, stationName);
                return Ok(stationName);
                //StationResponseDto? station = _stationunitOfWork.StationRepository.Find(b => b.Name == stationName);
                //if (station == null) return Ok(false);
                //var stationTemp = _stationunitOfWork.StationRepository.Delete(station);
                //_stationunitOfWork.complete();
                //return Ok(stationTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        private async Task<bool> isStationExists(string station_name)
        {
            var station = _context.TblStations.Where(s => s.Name == station_name).FirstOrDefault();
            if (station != null) return true;
            return false;
        }
    }
}
