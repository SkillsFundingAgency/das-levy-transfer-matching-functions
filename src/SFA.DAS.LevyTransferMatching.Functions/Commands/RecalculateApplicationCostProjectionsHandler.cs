using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RecalculateApplicationCostProjectionsHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;

    public RecalculateApplicationCostProjectionsHandler(ILevyTransferMatchingApi levyTransferMatchingApi)
    {
        _levyTransferMatchingApi = levyTransferMatchingApi;
    }

    [Function("RecalculateApplicationCostProjectionsHandler")]
    public async Task Run([TimerTrigger("0 0 22 13 6 *")] TimerInfo timer, ILogger logger)
    {
        logger.LogInformation("Recalculating application cost projections");
        
        try
        {
            await _levyTransferMatchingApi.RecalculateApplicationCostProjections();
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, $"Error recalculating application cost projections");
        }
    }
}