using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler
{  
    [Function("CreatedAccount")]
    public async Task Run([ServiceBusTrigger(QueueNames.CreatedAccount)] CreatedAccountEvent createdAccountEvent, ILogger log)
    {
        log.LogInformation($"Handling event: {createdAccountEvent}");
    }
}