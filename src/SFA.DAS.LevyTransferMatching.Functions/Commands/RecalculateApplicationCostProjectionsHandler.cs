using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RecalculateApplicationCostProjectionsHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
    private readonly ILogger<RecalculateApplicationCostProjectionsHandler> _logger;

    public RecalculateApplicationCostProjectionsHandler(ILevyTransferMatchingApi levyTransferMatchingApi, ILogger<RecalculateApplicationCostProjectionsHandler> logger)
    {
        _levyTransferMatchingApi = levyTransferMatchingApi;
        _logger = logger;
    }

    [Function("RecalculateApplicationCostProjectionsHandler")]
    public async Task Run([TimerTrigger("0 0 22 13 6 *")] TimerInfo timer)
    {
        _logger.LogInformation("Recalculating application cost projections");

        try
        {
            await _levyTransferMatchingApi.RecalculateApplicationCostProjections();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"Error recalculating application cost projections");
        }
    }
}