using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedForLegacyTopicPublishingEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;
    private readonly ILogger<ApplicationApprovedForLegacyTopicPublishingEventHandler> _logger;

    public ApplicationApprovedForLegacyTopicPublishingEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher, ILogger<ApplicationApprovedForLegacyTopicPublishingEventHandler> logger)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        _logger = logger;
    }

    [Function("ApplicationApprovedForLegacyTopicPublishing")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationApprovedForLegacyTopicPublishing)] ApplicationApprovedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationApprovedForLegacyTopicPublishing handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationApproved(@event.ApplicationId, @event.PledgeId, @event.ApprovedOn, @event.Amount, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
        }
    }
}