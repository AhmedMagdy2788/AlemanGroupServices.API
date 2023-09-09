using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using AlemanGroupServices.EF;
using static System.Collections.Specialized.BitVector32;
using System.Threading.Tasks;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TanksEquilibriumController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        private readonly MySQLDBContext _context;
        public TanksEquilibriumController(IStationUnitOfWork stationUnitOfWork, MySQLDBContext context)
        {
            _stationUnitOfWork = stationUnitOfWork;
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<TanksEquilibriumDto>>> GetAll()
        {
            try
            {
                var tanksEquilibriumDtos = await _context.Tbltanksequilibrium
                    .Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new TanksEquilibriumDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity,
                        Notes = tankEquilibrium.Notes
                    })
                    .ToListAsync();
                return Ok(tanksEquilibriumDtos);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }


        [HttpGet("GetById{Date}/{stationName}/{productName}")]
        public async Task<ActionResult<TanksEquilibriumDto?>> GetById(DateTime date, string stationName, string productName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entity = await _context.Tbltanksequilibrium
                    .Where(
                        c => c.Date == date && c.Station_Id == station.Station_Id && c.Product_Name == productName)
                    .Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new TanksEquilibriumDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity,
                        Notes = tankEquilibrium.Notes
                    })
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("getStationTanksEquilibrium{Date}/{stationName}")]
        public async Task<ActionResult<IEnumerable<TanksEquilibriumDto>>> getStationTanksEquilibrium(DateTime date, string stationName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entitiesList = await _context.Tbltanksequilibrium.Where(c => c.Date == date && c.Station_Id == station.Station_Id).Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new TanksEquilibriumDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity,
                        Notes = tankEquilibrium.Notes
                    }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetByProductName{productName}")]
        public async Task<ActionResult<IEnumerable<TanksEquilibriumDto>>> GetByProductName(string productName)
        {
            try
            {
                var entitiesList = await _context.Tbltanksequilibrium.Where(c => c.Product_Name == productName).Join(
                   _context.TblStations,
                   tankEquilibrium => tankEquilibrium.Station_Id,
                   station => station.Id,
                   (tankEquilibrium, station) => new TanksEquilibriumDto
                   {
                       Date = tankEquilibrium.Date,
                       Station_Name = station.Name,
                       Product_Name = tankEquilibrium.Product_Name,
                       Quantity = tankEquilibrium.Quantity,
                       Notes = tankEquilibrium.Notes
                   }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetTanksEquilibriumByStationName{stationName}")]
        public async Task<ActionResult<IEnumerable<TanksEquilibriumDto>>> GetTanksEquilibriumByStationName(string stationName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entitiesList = await _context.Tbltanksequilibrium.Where(c => c.Station_Id == station.Station_Id).Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new TanksEquilibriumDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity,
                        Notes = tankEquilibrium.Notes
                    }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetExpandedTanksEquilibriumByStationName")]
        public async Task<ActionResult<dynamic>> GetExpandedTanksEquilibriumByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL GetExpandedTanksEquilibriumByStationName('{stationName}');";
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
        public async Task<IActionResult> AddTankQuantity([FromBody] TanksEquilibriumDto tankDto)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == tankDto.Station_Name).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{tankDto.Station_Name}'");
                }
                var tankTemp = _stationUnitOfWork.TblTanksEquilibriumRepository.Add(new TanksEquilibrium
                {
                    Date = tankDto.Date,
                    Station_Id = station.Station_Id,
                    Product_Name = tankDto.Product_Name,
                    Quantity = tankDto.Quantity,
                    Notes = tankDto.Notes
                });
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfTankQuantities")]
        public async Task<IActionResult> AddRangeOfTankQuantities([FromBody] List<TanksEquilibriumDto> tanksDtos)
        {
            try
            {
                List<StationIdNamePairs> stationsIdNamePairs = await _context.TblStations.Select(s => new StationIdNamePairs { Id = s.Id, Name = s.Name }).ToListAsync();

                List<TanksEquilibrium> fulltanks = new List<TanksEquilibrium>();
                tanksDtos.ForEach(b =>
                {
                    if (stationsIdNamePairs.Any(e => e.Name == b.Station_Name))
                    {
                        var stationPair = stationsIdNamePairs.Where(e => e.Name.Equals(b.Station_Name)).FirstOrDefault();
                        fulltanks.Add(new TanksEquilibrium
                        {
                            Date = b.Date,
                            Station_Id = stationPair!.Id,
                            Product_Name = b.Product_Name,
                            Quantity = b.Quantity,
                            Notes = b.Notes
                        });
                    }

                });
                var tanksIEnumable = _stationUnitOfWork.TblTanksEquilibriumRepository.AddRange(fulltanks);
                _stationUnitOfWork.complete();
                return Ok(tanksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdateTankQuantity")]
        public async Task<IActionResult> UpdateTankQuantity([FromBody] TanksEquilibriumDto tankDto)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == tankDto.Station_Name).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{tankDto.Station_Name}'");
                }
                var tankTemp = _stationUnitOfWork.TblTanksEquilibriumRepository.Update(new TanksEquilibrium
                {
                    Date = tankDto.Date,
                    Station_Id = station.Station_Id,
                    Product_Name = tankDto.Product_Name,
                    Quantity = tankDto.Quantity,
                    Notes = tankDto.Notes
                });
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("DeleteTankQuantity{Date}/{stationName}/{productName}")]
        public async Task<IActionResult> DeleteTankQuantity(DateTime date, string stationName, string productName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                TanksEquilibrium? tank = _stationUnitOfWork.TblTanksEquilibriumRepository.Find(b => b.Product_Name == productName && b.Date == date && b.Station_Id == station.Station_Id);
                if (tank == null) return Ok(false);
                var tankTemp = _stationUnitOfWork.TblTanksEquilibriumRepository.Delete(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
