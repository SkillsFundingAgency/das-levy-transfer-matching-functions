using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;

    public ApplicationApprovedEventHandler(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [FunctionName("RunApplicationApprovedEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedEvent)] ApplicationApprovedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationApprovedEvent handler for application {@event.ApplicationId}");

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

            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}