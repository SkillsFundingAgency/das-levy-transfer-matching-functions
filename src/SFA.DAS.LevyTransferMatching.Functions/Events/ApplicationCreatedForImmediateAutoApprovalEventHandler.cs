using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedForImmediateAutoApprovalEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<ApplicationCreatedForImmediateAutoApprovalEventHandler> _logger;

    public ApplicationCreatedForImmediateAutoApprovalEventHandler(ILevyTransferMatchingApi api,
        ILogger<ApplicationCreatedForImmediateAutoApprovalEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("RunApplicationCreatedForImmediateAutoApprovalEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationCreatedForImmediateAutoApproval)] ApplicationCreatedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationCreatedForImmediateAutoApprovalEventHandler for application {@event.ApplicationId}");

        try
        {
            var request = new ApplicationCreatedForImmediateAutoApprovalRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId
            };

            await _api.ApplicationCreatedForImmediateAutoApproval(request);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, $"Error handling ApplicationCreatedForImmediateAutoApprovalEvent");
            throw;
        }
    }
}