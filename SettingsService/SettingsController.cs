using Microsoft.AspNetCore.Mvc;

namespace SettingsService;

[Route("/")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly ISettingsRepository repository;

    public SettingsController(ISettingsRepository repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    public Task SaveSettingsAsync([FromBody] SettingsModel settings)
    {
        return repository.SaveSettingsAsync(settings);
    }
}