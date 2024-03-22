using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnAfterAcceptanceEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<ApplicationWithdrawnAfterAcceptanceEventHandler> _logger;

    public ApplicationWithdrawnAfterAcceptanceEventHandler(ILevyTransferMatchingApi api, ILogger<ApplicationWithdrawnAfterAcceptanceEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("RunApplicationWithdrawnAfterAcceptanceEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationWithdrawnAfterAcceptance)] ApplicationWithdrawnAfterAcceptanceEvent @event)
    {
        _logger.LogInformation($"Handling {nameof(ApplicationWithdrawnAfterAcceptanceEvent)} handler for application {@event.ApplicationId}");

        var request = new ApplicationWithdrawnAfterAcceptanceRequest
        {
            ApplicationId = @event.ApplicationId,
            PledgeId = @event.PledgeId,
            Amount = @event.Amount,
        };

        try
        {
            await _api.ApplicationWithdrawnAfterAcceptance(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}