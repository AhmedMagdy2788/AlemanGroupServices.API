using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Const;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly IStationUnitOfWork
            _stationUnitOfWork;
        private readonly MySQLDBContext _dbContext;
        private readonly IMapper _mapper;
        public StationsController(IStationUnitOfWork stationunitOfWork, MySQLDBContext dbContext, IMapper mapper)
        {
            _stationUnitOfWork = stationunitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var stations = await GetStationsReponseAsync(station => true);
                return Ok(stations);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetRange")]
        public async Task<IActionResult> GetRange(int range, int offset = 0)
        {
            try
            {
                var stations = await GetStationsReponseAsync(station => true, range, offset);
                return Ok(stations);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("StationName")]
        public async Task<IActionResult> GetStationName(string stationName)
        {
            try
            {
                var stations = await GetStationsReponseAsync(station => station.Name == stationName);
                if (stations.Count() == 0)
                {
                    return NotFound($"There is no staion with Name {stationName}.");
                }
                return Ok(stations.FirstOrDefault());
            }
            catch (Exception ex) { return Problem(ex.ToString()); }

        }

        [HttpGet("GetByOwnerCompany")]
        public async Task<IActionResult> GetByOwnerCompany(String ownerCompany)
        {
            try
            {
                var company = await _stationUnitOfWork.SubcompanyRepository
                    .FindAsync(sc => sc.Name == ownerCompany);
                if (company == null)
                {
                    return NotFound($"There is no subcompany with Name {ownerCompany}.");
                }
                var stations = await _dbContext.TblStations
                    .Where(s => s.Owner_Company_Id == company.Id)
                    .Join(_dbContext.Tblsubcompanies,
                    station => station.Owner_Company_Id,
                    subcompany => subcompany.Id,
                    (station, subcompany) => new { station, subcompany })
                    .Join(_dbContext.Tblmarketingcompnies,
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
                var company = await _stationUnitOfWork.MarketingCompanyRepository
                    .FindAsync(mc => mc.Name == partnerCompany);
                if (company == null)
                {
                    return NotFound($"There is no Marketing company with Name {partnerCompany}.");
                }
                var stations = await _dbContext.TblStations
                    .Where(s => s.Partner_Ship_Id == company.Id).ToListAsync();
                return Ok(stations);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetstationsOrderedByID")]
        public async Task<IActionResult> GetstationsOrderedByID()
        {
            try
            {
                var stations = await _dbContext.TblStations
                    .OrderBy(s => s.Id)
                    .Join(_dbContext.Tblsubcompanies,
                    station => station.Owner_Company_Id,
                    subcompany => subcompany.Id,
                    (station, subcompany) => new { station, subcompany })
                    .Join(_dbContext.Tblmarketingcompnies,
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
        public async Task<IActionResult> Addstation([FromBody] StationCreateDto stationDto)
        {
            try
            {
                if (_stationUnitOfWork.StationRepository == null)
                {
                    return Problem("Entity set 'MySQLDBContext.StationRepository'  is null.");
                }
                var subcompany = _dbContext.Tblsubcompanies.Where(sc => sc.Name == stationDto.Owner_Company_Name).FirstOrDefault();
                if (subcompany == null) { return NotFound($"There is no Owner Company with Name {stationDto.Owner_Company_Name}"); }
                MarketingCompany? marketingCompny = null;
                if (stationDto.Partner_Ship_Name != null)
                {
                    marketingCompny = _dbContext.Tblmarketingcompnies.Where(mc => mc.Name == stationDto.Partner_Ship_Name).FirstOrDefault();
                    if (marketingCompny == null)
                    {
                        return NotFound($"There is no Partner company with Name {stationDto.Partner_Ship_Name}");
                    }
                }
                var addedStation = _dbContext.TblStations.Add(new Station
                {
                    Name = stationDto.Name,
                    Location = stationDto.Location,
                    Owner_Company_Id = subcompany.Id,
                    Partner_Ship_Id = marketingCompny != null ? marketingCompny.Id : null
                }).Entity;
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    var station = await getStation(stationDto.Name);
                    if (station != null)
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
        public async Task<IActionResult> AddRangeOfstations([FromBody] List<StationCreateDto> stations)
        {
            try
            {
                if (_stationUnitOfWork.StationRepository == null)
                {
                    return Problem("Entity set 'MySQLDBContext.StationRepository' is null.");
                }

                var addedStations = new List<Station>();
                var addedStationsDto = new List<StationCreateDto>();
                var duplicateStationsDto = new List<StationCreateDto>();
                var conflictStationsDto = new List<StationCreateDto>();
                var incompatableStationsDto = new List<StationCreateDto>();

                foreach (var stationDto in stations)
                {
                    if (addedStationsDto.Any(e => e.Name == stationDto.Name))
                    {
                        duplicateStationsDto.Add(stationDto);
                        continue;
                    }
                    var subcompany = _dbContext.Tblsubcompanies.Where(sc => sc.Name == stationDto.Owner_Company_Name).FirstOrDefault();

                    if (subcompany == null)
                    {
                        incompatableStationsDto.Add(stationDto);
                        continue;
                    }

                    MarketingCompany? marketingCompny = null;

                    if (stationDto.Partner_Ship_Name != null)
                    {
                        marketingCompny = _dbContext.Tblmarketingcompnies.Where(mc => mc.Name == stationDto.Partner_Ship_Name).FirstOrDefault();

                        if (marketingCompny == null)
                        {
                            incompatableStationsDto.Add(stationDto);
                            continue;
                        }
                    }

                    var existingStation = _dbContext.TblStations.Where(s => s.Name == stationDto.Name).FirstOrDefault();

                    if (existingStation != null)
                    {
                        conflictStationsDto.Add(stationDto);
                        continue;
                    }

                    var addedStation = _dbContext.TblStations.Add(new Station
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
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    foreach (var addedStation in addedStationsDto)
                    {
                        var station = await getStation(addedStation.Name);
                        if (station != null)
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
        }

        [HttpPut("Updatestation")]
        public async Task<IActionResult> Updatestation([FromBody] StationResponseDto station)
        {
            try
            {
                //string sql = "UPDATE StationRepository SET  station_id= @Id, location = @Location, owner_company = @Owner_Company_Name, partner_ship = @Partner_Ship_Name WHERE Name = @Name;";
                string sql = "CALL update_station(@Name, @Id, @Location, @Owner_Company_Name, @Partner_Ship_Name)";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<StationResponseDto>(sql, station);
                return Ok(station);
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
        public async Task<IActionResult> Deletestation(String stationName)
        {
            try
            {
                string sql = $"DELETE FROM StationRepository WHERE Id = '{stationName}'";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<string>(sql, stationName);
                return Ok(stationName);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        private async Task<IEnumerable<StationResponseDto>> GetStationsReponseAsync(Expression<Func<StationResponseDto, bool>> match, int? take = null, int? skip = null, Expression<Func<StationResponseDto, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            var query = _dbContext.TblStations
                .Join(_dbContext.Tblsubcompanies,
                    station => station.Owner_Company_Id,
                    subcompany => subcompany.Id,
                    (station, subcompany) => new { station, subcompany })
                    .Join(_dbContext.Tblmarketingcompnies,
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
                    );
            query = query.Where(match);
            if (take.HasValue)
                query = query.Take(take.Value);
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            return await query.ToListAsync();
        }
        private async Task<Station?> getStation(string station_name)
        {
            var station = await _stationUnitOfWork.StationRepository.FindAsync(s => s.Name == station_name);
            if (station != null) return station;
            return null;
        }
    }
}
