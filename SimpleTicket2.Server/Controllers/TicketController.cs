using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTicket2.Server.Data;
using SimpleTicket2.Server;
using System.Linq.Dynamic.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleTicket2.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Ticket
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false
        )
        {
            var query = _context.Tickets.AsQueryable();
            if(!string.IsNullOrEmpty(sortBy))
            {
                var sortOrder = sortDesc ? "descending" : "ascending";
                query = query.OrderBy($"{sortBy} {sortOrder}");
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }

            var totalCount = await query.CountAsync();

            // Pagination
            List<Ticket> items;
            if (pageSize == -1)
            {
                items = await query.ToListAsync();
            }
            else
            {
                items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return Ok(new
            {
                items,
                totalCount
            });
        }

        // GET: /Ticket/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> Get(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        // POST: /Ticket
        [HttpPost]
        public async Task<ActionResult<Ticket>> Post([FromBody] Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);
        }

        // PUT: /Ticket/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tickets.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: /Ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
