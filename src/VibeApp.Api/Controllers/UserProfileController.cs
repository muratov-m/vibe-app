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
    /// Import batch of user profiles from JSON and sync with database
    /// Deletes users not present in the import list
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult> ImportUserProfiles([FromBody] List<UserProfileImportDto> dtos, CancellationToken cancellationToken)
    {
        try
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { error = "No profiles provided for import" });
            }

            var result = await _userProfileService.ImportUserProfilesAsync(dtos, cancellationToken);
            
            return Ok(new 
            { 
                totalProcessed = result.TotalProcessed,
                created = result.Created,
                updated = result.Updated,
                deleted = result.Deleted,
                errors = result.Errors,
                message = $"Successfully processed {result.TotalProcessed} profiles. Created: {result.Created}, Updated: {result.Updated}, Deleted: {result.Deleted}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing user profiles");
            return StatusCode(500, new { error = "Failed to import user profiles", details = ex.Message });
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
