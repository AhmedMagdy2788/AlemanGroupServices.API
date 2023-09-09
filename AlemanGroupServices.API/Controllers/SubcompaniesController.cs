using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcompaniesController : ControllerBase
    {
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
                    (subcompany) => subcompany.Name == subcompanyName));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpGet("GetSubcompaniesOrderedByName")]
        public IActionResult GetSubcompaniesOrderedByName()
        {
            try
            {
                return Ok(_stationUnitOfWork.SubcompanyRepository.FindAll(b => true
                , null, null, b => b.Name));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("Addsubcompany")]
        public IActionResult Addsubcompany([FromBody] Subcompany subcompany)
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
        public IActionResult AddRangeOfsubcompanys([FromBody] List<Subcompany> subcompanys)
        {
            try
            {
                List<Subcompany> fullsubcompanys = new List<Subcompany>();
                subcompanys.ForEach(b => fullsubcompanys.Add(b));
                var subcompanysIEnumable = _stationUnitOfWork.SubcompanyRepository.AddRange(subcompanys);
                _stationUnitOfWork.complete();
                return Ok(subcompanysIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("Updatesubcompany")]
        public IActionResult Updatesubcompany([FromBody] Subcompany subcompany)
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
                Subcompany? subcompany = _stationUnitOfWork.SubcompanyRepository.Find(b => b.Name == subcompanyName);
                if (subcompany == null) return Ok(false);
                var subcompanyTemp = _stationUnitOfWork.SubcompanyRepository.Delete(subcompany);
                _stationUnitOfWork.complete();
                return Ok(subcompanyTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
