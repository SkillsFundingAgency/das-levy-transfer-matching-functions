using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler(ILogger<CreatedAccountEventHandler> log): IHandleMessages<CreatedAccountEvent>
{  
    public Task Handle(CreatedAccountEvent message, IMessageHandlerContext context)
    {
        log.LogInformation("Handling event: {EventName}", nameof(CreatedAccountEvent));

        return Task.CompletedTask;
    }
}