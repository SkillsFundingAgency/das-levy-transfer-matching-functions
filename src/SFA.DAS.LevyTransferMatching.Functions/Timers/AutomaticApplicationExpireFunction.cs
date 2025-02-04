using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationExpireFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationExpireFunction> log)
{
    [Function("ApplicationsWithAutomaticExpireFunction")]
    public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer)
    {
        log.LogInformation("Executing ApplicationsWithAutomaticExpireFunction");

        await RunApplicationsWithAutomaticExpireFunction();
    }

    [Function("HttpAutomaticApplicationExpireFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationExpireFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticExpire")] HttpRequest req)
    {
        log.LogInformation("Executing HTTP Triggered {FunctionName}", nameof(HttpAutomaticApplicationExpireFunction));

        await RunApplicationsWithAutomaticExpireFunction();

        return new OkObjectResult($"{nameof(HttpAutomaticApplicationExpireFunction)} successfully completed.");
    }

    private async Task RunApplicationsWithAutomaticExpireFunction()
    {
        try
        {
            var applications = await api.GetApplicationsForAutomaticExpire();

            if (applications != null)
            {
                log.LogInformation("GetApplicationsForAutomaticExpire returns {count} applications",
                applications.ApplicationIdsToExpire.Count());

                foreach (var id in applications.ApplicationIdsToExpire)
                {
                    log.LogInformation("auto-expiring application {id}", id);
                    await api.ExpireAcceptedFunding(new ExpireAcceptedFundingRequest { ApplicationId = id });
                }
            }
            else
            {
                log.LogInformation("GetApplicationsForAutomaticExpire returns NULL");
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error executing {MethodName}", nameof(RunApplicationsWithAutomaticExpireFunction));
            throw;
        }
    }
}
