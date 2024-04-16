using NServiceBus;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEmailEventHandler(
    ILevyTransferMatchingApi api,
    IEncodingService encodingService,
    ILogger<ApplicationApprovedEmailEventHandler> log)
    : IHandleMessages<ApplicationApprovedEvent>
{
    public async Task Handle(ApplicationApprovedEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {message.ApplicationId}");

        var request = new ApplicationApprovedEmailRequest
        {
            PledgeId = message.PledgeId,
            ApplicationId = message.ApplicationId,
            ReceiverId = message.ReceiverAccountId,
            EncodedApplicationId = encodingService.Encode(message.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await api.ApplicationApprovedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {message.ApplicationId}");
        }
    }
}