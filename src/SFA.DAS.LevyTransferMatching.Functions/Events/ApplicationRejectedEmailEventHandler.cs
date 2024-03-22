using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationRejectedEmailEventHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
    private readonly IEncodingService _encodingService;
    private readonly EmailNotificationsConfiguration _config;
    private readonly ILogger<ApplicationRejectedEmailEventHandler> _logger;

    public ApplicationRejectedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, EmailNotificationsConfiguration config,
        ILogger<ApplicationRejectedEmailEventHandler> logger)
    {
        _levyTransferMatchingApi = api;
        _encodingService = encodingService;
        _config = config;
        _logger = logger;
    }

    [Function("ApplicationRejectedEmailEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationRejectedEmail)] ApplicationRejectedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationRejectedEmailEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationRejectedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            BaseUrl = _config.ViewTransfersBaseUrl,
            EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await _levyTransferMatchingApi.ApplicationRejectedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationRejectedEmailEvent for application {@event.ApplicationId}");
        }
    }
}