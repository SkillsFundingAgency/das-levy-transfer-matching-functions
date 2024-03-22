using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class PledgeCreditedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<PledgeCreditedEventHandler> _logger;

    public PledgeCreditedEventHandler(ILevyTransferMatchingApi api, ILogger<PledgeCreditedEventHandler> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("PledgeCreditedEventHandler")]
    public async Task Run([ServiceBusTrigger(QueueNames.PledgeCredited)] PledgeCreditedEvent @event)
    {
        _logger.LogInformation($"Handling {nameof(PledgeCreditedEvent)} for pledge {@event.PledgeId}");

        try
        {
            var response = await _api.GetApplicationsForAutomaticApproval(@event.PledgeId);

            foreach (var app in response.Applications)
            {
                await _api.ApproveApplication(new ApproveApplicationRequest
                {
                    ApplicationId = app.Id,
                    PledgeId = app.PledgeId
                });
            }
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling PledgeCreditedEvent for pledge {@event.PledgeId}");
        }
    }
}