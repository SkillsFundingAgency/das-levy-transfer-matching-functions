using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationCreatedEmailEventHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
    private readonly IEncodingService _encodingService;
    private readonly ILogger<ApplicationCreatedEmailEventHandler> _logger;

    public ApplicationCreatedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, ILogger<ApplicationCreatedEmailEventHandler> logger)
    {
        _levyTransferMatchingApi = api;
        _encodingService = encodingService;
        _logger = logger;
    }

    [Function("ApplicationCreatedEmailEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationCreatedEmailEvent)] ApplicationCreatedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationCreatedEmailEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationCreatedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await _levyTransferMatchingApi.ApplicationCreatedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationCreatedEmailEvent for application {@event.ApplicationId}");
        }
    }
}