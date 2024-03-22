using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;
    private readonly ILogger<ApplicationRejectedEventHandler> _logger;

    public ApplicationRejectedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher,
        ILogger<ApplicationRejectedEventHandler> logger)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        _logger = logger;
    }

    [Function("RunApplicationRejectedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationRejected)] ApplicationRejectedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationRejected handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationRejected(@event.ApplicationId, @event.PledgeId, @event.RejectedOn, @event.Amount, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling ApplicationRejectedEvent for application {@event.ApplicationId}");
        }
    }
}