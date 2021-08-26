using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class ApplicationApprovedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public ApplicationApprovedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationApprovedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedEvent)] ApplicationApprovedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationApprovedEvent handler for application {@event.ApplicationId}");

            var request = new ApplicationApprovedRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                Amount = @event.Amount
            };

            log.LogInformation($"Debiting pledge {request.PledgeId} by {request.Amount}");

            await _api.ApplicationApproved(request);

            log.LogInformation("Completed");
        }
    }
}
