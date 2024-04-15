using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Bindings;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler
{  
    [Function("CreatedAccount")]
    public async Task Run([NServiceBusTriggerOutput(Endpoint = QueueNames.CreatedAccount)] CreatedAccountEvent createdAccountEvent, ILogger log)
    {
        log.LogInformation($"Handling event: {createdAccountEvent}");
    }
}