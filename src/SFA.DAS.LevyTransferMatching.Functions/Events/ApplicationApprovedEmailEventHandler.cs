using NServiceBus;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Constants;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEmailEventHandler(
    ILevyTransferMatchingApi api,
    IEncodingService encodingService,
    EmailNotificationsConfiguration config,
    ILogger<ApplicationApprovedEmailEventHandler> log)
    : IHandleMessages<ApplicationApprovedEvent>
{
    public async Task Handle(ApplicationApprovedEvent message, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationApprovedEmailEvent handler for application {ApplicationId}", message.ApplicationId);

        var request = new ApplicationApprovedEmailRequest
        {
            PledgeId = message.PledgeId,
            ApplicationId = message.ApplicationId,
            ReceiverId = message.ReceiverAccountId,
            EncodedAccountId = encodingService.Encode(message.ReceiverAccountId, EncodingType.PublicAccountId),
            EncodedApplicationId = encodingService.Encode(message.ApplicationId, EncodingType.PledgeApplicationId),
            TransfersBaseUrl = config.ViewTransfersBaseUrl,
            UnsubscribeUrl = config.ViewAccountBaseUrl + NotificationConstants.NotificationSettingsPath
        };

        try
        {
            await api.ApplicationApprovedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            log.LogError(ex, "Error handling ApplicationApprovedEmailEvent for application {ApplicationId}", message.ApplicationId);
        }
    }
}