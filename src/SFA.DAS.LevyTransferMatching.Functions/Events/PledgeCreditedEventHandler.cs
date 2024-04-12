using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeCreditedEventHandler(ILevyTransferMatchingApi api)
{
    [Function("PledgeCreditedEventHandler")]
    public async Task Run([ServiceBusTrigger(QueueNames.PledgeCredited)] PledgeCreditedEvent @event, ILogger log)
    {
         log.LogInformation($"Handling {nameof(PledgeCreditedEvent)} for pledge {@event.PledgeId}");

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
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, $"Error handling PledgeCreditedEvent for pledge {@event.PledgeId}");
        }
    }
}