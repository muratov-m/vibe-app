using Microsoft.EntityFrameworkCore;
using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

/// <summary>
/// Service for managing user profiles
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IRepository<UserSkill> _userSkillRepository;
    private readonly IRepository<UserLookingFor> _userLookingForRepository;
    private readonly IUserProfileEmbeddingService _embeddingService;

    public UserProfileService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserSkill> userSkillRepository,
        IRepository<UserLookingFor> userLookingForRepository,
        IUserProfileEmbeddingService embeddingService)
    {
        _userProfileRepository = userProfileRepository;
        _userSkillRepository = userSkillRepository;
        _userLookingForRepository = userLookingForRepository;
        _embeddingService = embeddingService;
    }

    public async Task<BatchImportResult> ImportUserProfilesAsync(List<UserProfileImportDto> dtos, CancellationToken cancellationToken = default)
    {
        var result = new BatchImportResult
        {
            TotalProcessed = dtos.Count
        };

        try
        {
            // Get all existing profile IDs (only IDs, not full entities)
            var existingProfileIds = await _userProfileRepository.GetQueryable()
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);
            
            // Get IDs from import list
            var importIds = dtos.Select(d => d.Id).ToHashSet();
            
            // Find profile IDs to delete (exist in DB but not in import list)
            var profileIdsToDelete = existingProfileIds.Where(id => !importIds.Contains(id)).ToList();
            
            // Delete profiles not in import list
            foreach (var profileId in profileIdsToDelete)
            {
                // Delete embedding first
                await _embeddingService.DeleteEmbeddingAsync(profileId);
                
                // Delete profile
                await _userProfileRepository.DeleteAsync(profileId);
                result.Deleted++;
            }
            
            // Process each profile in import list
            foreach (var dto in dtos)
            {
                try
                {
                    // Load profile with related entities
                    var existingProfile = await _userProfileRepository.GetQueryable()
                        .Include(p => p.Skills)
                        .Include(p => p.LookingFor)
                        .FirstOrDefaultAsync(p => p.Id == dto.Id);
                    
                    if (existingProfile != null)
                    {
                        // Update existing profile
                        await UpdateExistingProfile(existingProfile, dto);
                        result.Updated++;
                    }
                    else
                    {
                        // Create new profile
                        await CreateNewProfile(dto);
                        result.Created++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error processing profile ID {dto.Id}: {ex.Message}");
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"General error: {ex.Message}");
            return result;
        }
    }

    private async Task CreateNewProfile(UserProfileImportDto dto)
    {
        var userProfile = new UserProfile
        {
            Id = dto.Id,
            Name = dto.Name,
            Telegram = dto.Telegram,
            LinkedIn = dto.Linkedin,
            Bio = dto.Bio,
            Email = dto.Email,
            Photo = dto.Photo,
            HasStartup = dto.HasStartup,
            StartupStage = dto.StartupStage,
            StartupDescription = dto.StartupDescription,
            StartupName = dto.StartupName,
            CanHelp = dto.CanHelp,
            NeedsHelp = dto.NeedsHelp,
            AiUsage = dto.AiUsage,
            CreatedAt = DateTime.UtcNow
        };

        // Add skills
        foreach (var skill in dto.Skills)
        {
            userProfile.Skills.Add(new UserSkill { Skill = skill, UserProfileId = dto.Id });
        }

        // Add lookingFor items
        foreach (var lookingFor in dto.LookingFor)
        {
            userProfile.LookingFor.Add(new UserLookingFor { LookingFor = lookingFor, UserProfileId = dto.Id });
        }

        await _userProfileRepository.AddAsync(userProfile);
        
        // Generate embedding synchronously (non-blocking for user, but in same transaction)
        await _embeddingService.GenerateAndSaveEmbeddingAsync(dto.Id);
    }

    private async Task UpdateExistingProfile(UserProfile existingProfile, UserProfileImportDto dto)
    {
        // Update basic properties
        existingProfile.Name = dto.Name;
        existingProfile.Telegram = dto.Telegram;
        existingProfile.LinkedIn = dto.Linkedin;
        existingProfile.Bio = dto.Bio;
        existingProfile.Email = dto.Email;
        existingProfile.Photo = dto.Photo;
        existingProfile.HasStartup = dto.HasStartup;
        existingProfile.StartupStage = dto.StartupStage;
        existingProfile.StartupDescription = dto.StartupDescription;
        existingProfile.StartupName = dto.StartupName;
        existingProfile.CanHelp = dto.CanHelp;
        existingProfile.NeedsHelp = dto.NeedsHelp;
        existingProfile.AiUsage = dto.AiUsage;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        // Remove old skills and add new ones
        foreach (var skill in existingProfile.Skills.ToList())
        {
            await _userSkillRepository.DeleteAsync(skill);
        }
        existingProfile.Skills.Clear();
        foreach (var skill in dto.Skills)
        {
            existingProfile.Skills.Add(new UserSkill { Skill = skill, UserProfileId = existingProfile.Id });
        }

        // Remove old lookingFor and add new ones
        foreach (var lookingFor in existingProfile.LookingFor.ToList())
        {
            await _userLookingForRepository.DeleteAsync(lookingFor);
        }
        existingProfile.LookingFor.Clear();
        foreach (var lookingFor in dto.LookingFor)
        {
            existingProfile.LookingFor.Add(new UserLookingFor { LookingFor = lookingFor, UserProfileId = existingProfile.Id });
        }

        await _userProfileRepository.UpdateAsync(existingProfile);
        
        // Regenerate embedding synchronously
        await _embeddingService.GenerateAndSaveEmbeddingAsync(existingProfile.Id);
    }

    public async Task<UserProfile?> GetUserProfileByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _userProfileRepository.GetQueryable()
            .Include(p => p.Skills)
            .Include(p => p.LookingFor)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _userProfileRepository.GetQueryable()
            .Include(p => p.Skills)
            .Include(p => p.LookingFor)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserProfile> UpdateUserProfileAsync(int id, UserProfileImportDto dto, CancellationToken cancellationToken = default)
    {
        var existingProfile = await GetUserProfileByIdAsync(id, cancellationToken);
        if (existingProfile == null)
        {
            throw new KeyNotFoundException($"User profile with ID {id} not found");
        }

        await UpdateExistingProfile(existingProfile, dto);
        return existingProfile;
    }

    public async Task<bool> DeleteUserProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        var profile = await GetUserProfileByIdAsync(id, cancellationToken);
        if (profile == null)
        {
            return false;
        }

        // Delete associated embedding first
        await _embeddingService.DeleteEmbeddingAsync(id, cancellationToken);
        
        // Delete the profile (cascade will delete related entities)
        await _userProfileRepository.DeleteAsync(profile);
        return true;
    }
}
