using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeCreditedEventHandler(ILevyTransferMatchingApi api, ILogger<PledgeCreditedEventHandler> log) : IHandleMessages<PledgeCreditedEvent>
{
    public async Task Handle(PledgeCreditedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling {EventName} for pledge {PledgeId}", nameof(PledgeCreditedEvent), @event.PledgeId);

        try
        {
            var response = await api.GetApplicationsForAutomaticApproval(@event.PledgeId);

            foreach (var app in response.Applications)
            {
                await api.ApproveApplication(new ApproveApplicationRequest 
                { 
                    ApplicationId = app.Id, 
                    PledgeId = app.PledgeId 
                });
            }
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            log.LogError(ex, "Error handling PledgeCreditedEvent for pledge {PledgeId}", @event.PledgeId);
        }
    }
}