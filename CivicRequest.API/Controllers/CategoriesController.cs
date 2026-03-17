using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CivicRequest.API.Data;

namespace CivicRequest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // GET: api/categories/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound("Kategoria nuk u gjet!");
            return Ok(category);
        }
    }
}