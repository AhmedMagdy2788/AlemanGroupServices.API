using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingCompaniesController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationunitOfWork;
        public MarketingCompaniesController(IStationUnitOfWork stationunitOfWork)
        {
            _stationunitOfWork = stationunitOfWork;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_stationunitOfWork.MarketingCompanyRepository.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByMarketingCompanyName")]
        public IActionResult GetByMarketingCompanyName(string marketingCompanyName)
        {
            try
            {
                return Ok(_stationunitOfWork.MarketingCompanyRepository.GetById(marketingCompanyName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByMarketingCompanyNameAsync")]
        public async Task<IActionResult> GetByMarketingCompanyNameAsync(string marketingCompanyName)
        {
            try
            {
                return Ok(await _stationunitOfWork.MarketingCompanyRepository.GetByIdAsync(marketingCompanyName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetmarketingCompanysOrderedByName")]
        public IActionResult GetmarketingCompanysOrderedByName()
        {
            try
            {
                return Ok(_stationunitOfWork.MarketingCompanyRepository.FindAll(
                b => true
                    , null, null, b => b.Marketing_comany));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddmarketingCompany")]
        public IActionResult AddmarketingCompany([FromBody] MarketingCompny marketingCompany)
        {
            try
            {
                var marketingCompanyTemp = _stationunitOfWork.MarketingCompanyRepository.Add(marketingCompany);
                _stationunitOfWork.complete();
                return Ok(marketingCompanyTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddRangeOfMarketingcompanies")]
        public IActionResult AddRangeOfMarketingcompanies([FromBody] List<MarketingCompny> marketingCompanies)
        {
            try
            {
                List<MarketingCompny> fullmarketingCompanies = new List<MarketingCompny>();
                marketingCompanies.ForEach(b => fullmarketingCompanies.Add(b));
                var marketingCompaniesIEnumable = _stationunitOfWork.MarketingCompanyRepository.AddRange(marketingCompanies);
                _stationunitOfWork.complete();
                return Ok(marketingCompaniesIEnumable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdatemarketingCompany")]
        public IActionResult UpdatemarketingCompany([FromBody] MarketingCompny marketingCompany)
        {
            try
            {
                var marketingCompanyTemp = _stationunitOfWork.MarketingCompanyRepository.Update(marketingCompany);
                _stationunitOfWork.complete();
                return Ok(marketingCompanyTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeletemarketingCompany")]
        public IActionResult DeletemarketingCompany(String marketingCompanyName)
        {
            try
            {
                MarketingCompny? marketingCompany = _stationunitOfWork.MarketingCompanyRepository.Find(b => b.Marketing_comany == marketingCompanyName);
                if (marketingCompany == null) return Ok(false);
                var marketingCompanyTemp = _stationunitOfWork.MarketingCompanyRepository.Delete(marketingCompany);
                _stationunitOfWork.complete();
                return Ok(marketingCompanyTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
