using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using SimpleTicket2.Server.Models;

public class DeleteUserHandler : AuthorizationHandler<DeleteUserRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteUserRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return;

        // Example: Only allow users with a specific claim or username
        // Replace this logic with your own (e.g., check a database, config, etc.)
        if (context.User.HasClaim("CanDeleteUsers", "true") || context.User.Identity.Name == "superadmin")
        {
            context.Succeed(requirement);
        }
    }
}