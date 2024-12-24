using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationDeclineFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationDeclineFunction> log)
{
    [Function("ApplicationsWithAutomaticDeclineFunction")]
    public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer)
    {
        log.LogInformation("Executing ApplicationsWithAutomaticDeclineFunction");

        await RunApplicationsWithAutomaticDeclineFunction();
    }

    [Function("HttpAutomaticApplicationDeclineFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationDeclineFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticDecline")] HttpRequest req)
    {
        log.LogInformation("Executing HTTP Triggered {FunctionName}", nameof(HttpAutomaticApplicationDeclineFunction));

        await RunApplicationsWithAutomaticDeclineFunction();

        return new OkObjectResult($"{nameof(HttpAutomaticApplicationDeclineFunction)} successfully completed.");
    }

    private async Task RunApplicationsWithAutomaticDeclineFunction()
    {
        try
        {
            var applications = await api.GetApplicationsForAutomaticDecline();
            log.LogInformation("GetApplicationsForAutomaticDecline returns {count} applications", 
                applications?.ApplicationsToDecline.Count());

            foreach (var id in applications.ApplicationsToDecline)
            {
                log.LogInformation("auto-declining application {id}", id);
                await api.DeclineAcceptedFunding(new DeclineAcceptedFundingRequest { ApplicationId = id});
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error executing {MethodName}", nameof(RunApplicationsWithAutomaticDeclineFunction));
            throw;
        }
    }
}