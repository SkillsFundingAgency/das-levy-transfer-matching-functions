using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class PledgeClosedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public PledgeClosedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunPledgeClosedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.PledgeClosed)] PledgeClosedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling RunPledgeClosedEvent for pledge {@event.PledgeId}");

            try
            {              
                if (@event.InsufficientFunds)
                {
                    log.LogInformation($"Applications for Pledge {@event.PledgeId} are being closed due to insufficient remaining pledge amount");

                    await _api.RejectPledgeApplications(new RejectPledgeApplicationsRequest { PledgeId = @event.PledgeId });
                }
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error handling RunPledgeClosedEvent");
                throw;
            }
        }
    }
}