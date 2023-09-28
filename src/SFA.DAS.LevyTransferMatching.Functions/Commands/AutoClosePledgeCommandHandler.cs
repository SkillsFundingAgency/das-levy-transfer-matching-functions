using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Commands;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class AutoClosePledgeCommandHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public AutoClosePledgeCommandHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunAutoClosePledgeCommand")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.AutoClosePledgeCommand)] AutoClosePledgeCommand @event, ILogger log)
        {
            log.LogInformation($"Handling AutoClosePledgeCommandHandler for pledge {@event.PledgeId}");

            try
            {
                var request = new AutoClosePledgeRequest
                {
                    PledgeId = @event.PledgeId,
                };

                var result = await _api.ClosePledge(request);

                if (result.IsSuccessStatusCode)
                {
                    await _api.RejectPledgeApplications(new RejectPledgeApplicationsRequest { PledgeId = @event.PledgeId });
                }
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error handling AutoClosePledgeCommand");
                throw;
            }
        }
    }
}
