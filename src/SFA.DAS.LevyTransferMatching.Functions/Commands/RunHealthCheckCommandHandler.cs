using Microsoft.Extensions.Caching.Distributed;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Commands;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RunHealthCheckCommandHandler
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RunHealthCheckCommandHandler> _logger;

    public RunHealthCheckCommandHandler(IDistributedCache distributedCache, ILogger<RunHealthCheckCommandHandler> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    [Function("RunHealthCheckCommand")]
    public async Task Run([ServiceBusTrigger(QueueNames.RunHealthCheck)] RunHealthCheckCommand runHealthCheck)
    {
        _logger.LogInformation($"Handling command: {runHealthCheck}");

        await _distributedCache.SetStringAsync(runHealthCheck.MessageId, "OK");
    }
}