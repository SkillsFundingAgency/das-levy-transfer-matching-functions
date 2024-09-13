using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Functions.Api;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Functions.Commands;

public class PledgeOptionsEmailsCommandHandler(
    ILevyTransferMatchingApi levyTransferMatchingApi,
    IEncodingService encodingService,
    EmailNotificationsConfiguration config)
{
    [Function("PledgeOptionsEmailCommand")]
    public async Task Run([TimerTrigger("0 0 8 1 5 *")] TimerInfo timer, ILogger logger)
    {
        logger.LogInformation("Sending pledge options emails");
        var response = await levyTransferMatchingApi.GetPledgeOptionsEmailData();

        var sendEmailsRequest = new SendEmailsRequest { EmailDataList = [] };
        
        foreach (var emailData in response.EmailDataList)
        {
            var tokens = new Dictionary<string, string>
            {
                { "EmployerName", emailData.EmployerName },
                { "BaseUrl", config.ViewAccountBaseUrl },
                { "EncodedAccountId", encodingService.Encode(emailData.AccountId, EncodingType.AccountId) },
                { "FinancialYearStart", emailData.FinancialYearStart },
                { "FinancialYearEnd", emailData.FinancialYearEnd }
            };

            sendEmailsRequest.EmailDataList.Add(new SendEmailsRequest.EmailData(config.PledgeOptionsTemplateName, emailData.RecipientEmailAddress, tokens));
        }

        try
        {
            await levyTransferMatchingApi.SendEmails(sendEmailsRequest);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode != HttpStatusCode.BadRequest) throw;

            logger.LogError(ex, "Error sending pledge options emails");
        }
    }
}