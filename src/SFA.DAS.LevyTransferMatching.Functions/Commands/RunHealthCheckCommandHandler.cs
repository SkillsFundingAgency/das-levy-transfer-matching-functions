using Microsoft.Extensions.Caching.Distributed;
using NServiceBus;
using SFA.DAS.LevyTransferMatching.Messages.Commands;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class RunHealthCheckCommandHandler(IDistributedCache distributedCache, ILogger<RunHealthCheckCommandHandler> log) : IHandleMessages<RunHealthCheckCommand>
{
    public async Task Handle(RunHealthCheckCommand runHealthCheck, IMessageHandlerContext context)
    {
        log.LogInformation("Handling command: {CommandName}", nameof(RunHealthCheckCommand));
        
        await distributedCache.SetStringAsync(runHealthCheck.MessageId, "OK", context.CancellationToken);
    }
}