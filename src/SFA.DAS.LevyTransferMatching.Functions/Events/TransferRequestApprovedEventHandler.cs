using RestEase;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class TransferRequestApprovedEventHandler(ILevyTransferMatchingApi api)
{
    [Function("RunTransferRequestApprovedEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.TransferRequestApprovedEvent)] TransferRequestApprovedEvent @event, ILogger log)
    {
        if (@event.PledgeApplicationId != null)
        {

            log.LogInformation($"Handling TransferRequestApprovedEvent handler for application {@event.PledgeApplicationId}");

            var request = new TransferRequestApprovedRequest
            {
                ApplicationId = @event.PledgeApplicationId.Value,
                NumberOfApprentices = @event.NumberOfApprentices,
                Amount = @event.FundingCap.HasValue ? (int)decimal.Round(@event.FundingCap.Value) : 0
            };

            try
            {
                await api.DebitApplication(request);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                log.LogError(ex, $"Error handling TransferRequestApprovedEvent for application {@event.PledgeApplicationId}");
            }
        }
    }
}