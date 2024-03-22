using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.LevyTransferMatching.Infrastructure;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class CreatedAccountEventHandler
{
    private readonly ILogger<CreatedAccountEventHandler> _logger;

    public CreatedAccountEventHandler(ILogger<CreatedAccountEventHandler> logger)
    {
        _logger = logger;
    }

    [Function("CreatedAccount")]
    public async Task Run([ServiceBusTrigger(QueueNames.CreatedAccount)] CreatedAccountEvent createdAccountEvent)
    {
        _logger.LogInformation($"Handling event: {createdAccountEvent}");
    }
}