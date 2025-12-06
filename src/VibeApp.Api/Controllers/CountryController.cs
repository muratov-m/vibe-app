using Microsoft.AspNetCore.Mvc;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly ILogger<CountryController> _logger;

    public CountryController(
        ICountryService countryService,
        ILogger<CountryController> logger)
    {
        _countryService = countryService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncCountries(CancellationToken cancellationToken)
    {
        try
        {
            await _countryService.SyncCountriesAsync(cancellationToken);
            return Ok(new { Message = "Countries synced successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing countries");
            return StatusCode(500, new { Error = "Failed to sync countries", Details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken)
    {
        try
        {
            var countries = await _countryService.GetAllCountriesAsync(cancellationToken);
            return Ok(countries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting countries");
            return StatusCode(500, new { Error = "Failed to get countries", Details = ex.Message });
        }
    }
}

