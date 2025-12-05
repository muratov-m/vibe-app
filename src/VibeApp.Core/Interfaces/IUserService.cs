using Microsoft.AspNetCore.Identity;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    Task<IdentityUser?> GetUserByIdAsync(string userId);
    Task<IdentityUser?> GetUserByEmailAsync(string email);
    Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
    Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
    Task<IdentityResult> UpdateUserAsync(IdentityUser user);
    Task<IdentityResult> DeleteUserAsync(string userId);
    Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword);
}

