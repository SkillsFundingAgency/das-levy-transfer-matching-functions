using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ChangedAccountNameEventHandler
{
    private readonly ILogger<ChangedAccountNameEventHandler> _logger;

    public ChangedAccountNameEventHandler(ILogger<ChangedAccountNameEventHandler> logger)
    {
        _logger = logger;
    }

    [Function("ChangedAccountName")]
    public async Task Run([ServiceBusTrigger(QueueNames.ChangedAccountName)] ChangedAccountNameEvent changedAccountNameEvent)
    {
        _logger.LogInformation($"Handling event: {changedAccountNameEvent}");
    }
}