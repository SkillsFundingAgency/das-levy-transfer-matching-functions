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
    public class PledgeCreditedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public PledgeCreditedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("PledgeCreditedEventHandler")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.PledgeCredited)] PledgeCreditedEvent @event, ILogger log)
        {
             log.LogInformation($"Handling {nameof(PledgeCreditedEvent)} for pledge {@event.PledgeId}");

            try
            {
                var response = await _api.GetApplicationsForAutomaticApproval(@event.PledgeId);

                foreach (var app in response.Applications)
                {
                    await _api.ApproveApplication(new ApproveApplicationRequest 
                    { 
                        ApplicationId = app.Id, 
                        PledgeId = app.PledgeId 
                    });
                }
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling PledgeCreditedEvent for pledge {@event.PledgeId}");
            }
        }
    }
}
