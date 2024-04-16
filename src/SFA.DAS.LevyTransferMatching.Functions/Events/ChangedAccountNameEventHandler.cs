using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ChangedAccountNameEventHandler(ILogger log) : IHandleMessages<ChangedAccountNameEvent>
{
    public Task Handle(ChangedAccountNameEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling event: {message}");

        return Task.CompletedTask;
    }
}