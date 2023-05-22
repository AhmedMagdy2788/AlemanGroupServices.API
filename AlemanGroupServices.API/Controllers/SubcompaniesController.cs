using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AlemanGroupServices.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcompaniesController : ControllerBase
    {
        //private readonly IBaseRepository<Tblsubcompany>
        //    _tblSubcompaniesRepository;

        private readonly IStationUnitOfWork
            _stationUnitOfWork;
        public SubcompaniesController(IStationUnitOfWork stationunitOfWork)
        {
            _stationUnitOfWork = stationunitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationUnitOfWork.SubcompanyRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int subcompanyId)
        {
            try
            {
                return Ok(await _stationUnitOfWork.SubcompanyRepository.GetByIdAsync(subcompanyId));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetBySubcompanyName")]
        public IActionResult GetBySubcompanyName(string subcompanyName)
        {
            try
            {
                return Ok(_stationUnitOfWork.SubcompanyRepository.Find(
                    (subcompany) => subcompany.Subcompany_name == subcompanyName));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }


        //[HttpGet("GetByStationName")]
        //public IActionResult GetByStationName(String stationName)
        //{
        //    try
        //    {
        //        return Ok(_stationUnitOfWork.SubcompanyRepository.FindAll(
        //        b => b.Station_Name == stationName));
        //    }
        //    catch (Exception ex) { return BadRequest(ex.ToString()); }

        //}

        [HttpGet("GetSubcompaniesOrderedByName")]
        public IActionResult GetSubcompaniesOrderedByName()
        {
            try
            {
                return Ok(_stationUnitOfWork.SubcompanyRepository.FindAll(b => true
                , null, null, b => b.Subcompany_name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        //[HttpGet("GetStationTanksOrderedByNo")]
        //public IActionResult GetTanksOrderedByNo(string stationName)
        //{
        //    try
        //    {
        //        return Ok(_stationUnitOfWork.SubcompanyRepository.FindAll(b => b.Station_Name == stationName
        //        , null, null, b => b.Tank_No));
        //    }
        //    catch (Exception ex) { return BadRequest(ex.ToString()); }
        //}

        [HttpPost("Addsubcompany")]
        public IActionResult Addsubcompany([FromBody] Tblsubcompany subcompany)
        {
            try
            {
                var subcompanyTemp = _stationUnitOfWork.SubcompanyRepository.Add(subcompany);
                _stationUnitOfWork.complete();
                return Ok(subcompanyTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfsubcompanys")]
        public IActionResult AddRangeOfsubcompanys([FromBody] List<Tblsubcompany> subcompanys)
        {
            try
            {
                List<Tblsubcompany> fullsubcompanys = new List<Tblsubcompany>();
                subcompanys.ForEach(b => fullsubcompanys.Add(b));
                var subcompanysIEnumable = _stationUnitOfWork.SubcompanyRepository.AddRange(subcompanys);
                _stationUnitOfWork.complete();
                return Ok(subcompanysIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("Updatesubcompany")]
        public IActionResult Updatesubcompany([FromBody] Tblsubcompany subcompany)
        {
            try
            {
                var subcompanyTemp = _stationUnitOfWork.SubcompanyRepository.Update(subcompany);
                _stationUnitOfWork.complete();
                return Ok(subcompanyTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("Deletesubcompany")]
        public IActionResult Deletesubcompany(string subcompanyName)
        {
            try
            {
                Tblsubcompany? subcompany = _stationUnitOfWork.SubcompanyRepository.Find(b => b.Subcompany_name == subcompanyName);
                if (subcompany == null) return Ok(false);
                var subcompanyTemp = _stationUnitOfWork.SubcompanyRepository.Delete(subcompany);
                _stationUnitOfWork.complete();
                return Ok(subcompanyTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
