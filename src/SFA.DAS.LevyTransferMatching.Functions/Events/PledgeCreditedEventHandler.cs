using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.EventHandlers
{
    public class PledgeCreditedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;


        public PledgeCreditedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        //replace event wiht PledgeCreditedEvent
        [FunctionName("PledgeCreditedEventHandler")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.PledgeCredited)] ApplicationCreatedEvent @event, ILogger log)
        {
            //REPLACE ApplicationCreatedEvent with PledgeCreditedEvent once available
            // log.LogInformation($"Handling {nameof(PledgeCreditedEvent)} handler for pledge {@event.PledgeId}");

            var request = new ApplicationAutomaticApprovalRequest
            {
                PledgeId = @event.PledgeId,               
            };

            try
            {
                var response = await _api.ApplicationsWithAutomaticApproval(request);

                if (response != null)
                {
                    foreach (var app in response.Applications)
                    {
                        
                        await _api.ApproveAutomaticApplication(new ApproveAutomaticApplicationRequest 
                        { 
                            ApplicationId = app.Id, 
                            PledgeId = app.PledgeId 
                        });
                    }
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
