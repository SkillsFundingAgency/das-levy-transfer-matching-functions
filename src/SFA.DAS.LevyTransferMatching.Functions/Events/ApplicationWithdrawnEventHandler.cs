using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationWithdrawnEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
{
    [Function("RunApplicationWithdrawnEvent")]
    public async Task Run([QueueTrigger(QueueNames.ApplicationWithdrawn)] ApplicationWithdrawnEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationWithdrawn handler for application {@event.ApplicationId}");

        try
        {
            var legacyMessage = new Messages.Legacy.PledgeApplicationWithdrawn(@event.ApplicationId, @event.PledgeId, @event.WithdrawnOn, @event.TransferSenderId);
            await legacyTopicMessagePublisher.PublishAsync(legacyMessage);
        }
        catch (Exception ex)
        {
            log.LogError(ex, $"Error handling ApplicationWithdrawnEvent for application {@event.ApplicationId}");
        }
    }
}