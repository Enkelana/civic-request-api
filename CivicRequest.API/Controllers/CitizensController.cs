using CivicRequest.API.Data;
using CivicRequest.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CivicRequest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CitizensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitizensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/citizens
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var citizens = await _context.Citizens.ToListAsync();
            return Ok(citizens);
        }

        // GET: api/citizens/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var citizen = await _context.Citizens
                .Include(c => c.Requests)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citizen == null) return NotFound("Qytetari nuk u gjet!");
            return Ok(citizen);
        }

        // POST: api/citizens
        [HttpPost]
        public async Task<IActionResult> Create(Citizen citizen)
        {
            // Kontrollo nëse email ekziston
            var exists = await _context.Citizens
                .AnyAsync(c => c.Email == citizen.Email);

            if (exists) return BadRequest("Ky email është i regjistruar!");

            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = citizen.Id }, citizen);
        }

        // PUT: api/citizens/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Citizen updated)
        {
            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null) return NotFound("Qytetari nuk u gjet!");

            citizen.FullName = updated.FullName;
            citizen.Email = updated.Email;
            citizen.Phone = updated.Phone;

            await _context.SaveChangesAsync();
            return Ok(citizen);
        }
    }
}