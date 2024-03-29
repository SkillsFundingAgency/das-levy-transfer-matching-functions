﻿using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedForImmediateAutoApprovalEventHandler
{
    private readonly ILevyTransferMatchingApi _api;

    public ApplicationCreatedForImmediateAutoApprovalEventHandler(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [FunctionName("RunApplicationCreatedForImmediateAutoApprovalEvent")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.ApplicationCreatedForImmediateAutoApproval)] ApplicationCreatedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationCreatedForImmediateAutoApprovalEventHandler for application {@event.ApplicationId}");

        try
        {
            var request = new ApplicationCreatedForImmediateAutoApprovalRequest
            {
                PledgeId = @event.PledgeId,
                ApplicationId = @event.ApplicationId
            };
            
            await _api.ApplicationCreatedForImmediateAutoApproval(request);
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error handling ApplicationCreatedForImmediateAutoApprovalEvent");
            throw;
        }
    }
}