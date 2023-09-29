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
    public class ApplicationApprovedEmailEventHandler
    {
        private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
        private readonly IEncodingService _encodingService;

        public ApplicationApprovedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService)
        {
            _levyTransferMatchingApi = api;
            _encodingService = encodingService;
        }

        [FunctionName("ApplicationApprovedEmailEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationApprovedEmail)] ApplicationApprovedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {@event.ApplicationId}");

            var request = new ApplicationApprovedEmailRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId,
                ReceiverId = @event.ReceiverAccountId,
                EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
            };

            try
            {
                await _levyTransferMatchingApi.ApplicationApprovedEmail(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
            }
        }
    }
}
