using NServiceBus;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher, ILogger log) : IHandleMessages<ApplicationCreatedEvent>
{
    public async Task Handle(ApplicationCreatedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling ApplicationCreated handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationCreated(@event.ApplicationId, @event.PledgeId, @event.CreatedOn, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationCreatedEvent for application {@event.ApplicationId}");
        }
    }
}