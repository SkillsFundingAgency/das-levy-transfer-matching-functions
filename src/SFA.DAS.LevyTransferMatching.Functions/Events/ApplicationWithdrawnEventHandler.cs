using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnEventHandler
{
    private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

    public ApplicationWithdrawnEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
    {
        _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
    }

    [FunctionName("RunApplicationWithdrawnEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationWithdrawn)] ApplicationWithdrawnEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationWithdrawn handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationWithdrawn(@event.ApplicationId, @event.PledgeId, @event.WithdrawnOn, @event.TransferSenderId);
            await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationWithdrawnEvent for application {@event.ApplicationId}");
        }
    }
}