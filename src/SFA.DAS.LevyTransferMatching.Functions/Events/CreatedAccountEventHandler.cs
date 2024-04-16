using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler(ILogger log): IHandleMessages<CreatedAccountEvent>
{  
    public Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling event: {message}");

        return Task.CompletedTask;
    }
}