using NServiceBus;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, ILogger log)
    : IHandleMessages<ApplicationApprovedEvent>
{
    public async Task Handle(ApplicationApprovedEvent @event, IMessageHandlerContext context)
    {
        log.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationApprovedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            EncodedApplicationId = encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await api.ApplicationApprovedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
        }
    }
}

//{
//    [Function("ApplicationApprovedEmailEvent")]
//    public async Task Run([QueueTrigger(QueueNames.ApplicationApprovedEmail)] ApplicationApprovedEvent @event, ILogger log)
//    {
//        log.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {@event.ApplicationId}");

//        var request = new ApplicationApprovedEmailRequest
//        {
//            PledgeId = @event.PledgeId,
//            ApplicationId = @event.ApplicationId,
//            ReceiverId = @event.ReceiverAccountId,
//            EncodedApplicationId = encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
//        };

//        try
//        {
//            await api.ApplicationApprovedEmail(request);
//        }
//        catch (ApiException ex)
//        {
//            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

//            log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
//        }
//    }
//}