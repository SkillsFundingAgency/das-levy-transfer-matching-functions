using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler
{  
    [FunctionName("CreatedAccount")]
    public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.CreatedAccount)] CreatedAccountEvent createdAccountEvent, ILogger log)
    {
        log.LogInformation($"Handling event: {createdAccountEvent}");
    }
}