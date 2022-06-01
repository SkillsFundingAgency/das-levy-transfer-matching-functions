using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationWithdrawnAfterAcceptanceEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public ApplicationWithdrawnAfterAcceptanceEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunApplicationWithdrawnAfterAcceptanceEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationWithdrawnAfterAcceptance)] ApplicationWithdrawnAfterAcceptanceEvent @event, ILogger log)
        {
            log.LogInformation($"Handling {nameof(ApplicationWithdrawnAfterAcceptanceEvent)} handler for application {@event.ApplicationId}");
         
            var request = new ApplicationWithdrawnAfterAcceptanceRequest
            {
                ApplicationId = @event.ApplicationId,
                PledgeId = @event.PledgeId,
                Amount = @event.Amount,
            };
         
            try
            {
                await _api.ApplicationWithdrawnAfterAcceptance(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;
         
                log.LogError(ex, $"Error handling ApplicationApprovedEvent for application {@event.ApplicationId}");
            }
        }
    }
}
