using NServiceBus;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher, ILogger log) : IHandleMessages<ApplicationRejectedEvent>
{
    public async Task Handle(ApplicationRejectedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationRejected handler for application {ApplicationId}", @event.ApplicationId);

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationRejected(@event.ApplicationId, @event.PledgeId, @event.RejectedOn, @event.Amount, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error handling ApplicationRejectedEvent for application {ApplicationId}", @event.ApplicationId);
        }
    }
}