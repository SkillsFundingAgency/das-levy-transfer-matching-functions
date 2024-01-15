using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingDeclinedEventHandler
{
    private readonly ILevyTransferMatchingApi _api;

    public ApplicationFundingDeclinedEventHandler(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [FunctionName("RunApplicationFundingDeclinedEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationFundingDeclined)] ApplicationFundingDeclinedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling {nameof(ApplicationFundingDeclinedEvent)} handler for application {@event.ApplicationId}");
     
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
     
            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}