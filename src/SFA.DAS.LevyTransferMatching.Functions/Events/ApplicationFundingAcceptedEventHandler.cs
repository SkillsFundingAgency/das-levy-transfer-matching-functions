using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Bindings;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingAcceptedEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunApplicationFundingAcceptedEvent")]
    public async Task Run([NServiceBusTriggerOutput(Endpoint = QueueNames.ApplicationFundingAccepted)] ApplicationFundingAcceptedEvent @event, ILogger log)
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
                await api.RejectPledgeApplications(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling {nameof(ApplicationFundingAcceptedEvent)} for application {@event.ApplicationId}");
            }
        }          
    }
}