using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationApprovalFunction
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<AutomaticApplicationApprovalFunction> _logger;

    public AutomaticApplicationApprovalFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationApprovalFunction> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("ApplicationsWithAutomaticApprovalFunction")]
    public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Executing ApplicationsWithAutomaticApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction();
          
    }

    [Function("HttpAutomaticApplicationApprovalFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationApprovalFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticApproval")] HttpRequest req)
    {
        _logger.LogInformation($"Executing HTTP Triggered HttpAutomaticApplicationApprovalFunction");

        await RunApplicationsWithAutomaticApprovalFunction();

        return new OkObjectResult("ApplicationsWithAutomaticApproval successfully ran");
    }

    private async Task RunApplicationsWithAutomaticApprovalFunction()
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
            _logger.LogError(ex, $"Error executing RunApplicationsWithAutomaticApprovalFunction");
            throw;
        }
    }
}