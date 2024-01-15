using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationRejectionFunction
{
    private readonly ILevyTransferMatchingApi _api;

    public AutomaticApplicationRejectionFunction(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [FunctionName("ApplicationsWithAutomaticRejectionFunction")]
    public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo timer, ILogger log)
    {
        log.LogInformation($"Executing ApplicationsWithAutomaticRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction(log);
    }

    [FunctionName("HttpAutomaticApplicationRejectionFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationRejectionFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticRejection")] HttpRequest req, ILogger log)
    {
        log.LogInformation($"Executing HTTP Triggered HttpAutomaticApplicationRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction(log);

        return new OkObjectResult("ApplicationsWithAutomaticRejection successfully ran");
    }

    private async Task RunApplicationsWithAutomaticRejectionFunction(ILogger log)
    {
        try
        {
            var applications = await _api.GetApplicationsForAutomaticRejection();

            foreach (var app in applications.Applications)
            {
                await _api.RejectApplication(new RejectApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
            }
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error executing RunApplicationsWithAutomaticRejectionFunction");
            throw;
        }
    }
}