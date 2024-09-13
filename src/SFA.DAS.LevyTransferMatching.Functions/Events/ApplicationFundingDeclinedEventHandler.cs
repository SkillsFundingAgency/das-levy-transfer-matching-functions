using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingDeclinedEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<ApplicationFundingDeclinedEvent>
{
    public async Task Handle(ApplicationFundingDeclinedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling {EventName} handler for application {ApplicationId}", nameof(ApplicationFundingDeclinedEvent), @event.ApplicationId);

        var request = new ApplicationFundingDeclinedRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            Amount = @event.Amount,
        };

        try
        {
            await api.ApplicationFundingDeclined(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, "Error handling {EventName} for application {ApplicationId}", nameof(ApplicationFundingDeclinedEvent), @event.ApplicationId);
        }
    }
}