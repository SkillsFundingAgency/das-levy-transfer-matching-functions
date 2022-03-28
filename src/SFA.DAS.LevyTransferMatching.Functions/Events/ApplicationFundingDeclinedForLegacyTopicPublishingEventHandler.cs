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
    public class ApplicationFundingDeclinedForLegacyTopicPublishingEventHandler
    {
        private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

        public ApplicationFundingDeclinedForLegacyTopicPublishingEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
        {
            _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        }

        [FunctionName("ApplicationFundingDeclinedForLegacyTopicPublishing")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationFundingDeclinedForLegacyTopicPublishing)] ApplicationFundingDeclinedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationFundingDeclinedForLegacyTopicPublishing handler for application {@event.ApplicationId}");

            try
            {
                var legacyMessage = new Messages.Legacy.PledgeApplicationFundingDeclined(@event.ApplicationId, @event.PledgeId, @event.DeclinedOn, @event.AccountId);
                await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error handling ApplicationFundingDeclinedEvent for application {@event.ApplicationId}");
            }
        }
    }
}
