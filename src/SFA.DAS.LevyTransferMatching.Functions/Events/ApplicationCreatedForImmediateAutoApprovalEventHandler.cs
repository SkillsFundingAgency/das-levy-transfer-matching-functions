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
    public class ApplicationCreatedForImmediateAutoApprovalEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public ApplicationCreatedForImmediateAutoApprovalEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationCreatedForImmediateAutoApprovalEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationCreated)] ApplicationCreatedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationCreatedForImmediateAutoApprovalEventHandler for application {@event.ApplicationId}");

            try
            {
                var request = new ApplicationCreatedForImmediateAutoApprovalRequest
                {
                    PledgeId = @event.PledgeId,
                    ApplicationId = @event.ApplicationId
                };
                
                await _api.ApplicationCreatedForImmediateAutoApproval(request);
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error handling ApplicationCreatedForImmediateAutoApprovalEvent");
                throw;
            }
        }
    }
}
