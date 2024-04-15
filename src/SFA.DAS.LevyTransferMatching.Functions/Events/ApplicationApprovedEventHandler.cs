using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEventHandler(ILevyTransferMatchingApi api, ILogger log) : IHandleMessages<ApplicationApprovedEvent>
{
    // [Function("RunApplicationApprovedEvent")]
    // public async Task Run([NServiceBusTriggerInput(Endpoint = QueueNames.ApplicationApprovedEvent)] ApplicationApprovedEvent @event, ILogger log)
    // {
    //     log.LogInformation($"Handling ApplicationApprovedEvent handler for application {@event.ApplicationId}");
    //
    //     var request = new ApplicationApprovedRequest
    //     {
    //         PledgeId = @event.PledgeId,
    //         ApplicationId = @event.ApplicationId,
    //         Amount = @event.Amount
    //     };
    //
    //     try
    //     {
    //         await api.ApplicationApproved(request);
    //     }
    //     catch (ApiException ex)
    //     {
    //         if (ex.StatusCode != HttpStatusCode.BadRequest) throw;
    //
    //         log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
    //     }
    // }

    public async Task Handle(ApplicationApprovedEvent @event, IMessageHandlerContext context)
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
            await api.ApplicationApproved(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}