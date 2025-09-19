using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeautyCenterApi.Models;
using BeautyCenterApi.Repositories;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Services;
using BeautyCenterApi.DTOs;
using BCrypt.Net;

namespace BeautyCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantService _tenantService;

        public SuperAdminController(
            ITenantRepository tenantRepository,
            IUserRepository userRepository,
            ITenantService tenantService)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _tenantService = tenantService;
        }

        // Tenant yönetimi
        [HttpGet("tenants")]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAllTenants()
        {
            var tenants = await _tenantRepository.GetAllTenantsAsync();
            var tenantDtos = tenants.Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                SubDomain = t.SubDomain,
                Email = t.Email,
                Phone = t.Phone,
                IsActive = t.IsActive,
                SubscriptionPlan = t.SubscriptionPlan,
                SubscriptionStartDate = t.SubscriptionStartDate,
                SubscriptionEndDate = t.SubscriptionEndDate,
                MaxUsers = t.MaxUsers,
                MaxCustomers = t.MaxCustomers,
                CreatedAt = t.CreatedAt,
                UserCount = t.Users.Count(u => u.IsActive),
                CustomerCount = 0 // Bu ayrı bir sorgu gerektirir
            });

            return Ok(tenantDtos);
        }

        [HttpGet("tenants/{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(int id)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(id);
            if (tenant == null)
                return NotFound();

            var tenantDto = new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                SubDomain = tenant.SubDomain,
                Description = tenant.Description,
                Address = tenant.Address,
                Phone = tenant.Phone,
                Email = tenant.Email,
                Website = tenant.Website,
                City = tenant.City,
                Country = tenant.Country,
                PostalCode = tenant.PostalCode,
                IsActive = tenant.IsActive,
                SubscriptionPlan = tenant.SubscriptionPlan,
                SubscriptionStartDate = tenant.SubscriptionStartDate,
                SubscriptionEndDate = tenant.SubscriptionEndDate,
                MaxUsers = tenant.MaxUsers,
                MaxCustomers = tenant.MaxCustomers,
                CreatedAt = tenant.CreatedAt,
                UpdatedAt = tenant.UpdatedAt,
                UserCount = tenant.Users.Count(u => u.IsActive),
                CustomerCount = tenant.Customers.Count(c => c.IsActive)
            };

            return Ok(tenantDto);
        }

        [HttpPost("tenants")]
        public async Task<ActionResult<TenantDto>> CreateTenant(CreateTenantDto createTenantDto)
        {
            // SubDomain benzersizlik kontrolü
            if (await _tenantRepository.SubDomainExistsAsync(createTenantDto.SubDomain))
            {
                return BadRequest("Bu subdomain zaten kullanılıyor.");
            }

            var tenant = new Tenant
            {
                Name = createTenantDto.Name,
                SubDomain = createTenantDto.SubDomain,
                Description = createTenantDto.Description,
                Address = createTenantDto.Address,
                Phone = createTenantDto.Phone,
                Email = createTenantDto.Email,
                Website = createTenantDto.Website,
                City = createTenantDto.City,
                Country = createTenantDto.Country ?? "Türkiye",
                PostalCode = createTenantDto.PostalCode,
                SubscriptionPlan = createTenantDto.SubscriptionPlan ?? "Basic",
                SubscriptionStartDate = createTenantDto.SubscriptionStartDate ?? DateTime.UtcNow,
                SubscriptionEndDate = createTenantDto.SubscriptionEndDate,
                MaxUsers = createTenantDto.MaxUsers ?? 5,
                MaxCustomers = createTenantDto.MaxCustomers ?? 100
            };

            var createdTenant = await _tenantRepository.CreateTenantAsync(tenant);

            var tenantDto = new TenantDto
            {
                Id = createdTenant.Id,
                Name = createdTenant.Name,
                SubDomain = createdTenant.SubDomain,
                Email = createdTenant.Email,
                IsActive = createdTenant.IsActive,
                SubscriptionPlan = createdTenant.SubscriptionPlan,
                CreatedAt = createdTenant.CreatedAt
            };

            return CreatedAtAction(nameof(GetTenant), new { id = createdTenant.Id }, tenantDto);
        }

        [HttpPut("tenants/{id}")]
        public async Task<ActionResult<TenantDto>> UpdateTenant(int id, UpdateTenantDto updateTenantDto)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(id);
            if (tenant == null)
                return NotFound();

            // SubDomain değiştiriliyorsa benzersizlik kontrolü
            if (tenant.SubDomain != updateTenantDto.SubDomain &&
                await _tenantRepository.SubDomainExistsAsync(updateTenantDto.SubDomain))
            {
                return BadRequest("Bu subdomain zaten kullanılıyor.");
            }

            tenant.Name = updateTenantDto.Name;
            tenant.SubDomain = updateTenantDto.SubDomain;
            tenant.Description = updateTenantDto.Description;
            tenant.Address = updateTenantDto.Address;
            tenant.Phone = updateTenantDto.Phone;
            tenant.Email = updateTenantDto.Email;
            tenant.Website = updateTenantDto.Website;
            tenant.City = updateTenantDto.City;
            tenant.Country = updateTenantDto.Country;
            tenant.PostalCode = updateTenantDto.PostalCode;
            tenant.IsActive = updateTenantDto.IsActive;
            tenant.SubscriptionPlan = updateTenantDto.SubscriptionPlan;
            tenant.SubscriptionStartDate = updateTenantDto.SubscriptionStartDate;
            tenant.SubscriptionEndDate = updateTenantDto.SubscriptionEndDate;
            tenant.MaxUsers = updateTenantDto.MaxUsers;
            tenant.MaxCustomers = updateTenantDto.MaxCustomers;

            var updatedTenant = await _tenantRepository.UpdateTenantAsync(tenant);

            var tenantDto = new TenantDto
            {
                Id = updatedTenant.Id,
                Name = updatedTenant.Name,
                SubDomain = updatedTenant.SubDomain,
                IsActive = updatedTenant.IsActive,
                UpdatedAt = updatedTenant.UpdatedAt
            };

            return Ok(tenantDto);
        }

        [HttpDelete("tenants/{id}")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var exists = await _tenantRepository.TenantExistsAsync(id);
            if (!exists)
                return NotFound();

            await _tenantRepository.DeleteTenantAsync(id);
            return NoContent();
        }

        // Admin kullanıcı yönetimi
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                TenantId = u.TenantId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });

            return Ok(userDtos);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role,
                TenantId = user.TenantId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(userDto);
        }

        [HttpPost("users")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            // Username benzersizlik kontrolü
            var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");
            }

            // Email benzersizlik kontrolü
            var existingEmail = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingEmail != null)
            {
                return BadRequest("Bu email adresi zaten kullanılıyor.");
            }

            // TenantId kontrolü (TenantAdmin için)
            if (createUserDto.Role == "TenantAdmin" && createUserDto.TenantId.HasValue)
            {
                var tenant = await _tenantRepository.GetTenantByIdAsync(createUserDto.TenantId.Value);
                if (tenant == null)
                {
                    return BadRequest("Belirtilen tenant bulunamadı.");
                }
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Phone = createUserDto.Phone,
                Role = createUserDto.Role,
                TenantId = createUserDto.Role == "SuperAdmin" ? null : createUserDto.TenantId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Role = createdUser.Role,
                TenantId = createdUser.TenantId,
                IsActive = createdUser.IsActive,
                CreatedAt = createdUser.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, userDto);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Username değiştiriliyorsa benzersizlik kontrolü
            if (user.Username != updateUserDto.Username)
            {
                var existingUser = await _userRepository.GetByUsernameAsync(updateUserDto.Username);
                if (existingUser != null)
                {
                    return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");
                }
            }

            // Email değiştiriliyorsa benzersizlik kontrolü
            if (user.Email != updateUserDto.Email)
            {
                var existingEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
                if (existingEmail != null)
                {
                    return BadRequest("Bu email adresi zaten kullanılıyor.");
                }
            }

            user.Username = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Phone = updateUserDto.Phone;
            user.Role = updateUserDto.Role;
            user.TenantId = updateUserDto.Role == "SuperAdmin" ? null : updateUserDto.TenantId;
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Şifre değiştiriliyorsa
            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _userRepository.UpdateAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                TenantId = user.TenantId,
                IsActive = user.IsActive,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(userDto);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // SuperAdmin'in silinmesini engelle
            if (user.Role == "SuperAdmin")
            {
                return BadRequest("SuperAdmin kullanıcıları silinemez.");
            }

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }

        // Sistem geneli istatistikler
        [HttpGet("dashboard")]
        public async Task<ActionResult<SuperAdminDashboardDto>> GetDashboard()
        {
            var tenants = await _tenantRepository.GetAllTenantsAsync();
            var totalUsers = await _userRepository.GetUserCountAsync();

            var dashboard = new SuperAdminDashboardDto
            {
                TotalTenants = tenants.Count(),
                ActiveTenants = tenants.Count(t => t.IsActive),
                TotalUsers = totalUsers,
                TotalRevenue = 0, // Bu hesaplama ayrı bir service gerektirir
                NewTenantsThisMonth = tenants.Count(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-30))
            };

            return Ok(dashboard);
        }
    }
}