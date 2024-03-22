using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;
    private readonly ILogger<ApplicationCreatedEventHandler> _logger;

    public ApplicationCreatedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher, ILogger<ApplicationCreatedEventHandler> logger)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        _logger = logger;
    }

    [Function("RunApplicationCreatedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationCreated)] ApplicationCreatedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationCreated handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationCreated(@event.ApplicationId, @event.PledgeId, @event.CreatedOn, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling ApplicationCreatedEvent for application {@event.ApplicationId}");
        }
    }
}