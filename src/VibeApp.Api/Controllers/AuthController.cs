using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace VibeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser 
            { 
                UserName = dto.Email, 
                Email = dto.Email 
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created successfully", dto.Email);
                return Ok(new { message = "User registered successfully", userId = user.Id });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, new { error = "Failed to register user" });
        }
    }

    /// <summary>
    /// Login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(
                dto.Email, 
                dto.Password, 
                dto.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                var roles = await _userManager.GetRolesAsync(user!);
                
                _logger.LogInformation("User {Email} logged in successfully", dto.Email);
                return Ok(new 
                { 
                    message = "Login successful",
                    userId = user!.Id,
                    email = user.Email,
                    roles = roles
                });
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { error = "User account locked out" });
            }

            return Unauthorized(new { error = "Invalid email or password" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { error = "Failed to login" });
        }
    }

    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { error = "Failed to logout" });
        }
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            // Check if user is actually authenticated
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new { error = "User is not authenticated" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { error = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new 
            { 
                userId = user.Id,
                email = user.Email,
                roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { error = "Failed to get user info" });
        }
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));
            }

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role {Role} assigned to user {Email}", dto.Role, dto.Email);
                return Ok(new { message = $"Role '{dto.Role}' assigned successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role");
            return StatusCode(500, new { error = "Failed to assign role" });
        }
    }

    /// <summary>
    /// Remove role from user (Admin only)
    /// </summary>
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, dto.Role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Role {Role} removed from user {Email}", dto.Role, dto.Email);
                return Ok(new { message = $"Role '{dto.Role}' removed successfully" });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role");
            return StatusCode(500, new { error = "Failed to remove role" });
        }
    }

    /// <summary>
    /// Initialize Admin role and user (development only)
    /// </summary>
    [HttpPost("init-admin")]
    public async Task<IActionResult> InitializeAdmin([FromBody] RegisterDto dto)
    {
        try
        {
            // Only allow in development
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return Forbid();
            }

            // Create Admin role if not exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                // Add to Admin role if not already
                if (!await _userManager.IsInRoleAsync(existingUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "Admin");
                }
                return Ok(new { message = "Admin role assigned to existing user", userId = existingUser.Id });
            }

            // Create new admin user
            var user = new IdentityUser 
            { 
                UserName = dto.Email, 
                Email = dto.Email 
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                _logger.LogInformation("Admin user {Email} created successfully", dto.Email);
                return Ok(new { message = "Admin user created successfully", userId = user.Id });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing admin");
            return StatusCode(500, new { error = "Failed to initialize admin" });
        }
    }
}

// DTOs
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password, bool RememberMe = false);
public record AssignRoleDto(string Email, string Role);

