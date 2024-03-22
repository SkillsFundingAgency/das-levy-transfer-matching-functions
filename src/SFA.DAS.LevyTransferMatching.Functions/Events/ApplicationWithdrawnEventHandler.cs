using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;
    private readonly ILogger<ApplicationWithdrawnEventHandler> _logger;

    public ApplicationWithdrawnEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher,
        ILogger<ApplicationWithdrawnEventHandler> logger)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
    }

    [Function("RunApplicationWithdrawnEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationWithdrawn)] ApplicationWithdrawnEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationWithdrawn handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationWithdrawn(@event.ApplicationId, @event.PledgeId, @event.WithdrawnOn, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling ApplicationWithdrawnEvent for application {@event.ApplicationId}");
        }
    }
}