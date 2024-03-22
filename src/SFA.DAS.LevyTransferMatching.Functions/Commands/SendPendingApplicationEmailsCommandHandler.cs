using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class SendPendingApplicationEmailsCommandHandler
{
    private readonly ILevyTransferMatchingApi _levyTransferMatchingApi;
    private readonly IEncodingService _encodingService;
    private readonly EmailNotificationsConfiguration _config;
    private readonly ILogger<SendPendingApplicationEmailsCommandHandler> _logger;

    public SendPendingApplicationEmailsCommandHandler(ILevyTransferMatchingApi levyTransferMatchingApi,
        IEncodingService encodingService,
        EmailNotificationsConfiguration config, ILogger<SendPendingApplicationEmailsCommandHandler> logger)
    {
        _levyTransferMatchingApi = levyTransferMatchingApi;
        _encodingService = encodingService;
        _config = config;
        _logger = logger;
    }

    [Function("SendPendingApplicationEmailsCommand")]
    public async Task Run([TimerTrigger("0 0 8 * * 1")] TimerInfo timer)
    {
        _logger.LogInformation("Sending pending application emails");
        var response = await _levyTransferMatchingApi.GetPendingApplicationEmailData();

        var sendEmailsRequest = new SendEmailsRequest { EmailDataList = new List<SendEmailsRequest.EmailData>() };

        foreach (var emailData in response.EmailDataList)
        {
            var tokens = new Dictionary<string, string>
            {
                { "EmployerName", emailData.EmployerName },
                { "NumberOfApplications", emailData.NumberOfApplications.ToString() },
                { "BaseUrl", _config.ViewTransfersBaseUrl },
                { "EncodedAccountId", _encodingService.Encode(emailData.AccountId, EncodingType.AccountId) },
                { "ApplicationsText", emailData.NumberOfApplications == 1 ? "application" : "applications" }
            };

            sendEmailsRequest.EmailDataList.Add(new SendEmailsRequest.EmailData(_config.PendingApplicationsTemplateName, emailData.RecipientEmailAddress, tokens));
        }

        try
        {
            await _levyTransferMatchingApi.SendEmails(sendEmailsRequest);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            _logger.LogError(ex, $"Error sending emails");
        }
    }
}