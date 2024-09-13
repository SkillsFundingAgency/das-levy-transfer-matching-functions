using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnAfterAcceptanceEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<ApplicationWithdrawnAfterAcceptanceEvent>
{
    public async Task Handle(ApplicationWithdrawnAfterAcceptanceEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling {EventName} handler for application {ApplicationId}", nameof(ApplicationWithdrawnAfterAcceptanceEvent), @event.ApplicationId);

        var request = new ApplicationWithdrawnAfterAcceptanceRequest
        {
            ApplicationId = @event.ApplicationId,
            PledgeId = @event.PledgeId,
            Amount = @event.Amount,
        };

        try
        {
            await api.ApplicationWithdrawnAfterAcceptance(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, "Error handling ApplicationApprovedEvent for application {@event.ApplicationId}", @event.ApplicationId);
        }
    }
}