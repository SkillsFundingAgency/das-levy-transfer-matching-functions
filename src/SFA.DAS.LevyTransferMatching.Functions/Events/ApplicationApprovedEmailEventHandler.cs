using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure;
using SFA.DAS.LevyTransferMatching.Messages.Events;


namespace SFA.DAS.LevyTransferMatching.Functions.Events;

public class ApplicationApprovedEmailEventHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
    private readonly IEncodingService _encodingService;
    private ILogger<ApplicationApprovedEmailEventHandler> _logger;

    public ApplicationApprovedEmailEventHandler(ILevyTransferMatchingApi api, IEncodingService encodingService, ILogger<ApplicationApprovedEmailEventHandler> logger)
    {
        _levyTransferMatchingApi = api;
        _encodingService = encodingService;
        _logger = logger;
    }

    [Function("ApplicationApprovedEmailEvent")]
    public async Task Run([ServiceBusTrigger(QueueNames.ApplicationApprovedEmail)] ApplicationApprovedEvent @event)
    {
        _logger.LogInformation($"Handling ApplicationApprovedEmailEvent handler for application {@event.ApplicationId}");

        var request = new ApplicationApprovedEmailRequest
        {
            PledgeId = @event.PledgeId,
            ApplicationId = @event.ApplicationId,
            ReceiverId = @event.ReceiverAccountId,
            EncodedApplicationId = _encodingService.Encode(@event.ApplicationId, EncodingType.PledgeApplicationId)
        };

        try
        {
            await _levyTransferMatchingApi.ApplicationApprovedEmail(request);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error handling ApplicationApprovedEmailEvent for application {@event.ApplicationId}");
        }
    }
}