using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using RestEase;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class TransferRequestApprovedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;

        public TransferRequestApprovedEventHandler(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("RunTransferRequestApprovedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.TransferRequestApprovedEvent)] TransferRequestApprovedEvent @event, ILogger log)
        {
            if (!@event.PledgeApplicationId.HasValue)
            {
                log.LogInformation($"TransferRequestApprovedEvent for Transfer Request {@event.TransferRequestId} has no PledgeApplicationId; ignoring");
                return;
            }

            log.LogInformation($"Handling TransferRequestApprovedEvent handler for application {@event.PledgeApplicationId}");

            var request = new TransferRequestApprovedRequest
            {
                ApplicationId = @event.PledgeApplicationId.Value,
                NumberOfApprentices = @event.NumberOfApprentices,
                Amount = @event.FundingCap.HasValue ? (int)decimal.Round(@event.FundingCap.Value) : 0
            };

            try
            {
                await _api.DebitApplication(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling TransferRequestApprovedEvent for application {@event.PledgeApplicationId}");
            }
            
        }
    }
}
