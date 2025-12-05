using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for managing user profiles
/// </summary>
public interface IUserProfileService
{
    /// <summary>
    /// Import batch of user profiles from DTOs and sync with database
    /// Deletes users not present in the import list
    /// </summary>
    Task<BatchImportResult> ImportUserProfilesAsync(List<UserProfileImportDto> dtos, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user profile by ID
    /// </summary>
    Task<UserProfile?> GetUserProfileByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all user profiles
    /// </summary>
    Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update existing user profile
    /// </summary>
    Task<UserProfile> UpdateUserProfileAsync(int id, UserProfileImportDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete user profile
    /// </summary>
    Task<bool> DeleteUserProfileAsync(int id, CancellationToken cancellationToken = default);
}

