using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler(ILogger log): IHandleMessages<CreatedAccountEvent>
{  
    // [Function("CreatedAccount")]
    // public async Task Run([NServiceBusTriggerInput(Endpoint = QueueNames.CreatedAccount)] CreatedAccountEvent createdAccountEvent, ILogger log)
    // {
    //     log.LogInformation($"Handling event: {createdAccountEvent}");
    // }

    public Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling event: {message}");

        return Task.CompletedTask;
    }
}