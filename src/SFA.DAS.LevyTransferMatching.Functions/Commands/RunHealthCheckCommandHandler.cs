using Microsoft.Extensions.Caching.Distributed;
using NServiceBus;
using SFA.DAS.LevyTransferMatching.Messages.Commands;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RunHealthCheckCommandHandler: IHandleMessages<RunHealthCheckCommand>
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger _log;

    public RunHealthCheckCommandHandler(IDistributedCache distributedCache, ILogger log)
    {
        _distributedCache = distributedCache;
        _log = log;
    }
    
    public async Task Handle(RunHealthCheckCommand runHealthCheck, IMessageHandlerContext context)
    {
        _log.LogInformation($"Handling command: {runHealthCheck}");
        
        await _distributedCache.SetStringAsync(runHealthCheck.MessageId, "OK");
    }
    
    // [Function("RunHealthCheckCommand")]
    // public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.RunHealthCheck)] RunHealthCheckCommand runHealthCheck, ILogger log)
    // {
    //     log.LogInformation($"Handling command: {runHealthCheck}");
    //
    //     await _distributedCache.SetStringAsync(runHealthCheck.MessageId, "OK");
    // }
}