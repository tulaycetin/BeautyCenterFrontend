using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BeautyCenterApi.Interfaces;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            Console.WriteLine($"=== LOGIN ATTEMPT ===");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }
            
            if (!user.IsActive)
            {
                Console.WriteLine("User is not active");
                return null;
            }

            Console.WriteLine($"User found: {user.Username}");
            Console.WriteLine($"Stored hash: {user.PasswordHash}");
            
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            Console.WriteLine($"Password valid: {isValidPassword}");

            if (!isValidPassword)
                return null;

            Console.WriteLine("Login successful, generating token");
            return GenerateJwtToken(user);
        }

        public async Task<User?> RegisterAsync(User user, string password)
        {
            // Check if username or email already exists
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
                return null;

            existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                return null;

            // Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true; // Ensure user is active by default

            return await _userRepository.AddAsync(user);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
                throw new InvalidOperationException("JWT configuration is missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}