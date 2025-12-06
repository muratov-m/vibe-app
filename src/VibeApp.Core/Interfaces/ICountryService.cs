using VibeApp.Core.Entities;

namespace VibeApp.Core.Interfaces;

public interface ICountryService
{
    Task SyncCountriesAsync(CancellationToken cancellationToken = default);
    Task<List<Country>> GetAllCountriesAsync(CancellationToken cancellationToken = default);
}

