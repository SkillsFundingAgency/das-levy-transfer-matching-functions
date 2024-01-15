using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeDebitFailedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;

    public PledgeDebitFailedEventHandler(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [FunctionName("RunPledgeDebitFailedEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.PledgeDebitFailed)] PledgeDebitFailedEvent @event, ILogger log)
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
            await _api.PledgeDebitFailed(request);
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error handling PledgeDebitFailedEvent for application {@event.ApplicationId}");
            throw;
        }
    }
}