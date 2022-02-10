using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class BackfillApplicationCostingProjectionsHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;

        public BackfillApplicationCostingProjectionsHandler(ILevyTransferMatchingApi levyTransferMatchingApi)
        {
            _levyTransferMatchingApi = levyTransferMatchingApi;
        }

        [FunctionName("BackfillApplicationCostingProjectionsHandler")]
        public async Task Run([TimerTrigger("0 0 22 * * *")] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Backfilling application costing projections");
            
            try
            {
                await _levyTransferMatchingApi.BackfillApplicationCostingProjections();
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, $"Error backfilling costing projections");
            }
        }
    }
}
