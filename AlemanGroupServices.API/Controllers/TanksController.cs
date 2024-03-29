﻿using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlemanGroupServices.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TanksController : ControllerBase
{
    private readonly MySQLDBContext _context;
    private readonly IStationUnitOfWork
       _stationUnitOfWork;
    public TanksController(IStationUnitOfWork stationUnitOfWork, MySQLDBContext context)
    {
        _context = context;
        _stationUnitOfWork = stationUnitOfWork;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            if (_context.Tbltanks is null)
            {
                return NotFound("the table does not exist in database");
            }
            var result = await _context.Tbltanks
                .Join(_context.TblStations,
                tank => tank.Station_id,
                station => station.Id,
                (tank, station) => new TankDTO
                {
                    Tank_No = tank.Tank_No,
                    Tank_Name = tank.Tank_Name,
                    Station_Name = station.Name,
                    Max_Capacity = tank.Max_Capacity
                })
                .ToListAsync();
            return Ok(result);
        }
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    [HttpGet("GetByTankNo")]
    public async Task<IActionResult> GetByTankNo(uint tankNo)
    {
        try
        {
            if (_context.Tbltanks is null)
            {
                return NotFound("the table does not exist in database");
            }
            var result = await _context.Tbltanks
                .Where(tank => tank.Tank_No == tankNo)
                .Join(_context.TblStations,
                tank => tank.Station_id,
                station => station.Id,
                (tank, station) => new TankDTO
                {
                    Tank_No = tank.Tank_No,
                    Tank_Name = tank.Tank_Name,
                    Station_Name = station.Name,
                    Max_Capacity = tank.Max_Capacity
                })
                .FirstOrDefaultAsync();
            return Ok(result);
        }
        catch (Exception ex) { return Problem(ex.ToString()); }

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
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    [HttpGet("GetByStationName")]
    public async Task<IActionResult> GetByStationName(String stationName)
    {
        try
        {
            if (_context.Tbltanks is null)
            {
                return NotFound("the table does not exist in database");
            }
            var station = _context.TblStations.Where(station => station.Name == stationName).FirstOrDefault();
            if (station is null) return NotFound($"There is no station with Name {stationName}...");
            var result = await _context.Tbltanks
                .Where(tank => tank.Station_id == station.Id)
                .Join(_context.TblStations,
                tank => tank.Station_id,
                station => station.Id,
                (tank, station) => new TankDTO
                {
                    Tank_No = tank.Tank_No,
                    Tank_Name = tank.Tank_Name,
                    Station_Name = station.Name,
                    Max_Capacity = tank.Max_Capacity
                })
                .ToListAsync();
            return Ok(result);
            //string sql = $"CALL getStationTanks('{stationName}');";
            //var tanks = await _stationUnitOfWork.DataAccess.LoadData<TankDTO, string>(sql, stationName);
            //return Ok(tanks);
            //return Ok(_stationUnitOfWork.TankRepository.FindAll(
            //b => b.Id == 1));
        }
        catch (Exception ex) { return Problem(ex.ToString()); }

    }

    [HttpGet("GetStationTanksOrderedByName")]
    public async Task<IActionResult> GetTanksOrderedByName(string stationName)
    {
        try
        {
            Guid? stationId = await getStationId(stationName);
            if (stationId == null) return NotFound($"There is no station with name '{stationName}'");
            return Ok(_stationUnitOfWork.TankRepository.FindAll(b => b.Station_id == stationId
            , null, null, b => b.Tank_Name));
        }
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    [HttpGet("GetStationTanksOrderedByNo")]
    public async Task<IActionResult> GetTanksOrderedByNo(string stationName)
    {
        try
        {
            var station = await _stationUnitOfWork.StationRepository.FindAsync(s => s.Name == stationName);
            if (station == null)
            {
                return NotFound($"There is no Staion with name '{stationName}'.");
            }
            return Ok(_stationUnitOfWork.TankRepository.FindAll(b => b.Station_id == station.Id
            , null, null, b => b.Tank_No));
        }
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    [HttpPost("Addtank")]
    public async Task<IActionResult> Addtank([FromBody] TankDTO tank)
    {
        try
        {
            Guid? stationId = await getStationId(tank.Station_Name);
            if (stationId == null) return NotFound($"There is no station with name '{tank.Station_Name}'");
            var tankTemp = _stationUnitOfWork.TankRepository.Add(
                new Tbltank
                {
                    Tank_Name = tank.Tank_Name,
                    Station_id = stationId ?? Guid.Empty,
                    Max_Capacity = tank.Max_Capacity,
                });
            _stationUnitOfWork.complete();
            return Ok(new TankDTO { Tank_No = tankTemp.Tank_No, Max_Capacity = tankTemp.Max_Capacity, Station_Name = tank.Station_Name, Tank_Name = tankTemp.Tank_Name });
        }
        catch (Exception ex) { return Problem(ex.ToString()); }

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
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    [HttpPut("Updatetank")]
    public async Task<IActionResult> Updatetank([FromHeader] uint tankNo, [FromBody] TankDTO tank)
    {
        try
        {
            var stationId = await getStationId(tank.Station_Name);
            if (stationId == null) { return NotFound($"There is no station with name '{tank.Station_Name}'"); }
            var tankTemp = _stationUnitOfWork.TankRepository.Update(new Tbltank
            {
                Tank_No = tank.Tank_No,
                Tank_Name = tank.Tank_Name,
                Max_Capacity = tank.Max_Capacity,
                Station_id = stationId ?? Guid.Empty,
            });
            _stationUnitOfWork.complete();
            return Ok(tankTemp);
        }
        catch (Exception ex)
        {
            return Problem(ex.ToString());
        }
    }

    [HttpDelete("Deletetank")]
    public IActionResult Deletetank([FromHeader] uint tankNo)
    {
        try
        {
            Tbltank? tank = _stationUnitOfWork.TankRepository.Find(b => b.Tank_No == tankNo);
            if (tank == null) return Ok(false);
            var tankTemp = _stationUnitOfWork.TankRepository.Delete(tank);
            _stationUnitOfWork.complete();
            return Ok(tankTemp);
        }
        catch (Exception ex) { return Problem(ex.ToString()); }
    }

    private async Task<Dictionary<string, Guid>> getStationsNameIdPairs()
    {
        string sql = $"select station_id, Id from tblstations";
        var stationIdNameList = await _stationUnitOfWork.DataAccess.LoadData<StationIdNamePairs, dynamic>(sql, new { });

        Dictionary<string, Guid> stationMap = stationIdNameList.ToDictionary(
            s => s.Name,
            s => s.Id
        );
        return stationMap;
    }

    private async Task<Dictionary<Guid, string>> getStationsIdNamePairs()
    {
        string sql = $"select station_id, Id from tblstations";
        var stationIdNameList = await _stationUnitOfWork.DataAccess.LoadData<StationIdNamePairs, dynamic>(sql, new { });

        Dictionary<Guid, string> stationMap = stationIdNameList.ToDictionary(
            s => s.Id,
            s => s.Name
        );
        return stationMap;
    }

    private async Task<Guid?> getStationId(string stationName)
    {
        var station = await _stationUnitOfWork.StationRepository.FindAsync(s => s.Name == stationName);
        if (station == null) return null;
        return station.Id;
    }
}
