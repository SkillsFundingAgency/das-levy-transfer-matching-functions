using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeDebitFailedEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunPledgeDebitFailedEvent")]
    public async Task Run([QueueTrigger(QueueNames.PledgeDebitFailed)] PledgeDebitFailedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling PledgeDebitFailedEvent handler for application {@event.ApplicationId}");

        var request = new PledgeDebitFailedRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            Amount = @event.Amount
        };

        try
        {
            await api.PledgeDebitFailed(request);
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error handling PledgeDebitFailedEvent for application {@event.ApplicationId}");
            throw;
        }
    }
}