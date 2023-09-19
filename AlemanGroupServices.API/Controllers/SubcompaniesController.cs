using AlemanGroupServices.Core;
using AlemanGroupServices.Core.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcompaniesController : ControllerBase
    {
        private readonly IStationUnitOfWork _stationUnitOfWork;
        private readonly IMapper _mapper;
        public SubcompaniesController(IStationUnitOfWork stationunitOfWork, IMapper mapper)
        {
            _stationUnitOfWork = stationunitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var scList = await _stationUnitOfWork.SubcompanyRepository.GetAllAsync();
                return Ok(scList.Select(SC => _mapper.Map<SubcompanyResponseDto>(SC)));
            }
            catch (Exception ex) { return Problem(ex.ToString()); }
        }

        [HttpGet("GetRange")]
        public async Task<IActionResult> GetRange(int range, int offset = 0)
        {
            try
            {
                var listSC = await _stationUnitOfWork.SubcompanyRepository.GetRangeAsync(range, sc => sc.Name, offset);
                return Ok(listSC.Select(SC => _mapper.Map<SubcompanyResponseDto>(SC)));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var entity = await _stationUnitOfWork.SubcompanyRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound($"There is no Subcompany with id '{id}'.");
                }
                return Ok(
                    _mapper.Map<SubcompanyResponseDto>(entity)
                  );
            }
            catch (Exception ex) { return Problem(ex.ToString()); }

        }

        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var entity = await _stationUnitOfWork.SubcompanyRepository
                    .FindAsync(e => e.Name == name);
                if (entity == null)
                {
                    return NotFound($"There is no Subcompany with name '{name}'.");
                }
                return Ok(
                    _mapper.Map<SubcompanyResponseDto>(entity)
                  );
            }
            catch (Exception ex) { return Problem(ex.ToString()); }

        }

        [HttpGet("GetAllOrderedByName")]
        public async Task<IActionResult> GetAllOrderedByName()
        {
            try
            {
                var list = await _stationUnitOfWork.SubcompanyRepository.FindAllAsync(b => true, null, null, b => b.Name);
                return Ok(list.Select(e => _mapper.Map<SubcompanyResponseDto>(e)));
            }
            catch (Exception ex) { return Problem(ex.ToString()); }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] SubcompanyCreateDto subcompany)
        {
            try
            {
                var entity = await _stationUnitOfWork.SubcompanyRepository.FindAsync(e => e.Name == subcompany.Name);
                if (entity != null)
                {
                    return BadRequest(_mapper.Map<SubcompanyCreateResponseDto>(subcompany, opts =>
                    {
                        opts.Items["Id"] = null;
                        opts.Items["Success"] = false;
                        opts.Items["ErrorMessage"] = $"Error: Subcompany name  '{entity.Name}' is already exist";
                    }));
                }
                var createdEntity = _stationUnitOfWork.SubcompanyRepository
                    .Add(_mapper
                    .Map<Subcompany>(subcompany));
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<SubcompanyCreateResponseDto>(createdEntity));
            }
            catch (Exception ex)
            {
                return BadRequest(_mapper.Map<SubcompanyCreateResponseDto>(subcompany, opts =>
                {
                    opts.Items["Id"] = null;
                    opts.Items["Success"] = false;
                    opts.Items["ErrorMessage"] = ex.Message;
                }));
            }
        }

        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRange([FromBody] List<SubcompanyCreateDto> subcompanys)
        {
            try
            {
                // validate the input
                if (subcompanys == null || subcompanys.Count == 0)
                {
                    return BadRequest("Input is null or empty");
                }
                // create two lists to store the successful and failed entities
                var successList = new List<Subcompany>();
                var failList = new List<SubcompanyCreateResponseDto>();

                //var fullmarketingCompanies = new List<MarketingCompany>();
                foreach (var sc in subcompanys)
                {
                    // check if the name is null or empty
                    if (string.IsNullOrEmpty(sc.Name))
                    {
                        throw new Exception("Name is required");
                    }

                    // check if the name already exists in the database or in the input list
                    if (await _stationUnitOfWork.SubcompanyRepository.AnyAsync(m => m.Name == sc.Name))
                    {
                        failList.Add(_mapper.Map<SubcompanyCreateResponseDto>(sc, opts =>
                        {
                            opts.Items["Id"] = null;
                            opts.Items["Success"] = false;
                            opts.Items["ErrorMessage"] = $"Error: Subcompany name  '{sc.Name}' is already exist";
                        }));
                    }
                    else if (
                        subcompanys.Count(i => i.Name == sc.Name) > 1)
                    {
                        if (successList.Any(i => sc.Name == i.Name))
                        {
                            failList.Add(_mapper.Map<SubcompanyCreateResponseDto>(sc, opts =>
                            {
                                opts.Items["Id"] = null;
                                opts.Items["Success"] = false;
                                opts.Items["ErrorMessage"] = $"Error: Subcompany name  '{sc.Name}' is dupplicated";
                            }));
                        }
                        else
                        {
                            successList.Add(_mapper.Map<Subcompany>(sc));
                        }
                    }
                    else
                    {
                        successList.Add(_mapper.Map<Subcompany>(sc));

                    }
                }

                var marketingCompaniesIEnumable = await _stationUnitOfWork.SubcompanyRepository.AddRangeAsync(successList);

                _stationUnitOfWork.complete();

                return Ok(
                    new
                    {
                        successList = marketingCompaniesIEnumable.Select(MC => _mapper.Map<SubcompanyResponseDto>(MC)),
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
        public async Task<IActionResult> Update(Guid id, [FromBody] SubcompanyCreateDto subcompany)
        {
            try
            {
                var entity = await _stationUnitOfWork.SubcompanyRepository.FindAsync(e => e.Id == id);
                if (entity == null)
                {
                    return NotFound($"There is no Subcompany with id '{id}'");
                }

                var duplicatedNameEntity = await _stationUnitOfWork.SubcompanyRepository.FindAsync(e => e.Name == subcompany.Name);
                if (duplicatedNameEntity != null && duplicatedNameEntity.Id != id)
                {
                    return BadRequest(_mapper.Map<SubcompanyCreateResponseDto>(subcompany, opts =>
                    {
                        opts.Items["Id"] = id;
                        opts.Items["Success"] = false;
                        opts.Items["ErrorMessage"] = $"Error: Subcompany name  '{entity.Name}' is already exist";
                    }));
                }

                entity.Name = subcompany.Name;
                entity.Address = subcompany.Address;
                entity.Tax_Card = subcompany.Tax_Card;
                entity.Commercial_Registration = subcompany.Commercial_Registration;
                entity.Fax = subcompany.Fax;
                entity.Email = subcompany.Email;

                _stationUnitOfWork.SubcompanyRepository.Update(entity);
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<SubcompanyResponseDto>(entity));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var entity = await _stationUnitOfWork.SubcompanyRepository.FindAsync(e => e.Id == id);
                if (entity == null)
                {
                    return NotFound($"There is no Subcompany with id '{id}'");
                }
                _stationUnitOfWork.SubcompanyRepository.Delete(entity);
                _stationUnitOfWork.complete();
                return Ok(_mapper.Map<SubcompanyResponseDto>(entity));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
