using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VibeApp.Core.Interfaces;
using VibeApp.Data;

namespace VibeApp.Api.Pages;

public class AdminModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmbeddingQueueService _embeddingQueueService;
    private readonly AppDbContext _dbContext;

    public int TotalUsers { get; private set; }
    public int QueueCount { get; private set; }
    public int TotalUserProfiles { get; private set; }

    public string QueueStatus =>
        QueueCount == 0
            ? "Очередь пуста"
            : $"В очереди {QueueCount} профилей на обработку";

    public AdminModel(
        UserManager<IdentityUser> userManager,
        IEmbeddingQueueService embeddingQueueService,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _embeddingQueueService = embeddingQueueService;
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        TotalUsers = await _userManager.Users.CountAsync();
        QueueCount = await _embeddingQueueService.GetQueueCountAsync();
        TotalUserProfiles = await _dbContext.UserProfiles.CountAsync();
    }
}


