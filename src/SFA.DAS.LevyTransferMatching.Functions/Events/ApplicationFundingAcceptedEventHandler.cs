using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationFundingAcceptedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public ApplicationFundingAcceptedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationFundingAcceptedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationFundingAccepted)] ApplicationFundingAcceptedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling {nameof(ApplicationFundingAcceptedEvent)} handler for application {@event.ApplicationId}");
            if (@event.RejectApplications)
            {
                log.LogInformation($"Rejecting Pengding applications for pledge {@event.PledgeId}");

                var request = new RejectPledgeApplicationsRequest
                {
                    PledgeId = @event.PledgeId
                };

                try
                {
                    await _api.RejectPledgeApplications(request);
                }
                catch (ApiException ex)
                {
                    if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                    log.LogError(ex, $"Error handling {nameof(ApplicationFundingAcceptedEvent)} for application {@event.ApplicationId}");
                }
            }          
        }
    }
}