using CivicRequest.API.Data;
using CivicRequest.API.DTOs;
using CivicRequest.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using static System.Net.WebRequestMethods;

namespace CivicRequest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/requests
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.Requests
                .Include(r => r.Citizen)
                .Include(r => r.Category)
                .ToListAsync();
            return Ok(requests);
        }

        // GET: api/requests/1
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

        // GET: api/requests/status/Pending
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

        // POST: api/requests
        [HttpPost]
        public async Task<IActionResult> Create(CreateRequestDto dto)
        {
            // Kontrollo nëse citizen ekziston
            var citizen = await _context.Citizens.FindAsync(dto.CitizenId);
            if (citizen == null) return NotFound("Qytetari nuk u gjet!");

            // Kontrollo nëse category ekziston
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
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        // PUT: api/requests/1/status
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
            return Ok(request);
        }

        // DELETE: api/requests/1
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
