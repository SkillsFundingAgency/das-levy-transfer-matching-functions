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
            log.LogInformation("ApplicationFundingExpiredEventHandler calling apim for application {ApplicationId}", message.ApplicationId);

            await api.ApplicationFundingExpired(request);

            log.LogInformation("ApplicationFundingExpiredEventHandler successfully called apim for application {ApplicationId}", message.ApplicationId);

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

