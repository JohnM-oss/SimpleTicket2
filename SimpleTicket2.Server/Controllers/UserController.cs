//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SimpleTicket2.Server.Data;
//using SimpleTicket2.Server.Models;

//namespace SimpleTicket2.Server.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: /User
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<User>>> Get()
//        {
//            var users = await _context.Users.ToListAsync();
//            return Ok(users);
//        }

//        // GET: /User/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<User>> Get(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//                return NotFound();
//            return Ok(user);
//        }

//        // POST: /User
//        [HttpPost]
//        public async Task<ActionResult<User>> Post([FromBody] User user)
//        {
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();
//            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
//        }

//        // PUT: /User/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody] User user)
//        {
//            if (id != user.Id)
//                return BadRequest();

//            _context.Entry(user).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.Users.Any(e => e.Id == id))
//                    return NotFound();
//                else
//                    throw;
//            }

//            return NoContent();
//        }

//        // DELETE: /User/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//                return NotFound();

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}