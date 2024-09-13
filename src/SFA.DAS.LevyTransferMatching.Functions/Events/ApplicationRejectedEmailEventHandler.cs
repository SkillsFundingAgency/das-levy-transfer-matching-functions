using NServiceBus;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEmailEventHandler(
    ILevyTransferMatchingApi api,
    IEncodingService encodingService,
    EmailNotificationsConfiguration config,
    ILogger log) : IHandleMessages<ApplicationRejectedEvent>
{
    public async Task Handle(ApplicationRejectedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationRejectedEmailEvent handler for application {ApplicationId}", @event.ApplicationId);

        var request = new ApplicationRejectedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            BaseUrl = config.ViewTransfersBaseUrl,
            EncodedApplicationId = encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await api.ApplicationRejectedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            log.LogError(ex, "Error handling ApplicationRejectedEmailEvent for application {ApplicationId}", @event.ApplicationId);
        }
    }
}