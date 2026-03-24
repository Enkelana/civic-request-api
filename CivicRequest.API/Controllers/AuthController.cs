using CivicRequest.API.DTOs;
using CivicRequest.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CivicRequest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Officer> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<Officer> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null) return BadRequest("Ky email është i regjistruar!");

            var officer = new Officer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(officer, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok("Zyrtari u regjistrua me sukses!");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var officer = await _userManager.FindByEmailAsync(dto.Email);
            if (officer == null) return Unauthorized("Email ose fjalëkalim i gabuar!");

            var validPassword = await _userManager.CheckPasswordAsync(officer, dto.Password);
            if (!validPassword) return Unauthorized("Email ose fjalëkalim i gabuar!");

            var token = GenerateToken(officer);

            return Ok(new AuthResponseDto
            {
                Token = token,
                FullName = officer.FullName,
                Email = officer.Email!,
                Role = officer.Role
            });
        }

        private string GenerateToken(Officer officer)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, officer.Id),
                new Claim(ClaimTypes.Email, officer.Email!),
                new Claim(ClaimTypes.Name, officer.FullName),
                new Claim(ClaimTypes.Role, officer.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var officer = await _userManager.FindByEmailAsync(email!);
            if (officer == null) return NotFound("Zyrtari nuk u gjet!");

            return Ok(new
            {
                officer.FullName,
                officer.Email,
                officer.Role,
                officer.CreatedAt
            });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (email == null) return Unauthorized("Token i pavlefshëm!");

            var officer = await _userManager.FindByEmailAsync(email);
            if (officer == null) return NotFound("Zyrtari nuk u gjet!");

            var isValid = await _userManager.CheckPasswordAsync(officer, dto.CurrentPassword);
            if (!isValid) return BadRequest("Fjalëkalimi aktual është i gabuar!");

            var token = await _userManager.GeneratePasswordResetTokenAsync(officer);
            var result = await _userManager.ResetPasswordAsync(officer, token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok("Fjalëkalimi u ndryshua me sukses!");
        }

    }
}