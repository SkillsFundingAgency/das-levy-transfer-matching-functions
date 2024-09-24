using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RecalculateApplicationCostProjectionsHandler(ILevyTransferMatchingApi levyTransferMatchingApi)
{
    [Function("RecalculateApplicationCostProjectionsHandler")]
    public async Task Run([TimerTrigger("0 0 22 13 6 *")] TimerInfo timer, ILogger<RecalculateApplicationCostProjectionsHandler> logger)
    {
        logger.LogInformation("Recalculating application cost projections");
        
        try
        {
            await levyTransferMatchingApi.RecalculateApplicationCostProjections();
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error recalculating application cost projections");
        }
    }
}