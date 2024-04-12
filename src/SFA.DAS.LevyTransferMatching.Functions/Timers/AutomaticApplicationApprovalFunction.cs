using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationApprovalFunction
{
    private readonly ILevyTransferMatchingApi _api;

    public AutomaticApplicationApprovalFunction(ILevyTransferMatchingApi api)
    {
        _api = api;
    }

    [Function("ApplicationsWithAutomaticApprovalFunction")]
    public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer, ILogger log)
    {
        log.LogInformation($"Executing ApplicationsWithAutomaticApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction(log);
          
    }

    [Function("HttpAutomaticApplicationApprovalFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationApprovalFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticApproval")] HttpRequest req, ILogger log)
    {
        log.LogInformation($"Executing HTTP Triggered HttpAutomaticApplicationApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction(log);

        return new OkObjectResult("ApplicationsWithAutomaticApproval successfully ran");
    }

    private async Task RunApplicationsWithAutomaticApprovalFunction(ILogger log)
    {
        try
        {
            var applications = await _api.GetApplicationsForAutomaticApproval();

            foreach (var app in applications.Applications)
            {
                await _api.ApproveApplication(new ApproveApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
            }
        }
        catch (ApiException ex)
        {
            log.LogError(ex, $"Error executing RunApplicationsWithAutomaticApprovalFunction");
            throw;
        }
    }
}