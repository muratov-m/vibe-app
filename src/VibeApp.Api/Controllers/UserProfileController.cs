using Microsoft.AspNetCore.Mvc;
using VibeApp.Core.DTOs;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(IUserProfileService userProfileService, ILogger<UserProfileController> logger)
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    /// <summary>
    /// Import user profile from JSON
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<int>> ImportUserProfile([FromBody] UserProfileImportDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await _userProfileService.ImportUserProfileAsync(dto, cancellationToken);
            return Ok(new { id = userProfile.Id, message = "User profile imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing user profile");
            return StatusCode(500, new { error = "Failed to import user profile" });
        }
    }

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUserProfile(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await _userProfileService.GetUserProfileByIdAsync(id, cancellationToken);
            if (userProfile == null)
            {
                return NotFound(new { error = "User profile not found" });
            }
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, new { error = "Failed to retrieve user profile" });
        }
    }

    /// <summary>
    /// Get all user profiles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAllUserProfiles(CancellationToken cancellationToken)
    {
        try
        {
            var userProfiles = await _userProfileService.GetAllUserProfilesAsync(cancellationToken);
            return Ok(userProfiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profiles");
            return StatusCode(500, new { error = "Failed to retrieve user profiles" });
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUserProfile(int id, [FromBody] UserProfileImportDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await _userProfileService.UpdateUserProfileAsync(id, dto, cancellationToken);
            return Ok(new { id = userProfile.Id, message = "User profile updated successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User profile not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new { error = "Failed to update user profile" });
        }
    }

    /// <summary>
    /// Delete user profile
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUserProfile(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userProfileService.DeleteUserProfileAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound(new { error = "User profile not found" });
            }
            return Ok(new { message = "User profile deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user profile");
            return StatusCode(500, new { error = "Failed to delete user profile" });
        }
    }
}

