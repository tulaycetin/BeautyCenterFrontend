using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.DTOs;
using BeautyCenterApi.Models;
using BeautyCenterApi.Data;
using BeautyCenterApi.Services;

namespace BeautyCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly BeautyCenterDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;

        public PaymentsController(
            IPaymentRepository paymentRepository,
            IAppointmentRepository appointmentRepository,
            BeautyCenterDbContext context,
            IMapper mapper,
            ITenantService tenantService)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _context = context;
            _mapper = mapper;
            _tenantService = tenantService;
        }

        [HttpGet("pending-appointments")]
        public async Task<ActionResult> GetPendingAppointments()
        {
            try
            {
                // Ödemesi tamamlanmamış randevuları getir
                var tenantId = _tenantService.GetCurrentTenantId();
                var pendingAppointments = await _context.Appointments
                    .Include(a => a.Customer)
                    .Include(a => a.ServiceType)
                    .Where(a => a.TenantId == tenantId && a.Status != "Cancelled" && a.Status != "Completed")
                    .Select(a => new
                    {
                        a.Id,
                        CustomerName = a.Customer.FirstName + " " + a.Customer.LastName,
                        a.CustomerId,
                        a.Customer.Phone,
                        ServiceName = a.ServiceType.Name,
                        a.ServiceType.Price,
                        a.FinalPrice,
                        a.AppointmentDate,
                        PaidAmount = _context.Payments
                            .Where(p => p.AppointmentId == a.Id && p.TenantId == tenantId)
                            .Sum(p => (decimal?)p.PaidAmount) ?? 0,
                        RemainingAmount = a.FinalPrice - (_context.Payments
                            .Where(p => p.AppointmentId == a.Id && p.TenantId == tenantId)
                            .Sum(p => (decimal?)p.PaidAmount) ?? 0)
                    })
                    .Where(a => a.RemainingAmount > 0)
                    .OrderBy(a => a.AppointmentDate)
                    .ToListAsync();

                return Ok(pendingAppointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary()
        {
            try
            {
                var summary = new PaymentSummaryDto();
                var tenantId = _tenantService.GetCurrentTenantId();

                // Tüm ödemeleri getir
                var allPayments = await _context.Payments
                    .Where(p => p.TenantId == tenantId)
                    .Include(p => p.Customer)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.ServiceType)
                    .Include(p => p.Installments)
                    .ToListAsync();

                summary.TotalPayments = allPayments.Count;
                summary.TotalRevenue = allPayments.Sum(p => p.TotalAmount);
                summary.TotalPaid = allPayments.Sum(p => p.PaidAmount);
                summary.TotalRemaining = allPayments.Sum(p => p.RemainingAmount);
                summary.CompletedPayments = allPayments.Count(p => p.PaymentStatus == "Completed");
                summary.PartialPayments = allPayments.Count(p => p.PaymentStatus == "Partial");
                summary.PendingPayments = allPayments.Count(p => p.PaymentStatus == "Pending");
                
                // Son 10 ödeme
                summary.RecentPayments = _mapper.Map<List<PaymentDto>>(
                    allPayments.OrderByDescending(p => p.PaymentDate).Take(10)
                );
                
                // Yaklaşan taksitler
                var upcomingInstallments = await _context.PaymentInstallments
                    .Include(pi => pi.Payment)
                        .ThenInclude(p => p.Customer)
                    .Where(pi => pi.Payment.TenantId == tenantId && !pi.IsPaid && pi.DueDate >= DateTime.Today)
                    .OrderBy(pi => pi.DueDate)
                    .Take(20)
                    .ToListAsync();
                    
                summary.UpcomingInstallments = _mapper.Map<List<PaymentInstallmentDto>>(upcomingInstallments);

                return Ok(summary);
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
                var tenantId = _tenantService.GetCurrentTenantId();
                var payment = await _context.Payments
                    .Include(p => p.Customer)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.ServiceType)
                    .Include(p => p.Installments)
                    .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
                    
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

        [HttpGet("date-range")]
        public async Task<ActionResult<List<PaymentDto>>> GetPaymentsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var payments = await _context.Payments
                    .Include(p => p.Customer)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.ServiceType)
                    .Include(p => p.Installments)
                    .Where(p => p.TenantId == tenantId && p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();

                var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);
                return Ok(paymentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Randevu bilgilerini getir
                var tenantId = _tenantService.GetCurrentTenantId();
                var appointment = await _context.Appointments
                    .Include(a => a.ServiceType)
                    .FirstOrDefaultAsync(a => a.Id == createPaymentDto.AppointmentId && a.TenantId == tenantId);
                    
                if (appointment == null)
                {
                    return BadRequest(new { message = "Appointment not found" });
                }

                // Yeni ödeme oluştur
                var payment = new Payment
                {
                    CustomerId = createPaymentDto.CustomerId,
                    AppointmentId = createPaymentDto.AppointmentId,
                    TotalAmount = createPaymentDto.TotalAmount,
                    PaidAmount = createPaymentDto.InitialPayment,
                    RemainingAmount = createPaymentDto.TotalAmount - createPaymentDto.InitialPayment,
                    PaymentMethod = createPaymentDto.PaymentMethod,
                    PaymentStatus = createPaymentDto.InitialPayment >= createPaymentDto.TotalAmount ? "Completed" : "Partial",
                    PaymentDate = createPaymentDto.PaymentDate,
                    Description = createPaymentDto.Description,
                    ReferenceNumber = createPaymentDto.ReferenceNumber,
                    TenantId = _tenantService.IsSuperAdmin() ? appointment.TenantId : _tenantService.GetCurrentTenantId()!.Value,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Taksitler varsa ekle
                if (createPaymentDto.InstallmentCount.HasValue && createPaymentDto.InstallmentCount > 0)
                {
                    var remainingAmount = payment.RemainingAmount;
                    var installmentAmount = remainingAmount / createPaymentDto.InstallmentCount.Value;
                    
                    for (int i = 1; i <= createPaymentDto.InstallmentCount.Value; i++)
                    {
                        var installment = new PaymentInstallment
                        {
                            PaymentId = payment.Id,
                            Amount = installmentAmount,
                            DueDate = createPaymentDto.PaymentDate.AddMonths(i),
                            IsPaid = false,
                            PaymentMethod = createPaymentDto.PaymentMethod,
                            Notes = $"{i}. Taksit",
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.PaymentInstallments.Add(installment);
                    }
                }
                else if (createPaymentDto.Installments != null && createPaymentDto.Installments.Any())
                {
                    // Manuel taksitler
                    foreach (var inst in createPaymentDto.Installments)
                    {
                        var installment = new PaymentInstallment
                        {
                            PaymentId = payment.Id,
                            Amount = inst.Amount,
                            DueDate = inst.DueDate,
                            IsPaid = false,
                            PaymentMethod = createPaymentDto.PaymentMethod,
                            Notes = inst.Notes,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.PaymentInstallments.Add(installment);
                    }
                }

                await _context.SaveChangesAsync();
                
                // Ödeme bilgilerini transaction içinde yükle (commit'ten önce)
                var createdPayment = await _context.Payments
                    .Include(p => p.Customer)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.ServiceType)
                    .Include(p => p.Installments)
                    .FirstOrDefaultAsync(p => p.Id == payment.Id);
                    
                var paymentDto = _mapper.Map<PaymentDto>(createdPayment);
                
                await transaction.CommitAsync();
                
                return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, paymentDto);
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    // Log rollback error but don't rethrow
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("installment/{installmentId}/pay")]
        public async Task<IActionResult> PayInstallment(int installmentId, [FromBody] PayInstallmentDto payDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var installment = await _context.PaymentInstallments
                    .Include(pi => pi.Payment)
                    .FirstOrDefaultAsync(pi => pi.Id == installmentId && pi.Payment.TenantId == tenantId);
                    
                if (installment == null)
                {
                    return NotFound(new { message = "Installment not found" });
                }

                if (installment.IsPaid)
                {
                    return BadRequest(new { message = "Installment already paid" });
                }

                // Taksiti öde
                installment.IsPaid = true;
                installment.PaidDate = DateTime.UtcNow;
                installment.PaymentMethod = payDto.PaymentMethod;
                if (!string.IsNullOrEmpty(payDto.Notes))
                {
                    installment.Notes = payDto.Notes;
                }

                // Ana ödemeyi güncelle
                var payment = installment.Payment;
                payment.PaidAmount += installment.Amount;
                payment.RemainingAmount -= installment.Amount;
                
                if (payment.RemainingAmount <= 0)
                {
                    payment.PaymentStatus = "Completed";
                }
                
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Installment paid successfully" });
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}/add-payment")]
        public async Task<IActionResult> AddPayment(int id, [FromBody] UpdatePaymentDto updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
                    
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found" });
                }

                // Yeni ödeme ekle
                payment.PaidAmount += updateDto.PaymentAmount;
                payment.RemainingAmount -= updateDto.PaymentAmount;
                
                if (payment.RemainingAmount <= 0)
                {
                    payment.PaymentStatus = "Completed";
                    payment.RemainingAmount = 0;
                }
                
                payment.UpdatedAt = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(updateDto.Description))
                {
                    payment.Description = payment.Description + " | " + updateDto.Description;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Payment updated successfully" });
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine($"Rollback error: {rollbackEx.Message}");
                }
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("installments/overdue")]
        public async Task<ActionResult> GetOverdueInstallments()
        {
            try
            {
                var tenantId = _tenantService.GetCurrentTenantId();
                var overdueInstallments = await _context.PaymentInstallments
                    .Include(pi => pi.Payment)
                        .ThenInclude(p => p.Customer)
                    .Where(pi => pi.Payment.TenantId == tenantId && !pi.IsPaid && pi.DueDate < DateTime.Today)
                    .Select(pi => new
                    {
                        pi.Id,
                        pi.Amount,
                        pi.DueDate,
                        CustomerName = pi.Payment.Customer.FirstName + " " + pi.Payment.Customer.LastName,
                        CustomerPhone = pi.Payment.Customer.Phone,
                        DaysOverdue = (DateTime.Today - pi.DueDate).Days
                    })
                    .OrderBy(pi => pi.DueDate)
                    .ToListAsync();

                return Ok(overdueInstallments);
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