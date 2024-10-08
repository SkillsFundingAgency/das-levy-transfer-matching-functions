﻿using NServiceBus;
using RestEase;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class TransferRequestApprovedEventHandler(ILevyTransferMatchingApi api, ILogger<TransferRequestApprovedEventHandler> log) : IHandleMessages<TransferRequestApprovedEvent>
{
    public async Task Handle(TransferRequestApprovedEvent @event, IMessageHandlerContext context)
    {
        if (@event.PledgeApplicationId != null)
        {
            log.LogInformation("Handling TransferRequestApprovedEvent handler for application {PledgeApplicationId}", @event.PledgeApplicationId);

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
                if (ex.StatusCode != HttpStatusCode.BadRequest)
                {
                    throw;
                }

                log.LogError(ex, "Error handling TransferRequestApprovedEvent for application {PledgeApplicationId}", @event.PledgeApplicationId);
            }
        }
    }
}