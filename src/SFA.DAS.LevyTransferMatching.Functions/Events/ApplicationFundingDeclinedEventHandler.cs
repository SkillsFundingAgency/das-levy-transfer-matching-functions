using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingDeclinedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<ApplicationFundingDeclinedEventHandler> _logger;

    public ApplicationFundingDeclinedEventHandler(ILevyTransferMatchingApi api, ILogger<ApplicationFundingDeclinedEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("RunApplicationFundingDeclinedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationFundingDeclined)] ApplicationFundingDeclinedEvent @event)
    {
        _logger.LogInformation($"Handling {nameof(ApplicationFundingDeclinedEvent)} handler for application {@event.ApplicationId}");

        var request = new ApplicationFundingDeclinedRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            Amount = @event.Amount,
        };

        try
        {
            await _api.ApplicationFundingDeclined(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}