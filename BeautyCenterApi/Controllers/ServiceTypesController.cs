using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.DTOs;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceTypesController : ControllerBase
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;
        private readonly IMapper _mapper;

        public ServiceTypesController(IServiceTypeRepository serviceTypeRepository, IMapper mapper)
        {
            _serviceTypeRepository = serviceTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceTypeDto>>> GetAllServiceTypes()
        {
            try
            {
                var serviceTypes = await _serviceTypeRepository.GetAllAsync();
                var serviceTypeDtos = _mapper.Map<IEnumerable<ServiceTypeDto>>(serviceTypes);
                return Ok(serviceTypeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ServiceTypeDto>>> GetActiveServiceTypes()
        {
            try
            {
                var serviceTypes = await _serviceTypeRepository.GetActiveServicesAsync();
                var serviceTypeDtos = _mapper.Map<IEnumerable<ServiceTypeDto>>(serviceTypes);
                return Ok(serviceTypeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceTypeDto>> GetServiceType(int id)
        {
            try
            {
                var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (serviceType == null)
                {
                    return NotFound(new { message = "Service type not found" });
                }

                var serviceTypeDto = _mapper.Map<ServiceTypeDto>(serviceType);
                return Ok(serviceTypeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<ServiceTypeDto>>> GetServiceTypesByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            try
            {
                var serviceTypes = await _serviceTypeRepository.GetByPriceRangeAsync(minPrice, maxPrice);
                var serviceTypeDtos = _mapper.Map<IEnumerable<ServiceTypeDto>>(serviceTypes);
                return Ok(serviceTypeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceTypeDto>> CreateServiceType([FromBody] CreateServiceTypeDto createServiceTypeDto)
        {
            try
            {
                // Check if service type name already exists
                var existingServiceType = await _serviceTypeRepository.GetByNameAsync(createServiceTypeDto.Name);
                if (existingServiceType != null)
                {
                    return BadRequest(new { message = "A service type with this name already exists" });
                }

                var serviceType = _mapper.Map<ServiceType>(createServiceTypeDto);
                var createdServiceType = await _serviceTypeRepository.AddAsync(serviceType);
                var createdServiceTypeDto = _mapper.Map<ServiceTypeDto>(createdServiceType);

                return CreatedAtAction(nameof(GetServiceType), new { id = createdServiceType.Id }, createdServiceTypeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateServiceType(int id, [FromBody] UpdateServiceTypeDto serviceTypeDto)
        {
            try
            {
                var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (serviceType == null)
                {
                    return NotFound(new { message = "Service type not found" });
                }

                // Check if service type name already exists for another service type
                var existingServiceType = await _serviceTypeRepository.GetByNameAsync(serviceTypeDto.Name);
                if (existingServiceType != null && existingServiceType.Id != id)
                {
                    return BadRequest(new { message = "A service type with this name already exists" });
                }

                _mapper.Map(serviceTypeDto, serviceType);
                serviceType.UpdatedAt = DateTime.UtcNow;
                
                await _serviceTypeRepository.UpdateAsync(serviceType);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateServiceTypeStatus(int id, [FromBody] object statusUpdate)
        {
            try
            {
                var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (serviceType == null)
                {
                    return NotFound(new { message = "Service type not found" });
                }

                // Extract IsActive from the request body
                var isActiveProperty = statusUpdate.GetType().GetProperty("IsActive");
                if (isActiveProperty == null)
                {
                    return BadRequest(new { message = "IsActive property is required" });
                }

                var newIsActive = (bool?)isActiveProperty.GetValue(statusUpdate);
                if (!newIsActive.HasValue)
                {
                    return BadRequest(new { message = "IsActive cannot be null" });
                }

                serviceType.IsActive = newIsActive.Value;
                serviceType.UpdatedAt = DateTime.UtcNow;
                await _serviceTypeRepository.UpdateAsync(serviceType);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            try
            {
                var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (serviceType == null)
                {
                    return NotFound(new { message = "Service type not found" });
                }

                await _serviceTypeRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}