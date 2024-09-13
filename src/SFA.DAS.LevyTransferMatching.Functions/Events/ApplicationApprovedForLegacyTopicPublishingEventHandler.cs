using NServiceBus;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedForLegacyTopicPublishingEventHandler(
    ILegacyTopicMessagePublisher legacyTopicMessagePublisher,
    ILogger log) : IHandleMessages<ApplicationApprovedEvent>
{
    public async Task Handle(ApplicationApprovedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationApprovedForLegacyTopicPublishing handler for application {ApplicationId}", @event.ApplicationId);

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationApproved(@event.ApplicationId, @event.PledgeId, @event.ApprovedOn, @event.Amount, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error handling ApplicationApprovedEvent for application {ApplicationId}", @event.ApplicationId);
        }
    }
}