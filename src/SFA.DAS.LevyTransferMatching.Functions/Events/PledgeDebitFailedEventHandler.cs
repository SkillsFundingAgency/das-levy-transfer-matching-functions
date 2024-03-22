using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeDebitFailedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<PledgeDebitFailedEventHandler> _logger;

    public PledgeDebitFailedEventHandler(ILevyTransferMatchingApi api, ILogger<PledgeDebitFailedEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("RunPledgeDebitFailedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.PledgeDebitFailed)] PledgeDebitFailedEvent @event)
    {
        _logger.LogInformation($"Handling PledgeDebitFailedEvent handler for application {@event.ApplicationId}");

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
            _logger.LogError(ex, $"Error handling PledgeDebitFailedEvent for application {@event.ApplicationId}");
            throw;
        }
    }
}