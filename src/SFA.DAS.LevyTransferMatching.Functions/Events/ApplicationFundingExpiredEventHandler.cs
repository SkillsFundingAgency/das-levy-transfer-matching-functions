using NServiceBus;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Messages.Events;

namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationFundingExpiredEventHandler(
    ILevyTransferMatchingApi api,
    ILogger<ApplicationFundingExpiredEventHandler> log)
    : IHandleMessages<ApplicationFundingExpiredEvent>
{
    public async Task Handle(ApplicationFundingExpiredEvent message, IMessageHandlerContext context)
    {
        log.LogInformation("Handling ApplicationFundingExpiredEvent handler for application {ApplicationId}", message.ApplicationId);

        var request = new ApplicationFundingExpiredRequest
        {
            ApplicationId = message.ApplicationId,
            PledgeId = message.PledgeId,
            Amount = message.Amount
        };

        try
        {
            await api.ApplicationFundingExpired(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            log.LogError(ex, "Error handling ApplicationFundingExpiredEvent for application {ApplicationId}", message.ApplicationId);
        }
    }
}

// todo remove once the messages package has been updated
public class ApplicationFundingExpiredEvent(int applicationId, int pledgeId, int amount)
{
    public int ApplicationId { get; } = applicationId;
    public int PledgeId { get; } = pledgeId;
    public int Amount { get; } = amount;
}

