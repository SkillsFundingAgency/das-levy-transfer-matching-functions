using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.Timers;

public class AutomaticApplicationRejectionFunction
{
    private readonly ILevyTransferMatchingApi _api;
    private readonly ILogger<AutomaticApplicationRejectionFunction> _logger;

    public AutomaticApplicationRejectionFunction(ILevyTransferMatchingApi api, ILogger<AutomaticApplicationRejectionFunction> logger)
    {
        _api = api;
        _logger = logger;
    }

    [Function("ApplicationsWithAutomaticRejectionFunction")]
    public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Executing ApplicationsWithAutomaticRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction();
    }

    [Function("HttpAutomaticApplicationRejectionFunction")]
    public async Task<IActionResult> HttpAutomaticApplicationRejectionFunction([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticRejection")] HttpRequest req)
    {
        _logger.LogInformation($"Executing HTTP Triggered HttpAutomaticApplicationRejectionFunction");

        await RunApplicationsWithAutomaticRejectionFunction();

        return new OkObjectResult("ApplicationsWithAutomaticRejection successfully ran");
    }

    private async Task RunApplicationsWithAutomaticRejectionFunction()
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
            _logger.LogError(ex, $"Error executing RunApplicationsWithAutomaticRejectionFunction");
            throw;
        }
    }
}