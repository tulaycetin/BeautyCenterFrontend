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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }

                var paymentDto = _mapper.Map<PaymentDto>(payment);
                return Ok(paymentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByCustomer(int customerId)
        {
            try
            {
                var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}/total")]
        public async Task<ActionResult<object>> GetCustomerTotalPayments(int customerId)
        {
            try
            {
                var totalPaid = await _paymentRepository.GetCustomerTotalPaymentsAsync(customerId);
                var remainingBalance = await _paymentRepository.GetCustomerRemainingBalanceAsync(customerId);
                
                return Ok(new { totalPaid, remainingBalance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByAppointment(int appointmentId)
        {
            try
            {
                var payments = await _paymentRepository.GetByAppointmentIdAsync(appointmentId);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("method/{paymentMethod}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByMethod(string paymentMethod)
        {
            try
            {
                var payments = await _paymentRepository.GetByPaymentMethodAsync(paymentMethod);
                var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("total")]
        public async Task<ActionResult<object>> GetTotalPayments([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var totalPayments = await _paymentRepository.GetTotalPaymentsAsync(startDate, endDate);
                return Ok(new { totalPayments, startDate, endDate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(createPaymentDto);
                var createdPayment = await _paymentRepository.AddAsync(payment);
                var createdPaymentDto = _mapper.Map<PaymentDto>(createdPayment);

                return CreatedAtAction(nameof(GetPayment), new { id = createdPayment.Id }, createdPaymentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] CreatePaymentDto updatePaymentDto)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }

                _mapper.Map(updatePaymentDto, payment);
                payment.UpdatedAt = DateTime.UtcNow;
                
                await _paymentRepository.UpdateAsync(payment);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }

                await _paymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}