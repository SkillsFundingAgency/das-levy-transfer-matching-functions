using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands
{
    public class ApplicationApprovedEventHandler
    {
        public ApplicationApprovedEventHandler()
        {
        }

        [FunctionName("RunApplicationApprovedEvent")]
        public async Task Run([NServiceBusTrigger(Endpoint = QueueNames.RunHealthCheck)] ApplicationApprovedEvent @event, ILogger log)
        {
            log.LogInformation($"Handling ApplicationApprovedEvent handler for application {@event.ApplicationId}");
        }
    }
}
