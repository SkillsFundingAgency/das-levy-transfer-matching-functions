using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<ApplicationApprovedEventHandler> _logger;

    public ApplicationApprovedEventHandler(ILevyTransferMatchingApi api, ILogger<ApplicationApprovedEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("RunApplicationApprovedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationApprovedEvent)] ApplicationApprovedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationApprovedEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationApprovedRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            Amount = @event.Amount
        };

        try
        {
            await _api.ApplicationApproved(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}