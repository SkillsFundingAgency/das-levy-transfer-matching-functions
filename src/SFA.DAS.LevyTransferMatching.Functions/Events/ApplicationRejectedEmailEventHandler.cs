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
    public class ApplicationRejectedEmailEventHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
        private readonly IEncodingService _encodingService;
        private readonly EmailNotificationsConfiguration _config;

        public ApplicationRejectedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, EmailNotificationsConfiguration config)
        {
            _levyTransferMatchingApi = api;
            _encodingService = encodingService;
            _config = config;
        }

        [FunctionName("ApplicationRejectedEmailEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationRejectedEmail)] ApplicationRejectedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationRejectedEmailEvent handler for application {@event.ApplicationId}");

            var request = new ApplicationRejectedEmailRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                ReceiverId = @event.ReceiverAccountId,
                BaseUrl= _config.ViewTransfersBaseUrl,
                EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
            };

            try
            {
                await _levyTransferMatchingApi.ApplicationRejectedEmail(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling ApplicationRejectedEmailEvent for application {@event.ApplicationId}");
            }
        }
    }
}
