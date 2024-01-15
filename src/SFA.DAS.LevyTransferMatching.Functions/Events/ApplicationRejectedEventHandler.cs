using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

    public ApplicationRejectedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
    }

    [FunctionName("RunApplicationRejectedEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationRejected)] ApplicationRejectedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationRejected handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationRejected(@event.ApplicationId, @event.PledgeId, @event.RejectedOn, @event.Amount, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationRejectedEvent for application {@event.ApplicationId}");
        }
    }
}