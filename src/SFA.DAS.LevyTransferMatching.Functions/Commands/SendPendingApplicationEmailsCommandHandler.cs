using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class SendPendingApplicationEmailsCommandHandler(
    ILevyTransferMatchingApi levyTransferMatchingApi,
    IEncodingService encodingService,
    EmailNotificationsConfiguration config)
{
    [Function("SendPendingApplicationEmailsCommand")]
    public async Task Run([TimerTrigger("0 0 8 * * 1")] TimerInfo timer, ILogger<SendPendingApplicationEmailsCommandHandler> logger)
    {
        logger.LogInformation("Sending pending application emails");
        var response = await levyTransferMatchingApi.GetPendingApplicationEmailData();

        var sendEmailsRequest = new SendEmailsRequest { EmailDataList = [] };
        
        foreach(var emailData in response.EmailDataList)
        {
            var tokens = new Dictionary<string, string>
            {
                { "EmployerName", emailData.EmployerName },
                { "NumberOfApplications", emailData.NumberOfApplications.ToString() },
                { "BaseUrl", config.ViewTransfersBaseUrl },
                { "EncodedAccountId", encodingService.Encode(emailData.AccountId, EncodingType.AccountId) },
                { "ApplicationsText", emailData.NumberOfApplications == 1 ? "application" : "applications" }
            };

            sendEmailsRequest.EmailDataList.Add(new SendEmailsRequest.EmailData(config.PendingApplicationsTemplateName, emailData.RecipientEmailAddress, tokens));
        }

        try
        {
            await levyTransferMatchingApi.SendEmails(sendEmailsRequest);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                throw;
            }

            logger.LogError(ex, "Error sending emails");
        }
    }
}