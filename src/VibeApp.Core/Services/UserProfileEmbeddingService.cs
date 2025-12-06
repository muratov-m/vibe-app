using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;

namespace VibeApp.Core.Services;

public class UserProfileEmbeddingService : IUserProfileEmbeddingService
{
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IRepository<UserProfileEmbedding> _embeddingRepository;
    private readonly IOpenAIGateway _openAIGateway;
    private readonly ILogger<UserProfileEmbeddingService> _logger;

    public UserProfileEmbeddingService(
        IRepository<UserProfile> userProfileRepository,
        IRepository<UserProfileEmbedding> embeddingRepository,
        IOpenAIGateway openAIGateway,
        ILogger<UserProfileEmbeddingService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _embeddingRepository = embeddingRepository;
        _openAIGateway = openAIGateway;
        _logger = logger;
    }

    public async Task GenerateAndSaveEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating embedding for user profile {UserProfileId}", userProfileId);

            // Get user profile with related data (Skills and LookingFor)
            var profile = await _userProfileRepository.GetQueryable()
                .Include(p => p.Skills)
                .Include(p => p.LookingFor)
                .FirstOrDefaultAsync(p => p.Id == userProfileId, cancellationToken);
            
            if (profile == null)
            {
                _logger.LogWarning("User profile {UserProfileId} not found", userProfileId);
                return;
            }

            // Generate embedding (placeholder for now)
            var embedding = await GenerateEmbeddingAsync(profile, cancellationToken);

            // Check if embedding already exists
            var existingEmbedding = await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);

            if (existingEmbedding != null)
            {
                // Update existing embedding
                existingEmbedding.Embedding = embedding;
                existingEmbedding.UpdatedAt = DateTime.UtcNow;
                await _embeddingRepository.UpdateAsync(existingEmbedding);
                _logger.LogInformation("Updated embedding for user profile {UserProfileId}", userProfileId);
            }
            else
            {
                // Create new embedding
                var newEmbedding = new UserProfileEmbedding
                {
                    UserProfileId = userProfileId,
                    Embedding = embedding,
                    CreatedAt = DateTime.UtcNow
                };
                await _embeddingRepository.AddAsync(newEmbedding);
                _logger.LogInformation("Created new embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for user profile {UserProfileId}", userProfileId);
            throw;
        }
    }

    public async Task DeleteEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var embedding = await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);
            if (embedding != null)
            {
                await _embeddingRepository.DeleteAsync(embedding);
                _logger.LogInformation("Deleted embedding for user profile {UserProfileId}", userProfileId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting embedding for user profile {UserProfileId}", userProfileId);
            throw;
        }
    }

    public async Task<UserProfileEmbedding?> GetEmbeddingAsync(int userProfileId, CancellationToken cancellationToken = default)
    {
        return await _embeddingRepository.FirstOrDefaultAsync(e => e.UserProfileId == userProfileId);
    }

    private async Task<Vector> GenerateEmbeddingAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var profileText = BuildProfileText(profile);
        _logger.LogInformation("Generating embedding for profile: {Name}, text length: {Length}", 
            profile.Name, profileText.Length);

        var embeddingArray = await _openAIGateway.GetEmbeddingAsync(profileText, cancellationToken: cancellationToken);
        
        return new Vector(embeddingArray);
    }

    private static string BuildProfileText(UserProfile profile)
    {
        var sb = new StringBuilder();

        // 1. Bio - MOST IMPORTANT for semantic search
        if (!string.IsNullOrWhiteSpace(profile.Bio))
            sb.AppendLine($"Bio: {profile.Bio}");

        // 2. Looking For - networking goals (high priority)
        if (profile.LookingFor?.Any() == true)
        {
            var lookingFor = string.Join(", ", profile.LookingFor.Select(l => l.LookingFor));
            sb.AppendLine($"Looking for: {lookingFor}");
        }

        // 3. Skills - professional skills
        if (profile.Skills?.Any() == true)
        {
            var skills = string.Join(", ", profile.Skills.Select(s => s.Skill));
            sb.AppendLine($"Skills: {skills}");
        }

        // 4. Location - for geographic search
        if (!string.IsNullOrWhiteSpace(profile.Country))
            sb.AppendLine($"Country: {profile.Country}");
        
        if (!string.IsNullOrWhiteSpace(profile.City))
            sb.AppendLine($"City: {profile.City}");

        // 5. Startup info
        if (profile.HasStartup)
        {
            if (!string.IsNullOrWhiteSpace(profile.StartupName))
                sb.AppendLine($"Startup: {profile.StartupName}");
            if (!string.IsNullOrWhiteSpace(profile.StartupDescription))
                sb.AppendLine($"About startup: {profile.StartupDescription}");
            if (!string.IsNullOrWhiteSpace(profile.StartupStage))
                sb.AppendLine($"Stage: {profile.StartupStage}");
        }

        // 6. Collaboration
        if (!string.IsNullOrWhiteSpace(profile.CanHelp))
            sb.AppendLine($"Can help: {profile.CanHelp}");
        
        if (!string.IsNullOrWhiteSpace(profile.NeedsHelp))
            sb.AppendLine($"Needs: {profile.NeedsHelp}");

        // 7. AI usage
        if (!string.IsNullOrWhiteSpace(profile.AiUsage))
            sb.AppendLine($"AI: {profile.AiUsage}");

        // 8. Name (at the end - less important for embeddings)
        sb.AppendLine($"Name: {profile.Name}");

        return sb.ToString().Trim();
    }
}

