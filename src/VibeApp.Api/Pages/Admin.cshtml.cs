using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VibeApp.Core.Interfaces;

namespace VibeApp.Api.Pages;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmbeddingQueueService _embeddingQueueService;

    public int TotalUsers { get; private set; }
    public int QueueCount { get; private set; }

    public string QueueStatus =>
        QueueCount == 0
            ? "Очередь пуста"
            : $"В очереди {QueueCount} профилей на обработку";

    public AdminModel(
        UserManager<IdentityUser> userManager,
        IEmbeddingQueueService embeddingQueueService)
    {
        _userManager = userManager;
        _embeddingQueueService = embeddingQueueService;
    }

    public async Task OnGetAsync()
    {
        TotalUsers = await _userManager.Users.CountAsync();
        QueueCount = await _embeddingQueueService.GetQueueCountAsync();
    }
}


