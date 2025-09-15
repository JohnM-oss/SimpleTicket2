using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTicket2.Server.Models;
using System.Security.Claims;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Username, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        _logger.LogInformation($"User creation result: {result.Succeeded}");
        foreach (var error in result.Errors)
            _logger.LogWarning($"Error: {error.Code} - {error.Description}");

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!string.IsNullOrEmpty(dto.Role))
        {
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return BadRequest($"Role '{dto.Role}' does not exist.");
            await _userManager.AddToRoleAsync(user, dto.Role);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);
        if (result.Succeeded)
            return Ok();
        return Unauthorized();
    }

    // GET: api/auth/users
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        var userList = new List<object>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            userList.Add(new
            {
                u.Id,
                u.UserName,
                u.Email,
                Roles = roles
            });
        }

        return Ok(userList);
    }

    // PUT: api/auth/users/{id}
    [HttpPut("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        user.UserName = dto.Username ?? user.UserName;
        user.Email = dto.Email ?? user.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    // DELETE: api/auth/users/{id}
    [HttpDelete("users/{id}")]
    [Authorize(Policy = "CanDeleteUsers")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

    // Example: Assign the "CanDeleteUsers" claim to a user by username
    [HttpPost("grant-delete-permission/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GrantDeletePermission(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound();

        var claim = new Claim("CanDeleteUsers", "true");
        var result = await _userManager.AddClaimAsync(user, claim);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Claim assigned successfully.");
    }
}

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// DTO for updating user
public class UpdateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
}