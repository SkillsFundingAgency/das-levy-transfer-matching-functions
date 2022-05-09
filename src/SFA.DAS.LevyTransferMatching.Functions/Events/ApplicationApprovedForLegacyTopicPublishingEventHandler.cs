using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationApprovedForLegacyTopicPublishingEventHandler
    {
        private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

        public ApplicationApprovedForLegacyTopicPublishingEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
        {
            _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        }

        [FunctionName("ApplicationApprovedForLegacyTopicPublishing")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedForLegacyTopicPublishing)] ApplicationApprovedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationApprovedForLegacyTopicPublishing handler for application {@event.ApplicationId}");

            try
            {
                var legacyMessage = new Messages.Legacy.PledgeApplicationApproved(@event.ApplicationId, @event.PledgeId, @event.ApprovedOn, @event.Amount, @event.TransferSenderId, @event.ReceiverAccountId);
                await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
            }
        }
    }
}
