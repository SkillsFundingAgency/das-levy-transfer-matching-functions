using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Functions.Bindings;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService)
{
    [Function("ApplicationCreatedEmailEvent")]
    public async Task Run([NServiceBusTriggerOutput(Endpoint = QueueNames.ApplicationCreatedEmailEvent)] ApplicationCreatedEvent @event, ILogger log)
    {
        log.LogInformation($"Handling ApplicationCreatedEmailEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationCreatedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            EncodedApplicationId = encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await api.ApplicationCreatedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, $"Error handling ApplicationCreatedEmailEvent for application {@event.ApplicationId}");
        }
    }
}