using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RestEase;
using SFA.DAS.LevyTransferMatching.Functions.Api;

namespace SFA.DAS.LevyTransferMatching.Functions.HttpTriggers
{
    public class TestAutomaticApplicationApprovalFunction
    {
        private readonly ILevyTransferMatchingApi _api;

        public TestAutomaticApplicationApprovalFunction(ILevyTransferMatchingApi api)
        {
            _api = api;
        }

        [FunctionName("TestApplicationsWithAutomaticApprovalFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "ApplicationsWithAutomaticApproval")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Executing HTTP Triggered TestApplicationsWithAutomaticApprovalFunction");

            try
            {
                var applications = await _api.GetApplicationsForAutomaticApproval();

                foreach (var app in applications.Applications)
                {
                    await _api.ApproveApplication(new ApproveApplicationRequest { ApplicationId = app.Id, PledgeId = app.PledgeId });
                }
                return new OkObjectResult("ApplicationsWithAutomaticApproval successfully ran");
            }
            catch (ApiException ex)
            {
                log.LogError(ex, $"Error executing HTTP Triggered TestApplicationsWithAutomaticApprovalFunction");
                throw;
            }
        }
    }
}
