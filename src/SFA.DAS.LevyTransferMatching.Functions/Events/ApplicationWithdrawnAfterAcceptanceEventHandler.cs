using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnAfterAcceptanceEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunApplicationWithdrawnAfterAcceptanceEvent")]
    public async Task Run([QueueTrigger(QueueNames.ApplicationWithdrawnAfterAcceptance)] ApplicationWithdrawnAfterAcceptanceEvent @event, ILogger log)
    {
        log.LogInformation($"Handling {nameof(ApplicationWithdrawnAfterAcceptanceEvent)} handler for application {@event.ApplicationId}");
     
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
     
            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}