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
    private readonly IRepository<UserCustomArray1> _customArray1Repository;
    private readonly IRepository<UserCustomArray2> _customArray2Repository;
    private readonly IRepository<UserCustomArray3> _customArray3Repository;
    private readonly IRepository<UserCustomArray4> _customArray4Repository;
    private readonly IRepository<UserCustomArray5> _customArray5Repository;
    private readonly IRepository<UserCustomArray6> _customArray6Repository;
    private readonly IRepository<UserCustomArray7> _customArray7Repository;

    public UserProfileService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserSkill> userSkillRepository,
        IRepository<UserLookingFor> userLookingForRepository,
        IRepository<UserCustomArray1> customArray1Repository,
        IRepository<UserCustomArray2> customArray2Repository,
        IRepository<UserCustomArray3> customArray3Repository,
        IRepository<UserCustomArray4> customArray4Repository,
        IRepository<UserCustomArray5> customArray5Repository,
        IRepository<UserCustomArray6> customArray6Repository,
        IRepository<UserCustomArray7> customArray7Repository)
    {
        _userProfileRepository = userProfileRepository;
        _userSkillRepository = userSkillRepository;
        _userLookingForRepository = userLookingForRepository;
        _customArray1Repository = customArray1Repository;
        _customArray2Repository = customArray2Repository;
        _customArray3Repository = customArray3Repository;
        _customArray4Repository = customArray4Repository;
        _customArray5Repository = customArray5Repository;
        _customArray6Repository = customArray6Repository;
        _customArray7Repository = customArray7Repository;
    }

    public async Task<UserProfile> ImportUserProfileAsync(UserProfileImportDto dto, CancellationToken cancellationToken = default)
    {
        var userProfile = new UserProfile
        {
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
            Custom1 = dto.Custom_1,
            Custom2 = dto.Custom_2,
            Custom3 = dto.Custom_3,
            Custom4 = dto.Custom_4,
            Custom5 = dto.Custom_5,
            Custom6 = dto.Custom_6,
            Custom7 = dto.Custom_7,
            CreatedAt = DateTime.UtcNow
        };

        // Add skills
        foreach (var skill in dto.Skills)
        {
            userProfile.Skills.Add(new UserSkill { Skill = skill });
        }

        // Add lookingFor items
        foreach (var lookingFor in dto.LookingFor)
        {
            userProfile.LookingFor.Add(new UserLookingFor { LookingFor = lookingFor });
        }

        // Add custom arrays
        foreach (var value in dto.Custom_array_1)
        {
            userProfile.CustomArray1.Add(new UserCustomArray1 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_2)
        {
            userProfile.CustomArray2.Add(new UserCustomArray2 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_3)
        {
            userProfile.CustomArray3.Add(new UserCustomArray3 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_4)
        {
            userProfile.CustomArray4.Add(new UserCustomArray4 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_5)
        {
            userProfile.CustomArray5.Add(new UserCustomArray5 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_6)
        {
            userProfile.CustomArray6.Add(new UserCustomArray6 { Value = value });
        }
        
        foreach (var value in dto.Custom_array_7)
        {
            userProfile.CustomArray7.Add(new UserCustomArray7 { Value = value });
        }

        await _userProfileRepository.AddAsync(userProfile);
        return userProfile;
    }

    public async Task<UserProfile?> GetUserProfileByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var profiles = await _userProfileRepository.GetAllAsync();
        return profiles
            .Include(p => p.Skills)
            .Include(p => p.LookingFor)
            .Include(p => p.CustomArray1)
            .Include(p => p.CustomArray2)
            .Include(p => p.CustomArray3)
            .Include(p => p.CustomArray4)
            .Include(p => p.CustomArray5)
            .Include(p => p.CustomArray6)
            .Include(p => p.CustomArray7)
            .FirstOrDefault(p => p.Id == id);
    }

    public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _userProfileRepository.GetAllAsync();
        return profiles
            .Include(p => p.Skills)
            .Include(p => p.LookingFor)
            .Include(p => p.CustomArray1)
            .Include(p => p.CustomArray2)
            .Include(p => p.CustomArray3)
            .Include(p => p.CustomArray4)
            .Include(p => p.CustomArray5)
            .Include(p => p.CustomArray6)
            .Include(p => p.CustomArray7)
            .ToList();
    }

    public async Task<UserProfile> UpdateUserProfileAsync(int id, UserProfileImportDto dto, CancellationToken cancellationToken = default)
    {
        var existingProfile = await GetUserProfileByIdAsync(id, cancellationToken);
        if (existingProfile == null)
        {
            throw new KeyNotFoundException($"User profile with ID {id} not found");
        }

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
        existingProfile.Custom1 = dto.Custom_1;
        existingProfile.Custom2 = dto.Custom_2;
        existingProfile.Custom3 = dto.Custom_3;
        existingProfile.Custom4 = dto.Custom_4;
        existingProfile.Custom5 = dto.Custom_5;
        existingProfile.Custom6 = dto.Custom_6;
        existingProfile.Custom7 = dto.Custom_7;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        // Remove old skills and add new ones
        foreach (var skill in existingProfile.Skills.ToList())
        {
            await _userSkillRepository.DeleteAsync(skill);
        }
        existingProfile.Skills.Clear();
        foreach (var skill in dto.Skills)
        {
            existingProfile.Skills.Add(new UserSkill { Skill = skill, UserProfileId = id });
        }

        // Remove old lookingFor and add new ones
        foreach (var lookingFor in existingProfile.LookingFor.ToList())
        {
            await _userLookingForRepository.DeleteAsync(lookingFor);
        }
        existingProfile.LookingFor.Clear();
        foreach (var lookingFor in dto.LookingFor)
        {
            existingProfile.LookingFor.Add(new UserLookingFor { LookingFor = lookingFor, UserProfileId = id });
        }

        // Update custom arrays
        await UpdateCustomArray(existingProfile.CustomArray1, dto.Custom_array_1, id, _customArray1Repository);
        await UpdateCustomArray(existingProfile.CustomArray2, dto.Custom_array_2, id, _customArray2Repository);
        await UpdateCustomArray(existingProfile.CustomArray3, dto.Custom_array_3, id, _customArray3Repository);
        await UpdateCustomArray(existingProfile.CustomArray4, dto.Custom_array_4, id, _customArray4Repository);
        await UpdateCustomArray(existingProfile.CustomArray5, dto.Custom_array_5, id, _customArray5Repository);
        await UpdateCustomArray(existingProfile.CustomArray6, dto.Custom_array_6, id, _customArray6Repository);
        await UpdateCustomArray(existingProfile.CustomArray7, dto.Custom_array_7, id, _customArray7Repository);

        await _userProfileRepository.UpdateAsync(existingProfile);
        return existingProfile;
    }

    public async Task<bool> DeleteUserProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        var profile = await GetUserProfileByIdAsync(id, cancellationToken);
        if (profile == null)
        {
            return false;
        }

        await _userProfileRepository.DeleteAsync(profile);
        return true;
    }

    private async Task UpdateCustomArray<T>(
        ICollection<T> existingCollection, 
        List<string> newValues, 
        int userProfileId, 
        IRepository<T> repository) where T : class, IEntity, new()
    {
        // Remove old items
        foreach (var item in existingCollection.ToList())
        {
            await repository.DeleteAsync(item);
        }
        existingCollection.Clear();

        // Add new items
        foreach (var value in newValues)
        {
            var newItem = new T();
            SetUserProfileId(newItem, userProfileId);
            SetValue(newItem, value);
            existingCollection.Add(newItem);
        }
    }

    private void SetUserProfileId<T>(T entity, int userProfileId) where T : class, IEntity
    {
        var property = entity.GetType().GetProperty("UserProfileId");
        property?.SetValue(entity, userProfileId);
    }

    private void SetValue<T>(T entity, string value) where T : class, IEntity
    {
        var property = entity.GetType().GetProperty("Value");
        property?.SetValue(entity, value);
    }
}

