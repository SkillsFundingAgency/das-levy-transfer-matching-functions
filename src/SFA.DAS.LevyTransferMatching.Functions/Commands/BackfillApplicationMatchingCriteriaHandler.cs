using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class BackfillApplicationMatchingCriteriaHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;

        public BackfillApplicationMatchingCriteriaHandler(ILevyTransferMatchingApi levyTransferMatchingApi)
        {
            _levyTransferMatchingApi = levyTransferMatchingApi;
        }

        [FunctionName("BackfillApplicationMatchingCriteriaHandler")]
        public async Task Run([TimerTrigger("0 0 22 * * *")] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Backfilling application matching criteria");
            
            try
            {
                await _levyTransferMatchingApi.BackfillApplicationMatchingCriteria();
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, $"Error backfilling matching criteria");
            }
        }
    }
}
