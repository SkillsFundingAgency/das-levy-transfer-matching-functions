using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingAcceptedEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<ApplicationFundingAcceptedEvent>
{
    public async Task Handle(ApplicationFundingAcceptedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling {EventName)} handler for application {ApplicationId}",nameof(ApplicationFundingAcceptedEvent), @event.ApplicationId);
        if (@event.RejectApplications)
        {
            log.LogInformation("Rejecting Pending applications for pledge {PledgeId}", @event.PledgeId);

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

                log.LogError(ex, "Error handling {EventName} for application {ApplicationId}", nameof(ApplicationFundingAcceptedEvent), @event.ApplicationId);
            }
        }
    }
}