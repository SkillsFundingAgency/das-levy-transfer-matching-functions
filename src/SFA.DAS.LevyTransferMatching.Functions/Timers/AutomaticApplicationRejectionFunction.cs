using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationRejectionFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationRejectionFunction> log)
{
    [Function("ApplicationsWithAutomaticRejectionFunction")]
    public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo timer)
    {
        log.LogInformation("Executing ApplicationsWithAutomaticRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction();
    }

    [Function("HttpAutomaticApplicationRejectionFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationRejectionFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticRejection")] HttpRequest req)
    {
        log.LogInformation("Executing HTTP Triggered {FunctionName}", nameof(HttpAutomaticApplicationRejectionFunction));

        await RunApplicationsWithAutomaticRejectionFunction();

        return new OkObjectResult($"{nameof(HttpAutomaticApplicationRejectionFunction)} successfully completed.");
    }

    private async Task RunApplicationsWithAutomaticRejectionFunction()
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