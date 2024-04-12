using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedForLegacyTopicPublishingEventHandler(
    ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
{
    [Function("ApplicationApprovedForLegacyTopicPublishing")]
    public async Task Run([QueueTrigger(QueueNames.ApplicationApprovedForLegacyTopicPublishing)] ApplicationApprovedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationApprovedForLegacyTopicPublishing handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationApproved(@event.ApplicationId, @event.PledgeId, @event.ApprovedOn, @event.Amount, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}