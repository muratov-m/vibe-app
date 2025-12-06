using VibeApp.Core.DTOs;
using VibeApp.Core.Entities;
using VibeApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VibeApp.Core.Services;

public class CountryService : ICountryService
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<UserProfile> _userProfileRepository;

    public CountryService(
        IRepository<Country> countryRepository,
        IRepository<UserProfile> userProfileRepository)
    {
        _countryRepository = countryRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task SyncCountriesAsync(CancellationToken cancellationToken = default)
    {
        // Get all distinct countries from user profiles
        var countries = await _userProfileRepository.GetQueryable()
            .Where(p => !string.IsNullOrWhiteSpace(p.Country))
            .GroupBy(p => p.Country)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        foreach (var countryData in countries)
        {
            var existingCountry = await _countryRepository.FirstOrDefaultAsync(c => c.Name == countryData.Name);

            if (existingCountry != null)
            {
                existingCountry.UserCount = countryData.Count;
                existingCountry.UpdatedAt = DateTime.UtcNow;
                await _countryRepository.UpdateAsync(existingCountry);
            }
            else
            {
                var newCountry = new Country
                {
                    Name = countryData.Name,
                    UserCount = countryData.Count,
                    CreatedAt = DateTime.UtcNow
                };
                await _countryRepository.AddAsync(newCountry);
            }
        }

        // Remove countries that no longer have users
        var countryNames = countries.Select(c => c.Name).ToHashSet();
        var allCountries = await _countryRepository.GetQueryable().ToListAsync(cancellationToken);
        var countriesToDelete = allCountries.Where(c => !countryNames.Contains(c.Name)).ToList();

        foreach (var country in countriesToDelete)
        {
            await _countryRepository.DeleteAsync(country);
        }
    }

    public async Task<List<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _countryRepository.GetQueryable()
            .OrderByDescending(c => c.UserCount)
            .ToListAsync(cancellationToken);
    }
}

