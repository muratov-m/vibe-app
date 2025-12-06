using VibeApp.Core.Entities;

namespace VibeApp.Core.Interfaces;

/// <summary>
/// Service for parsing and updating user profile structured data using AI
/// </summary>
public interface IUserProfileParsingService
{
    /// <summary>
    /// Parses user profile data and updates structured fields using AI
    /// Updates: ParsedShortBio, ParsedMainActivity, ParsedInterests, ParsedCountry, ParsedCity
    /// </summary>
    /// <param name="userProfileId">ID of the user profile to parse</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ParseAndUpdateProfileAsync(int userProfileId, CancellationToken cancellationToken = default);
}

