using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationApprovalFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationApprovalFunction> log)
{
    [Function("ApplicationsWithAutomaticApprovalFunction")]
    public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer)
    {
        log.LogInformation("Executing ApplicationsWithAutomaticApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction();
    }

    [Function("HttpAutomaticApplicationApprovalFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationApprovalFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticApproval")] HttpRequest req)
    {
        log.LogInformation("Executing HTTP Triggered HttpAutomaticApplicationApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction();

        return new OkObjectResult("ApplicationsWithAutomaticApproval successfully ran");
    }

    private async Task RunApplicationsWithAutomaticApprovalFunction()
    {
        try
        {
            var applications = await api.GetApplicationsForAutomaticApproval();

            foreach (var app in applications.Applications)
            {
                await api.ApproveApplication(new ApproveApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
            }
        }
        catch (ApiException ex)
        {
            log.LogError(ex, "Error executing RunApplicationsWithAutomaticApprovalFunction");
            throw;
        }
    }
}