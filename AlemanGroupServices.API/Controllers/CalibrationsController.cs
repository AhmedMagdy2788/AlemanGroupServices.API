using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using AlemanGroupServices.Core;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalibrationsController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        private readonly MySQLDBContext _context;

        public CalibrationsController(IStationUnitOfWork stationUnitOfWork, MySQLDBContext context)
        {
            _stationUnitOfWork = stationUnitOfWork;
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<CalibrationDto>>> GetAll()
        {
            try
            {
                var CalibrationDtos = await _context.Tblcalibration
                    .Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new CalibrationDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity
                    })
                    .ToListAsync();
                return Ok(CalibrationDtos);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetById{Date}/{stationName}/{productName}")]
        public async Task<ActionResult<CalibrationDto?>> GetById(DateTime date, string stationName, string productName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entity = await _context.Tblcalibration
                    .Where(
                        c => c.Date == date && c.Station_Id == station.Station_Id && c.Product_Name == productName)
                    .Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new CalibrationDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity
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

        [HttpGet("getStationCalibration{Date}/{stationName}")]
        public async Task<ActionResult<IEnumerable<CalibrationDto>>> getStationCalibration(DateTime date, string stationName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entitiesList = await _context.Tblcalibration.Where(c => c.Date == date && c.Station_Id == station.Station_Id).Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new CalibrationDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity
                    }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetByProductName{productName}")]
        public async Task<ActionResult<IEnumerable<CalibrationDto>>> GetByProductName(string productName)
        {
            try
            {
                var entitiesList = await _context.Tblcalibration.Where(c => c.Product_Name == productName).Join(
                   _context.TblStations,
                   tankEquilibrium => tankEquilibrium.Station_Id,
                   station => station.Id,
                   (tankEquilibrium, station) => new CalibrationDto
                   {
                       Date = tankEquilibrium.Date,
                       Station_Name = station.Name,
                       Product_Name = tankEquilibrium.Product_Name,
                       Quantity = tankEquilibrium.Quantity
                   }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetCalibrationByStationName{stationName}")]
        public async Task<ActionResult<IEnumerable<CalibrationDto>>> GetCalibrationByStationName(string stationName)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == stationName).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{stationName}'");
                }
                var entitiesList = await _context.Tblcalibration.Where(c => c.Station_Id == station.Station_Id).Join(
                    _context.TblStations,
                    tankEquilibrium => tankEquilibrium.Station_Id,
                    station => station.Id,
                    (tankEquilibrium, station) => new CalibrationDto
                    {
                        Date = tankEquilibrium.Date,
                        Station_Name = station.Station_Name,
                        Product_Name = tankEquilibrium.Product_Name,
                        Quantity = tankEquilibrium.Quantity
                    }).ToListAsync();
                return Ok(entitiesList);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetExpandedCalibrationByStationName")]
        public async Task<ActionResult<dynamic>> GetExpandedCalibrationByStationName(String stationName)
        {
            try
            {
                string sql = $"CALL GetExpandedCalibrationByStationName('{stationName}');";
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

        [HttpPost("AddCalibration")]
        public async Task<IActionResult> AddCalibration([FromBody] CalibrationDto tankDto)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == tankDto.Station_Name).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{tankDto.Station_Name}'");
                }
                var tankTemp = _stationUnitOfWork.CalibrationRepository.Add(new Calibration
                {
                    Date = tankDto.Date,
                    Station_Id = station.Station_Id,
                    Product_Name = tankDto.Product_Name,
                    Quantity = tankDto.Quantity
                });
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfCalibrations")]
        public async Task<IActionResult> AddRangeOfCalibrations([FromBody] List<CalibrationDto> tanksDtos)
        {
            try
            {
                List<StationIdNamePairs> stationsIdNamePairs = await _context.TblStations.Select(s => new StationIdNamePairs { Id = s.Id, Name = s.Name }).ToListAsync();

                List<Calibration> calibrationList = new List<Calibration>();
                tanksDtos.ForEach(b =>
                {
                    if (stationsIdNamePairs.Any(e => e.Name == b.Station_Name))
                    {
                        var stationPair = stationsIdNamePairs.Where(e => e.Name == b.Station_Name).FirstOrDefault();
                        calibrationList.Add(new Calibration
                        {
                            Date = b.Date,
                            Station_Id = stationPair!.Id,
                            Product_Name = b.Product_Name,
                            Quantity = b.Quantity
                        });
                    }

                });
                var tanksIEnumable = _stationUnitOfWork.CalibrationRepository.AddRange(calibrationList);
                _stationUnitOfWork.complete();
                return Ok(tanksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdateTankQuantity")]
        public async Task<IActionResult> UpdateTankQuantity([FromBody] CalibrationDto tankDto)
        {
            try
            {
                var station = await _context.TblStations.Select(s => new { s.Id, s.Name }).Where(s => s.Station_Name == tankDto.Station_Name).FirstOrDefaultAsync();
                if (station == null)
                {
                    return BadRequest($"there no station with Name '{tankDto.Station_Name}'");
                }
                var tankTemp = _stationUnitOfWork.CalibrationRepository.Update(new Calibration
                {
                    Date = tankDto.Date,
                    Station_Id = station.Station_Id,
                    Product_Name = tankDto.Product_Name,
                    Quantity = tankDto.Quantity
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
                Calibration? tank = _stationUnitOfWork.CalibrationRepository.Find(b => b.Product_Name == productName && b.Date == date && b.Station_Id == station.Station_Id);
                if (tank == null) return Ok(false);
                var tankTemp = _stationUnitOfWork.CalibrationRepository.Delete(tank);
                _stationUnitOfWork.complete();
                return Ok(tankTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        private bool CalibrationExists(DateTime date, int stationId, string productName)
        {
            return (_context.Tblcalibration?.Any(e => e.Date == date && e.Station_Id == stationId && e.Product_Name == productName)).GetValueOrDefault();
        }
    }
}
