using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingDeclinedEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunApplicationFundingDeclinedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationFundingDeclined)] ApplicationFundingDeclinedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling {nameof(ApplicationFundingDeclinedEvent)} handler for application {@event.ApplicationId}");
     
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
     
            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}