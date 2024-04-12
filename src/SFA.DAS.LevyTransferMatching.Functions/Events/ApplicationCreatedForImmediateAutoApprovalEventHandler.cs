using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedForImmediateAutoApprovalEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunApplicationCreatedForImmediateAutoApprovalEvent")]
    public async Task Run([QueueTrigger(QueueNames.ApplicationCreatedForImmediateAutoApproval)] ApplicationCreatedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationCreatedForImmediateAutoApprovalEventHandler for application {@event.ApplicationId}");

        try
        {
            var request = new ApplicationCreatedForImmediateAutoApprovalRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId
            };
            
            await api.ApplicationCreatedForImmediateAutoApproval(request);
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error handling ApplicationCreatedForImmediateAutoApprovalEvent");
            throw;
        }
    }
}