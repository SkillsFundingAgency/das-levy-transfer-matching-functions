using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class RecalculateApplicationCostProjectionsHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;

        public RecalculateApplicationCostProjectionsHandler(ILevyTransferMatchingApi levyTransferMatchingApi)
        {
            _levyTransferMatchingApi = levyTransferMatchingApi;
        }

        [FunctionName("RecalculateApplicationCostProjectionsHandler")]
        public async Task Run([TimerTrigger("0 6 6 5 *")] TimerInfo timer, ILogger logger)
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
}
