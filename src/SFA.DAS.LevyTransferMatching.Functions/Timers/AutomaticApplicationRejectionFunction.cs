using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationRejectionFunction(ILevyTransferMatchingApi api)
{
    [Function("ApplicationsWithAutomaticRejectionFunction")]
    public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo timer, ILogger log)
    {
        log.LogInformation("Executing ApplicationsWithAutomaticRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction(log);
    }

    [Function("HttpAutomaticApplicationRejectionFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationRejectionFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticRejection")] HttpRequest req, ILogger log)
    {
        log.LogInformation("Executing HTTP Triggered {FunctionName}", nameof(HttpAutomaticApplicationRejectionFunction));

        await RunApplicationsWithAutomaticRejectionFunction(log);

        return new OkObjectResult($"{nameof(HttpAutomaticApplicationRejectionFunction)} successfully completed.");
    }

    private async Task RunApplicationsWithAutomaticRejectionFunction(ILogger log)
    {
        try
        {
            var applications = await api.GetApplicationsForAutomaticRejection();

            foreach (var app in applications.Applications)
            {
                await api.RejectApplication(new RejectApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
            }
        }
        catch (ApiException ex)
        {
            log.LogError(ex, "Error executing {MethodName}", nameof(RunApplicationsWithAutomaticRejectionFunction));
            throw;
        }
    }
}