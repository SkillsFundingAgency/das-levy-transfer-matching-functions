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
    public class ApplicationCreatedEmailEventHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
        private readonly IEncodingService _encodingService;
        private readonly EmailNotificationsConfiguration _config;

        public ApplicationCreatedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, EmailNotificationsConfiguration config)
        {
            _levyTransferMatchingApi = api;
            _encodingService = encodingService;
            _config = config;
        }

        [FunctionName("ApplicationCreatedEmailEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedEmail)] ApplicationCreatedEmailEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationCreatedEmailEvent handler for application {@event.ApplicationId}");

            var request = new ApplicationCreatedEmailRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                ReceiverId = @event.TransferReceiverId,
                BaseUrl = _config.ViewTransfersBaseUrl,
                ReceiverEncodedAccountId = _encodingService.Encode(@event.ReceiverAccountId, EncodingType.AccountId)
            };

            try
            {
                await _levyTransferMatchingApi.ApplicationCreatedEmail(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
            }
        }
    }
}
