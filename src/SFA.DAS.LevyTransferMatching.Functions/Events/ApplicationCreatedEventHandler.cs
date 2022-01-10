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
    public class ApplicationCreatedEventHandler
    {
        private readonly ILegacyTopicMessagePublisher _legacyTopicMessagePublisher;

        public ApplicationCreatedEventHandler(ILegacyTopicMessagePublisher legacyTopicMessagePublisher)
        {
            _legacyTopicMessagePublisher = legacyTopicMessagePublisher;
        }

        [FunctionName("RunApplicationCreatedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationCreated)] ApplicationCreatedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationCreated handler for application {@event.ApplicationId}");

            try
            {
                var legacyMessage = new Messages.Legacy.PledgeApplicationCreated(@event.ApplicationId, @event.PledgeId, @event.CreatedOn, @event.TransferSenderId);
                await _legacyTopicMessagePublisher.PublishAsync(legacyMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error handling ApplicationCreatedEvent for application {@event.ApplicationId}");
            }
        }
    }
}
