using NServiceBus;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher, ILogger log) : IHandleMessages<ApplicationRejectedEvent>
{
    // [Function("RunApplicationRejectedEvent")]
    // public async Task Run([NServiceBusTriggerInput(Endpoint = QueueNames.ApplicationRejected)] ApplicationRejectedEvent @event, ILogger log)
    // {
    //     log.LogInformation($"Handling ApplicationRejected handler for application {@event.ApplicationId}");
    //
    //     try
    //     {
    //         var legacyMessage = new Messages.Legacy.PledgeApplicationRejected(@event.ApplicationId, @event.PledgeId, @event.RejectedOn, @event.Amount, @event.TransferSenderId);
    //         await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
    //     }
    //     catch (Exception ex)
    //     {
    //         log.LogError(ex, $"Error handling ApplicationRejectedEvent for application {@event.ApplicationId}");
    //     }
    // }

    public async Task Handle(ApplicationRejectedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling ApplicationRejected handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationRejected(@event.ApplicationId, @event.PledgeId, @event.RejectedOn, @event.Amount, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationRejectedEvent for application {@event.ApplicationId}");
        }
    }
}