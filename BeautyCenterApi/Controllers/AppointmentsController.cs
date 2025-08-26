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
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceTypeRepository _serviceTypeRepository;
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IServiceTypeRepository serviceTypeRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAllAsync();
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetWithDetailsAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
                return Ok(appointmentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByCustomer(int customerId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId);
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}/upcoming")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointments(int customerId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(customerId);
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetTodaysAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetTodaysAppointmentsAsync();
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatus(string status)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByStatusAsync(status);
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("service/{serviceTypeId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByService(int serviceTypeId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByServiceTypeIdAsync(serviceTypeId);
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate, endDate);
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<object>> GetRevenue([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var revenue = await _appointmentRepository.GetTotalRevenueAsync(startDate, endDate);
                return Ok(new { totalRevenue = revenue, startDate, endDate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                var appointment = _mapper.Map<Appointment>(createAppointmentDto);
                
                // Get service type to set price
                var serviceType = await _serviceTypeRepository.GetByIdAsync(createAppointmentDto.ServiceTypeId);
                if (serviceType == null)
                {
                    return BadRequest(new { message = "Service type not found" });
                }

                appointment.TotalPrice = serviceType.Price;
                appointment.FinalPrice = serviceType.Price - (createAppointmentDto.DiscountAmount ?? 0);
                
                var createdAppointment = await _appointmentRepository.AddAsync(appointment);
                var createdAppointmentDto = _mapper.Map<AppointmentDto>(createdAppointment);

                return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.Id }, createdAppointmentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                _mapper.Map(updateAppointmentDto, appointment);
                
                // Update final price if discount changed
                if (updateAppointmentDto.DiscountAmount.HasValue)
                {
                    appointment.FinalPrice = appointment.TotalPrice - updateAppointmentDto.DiscountAmount.Value;
                }

                // Update sessions remaining
                if (updateAppointmentDto.SessionsCompleted.HasValue && appointment.SessionsTotal.HasValue)
                {
                    appointment.SessionsRemaining = appointment.SessionsTotal.Value - updateAppointmentDto.SessionsCompleted.Value;
                }

                await _appointmentRepository.UpdateAsync(appointment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] object statusUpdate)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                // Extract status from the request body
                var statusProperty = statusUpdate.GetType().GetProperty("Status");
                if (statusProperty == null)
                {
                    return BadRequest(new { message = "Status property is required" });
                }

                var newStatus = statusProperty.GetValue(statusUpdate)?.ToString();
                if (string.IsNullOrEmpty(newStatus))
                {
                    return BadRequest(new { message = "Status cannot be empty" });
                }

                appointment.Status = newStatus;
                await _appointmentRepository.UpdateAsync(appointment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                await _appointmentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}