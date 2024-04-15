using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Functions.Bindings;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ChangedAccountNameEventHandler
{
    [Function("ChangedAccountName")]
    public async Task Run([NServiceBusTriggerOutput(Endpoint = QueueNames.ChangedAccountName)] ChangedAccountNameEvent changedAccountNameEvent, ILogger log)
    {
        log.LogInformation($"Handling event: {changedAccountNameEvent}");
    }
}