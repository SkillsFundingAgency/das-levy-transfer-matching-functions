using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationCreatedEmailEventHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
        private readonly IEncodingService _encodingService;

        public ApplicationCreatedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService)
        {
            _levyTransferMatchingApi = api;
            _encodingService = encodingService;
        }

        [FunctionName("ApplicationCreatedEmailEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationCreatedEmailEvent)] ApplicationCreatedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationCreatedEmailEvent handler for application {@event.ApplicationId}");

            var request = new ApplicationCreatedEmailRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                ReceiverId = @event.ReceiverAccountId,
                EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
            };

            try
            {
                await _levyTransferMatchingApi.ApplicationCreatedEmail(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling ApplicationCreatedEmailEvent for application {@event.ApplicationId}");
            }
        }
    }
}