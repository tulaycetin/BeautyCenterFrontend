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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository customerRepository, IPaymentRepository paymentRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetActiveCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetActiveCustomersAsync();
                var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<CustomerWithDetailsDto>> GetCustomerWithDetails(int id)
        {
            try
            {
                var customer = await _customerRepository.GetWithAppointmentsAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                var customerWithPayments = await _customerRepository.GetWithPaymentsAsync(id);
                var customerDto = _mapper.Map<CustomerWithDetailsDto>(customer);
                
                if (customerWithPayments != null)
                {
                    customerDto.Payments = _mapper.Map<List<PaymentDto>>(customerWithPayments.Payments);
                }

                customerDto.TotalPaid = await _paymentRepository.GetCustomerTotalPaymentsAsync(id);
                customerDto.RemainingBalance = await _paymentRepository.GetCustomerRemainingBalanceAsync(id);

                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var customers = await _customerRepository.SearchAsync(searchTerm);
                var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            try
            {
                // Check if phone number already exists
                var existingCustomer = await _customerRepository.GetByPhoneAsync(customerDto.Phone);
                if (existingCustomer != null)
                {
                    return BadRequest(new { message = "A customer with this phone number already exists" });
                }

                var customer = _mapper.Map<Customer>(customerDto);
                customer.CreatedAt = DateTime.UtcNow;
                
                var createdCustomer = await _customerRepository.AddAsync(customer);
                var createdCustomerDto = _mapper.Map<CustomerDto>(createdCustomer);

                return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                // Check if phone number already exists for another customer
                var existingCustomer = await _customerRepository.GetByPhoneAsync(customerDto.Phone);
                if (existingCustomer != null && existingCustomer.Id != id)
                {
                    return BadRequest(new { message = "A customer with this phone number already exists" });
                }

                _mapper.Map(customerDto, customer);
                customer.UpdatedAt = DateTime.UtcNow;
                
                await _customerRepository.UpdateAsync(customer);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                await _customerRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}