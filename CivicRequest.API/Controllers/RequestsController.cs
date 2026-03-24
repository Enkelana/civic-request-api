using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CivicRequest.API.Data;
using CivicRequest.API.Models;
using CivicRequest.API.DTOs;

namespace CivicRequest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Services.EmailService _emailService;

        public RequestsController(AppDbContext context, Services.EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.Requests
                .Include(r => r.Citizen)
                .Include(r => r.Category)
                .ToListAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _context.Requests
                .Include(r => r.Citizen)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound("Kërkesa nuk u gjet!");
            return Ok(request);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            if (!Enum.TryParse<RequestStatus>(status, out var requestStatus))
                return BadRequest("Status i pavlefshëm!");

            var requests = await _context.Requests
                .Include(r => r.Citizen)
                .Include(r => r.Category)
                .Where(r => r.Status == requestStatus)
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
      [FromQuery] string? title,
      [FromQuery] string? status,
      [FromQuery] int? categoryId)
        {
            var query = _context.Requests
                .Include(r => r.Citizen)
                .Include(r => r.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(r => r.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<RequestStatus>(status, out var s))
                query = query.Where(r => r.Status == s);

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(r => r.CategoryId == categoryId.Value);

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestDto dto)
        {
            var citizen = await _context.Citizens.FindAsync(dto.CitizenId);
            if (citizen == null) return NotFound("Qytetari nuk u gjet!");

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null) return NotFound("Kategoria nuk u gjet!");

            var request = new Request
            {
                Title = dto.Title,
                Description = dto.Description,
                CitizenId = dto.CitizenId,
                CategoryId = dto.CategoryId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            // Dërgo email njoftim
            try
            {
                var body = _emailService.KerkesaEReTemplate(
                    citizen.FullName,
                    request.Title,
                    category.Name
                );
                await _emailService.SendEmailAsync(
                    citizen.Email,
                    citizen.FullName,
                    "Kërkesa juaj u regjistrua - CivicRequest",
                    body
                );
            }
            catch { /* Email dështoi, por kërkesa u krijua */ }

            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateRequestDto dto)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound("Kërkesa nuk u gjet!");

            if (!Enum.TryParse<RequestStatus>(dto.Status, out var newStatus))
                return BadRequest("Status i pavlefshëm!");

            request.Status = newStatus;
            request.OfficerNotes = dto.OfficerNotes;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Dërgo email njoftim
            try
            {
                var citizen = await _context.Citizens.FindAsync(request.CitizenId);
                if (citizen != null)
                {
                    var statusText = request.Status switch
                    {
                        RequestStatus.InProgress => "Në Process",
                        RequestStatus.Resolved => "Zgjidhur",
                        RequestStatus.Rejected => "Refuzuar",
                        _ => "Në Pritje"
                    };
                    var body = _emailService.StatusNdryshimTemplate(
                        citizen.FullName,
                        request.Title,
                        statusText,
                        request.OfficerNotes ?? ""
                    );
                    await _emailService.SendEmailAsync(
                        citizen.Email,
                        citizen.FullName,
                        "Statusi i kërkesës suaj ndryshoi - CivicRequest",
                        body
                    );
                }
            }
            catch { /* Email dështoi, por statusi u përditësua */ }

            return Ok(request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound("Kërkesa nuk u gjet!");

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok("Kërkesa u fshi!");
        }
    }
}