using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedForImmediateAutoApprovalEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<ApplicationCreatedEvent>
{
    public async Task Handle(ApplicationCreatedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationCreatedForImmediateAutoApprovalEventHandler for application {@event.ApplicationId}", @event.ApplicationId);

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
            log.LogError(ex, "Error handling ApplicationCreatedForImmediateAutoApprovalEvent");
            throw;
        }
    }
}