using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events
{
    public class ApplicationFundingAcceptedEventHandler
    {
        private readonly ILevyTransferMatchingApi _api;
        private readonly ILogger<ApplicationFundingAcceptedEventHandler> _logger;

        public ApplicationFundingAcceptedEventHandler(ILevyTransferMatchingApi api, ILogger<ApplicationFundingAcceptedEventHandler> logger)
        {
            _api = api;
            _logger = logger;
        }

        [Function("RunApplicationFundingAcceptedEvent")]
        public async Task Run([ServiceBusTrigger(QueueNames.ApplicationFundingAccepted)] ApplicationFundingAcceptedEvent @event)
        {
            _logger.LogInformation($"Handling {nameof(ApplicationFundingAcceptedEvent)} handler for application {@event.ApplicationId}");
            if (@event.RejectApplications)
            {
                _logger.LogInformation($"Rejecting Pengding applications for pledge {@event.PledgeId}");

                var request = new RejectPledgeApplicationsRequest
                {
                    PledgeId = @event.PledgeId
                };

                try
                {
                    await _api.RejectPledgeApplications(request);
                }
                catch (ApiException ex)
                {
                    if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

                    _logger.LogError(ex, $"Error handling {nameof(ApplicationFundingAcceptedEvent)} for application {@event.ApplicationId}");
                }
            }
        }
    }
}