using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationApprovedEventHandlerForReceiverNotification
    {
        private readonly ILevyTransferMatchingApi __levyTransferMatchingApi;
        private readonly IEncodingService _encodingService;
        private readonly EmailNotificationsConfiguration _config;

        public ApplicationApprovedEventHandlerForReceiverNotification(ILevyTransferMatchingApi api, IEncodingService encodingService, EmailNotificationsConfiguration config)
        {
            __levyTransferMatchingApi = api;
            _encodingService = encodingService;
            _config = config;
        }

        [FunctionName("RunApplicationApprovedEventForReceiverNotification")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedEventForReceiverNotification)] ApplicationApprovedReceiverNotificationEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationApprovedEventForReceiverNotification handler for application {@event.ApplicationId}");

            var request = new ApplicationApprovedReceiverNotificationRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                ReceiverId = @event.TransferReceiverId,
                BaseUrl = _config.ViewTransfersBaseUrl,
                ReceiverEncodedAccountId = _encodingService.Encode(@event.ReceiverAccountId, EncodingType.AccountId)
            };

            try
            {
                await __levyTransferMatchingApi.ApplicationApprovedReceiverNotification(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling ApplicationApprovedEventForReceiverNotification for application {@event.ApplicationId}");
            }
        }
    }
}
