using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeDebitFailedEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<PledgeDebitFailedEvent>
{
    public async Task Handle(PledgeDebitFailedEvent @event, IMessageHandlerContext context)
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