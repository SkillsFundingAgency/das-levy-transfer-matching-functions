using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ChangedAccountNameEventHandler(ILogger log) : IHandleMessages<ChangedAccountNameEvent>
{
    // [Function("ChangedAccountName")]
    // public async Task Run([NServiceBusTriggerInput(Endpoint = QueueNames.ChangedAccountName)] ChangedAccountNameEvent changedAccountNameEvent, ILogger log)
    // {
    //     log.LogInformation($"Handling event: {changedAccountNameEvent}");
    // }

    public Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling event: {message}");

        return Task.CompletedTask;
    }
}