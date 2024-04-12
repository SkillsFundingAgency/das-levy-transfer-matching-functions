using NServiceBus;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEvent : IMessage
{
    public int ApplicationId { get; }
    public int PledgeId { get; }
    public DateTime ApprovedOn { get; }
    public int Amount { get; }
    public long TransferSenderId { get; set; }
    public long ReceiverAccountId { get; set; }

    public ApplicationApprovedEvent(
        int applicationId,
        int pledgeId,
        DateTime approvedOn,
        int amount,
        long transferSenderId,
        long receiverAccountId)
    {
        ApplicationId = applicationId;
        PledgeId = pledgeId;
        Amount = amount;
        ApprovedOn = approvedOn;
        TransferSenderId = transferSenderId;
        ReceiverAccountId = receiverAccountId;
    }
}

public class ApplicationApprovedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, ILogger<ApplicationApprovedEmailEventHandler> log) 
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

    // [Function("ApplicationApprovedEmailEvent")]
    // public async Task Run([ServiceBusTrigger(QueueNames.ApplicationApprovedEmail)] ApplicationApprovedEvent @event, ILogger log)
    // {
    //     log.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {@event.ApplicationId}");
    //
    //     var request = new ApplicationApprovedEmailRequest
    //     {
    //         PledgeId = @event.PledgeId,
    //         ApplicationId = @event.ApplicationId,
    //         ReceiverId = @event.ReceiverAccountId,
    //         EncodedApplicationId = encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
    //     };
    //
    //     try
    //     {
    //         await api.ApplicationApprovedEmail(request);
    //     }
    //     catch (ApiException ex)
    //     {
    //         if (ex.StatusCode != HttpStatusCode.BadRequest) throw;
    //
    //         log.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
    //     }
    // }
}