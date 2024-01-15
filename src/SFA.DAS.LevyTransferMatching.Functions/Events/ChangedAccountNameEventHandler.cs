using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ChangedAccountNameEventHandler
{
    [FunctionName("ChangedAccountName")]
    public void Run([NServiceBusTrigger(Endpoint = QueueNames.ChangedAccountName)] ChangedAccountNameEvent changedAccountNameEvent, ILogger log)
    {
        log.LogInformation($"Handling event: {changedAccountNameEvent}");
    }
}