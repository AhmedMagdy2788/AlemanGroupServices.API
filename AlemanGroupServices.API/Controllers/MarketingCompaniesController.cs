using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingCompaniesController : ControllerBase
    {
        private readonly IStationUnitOfWork _stationunitOfWork;
        private readonly IMapper _mapper;
        public MarketingCompaniesController(IStationUnitOfWork stationunitOfWork, IMapper mapper)
        {
            _stationunitOfWork = stationunitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(
                  _stationunitOfWork.MarketingCompanyRepository
                  .GetAll()
                  .Select(MC => _mapper
                    .Map<MarketingCompanyResponseDto>(MC)
                   )
                );
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetRange")]
        public IActionResult GetRange(int range, int offset = 0)
        {
            try
            {
                var listMC = _stationunitOfWork.MarketingCompanyRepository.GetRange(range, mc => mc.Name, offset);
                return Ok(listMC.Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(Guid id)
        {
            try
            {
                var mc = _stationunitOfWork.MarketingCompanyRepository.GetById(id);
                if (mc == null)
                {
                    return NotFound($"There is no Marketing Company with id '{id}'.");
                }
                return Ok(
                    _mapper.Map<MarketingCompanyResponseDto>(mc)
                  );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByName")]
        public IActionResult GetByName(string name)
        {
            try
            {
                var mc = _stationunitOfWork.MarketingCompanyRepository
                    .GetAll()
                    .FirstOrDefault(MC => MC.Name == name);
                if (mc == null)
                {
                    return NotFound($"There is no Marketing Company with name '{name}'.");
                }
                return Ok(
                    _mapper.Map<MarketingCompanyResponseDto>(mc)
                  );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllOrderedByName")]
        public IActionResult GetAllOrderedByName()
        {
            try
            {
                return Ok(_stationunitOfWork.MarketingCompanyRepository
                    .FindAll(b => true, null, null, b => b.Name)
                    .Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add")]
        public IActionResult AddmarketingCompany([FromBody] MarketingCompanyCreateDto marketingCompany)
        {
            try
            {
                var MC = _stationunitOfWork.MarketingCompanyRepository.Find(mc => mc.Name == marketingCompany.Name);
                if (MC != null)
                {
                    return BadRequest(_mapper.Map<MarketingCompanyCreateResponseDto>(marketingCompany, opts =>
                    {
                        opts.Items["Id"] = null;
                        opts.Items["Success"] = false;
                        opts.Items["ErrorMessage"] = $"Error: Marketing Company name  '{MC.Name}' is already exist";
                    }));
                }
                var marketingCompanyTemp = _stationunitOfWork.MarketingCompanyRepository
                    .Add(_mapper
                    .Map<MarketingCompany>(marketingCompany));
                _stationunitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyCreateResponseDto>(marketingCompanyTemp));
            }
            catch (Exception ex)
            {
                return BadRequest(_mapper.Map<MarketingCompanyCreateResponseDto>(marketingCompany, opts =>
                {
                    opts.Items["Id"] = null;
                    opts.Items["Success"] = false;
                    opts.Items["ErrorMessage"] = ex.Message;
                }));
            }
        }

        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRangeOfMarketingcompanies([FromBody] List<MarketingCompanyCreateDto> marketingCompanies)
        {
            try
            {
                // validate the input
                if (marketingCompanies == null || marketingCompanies.Count == 0)
                {
                    return BadRequest("Input is null or empty");
                }
                // create two lists to store the successful and failed entities
                var successList = new List<MarketingCompany>();
                var failList = new List<MarketingCompanyCreateResponseDto>();

                //var fullmarketingCompanies = new List<MarketingCompany>();
                foreach (var mc in marketingCompanies)
                {
                    // check if the name is null or empty
                    if (string.IsNullOrEmpty(mc.Name))
                    {
                        throw new Exception("Name is required");
                    }

                    // check if the name already exists in the database or in the input list
                    if (await _stationunitOfWork.MarketingCompanyRepository.AnyAsync(m => m.Name == mc.Name))
                    {
                        failList.Add(_mapper.Map<MarketingCompanyCreateResponseDto>(mc, opts =>
                        {
                            opts.Items["Id"] = null;
                            opts.Items["Success"] = false;
                            opts.Items["ErrorMessage"] = $"Error: Marketing Company name  '{mc.Name}' is already exist";
                        }));
                    }
                    else if (
                        marketingCompanies.Count(i => i.Name == mc.Name) > 1)
                    {
                        if (successList.Any(i => mc.Name == i.Name))
                        {
                            failList.Add(_mapper.Map<MarketingCompanyCreateResponseDto>(mc, opts =>
                            {
                                opts.Items["Id"] = null;
                                opts.Items["Success"] = false;
                                opts.Items["ErrorMessage"] = $"Error: Marketing Company name  '{mc.Name}' is dupplicated";
                            }));
                        }
                        else
                        {
                            successList.Add(_mapper.Map<MarketingCompany>(mc));
                        }
                    }
                    else
                    {
                        successList.Add(_mapper.Map<MarketingCompany>(mc));

                    }
                }

                var marketingCompaniesIEnumable = await _stationunitOfWork.MarketingCompanyRepository.AddRangeAsync(successList);

                _stationunitOfWork.complete();

                return Ok(
                    new
                    {
                        successList = marketingCompaniesIEnumable.Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)),
                        faildList = failList
                    });
            }
            catch (Exception ex)
            {
                // Print the outer exception message
                Console.WriteLine(ex.Message);

                // Check if there is an inner exception
                if (ex.InnerException != null)
                {
                    // Print the inner exception message
                    Console.WriteLine(ex.InnerException.Message);
                }
                return Problem(ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdatemarketingCompany(Guid id, [FromBody] MarketingCompanyCreateDto marketingCompany)
        {
            try
            {
                var mc = _stationunitOfWork.MarketingCompanyRepository.Find(MC => MC.Id == id);
                if (mc == null)
                {
                    return NotFound($"There is no Marketing Comapny with id '{id}'");
                }

                mc.Name = marketingCompany.Name;
                mc.Address = marketingCompany.Address;
                mc.Phone = marketingCompany.Phone;
                mc.Fax = marketingCompany.Fax;
                mc.Email = marketingCompany.Email;

                _stationunitOfWork.MarketingCompanyRepository.Update(mc);
                _stationunitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyResponseDto>(mc));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public IActionResult DeletemarketingCompany(Guid id)
        {
            try
            {
                var mc = _stationunitOfWork.MarketingCompanyRepository.Find(MC => MC.Id == id);
                if (mc == null)
                {
                    return NotFound($"There is no Marketing Comapny with id '{id}'");
                }
                _stationunitOfWork.MarketingCompanyRepository.Delete(mc);
                _stationunitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyResponseDto>(mc));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
