using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using AlemanGroupServices.Core;

namespace AlemanGroupServices.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PumpsTanksDetailsController : ControllerBase
{
    private readonly MySQLDBContext _context;
    private readonly IStationUnitOfWork _stationUnitOfWork;

    public PumpsTanksDetailsController(MySQLDBContext context, IStationUnitOfWork stationUnitOfWork)
    {  
        _context = context;
        _stationUnitOfWork = stationUnitOfWork;
    }

    // GET: api/PumpsTanksDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PumpsTanksDetail>>> Gettblpumpstanksdetails()
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }
        return await _context.tblpumpstanksdetails.ToListAsync();
    }

    // GET: api/PumpsTanksDetails/5
    [HttpGet("GetPumpsTanksDetailById")]
    public async Task<ActionResult<PumpsTanksDetail>> GetPumpsTanksDetailById(DateTime date, uint pumpNo)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }
        var pumpsTanksDetail = await _context.tblpumpstanksdetails
            .Where(e => e.Date == date && e.Pump_No == pumpNo)
            .FirstOrDefaultAsync();

        if (pumpsTanksDetail == null)
        {
            return NotFound();
        }

        return pumpsTanksDetail;
    }

    [HttpGet("GetPumpTanksAtGeneralDate")]
    public async Task<ActionResult<PumpsTanksDetail>> GetPumpTanksAtGeneralDate(DateTime date, uint pumpNo)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }

        DateTime? maxDate = null;
        var pumps = _context.tblpumpstanksdetails
            .Where(p => p.Date <= date && p.Pump_No == pumpNo);
        if (pumps.Any())
        {
            maxDate = pumps.Max(p => p.Date);
        }
        if (maxDate == null)
        {
            return NotFound($"there is no pumpNo with id '{pumpNo}' installed on tanks at Date  '{date}'");
        }
        var pumpsTanksDetail = await _context.tblpumpstanksdetails
            .Where(e => e.Date == maxDate && e.Pump_No == pumpNo)
            .FirstOrDefaultAsync();

        if (pumpsTanksDetail == null)
        {
            return NotFound();
        }

        return pumpsTanksDetail;
    }

    [HttpGet("GetAllPumpInstallations")]
    public async Task<ActionResult<IEnumerable<PumpsTanksDetail>>> GetAllPumpInstallations(uint pumpNo)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }
        bool isPumpExists = _context.tblstationspumps.Any(p=> p.PumpNo == pumpNo);
        if (!isPumpExists) { return NotFound($"There is no pump number = {pumpNo}"); }
        var pumpsTanksDetail = await _context.tblpumpstanksdetails
            .Where(e => e.Pump_No == pumpNo)
            .ToListAsync();

        if (pumpsTanksDetail.Count <=0)
        {
            return NotFound();
        }

        return pumpsTanksDetail;
    }

    [HttpGet("GetStationPumpsAtGenralDate")]
    public async Task<ActionResult<IEnumerable<PumpsCountersDailyReportDto>>> GetStationPumpsAtGenralDate(string stationName, DateTime date)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }
        #region Get Station Id Name Pairs
        StationIdNamePairs? station = _context.TblStations
            .Select(s => new StationIdNamePairs { Id = s.Id, Name = s.Name })
            .Where(p => p.Name == stationName)
            .FirstOrDefault();
        if (station == null)
        {
            return NotFound($"there is no station with this Name '{stationName}'");
        }
        _context.LogToConsole(station.Name);
        #endregion

        #region Get Station Tanks
        var stationTanks = await _context.Tbltanks
            .Select(t => new { t.Tank_No, t.Station_id })
            .Where(t => t.Station_id == station.Id)
            .ToListAsync();
        if (stationTanks == null)
        {
            return NotFound($"there is no tanks installed in station '{stationName}'");
        }
        #endregion

        var pumpsTanksDetailList = new List<PumpsCountersDailyReportDto>();
        //await _context.tblpumpstanksdetails.Where(p => stationTanks.Any(t => t.Tank_No == p.Tank_No)).ToListAsync();
        #region Get Product Type and Installed Pumps for each Tank at the report time
        foreach (var tank in stationTanks)
        {
            _context.LogToConsole($"station tanks: {tank.Tank_No}");
            PumpsCountersDailyReportDto pumpsCountersDailyReportDto = new PumpsCountersDailyReportDto();

            #region Get Tank Product Type at report date
            //Tbltankscontentstype? tankContentType = null;
            DateTime? tcMaxDate = null;
            var tcDateTemp = await _context.Tbltankscontentstype
                .Where(tc => tc.Tank_No == tank.Tank_No && tc.Date <= date)
                .ToListAsync();
            if (tcDateTemp != null)
            {
                tcMaxDate = tcDateTemp.Max(d => d.Date);
                pumpsCountersDailyReportDto.TankContentType =
                await _context.Tbltankscontentstype
                .Where(tc => tc.Tank_No == tank.Tank_No && tc.Date == tcMaxDate)
                .FirstOrDefaultAsync();
                if (pumpsCountersDailyReportDto.TankContentType != null)
                {
                    _context.LogToConsole($"Tank No '{tank.Tank_No}' contains product '{pumpsCountersDailyReportDto.TankContentType.Product}' ");
                }
            }
            #endregion

            #region Get the last installed pump on tank at reprot date
            DateTime? dateOfLastInstalledPumpOntank = null;
            var datesOfPumpsInstallationOnTank = await _context.tblpumpstanksdetails
                .Where(p => p.Date <= date && p.Tank_No == tank.Tank_No)
                .ToListAsync();
            if (datesOfPumpsInstallationOnTank.Count > 0)
            {
                dateOfLastInstalledPumpOntank = datesOfPumpsInstallationOnTank.Max(d => d.Date);
                var lastInstalledPumpOntank = await _context.tblpumpstanksdetails
                    .Where(p => p.Date == dateOfLastInstalledPumpOntank && p.Tank_No == tank.Tank_No)
                    .FirstOrDefaultAsync();
                if (lastInstalledPumpOntank != null)
                {
                    _context.LogToConsole($"Date of last Installed Pump on Tank No '{tank.Tank_No}' = '{dateOfLastInstalledPumpOntank}' and pump no = '{lastInstalledPumpOntank.Pump_No}'");
                    // Get the pump_no and check if it is installed on other tank at the report Date
                    DateTime? dateOfLastInstallationOfPump = null;
                    var listOfDates = await _context.tblpumpstanksdetails
                        .Where(p => p.Date <= date && p.Pump_No == lastInstalledPumpOntank.Pump_No)
                        .ToListAsync();
                    if (listOfDates != null)
                    {
                        dateOfLastInstallationOfPump = listOfDates.Max(d => d.Date);

                        var pumpLastInstallationTank = await _context.tblpumpstanksdetails
                            .Where(p => p.Date == dateOfLastInstallationOfPump && p.Pump_No == lastInstalledPumpOntank.Pump_No)
                            .FirstOrDefaultAsync();
                        _context.LogToConsole($"the last tank Pump No '{lastInstalledPumpOntank!.Pump_No}' were installed on is tank No '{pumpLastInstallationTank!.Tank_No}'");
                        if (pumpLastInstallationTank!.Tank_No == tank.Tank_No)
                        {
                            pumpsCountersDailyReportDto.PumpInstallationDetails = pumpLastInstallationTank;
                            pumpsTanksDetailList.Add(pumpsCountersDailyReportDto!);
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        if (pumpsTanksDetailList == null)
        {
            return NotFound();
        }

        return pumpsTanksDetailList;
    }

    [HttpGet("GetStationPumpTankDetailDtoAtGenralDate")]
    public async Task<ActionResult<IEnumerable<PumpTankDetailDto>>> GetStationPumpTankDetailDtoAtGenralDate(string stationName, DateTime date)
    {
        try
        {
            if (_context.tblpumpstanksdetails == null)
            {
                return NotFound();
            }

            #region Get Station Id Name Pairs
            StationIdNamePairs? station = _context.TblStations
                .Select(s => new StationIdNamePairs { Id = s.Id, Name = s.Name })
                .Where(p => p.Name == stationName)
                .FirstOrDefault();
            if (station == null)
            {
                return NotFound($"there is no station with this Name '{stationName}'");
            }
            _context.LogToConsole(station.Name);
            #endregion

            #region Get Station Tanks
            var stationTanks = await _context.Tbltanks
                .Select(t => new { t.Tank_No, t.Station_id })
                .Where(t => t.Station_id == station.Id)
                .ToListAsync();
            if (stationTanks == null)
            {
                return NotFound($"there is no tanks installed in station '{stationName}'");
            }
            #endregion

            var pumpsTanksDetailList = new List<PumpTankDetailDto>();
            #region Get Product Type and Installed Pumps for each Tank at the report time
            foreach (var tank in stationTanks)
            {
                _context.LogToConsole($"station tanks: {tank.Tank_No}");
                PumpTankDetailDto pumpTankDetailDto = new PumpTankDetailDto();

                #region Get Tank Product Type at report date
                //Tbltankscontentstype? tankContentType = null;
                DateTime? tcMaxDate = null;
                var tcDateTemp = await _context.Tbltankscontentstype
                    .Where(tc => tc.Tank_No == tank.Tank_No && tc.Date <= date)
                    .ToListAsync();
                var tankContentType = new Tbltankscontentstype();
                if (tcDateTemp != null)
                {
                    tcMaxDate = tcDateTemp.Max(d => d.Date);
                    tankContentType =
                    await _context.Tbltankscontentstype
                    .Where(tc => tc.Tank_No == tank.Tank_No && tc.Date == tcMaxDate)
                    .FirstOrDefaultAsync();
                    if (tankContentType != null)
                    {
                        _context.LogToConsole($"Tank No '{tank.Tank_No}' contains product '{tankContentType.Product}' ");
                    }
                }
                #endregion

                #region Get the last installed pump on tank at reprot date
                DateTime? dateOfLastInstalledPumpOntank = null;
                var datesOfPumpsInstallationOnTank = await _context.tblpumpstanksdetails
                    .Where(p => p.Date <= date && p.Tank_No == tank.Tank_No)
                    .ToListAsync();
                if (datesOfPumpsInstallationOnTank.Count > 0)
                {
                    dateOfLastInstalledPumpOntank = datesOfPumpsInstallationOnTank.Max(d => d.Date);
                    var lastInstalledPumpOntank = await _context.tblpumpstanksdetails
                        .Where(p => p.Date == dateOfLastInstalledPumpOntank && p.Tank_No == tank.Tank_No)
                        .FirstOrDefaultAsync();
                    if (lastInstalledPumpOntank != null)
                    {
                        _context.LogToConsole($"Date of last Installed Pump on Tank No '{tank.Tank_No}' = '{dateOfLastInstalledPumpOntank}' and pump no = '{lastInstalledPumpOntank.Pump_No}'");
                        // Get the pump_no and check if it is installed on other tank at the report Date
                        DateTime? dateOfLastInstallationOfPump = null;
                        var listOfDates = await _context.tblpumpstanksdetails
                            .Where(p => p.Date <= date && p.Pump_No == lastInstalledPumpOntank.Pump_No)
                            .ToListAsync();
                        if (listOfDates != null)
                        {
                            dateOfLastInstallationOfPump = listOfDates.Max(d => d.Date);
                            var pumpLastInstallationTank = await _context.tblpumpstanksdetails
                                .Where(p => p.Date == dateOfLastInstallationOfPump && p.Pump_No == lastInstalledPumpOntank.Pump_No)
                                .FirstOrDefaultAsync();
                            _context.LogToConsole($"the last tank Pump No '{lastInstalledPumpOntank!.Pump_No}' were installed on is tank No '{pumpLastInstallationTank!.Tank_No}'");
                            if (pumpLastInstallationTank!.Tank_No == tank.Tank_No)
                            {
                                string? pump_name = _context.tblstationspumps
                                    .Where(p => p.PumpNo == pumpLastInstallationTank.Pump_No)
                                    .Select(p => p.PumpName)
                                    .FirstOrDefault();
                                string? tank_name = _context.Tbltanks
                                    .Where(p => p.Tank_No == pumpLastInstallationTank.Tank_No)
                                    .Select(p => p.Tank_Name)
                                    .FirstOrDefault();
                                pumpsTanksDetailList.Add(new PumpTankDetailDto
                                {
                                    Date = pumpLastInstallationTank.Date,
                                    Pump_No = pumpLastInstallationTank.Pump_No,
                                    pump_Name = pump_name ?? $"طلمبة {pumpLastInstallationTank.Pump_No}",
                                    Tank_No = pumpLastInstallationTank.Tank_No,
                                    Tank_Name = tank_name ?? $"تانك {pumpLastInstallationTank.Tank_No}",
                                    tankContentType = tankContentType!.Product
                                });
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            if (pumpsTanksDetailList == null)
            {
                return NotFound();
            }

            return pumpsTanksDetailList;
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    // PUT: api/PumpsTanksDetails/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    public async Task<IActionResult> PutPumpsTanksDetail([FromHeader] DateTime date, [FromHeader] uint pump_No, PumpsTanksDetail pumpsTanksDetail)
    {
        if (date != pumpsTanksDetail.Date || pump_No != pumpsTanksDetail.Pump_No)
        {
            return BadRequest("date and pumpNo are the primary key of this object and they dont match the updated object");
        }

        _context.Entry(pumpsTanksDetail).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PumpsTanksDetailExists(date, pump_No))
            {
                return NotFound($"There is no Pump Tank Details exist with date = {date}, and pump no = {pump_No}");
            }
            else
            {
                throw;
            }
        }

        return Ok(pumpsTanksDetail);
    }

    // POST: api/PumpsTanksDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<PumpsTanksDetail>> PostPumpsTanksDetail(PumpsTanksDetail pumpsTanksDetail)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return Problem("Entity set 'MySQLDBContext.tblpumpstanksdetails'  is null.");
        }
        _context.tblpumpstanksdetails.Add(pumpsTanksDetail);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (PumpsTanksDetailExists(pumpsTanksDetail.Date, pumpsTanksDetail.Pump_No))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("GetPumpsTanksDetailById", new { date = pumpsTanksDetail.Date, pumpNo = pumpsTanksDetail.Pump_No }, pumpsTanksDetail);
    }

    // DELETE: api/PumpsTanksDetails/5
    [HttpDelete]
    public async Task<IActionResult> DeletePumpsTanksDetail([FromHeader] DateTime date, [FromHeader] uint pump_No)
    {
        if (_context.tblpumpstanksdetails == null)
        {
            return NotFound();
        }
        var pumpsTanksDetail = await _context.tblpumpstanksdetails
            .Where(e => e.Date == date && e.Pump_No == pump_No)
            .FirstOrDefaultAsync();
        if (pumpsTanksDetail == null)
        {
            return NotFound($"There is no pump tank details with date = { date} and pump_no = {pump_No}");
        }

        _context.tblpumpstanksdetails.Remove(pumpsTanksDetail);
        await _context.SaveChangesAsync();

        return Ok(pumpsTanksDetail);
    }

    private bool PumpsTanksDetailExists(DateTime date, uint pumpNo)
    {
        return (_context.tblpumpstanksdetails?.Any(e => e.Date == date && e.Pump_No == pumpNo)).GetValueOrDefault();
    }
}
