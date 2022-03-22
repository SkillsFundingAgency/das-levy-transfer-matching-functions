using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Legacy;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationFundingAcceptedForLegacyTopicPublishingEventHandler
    {
        private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

        public ApplicationFundingAcceptedForLegacyTopicPublishingEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
        {
            _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        }

        [FunctionName("ApplicationFundingAcceptedForLegacyTopicPublishing")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationFundingAcceptedForLegacyTopicPublishing)] ApplicationFundingAcceptedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationFundingAcceptedForLegacyTopicPublishing handler for application {@event.ApplicationId}");

            try
            {
                var legacyMessage = new Messages.Legacy.PledgeApplicationFundingAccepted(@event.ApplicationId, @event.PledgeId, @event.AccountId, @event.AcceptedOn);
                await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error handling ApplicationFundingAcceptedEvent for application {@event.ApplicationId}");
            }
        }
    }
}
