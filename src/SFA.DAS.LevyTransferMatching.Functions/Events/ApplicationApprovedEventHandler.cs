using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEventHandler(ILevyTransferMatchingApi api, ILogger<ApplicationApprovedEventHandler> log) : IHandleMessages<ApplicationApprovedEvent>
{
    public async Task Handle(ApplicationApprovedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationApprovedEvent handler for application {ApplicationId}", @event.ApplicationId);

        var request = new ApplicationApprovedRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            Amount = @event.Amount
        };

        try
        {
            await api.ApplicationApproved(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            log.LogError(ex, "Error handling ApplicationApprovedEvent for application {ApplicationId}", @event.ApplicationId);
        }
    }
}