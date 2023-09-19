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
        private readonly IStationUnitOfWork _stationUnitOfWork;
        private readonly IMapper _mapper;
        public MarketingCompaniesController(IStationUnitOfWork stationunitOfWork, IMapper mapper)
        {
            _stationUnitOfWork = stationunitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entityList = await _stationUnitOfWork.MarketingCompanyRepository.GetAllAsync();
                return Ok(entityList.Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetRange")]
        public async Task<IActionResult> GetRange(int range, int offset = 0)
        {
            try
            {
                var entityList = await _stationUnitOfWork.MarketingCompanyRepository.GetRangeAsync(range, mc => mc.Name, offset);
                return Ok(entityList.Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            try
            {
                var entity = await _stationUnitOfWork.MarketingCompanyRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound($"There is no Marketing Company with id '{id}'.");
                }
                return Ok(
                    _mapper.Map<MarketingCompanyResponseDto>(entity)
                  );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var entity = await _stationUnitOfWork.MarketingCompanyRepository
                    .FindAsync(e => e.Name == name);
                if (entity == null)
                {
                    return NotFound($"There is no Marketing Company with name '{name}'.");
                }
                return Ok(
                    _mapper.Map<MarketingCompanyResponseDto>(entity)
                  );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllOrderedByName")]
        public async Task<IActionResult> GetAllOrderedByName()
        {
            try
            {
                var entityList = await _stationUnitOfWork.MarketingCompanyRepository.FindAllAsync(b => true, null, null, b => b.Name);
                return Ok(entityList.Select(MC => _mapper.Map<MarketingCompanyResponseDto>(MC)));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] MarketingCompanyCreateDto marketingCompany)
        {
            try
            {
                var entity = await _stationUnitOfWork.MarketingCompanyRepository.FindAsync(e => e.Name == marketingCompany.Name);
                if (entity != null)
                {
                    return BadRequest(_mapper.Map<MarketingCompanyCreateResponseDto>(marketingCompany, opts =>
                    {
                        opts.Items["Id"] = null;
                        opts.Items["Success"] = false;
                        opts.Items["ErrorMessage"] = $"Error: Marketing Company name  '{entity.Name}' is already exist";
                    }));
                }
                var createdEntity = _stationUnitOfWork.MarketingCompanyRepository
                    .Add(_mapper
                    .Map<MarketingCompany>(marketingCompany));
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyCreateResponseDto>(createdEntity));
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
        public async Task<IActionResult> AddRange([FromBody] List<MarketingCompanyCreateDto> marketingCompanies)
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
                    if (await _stationUnitOfWork.MarketingCompanyRepository.AnyAsync(m => m.Name == mc.Name))
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

                var marketingCompaniesIEnumable = await _stationUnitOfWork.MarketingCompanyRepository.AddRangeAsync(successList);

                _stationUnitOfWork.complete();

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
        public async Task<IActionResult> UpdatemarketingCompany(Guid id, [FromBody] MarketingCompanyCreateDto marketingCompany)
        {
            try
            {
                var entity = await _stationUnitOfWork.MarketingCompanyRepository.FindAsync(e => e.Id == id);
                if (entity == null)
                {
                    return NotFound($"There is no Marketing Comapny with id '{id}'");
                }
                var duplicatedNameEntity = await _stationUnitOfWork.MarketingCompanyRepository.FindAsync(e => e.Name == marketingCompany.Name);
                if (duplicatedNameEntity != null && duplicatedNameEntity.Id != id)
                {
                    return BadRequest(_mapper.Map<MarketingCompanyCreateResponseDto>(marketingCompany, opts =>
                    {
                        opts.Items["Id"] = id;
                        opts.Items["Success"] = false;
                        opts.Items["ErrorMessage"] = $"Error: Marketing Company name  '{entity.Name}' is already exist";
                    }));
                }

                entity.Name = marketingCompany.Name;
                entity.Address = marketingCompany.Address;
                entity.Phone = marketingCompany.Phone;
                entity.Fax = marketingCompany.Fax;
                entity.Email = marketingCompany.Email;

                _stationUnitOfWork.MarketingCompanyRepository.Update(entity);
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyResponseDto>(entity));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeletemarketingCompany(Guid id)
        {
            try
            {
                var entity = await _stationUnitOfWork.MarketingCompanyRepository.FindAsync(e => e.Id == id);
                if (entity == null)
                {
                    return NotFound($"There is no Marketing Comapny with id '{id}'");
                }
                _stationUnitOfWork.MarketingCompanyRepository.Delete(entity);
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<MarketingCompanyResponseDto>(entity));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
